using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VsQuickTest.basic.web
{
    class WebClientUtil
    {
        public static string getUriSource(string uri) {
            //string uri = "http://www.cnblogs.com";
            WebClient client = new WebClient();
            Stream st = client.OpenRead(uri);
            StreamReader reader = new StreamReader(st);

            string rtn = reader.ReadToEnd();

            reader.Close();
            st.Close();

            return rtn;
        }
        
    }
}
