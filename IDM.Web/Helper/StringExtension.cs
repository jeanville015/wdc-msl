using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDM.Web.Helper
{
    public static class StringExtensions
    {
        public static string OrDash(this string value) =>
            string.IsNullOrEmpty(value) ? "-" : value;
    }
}