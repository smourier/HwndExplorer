using System;
using System.Windows.Forms;

namespace HwndExplorer
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            Icon = Resources.HwndExplorer;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => Close();
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}