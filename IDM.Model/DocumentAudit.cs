using System;

namespace IDM.Model
{
    public class DocumentAudit
    {
        public string StoredBy { get; set; }
        public DateTime StoreTs { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTs { get; set; }

        //public string STOREDBY { get; set; }
        //public DateTime STORETS { get; set; }
        //public string UPDATEDBY { get; set; }
        //public DateTime UPDATEDTS { get; set; }
    }
}
