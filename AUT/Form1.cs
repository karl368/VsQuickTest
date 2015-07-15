using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VsQuickTest.AUT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string tb = textBox2.Text;
            string cb = comboBox1.Text;

            if (tb == cb)
                listBox1.Items.Add("Even");
            else if (tb == "paper" && cb == "rock" ||
                tb == "rock" && cb == "scissor" ||
                tb == "scissor" && cb == "paper")
                listBox1.Items.Add("win");
            else
                listBox1.Items.Add("loss");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
