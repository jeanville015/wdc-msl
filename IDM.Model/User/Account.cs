using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Model.User
{
    public class Account : DocumentAudit
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string AdName { get; set; }
        public string User_Id { get; set; }
        public string User_Role { get; set; }
        public string User_Group { get; set; }
        public List<int> User_Analysis { get; set; }
        public string User_Analysis_String_Group { get; set; }
        public DateTime LastLoginTs { get; set; }
        public string ActiveFlag { get; set; }  // Changed from int to string to match 'Y'/'N' in database
    }
}
