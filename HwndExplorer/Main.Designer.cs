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
            viewToolStripMenuItem = new ToolStripMenuItem();
            orderWindowsByToolStripMenuItem = new ToolStripMenuItem();
            handleToolStripMenuItem = new ToolStripMenuItem();
            classNameToolStripMenuItem = new ToolStripMenuItem();
            titleToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            ascendingToolStripMenuItem = new ToolStripMenuItem();
            descendingToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            splitContainerMain = new SplitContainer();
            propertyGridMain = new PropertyGrid();
            menuStripMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            SuspendLayout();
            // 
            // menuStripMain
            // 
            menuStripMain.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, viewToolStripMenuItem, helpToolStripMenuItem });
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
            exitToolStripMenuItem.Size = new Size(135, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { orderWindowsByToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "&View";
            // 
            // orderWindowsByToolStripMenuItem
            // 
            orderWindowsByToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { handleToolStripMenuItem, classNameToolStripMenuItem, titleToolStripMenuItem, toolStripSeparator1, ascendingToolStripMenuItem, descendingToolStripMenuItem });
            orderWindowsByToolStripMenuItem.Name = "orderWindowsByToolStripMenuItem";
            orderWindowsByToolStripMenuItem.Size = new Size(180, 22);
            orderWindowsByToolStripMenuItem.Text = "Order Windows By";
            // 
            // handleToolStripMenuItem
            // 
            handleToolStripMenuItem.CheckOnClick = true;
            handleToolStripMenuItem.Name = "handleToolStripMenuItem";
            handleToolStripMenuItem.Size = new Size(180, 22);
            handleToolStripMenuItem.Text = "Handle";
            handleToolStripMenuItem.Click += handleToolStripMenuItem_Click;
            // 
            // classNameToolStripMenuItem
            // 
            classNameToolStripMenuItem.CheckOnClick = true;
            classNameToolStripMenuItem.Name = "classNameToolStripMenuItem";
            classNameToolStripMenuItem.Size = new Size(180, 22);
            classNameToolStripMenuItem.Text = "Class Name";
            classNameToolStripMenuItem.Click += classNameToolStripMenuItem_Click;
            // 
            // titleToolStripMenuItem
            // 
            titleToolStripMenuItem.Checked = true;
            titleToolStripMenuItem.CheckOnClick = true;
            titleToolStripMenuItem.CheckState = CheckState.Checked;
            titleToolStripMenuItem.Name = "titleToolStripMenuItem";
            titleToolStripMenuItem.Size = new Size(180, 22);
            titleToolStripMenuItem.Text = "Title";
            titleToolStripMenuItem.Click += titleToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(177, 6);
            // 
            // ascendingToolStripMenuItem
            // 
            ascendingToolStripMenuItem.Checked = true;
            ascendingToolStripMenuItem.CheckOnClick = true;
            ascendingToolStripMenuItem.CheckState = CheckState.Checked;
            ascendingToolStripMenuItem.Name = "ascendingToolStripMenuItem";
            ascendingToolStripMenuItem.Size = new Size(180, 22);
            ascendingToolStripMenuItem.Text = "Ascending";
            ascendingToolStripMenuItem.Click += ascendingToolStripMenuItem_Click;
            // 
            // descendingToolStripMenuItem
            // 
            descendingToolStripMenuItem.CheckOnClick = true;
            descendingToolStripMenuItem.Name = "descendingToolStripMenuItem";
            descendingToolStripMenuItem.Size = new Size(180, 22);
            descendingToolStripMenuItem.Text = "Descending";
            descendingToolStripMenuItem.Click += descendingToolStripMenuItem_Click;
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
            aboutToolStripMenuItem.Size = new Size(116, 22);
            aboutToolStripMenuItem.Text = "About...";
            aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // splitContainerMain
            // 
            splitContainerMain.Dock = DockStyle.Fill;
            splitContainerMain.Location = new Point(0, 24);
            splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel2
            // 
            splitContainerMain.Panel2.Controls.Add(propertyGridMain);
            splitContainerMain.Size = new Size(1123, 675);
            splitContainerMain.SplitterDistance = 529;
            splitContainerMain.TabIndex = 1;
            // 
            // propertyGridMain
            // 
            propertyGridMain.Dock = DockStyle.Fill;
            propertyGridMain.HelpVisible = false;
            propertyGridMain.Location = new Point(0, 0);
            propertyGridMain.Name = "propertyGridMain";
            propertyGridMain.PropertySort = PropertySort.Alphabetical;
            propertyGridMain.Size = new Size(590, 675);
            propertyGridMain.TabIndex = 0;
            propertyGridMain.ToolbarVisible = false;
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
        private PropertyGrid propertyGridMain;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem orderWindowsByToolStripMenuItem;
        private ToolStripMenuItem handleToolStripMenuItem;
        private ToolStripMenuItem classNameToolStripMenuItem;
        private ToolStripMenuItem titleToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem ascendingToolStripMenuItem;
        private ToolStripMenuItem descendingToolStripMenuItem;
    }
}