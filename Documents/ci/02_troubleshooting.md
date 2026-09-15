# CaseTracker — Deployment & CI/CD Troubleshooting Guide

> **Document Version:** 1.0  
> **Environment:** GitHub Actions & MonsterASP.net (IIS WebDeploy)  
> **Parent Documentation:** [Documentation Center](../README.md) | [CI/CD Pipeline](./01_ci_cd_pipeline.md)

---

## Diagnostic Matrix

| Error Code / Message | Primary Cause | Immediate Fix |
| :--- | :--- | :--- |
| **`401 Unauthorized`** | Incorrect password or WebDeploy disabled on MonsterASP | Check **Visual Studio access** tab on MonsterASP; update password |
| **`Unrecognized argument '"-dest:...'`** | PowerShell argument parser wrapping quotes around `-dest:` | Use PowerShell argument array `@msdeployArgs` |
| **`Process cannot access file (Locked DLL)`** | IIS handling active requests with loaded `.dll` in memory | Enable `-enableRule:AppOffline` in MSDeploy |
| **`Deployment secrets missing!`** | GitHub repository secrets not populated or misnamed | Add `WEBSITE_NAME`, `SERVER_COMPUTER_NAME`, `SERVER_USERNAME`, `SERVER_PASSWORD` |
| **`Unable to connect to remote server`** | Firewall blocking port 8172 or wrong hostname | Ensure host is `https://site90440.siteasp.net:8172/msdeploy.axd` |

---

## 1. Resolving `Error: (401) Unauthorized`

### Root Cause
On MonsterASP.net, **FTP and WebDeploy credentials are often completely different**.
Looking at the `FTP/SFTP access` tab does NOT give you the WebDeploy password.

### Resolution Steps
1. Log in to [MonsterASP Control Panel](https://admin.monsterasp.net/).
2. Select your website (`site90440`).
3. Click the **`</> Visual Studio access`** tab (located next to `FTP/SFTP access`).
4. Verify WebDeploy status says **Enabled**.
5. Copy the **Password** field displayed in this tab.
6. In GitHub, go to **Settings &rarr; Secrets and variables &rarr; Actions &rarr; `SERVER_PASSWORD`** and update it with this exact value.

---

## 2. Resolving `Unrecognized argument '"-dest:..."'. All arguments must begin with "-"`

### Root Cause
In PowerShell (`pwsh`), when calling a native Windows command (`msdeploy.exe`) with an argument containing internal double quotes (e.g. `-dest:iisApp="$site",computerName="..."`), PowerShell automatically escapes internal quotes with `\"` and wraps the entire parameter in outer double quotes:
```text
"-dest:iisApp=\"$site\",computerName=\"$destComputer\"..."
```
Because the argument starts with a literal double quote `"` rather than a dash `-`, `msdeploy.exe` immediately rejects it with:
```text
Error: Unrecognized argument '"-dest:...' All arguments must begin with "-".
```

### Resolution
Define arguments in a native PowerShell string array (`@()`) without internal double quotes:
```powershell
$destArg = "contentPath=${site},computerName=${destComputer},userName=${username},password=${password},authtype=Basic,includeAcls=False"

$msdeployArgs = @(
    "-verb:sync",
    "-source:contentPath=$publishPath",
    "-dest:$destArg",
    "-enableRule:AppOffline",
    "-allowUntrusted",
    "-disableLink:AppPoolExtension",
    "-disableLink:ContentExtension",
    "-disableLink:CertificateExtension",
    "-retryAttempts:5",
    "-retryInterval:3000"
)

& $msdeployPath $msdeployArgs
```

---

## 3. Resolving File-Locking Errors During Deploy

### Symptoms
```text
Error: An error occurred when reading or writing to a file... The process cannot access the file 'CaseTracker.dll' because it is being used by another process.
```

### Resolution
Ensure `-enableRule:AppOffline` is passed to `msdeploy.exe`. 
This instructs IIS to:
1. Place a temporary `app_offline.htm` file in `/wwwroot`.
2. Wait for the ASP.NET Core module (`AspNetCoreModuleV2`) to gracefully shut down the app pool and release all DLL locks.
3. Sync the new application binaries.
4. Delete `app_offline.htm`, restarting the application with the fresh deployment.

---

## 4. Alternative: Switching to Direct FTP Deployment

If port 8172 is blocked by corporate firewalls or WebDeploy is unavailable, deploy directly via FTP:

```yaml
      - name: Deploy via FTP
        uses: SamKirkland/FTP-Deploy-Action@v4.3.5
        with:
          server: site90440.siteasp.net
          username: ${{ secrets.SERVER_USERNAME }}
          password: ${{ secrets.FTP_PASSWORD }}
          local-dir: ./publish/
          server-dir: /wwwroot/
```
*(FTP credentials are found under the **FTP/SFTP access** tab in the MonsterASP panel).*
