using System;
using System.Reflection;
using System.Windows.Forms;
using HwndExplorer.Utilities;

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
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e) => this.ShowMessage(Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyTitleAttribute>()!.Title + " - " + (IntPtr.Size == 4 ? "32" : "64") + "-bit" + Environment.NewLine + "Copyright (C) 2021-" + DateTime.Now.Year + " Simon Mourier. All rights reserved.");

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            LoadTopLevelWindows();
        }

        private void LoadTopLevelWindows()
        {
            treeViewWindows.Nodes.Clear();
            foreach (var window in Win32Window.TopLevelWindows)
            {
                AddWindow(window, null);
            }
        }

        private void AddWindow(Win32Window window, TreeNode? parent)
        {
            TreeNode node;
            if (parent == null)
            {
                node = treeViewWindows.Nodes.Add(window.ToString());
            }
            else
            {
                node = parent.Nodes.Add(window.ToString());
            }

            node.Tag = window;
        }

        private void TreeViewWindows_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyGridMain.SelectedObject = treeViewWindows.SelectedNode?.Tag;
        }
    }
}