using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VsQuickTest.chap28_dataset
{
    public partial class CasesWindow : Form
    {
        public CasesWindow()
        {
            InitializeComponent();
        }

        private void casesBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.casesBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.cases);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'cases._cases' table. You can move, or remove it, as needed.
            this.casesTableAdapter.Fill(this.cases._cases);

        }
    }
}
