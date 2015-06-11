using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VsQuickTest.basic.function;
using VsQuickTest.chap28_dataset;

namespace VsQuickTest
{
    public partial class QuickForm : Form
    {
        public QuickForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            helloWord();
        }

        private void helloWord()
        {
            //System.Console.WriteLine("Hello, C#!");
            MessageBox.Show("Hello, C#!");
        }

        private void testLinqStatement()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var lowNums =
                 from n in numbers
                 where n < 5
                 select n;

            String result = "Numbers < 5:";
            foreach (var x in lowNums)
            {
                //Console.WriteLine(x);
                result = result + "\n" + x;
            }
            MessageBox.Show(result);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            testLinqStatement();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new CasesWindow().ShowDialog();
        }

        private void defineAndCallFunc_Click(object sender, EventArgs e)
        {
            new HandleMessage("where?").Handle();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        
    }
}
