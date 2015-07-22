/*
 * 
 * History
 * Date        Ver Author        Change Description
 * ----------- --- ------------- ----------------------------------------
 * 21 Jul 2015 001 karl          Run UI test by reflection
 * 22 Jul 2015 002 karl          add method to access property
 */
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VsQuickTest.basic.test.reflectuitest.ReflectAutApp
{
    class ReflectAutApp
    {
        [Test]
        public void evenTest() 
        {
            Form form = LaunchApp("..\\..\\..\\AUT\\bin\\Debug\\AUT.exe", 
               "AUT.Form1", 3000);
            Point p = (Point)GetFormPropertyValue(form, "Location");
            String locationMsg = "Form location = " + p.X + " " + p.Y;
            Console.WriteLine(locationMsg);
            MessageBox.Show(locationMsg, "Form location");
        }

        private static Form LaunchApp(String path, String formName, int timeout)
        {
            Form theForm = null;
            try
            {
                Console.WriteLine("Launching Form");
                Assembly a = Assembly.LoadFrom(path);
                Type t1 = a.GetType(formName);
                theForm = (Form)a.CreateInstance(t1.FullName);
                AppState aps = new AppState(theForm);
                ThreadStart ts = new ThreadStart(aps.RunApp);
                Thread thread = new Thread(ts);
                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = true;
                thread.Start();
                //Thread.Sleep(timeout);
                Console.WriteLine("\nForm launched");
            }
            catch (Exception ex)
            {
                throw new Exception("Fatal error: " + ex.Message);
            }
            return theForm;
            
        }

        delegate object GetFormPropertyValueHandler(Form f, string propertyName);

        static object GetFormPropertyValue(Form f, string propertyName)
        {
            if (f.InvokeRequired)
            {
                Delegate d = new GetFormPropertyValueHandler(GetFormPropertyValue);
                object[] o = new object[] { f, propertyName };
                object iResult = f.Invoke(d, o);
                return iResult;
            }
            else
            {
                Type t = f.GetType();
                PropertyInfo pi = t.GetProperty(propertyName);
                object gResult = pi.GetValue(f, null);
                return gResult;
            }
        }
    }

    class AppState
    {
        public readonly Form formToRun;
        public AppState(Form f)
        {
            this.formToRun = f;
        }
        public void RunApp()
        {
            Application.Run(formToRun);
        }
    }

}
