using System;
using System.Collections.Generic;

namespace IDM.DTO.Main
{
    public class DynamicStagingDTO : DocumentAuditDTO
    {
        // Core properties that exist in all tables
        public string AmethystJob { get; set; }
        public string Analysis { get; set; }
        
        // Dynamic properties for table-specific columns
        public Dictionary<string, object> AdditionalProperties { get; set; } = new Dictionary<string, object>();
        
        // Helper to get any property (core or additional)
        public object GetProperty(string propertyName)
        {
            var coreProperty = GetType().GetProperty(propertyName);
            if (coreProperty != null)
                return coreProperty.GetValue(this);
                
            return AdditionalProperties.TryGetValue(propertyName, out var value) ? value : null;
        }
        
        // Helper to set any property (core or additional)
        public void SetProperty(string propertyName, object value)
        {
            var coreProperty = GetType().GetProperty(propertyName);
            if (coreProperty != null && coreProperty.CanWrite)
            {
                coreProperty.SetValue(this, value);
            }
            else
            {
                AdditionalProperties[propertyName] = value;
            }
        }
    }
}
