using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Model.Common
{
    public class BDPUploadVariables 
    {
        ///// <summary>
        ///// Primary key id of DATA_STAGING item to be procecessed.
        ///// </summary>
        //public int ObjectId { get; set; }

        ///// <summary>
        ///// datafield name of DATA_STAGING item to be procecessed.
        ///// </summary>
        //public string DataName { get; set; }

        /// <summary>
        /// this will be the reference in dynamic data_staging [table] updating using [pkid] 
        /// </summary>
        public DataIdValuePair _DataIdValuePair { get; set; } = new DataIdValuePair();

        /// <summary>
        /// this will be reference in dynamic data_staging(main,details) updating using [pkid]
        /// </summary>
        public DataNameValuePair _DataNameValuePair { get; set; } = new DataNameValuePair();

        /// <summary>
        /// file dir of the images that will be subject for upload and preview
        /// </summary>
        public string OriginFileDirectory { get; set; }

        /// <summary>
        /// BDP Upload file directory
        /// </summary>
        public string FileDirectory { get; set; }
        
        /// <summary>
        /// programmed template name of image files
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// filename extracted from OriginFileDirectory
        /// </summary>
        public string ExtractedFileName { get; set; }
        
        ///// <summary>
        ///// created BDP URL
        ///// </summary>
        //public string NewURL { get; set; }

    }

    public class DataIdValuePair
    {
        public string IdName { get; set; }
        public int IdValue { get; set; }

    }

    public class DataNameValuePair
    {
        public string DataName { get; set; }
        public string DataValue { get; set; }

    }
}
