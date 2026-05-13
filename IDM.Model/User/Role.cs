namespace IDM.Model.User
{
    public class Role : DocumentAudit
    {
        public int Id { get; set; }
        public string User_Role { get; set; }
        public string ActiveFlag { get; set; }
    }
}
