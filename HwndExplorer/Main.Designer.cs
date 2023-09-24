using System.Drawing;
using System.Windows.Forms;

namespace HwndExplorer
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStripMain = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            splitContainerMain = new SplitContainer();
            treeViewWindows = new TreeView();
            propertyGridMain = new PropertyGrid();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            menuStripMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            SuspendLayout();
            // 
            // menuStripMain
            // 
            menuStripMain.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, helpToolStripMenuItem });
            menuStripMain.Location = new Point(0, 0);
            menuStripMain.Name = "menuStripMain";
            menuStripMain.Size = new Size(1123, 24);
            menuStripMain.TabIndex = 0;
            menuStripMain.Text = "menuStripMain";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            exitToolStripMenuItem.Size = new Size(180, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
            // 
            // splitContainerMain
            // 
            splitContainerMain.Dock = DockStyle.Fill;
            splitContainerMain.Location = new Point(0, 24);
            splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            splitContainerMain.Panel1.Controls.Add(treeViewWindows);
            // 
            // splitContainerMain.Panel2
            // 
            splitContainerMain.Panel2.Controls.Add(propertyGridMain);
            splitContainerMain.Size = new Size(1123, 675);
            splitContainerMain.SplitterDistance = 374;
            splitContainerMain.TabIndex = 1;
            // 
            // treeViewWindows
            // 
            treeViewWindows.Dock = DockStyle.Fill;
            treeViewWindows.Location = new Point(0, 0);
            treeViewWindows.Name = "treeViewWindows";
            treeViewWindows.Size = new Size(374, 675);
            treeViewWindows.TabIndex = 0;
            // 
            // propertyGridMain
            // 
            propertyGridMain.Dock = DockStyle.Fill;
            propertyGridMain.HelpVisible = false;
            propertyGridMain.Location = new Point(0, 0);
            propertyGridMain.Name = "propertyGridMain";
            propertyGridMain.Size = new Size(745, 675);
            propertyGridMain.TabIndex = 0;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(180, 22);
            aboutToolStripMenuItem.Text = "About...";
            aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1123, 699);
            Controls.Add(splitContainerMain);
            Controls.Add(menuStripMain);
            MainMenuStrip = menuStripMain;
            Name = "Main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Hwnd Explorer";
            menuStripMain.ResumeLayout(false);
            menuStripMain.PerformLayout();
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStripMain;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private SplitContainer splitContainerMain;
        private TreeView treeViewWindows;
        private PropertyGrid propertyGridMain;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
    }
}