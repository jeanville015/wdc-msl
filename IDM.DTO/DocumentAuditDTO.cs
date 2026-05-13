using System;

namespace IDM.DTO
{
    public class DocumentAuditDTO
    {
        public string StoredBy { get; set; }
        public DateTime StoreTs { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTs { get; set; }
    }
}
