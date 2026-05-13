namespace IDM.DTO.User
{
    public class RoleDTO : DocumentAuditDTO
    {
        public int Id { get; set; }
        public string User_Role { get; set; }
        public string ActiveFlag { get; set; }

    }
}
