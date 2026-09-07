namespace CaseTrackerApplication.DTOs.UserRoles
{
    public class AssignUserRoleRequest
    {
        public Guid UserId { get; set; }

        public string RoleId { get; set; } = string.Empty;
    }
}
