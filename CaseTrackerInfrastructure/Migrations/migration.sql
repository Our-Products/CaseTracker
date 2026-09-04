CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    CREATE TABLE law_firms (
        law_firm_id uuid NOT NULL,
        firm_name text NOT NULL,
        registration_number text,
        address_line1 text NOT NULL,
        address_line2 text,
        city text NOT NULL,
        district text NOT NULL,
        state text NOT NULL,
        pincode integer NOT NULL,
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone NOT NULL,
        CONSTRAINT "PK_law_firms" PRIMARY KEY (law_firm_id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    CREATE TABLE roles (
        role_id character varying(20) NOT NULL,
        role_name character varying(50) NOT NULL,
        description text,
        status character varying(20) NOT NULL,
        created_at timestamptz NOT NULL,
        updated_at timestamptz NOT NULL,
        CONSTRAINT "PK_roles" PRIMARY KEY (role_id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    CREATE TABLE users (
        user_id uuid NOT NULL,
        mobile_number text NOT NULL,
        email text NOT NULL,
        password text NOT NULL,
        status text NOT NULL,
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone NOT NULL,
        created_by text,
        updated_by text,
        CONSTRAINT "PK_users" PRIMARY KEY (user_id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    CREATE TABLE lawyers (
        lawyer_id uuid NOT NULL,
        user_id uuid NOT NULL,
        law_firm_id uuid,
        full_name text NOT NULL,
        bar_council_id text,
        bar_council_name text,
        enrollment_date timestamp with time zone,
        status text NOT NULL,
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone NOT NULL,
        CONSTRAINT "PK_lawyers" PRIMARY KEY (lawyer_id),
        CONSTRAINT "FK_lawyers_law_firms_law_firm_id" FOREIGN KEY (law_firm_id) REFERENCES law_firms (law_firm_id) ON DELETE SET NULL,
        CONSTRAINT "FK_lawyers_users_user_id" FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    CREATE TABLE user_law_firms (
        user_id uuid NOT NULL,
        law_firm_id uuid NOT NULL,
        joined_at timestamp with time zone NOT NULL,
        status text NOT NULL,
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone NOT NULL,
        CONSTRAINT "PK_user_law_firms" PRIMARY KEY (user_id, law_firm_id),
        CONSTRAINT "FK_user_law_firms_law_firms_law_firm_id" FOREIGN KEY (law_firm_id) REFERENCES law_firms (law_firm_id) ON DELETE CASCADE,
        CONSTRAINT "FK_user_law_firms_users_user_id" FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    CREATE TABLE user_role (
        user_id uuid NOT NULL,
        role_id character varying(20) NOT NULL,
        created_at timestamptz NOT NULL,
        updated_at timestamptz NOT NULL,
        created_by character varying(255) NOT NULL,
        updated_by character varying(255) NOT NULL,
        CONSTRAINT "PK_user_role" PRIMARY KEY (user_id, role_id),
        CONSTRAINT "FK_user_role_roles_role_id" FOREIGN KEY (role_id) REFERENCES roles (role_id) ON DELETE CASCADE,
        CONSTRAINT "FK_user_role_users_user_id" FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    INSERT INTO roles (role_id, created_at, description, role_name, status, updated_at)
    VALUES ('R001', TIMESTAMPTZ '2026-09-04T00:00:00+00:00', 'Legal professional who manages cases and clients', 'Lawyer', 'Active', TIMESTAMPTZ '2026-09-04T00:00:00+00:00');
    INSERT INTO roles (role_id, created_at, description, role_name, status, updated_at)
    VALUES ('R002', TIMESTAMPTZ '2026-09-04T00:00:00+00:00', 'Staff member who assists lawyers', 'Staff', 'Active', TIMESTAMPTZ '2026-09-04T00:00:00+00:00');
    INSERT INTO roles (role_id, created_at, description, role_name, status, updated_at)
    VALUES ('R003', TIMESTAMPTZ '2026-09-04T00:00:00+00:00', 'Law-firm administrator', 'Admin', 'Active', TIMESTAMPTZ '2026-09-04T00:00:00+00:00');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    CREATE INDEX ix_law_firms_firm_name ON law_firms (firm_name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    CREATE INDEX ix_lawyers_bar_council_id ON lawyers (bar_council_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    CREATE INDEX "IX_lawyers_law_firm_id" ON lawyers (law_firm_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    CREATE INDEX ix_lawyers_status ON lawyers (status);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    CREATE UNIQUE INDEX ix_lawyers_user_id ON lawyers (user_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    CREATE UNIQUE INDEX ix_roles_role_name ON roles (role_name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    CREATE INDEX "IX_user_law_firms_law_firm_id" ON user_law_firms (law_firm_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    CREATE INDEX "IX_user_role_role_id" ON user_role (role_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    CREATE INDEX ix_users_status ON users (status);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    CREATE UNIQUE INDEX ux_users_email ON users (email);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    CREATE UNIQUE INDEX ux_users_mobile_number ON users (mobile_number);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100855_Initial-1') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260904100855_Initial-1', '8.0.11');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    CREATE TABLE law_firms (
        law_firm_id uuid NOT NULL,
        firm_name text NOT NULL,
        registration_number text,
        address_line1 text NOT NULL,
        address_line2 text,
        city text NOT NULL,
        district text NOT NULL,
        state text NOT NULL,
        pincode integer NOT NULL,
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone NOT NULL,
        CONSTRAINT "PK_law_firms" PRIMARY KEY (law_firm_id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    CREATE TABLE roles (
        role_id character varying(20) NOT NULL,
        role_name character varying(50) NOT NULL,
        description text,
        status character varying(20) NOT NULL,
        created_at timestamptz NOT NULL,
        updated_at timestamptz NOT NULL,
        CONSTRAINT "PK_roles" PRIMARY KEY (role_id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    CREATE TABLE users (
        user_id uuid NOT NULL,
        mobile_number text NOT NULL,
        email text NOT NULL,
        password text NOT NULL,
        status text NOT NULL,
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone NOT NULL,
        created_by text,
        updated_by text,
        CONSTRAINT "PK_users" PRIMARY KEY (user_id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    CREATE TABLE lawyers (
        lawyer_id uuid NOT NULL,
        user_id uuid NOT NULL,
        law_firm_id uuid,
        full_name text NOT NULL,
        bar_council_id text,
        bar_council_name text,
        enrollment_date timestamp with time zone,
        status text NOT NULL,
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone NOT NULL,
        CONSTRAINT "PK_lawyers" PRIMARY KEY (lawyer_id),
        CONSTRAINT "FK_lawyers_law_firms_law_firm_id" FOREIGN KEY (law_firm_id) REFERENCES law_firms (law_firm_id) ON DELETE SET NULL,
        CONSTRAINT "FK_lawyers_users_user_id" FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    CREATE TABLE user_law_firms (
        user_id uuid NOT NULL,
        law_firm_id uuid NOT NULL,
        joined_at timestamp with time zone NOT NULL,
        status text NOT NULL,
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone NOT NULL,
        CONSTRAINT "PK_user_law_firms" PRIMARY KEY (user_id, law_firm_id),
        CONSTRAINT "FK_user_law_firms_law_firms_law_firm_id" FOREIGN KEY (law_firm_id) REFERENCES law_firms (law_firm_id) ON DELETE CASCADE,
        CONSTRAINT "FK_user_law_firms_users_user_id" FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    CREATE TABLE user_role (
        user_id uuid NOT NULL,
        role_id character varying(20) NOT NULL,
        created_at timestamptz NOT NULL,
        updated_at timestamptz NOT NULL,
        created_by character varying(255) NOT NULL,
        updated_by character varying(255) NOT NULL,
        CONSTRAINT "PK_user_role" PRIMARY KEY (user_id, role_id),
        CONSTRAINT "FK_user_role_roles_role_id" FOREIGN KEY (role_id) REFERENCES roles (role_id) ON DELETE CASCADE,
        CONSTRAINT "FK_user_role_users_user_id" FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    INSERT INTO roles (role_id, created_at, description, role_name, status, updated_at)
    VALUES ('R001', TIMESTAMPTZ '2026-09-04T00:00:00+00:00', 'Legal professional who manages cases and clients', 'Lawyer', 'Active', TIMESTAMPTZ '2026-09-04T00:00:00+00:00');
    INSERT INTO roles (role_id, created_at, description, role_name, status, updated_at)
    VALUES ('R002', TIMESTAMPTZ '2026-09-04T00:00:00+00:00', 'Staff member who assists lawyers', 'Staff', 'Active', TIMESTAMPTZ '2026-09-04T00:00:00+00:00');
    INSERT INTO roles (role_id, created_at, description, role_name, status, updated_at)
    VALUES ('R003', TIMESTAMPTZ '2026-09-04T00:00:00+00:00', 'Law-firm administrator', 'Admin', 'Active', TIMESTAMPTZ '2026-09-04T00:00:00+00:00');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    CREATE INDEX ix_law_firms_firm_name ON law_firms (firm_name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    CREATE INDEX ix_lawyers_bar_council_id ON lawyers (bar_council_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    CREATE INDEX "IX_lawyers_law_firm_id" ON lawyers (law_firm_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    CREATE INDEX ix_lawyers_status ON lawyers (status);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    CREATE UNIQUE INDEX ix_lawyers_user_id ON lawyers (user_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    CREATE UNIQUE INDEX ix_roles_role_name ON roles (role_name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    CREATE INDEX "IX_user_law_firms_law_firm_id" ON user_law_firms (law_firm_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    CREATE INDEX "IX_user_role_role_id" ON user_role (role_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    CREATE INDEX ix_users_status ON users (status);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    CREATE UNIQUE INDEX ux_users_email ON users (email);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    CREATE UNIQUE INDEX ux_users_mobile_number ON users (mobile_number);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260904100943_Initial-2') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260904100943_Initial-2', '8.0.11');
    END IF;
END $EF$;
COMMIT;

