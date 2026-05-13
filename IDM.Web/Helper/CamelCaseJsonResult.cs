using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Text;
using System.Web.Mvc;


namespace IDM.Web.Helper
{
    public class CamelCaseJsonResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;
            response.ContentType = "application/json";

            if (Data != null)
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var serialized = JsonConvert.SerializeObject(Data, settings);
                response.ContentEncoding = Encoding.UTF8;
                response.Write(serialized);
            }
        }
    }
}