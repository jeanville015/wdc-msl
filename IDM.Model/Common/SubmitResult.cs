using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Model.Common
{
    public class SubmitResult
    {
        public bool Success { get; set; }
        public string Error { get; set; } = string.Empty;

        public static SubmitResult Ok() => new SubmitResult { Success = true };
        public static SubmitResult OkWithWarning(string warning) => new SubmitResult { Success = true, Error = warning };
        public static SubmitResult Fail(string error) => new SubmitResult { Success = false, Error = error };
    }


}
