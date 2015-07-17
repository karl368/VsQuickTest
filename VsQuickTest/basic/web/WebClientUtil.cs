/*
 * History
 * Date        Ver Author        Change Description
 * ----------- --- ------------- ----------------------------------------
 * 15 Jul 2015 001 Karl          how to user web client to get page source
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VsQuickTest.basic.language.web
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
