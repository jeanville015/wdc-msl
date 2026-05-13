using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDM.Web.DataAccessModel
{
    public class Tag
    {
        public string Name;

        public string Value;

        public string TargetParameter = null;

        public int TargetSample = -1;

        public Tag(string name)
        {
            Name = name;
        }

        public Tag(string name, string value, string targetParameter = null, int targetSample = -1)
        {
            Name = name;
            Value = value;
            TargetParameter = targetParameter;
            TargetSample = targetSample;
        }
    }
}