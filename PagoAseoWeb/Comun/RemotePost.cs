using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace SocialAseoWeb.Comun
{
    public class RemotePost
    {
        private NameValueCollection Inputs = new NameValueCollection();
        public string Url = "";
        public string Method = "post";
        public string FormName = "form1";

        public void Add(string name, string value)
        {
            Inputs.Add(name, value);
        }

        public void Post()
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write("<html><head>");
            HttpContext.Current.Response.Write(string.Format("</head><body onload=\"document.{0}.submit()\">", FormName));
            HttpContext.Current.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url));
            for (int i = 0; i < Inputs.Keys.Count; i++)
            {
                HttpContext.Current.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", Inputs.Keys[i], Inputs[Inputs.Keys[i]]));
            }
            HttpContext.Current.Response.Write("</form>");
            HttpContext.Current.Response.Write("</body></html>");
            HttpContext.Current.Response.End();
        }
    }
}