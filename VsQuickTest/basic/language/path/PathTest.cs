
using NUnit.Framework;
/*
 * History
 * Date        Ver Author        Change Description
 * ----------- --- ------------- ----------------------------------------
 * 22 Mar 2015 001 Karl          How to get all kinds of paths
 *                               Get exe file located folder path
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VsQuickTest.basic.language.path
{
    class PathTest
    {
        [Test]
        public void exeFileLocatedPath() {
            Console.WriteLine(System.Environment.CurrentDirectory);
            // E:\vsWorkspace\Projects\VsQuickTest\VsQuickTest\bin\Debug
            MessageBox.Show(System.Environment.CurrentDirectory);
        }
    }
}
