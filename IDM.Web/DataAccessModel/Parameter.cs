using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDM.Web.DataAccessModel
{
    public class Parameter
    {
        public string Name;

        public List<double?> Samples = new List<double?>();

        public bool Editable;

        public Parameter(string name) : this(name, false) { }

        public Parameter(string name, bool editable)
        {
            Name = name;
            Editable = editable;
        }
    }
}