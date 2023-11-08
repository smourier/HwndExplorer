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
            noneToolStripMenuItem = new ToolStripMenuItem();
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
            menuStripMain.Size = new Size(1407, 24);
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
            orderWindowsByToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { noneToolStripMenuItem, handleToolStripMenuItem, classNameToolStripMenuItem, titleToolStripMenuItem, toolStripSeparator1, ascendingToolStripMenuItem, descendingToolStripMenuItem });
            orderWindowsByToolStripMenuItem.Name = "orderWindowsByToolStripMenuItem";
            orderWindowsByToolStripMenuItem.Size = new Size(172, 22);
            orderWindowsByToolStripMenuItem.Text = "Order Windows By";
            // 
            // noneToolStripMenuItem
            // 
            noneToolStripMenuItem.Checked = true;
            noneToolStripMenuItem.CheckState = CheckState.Checked;
            noneToolStripMenuItem.Name = "noneToolStripMenuItem";
            noneToolStripMenuItem.Size = new Size(136, 22);
            noneToolStripMenuItem.Text = "(None)";
            noneToolStripMenuItem.Click += noneToolStripMenuItem_Click;
            // 
            // handleToolStripMenuItem
            // 
            handleToolStripMenuItem.CheckOnClick = true;
            handleToolStripMenuItem.Name = "handleToolStripMenuItem";
            handleToolStripMenuItem.Size = new Size(136, 22);
            handleToolStripMenuItem.Text = "Handle";
            handleToolStripMenuItem.Click += handleToolStripMenuItem_Click;
            // 
            // classNameToolStripMenuItem
            // 
            classNameToolStripMenuItem.CheckOnClick = true;
            classNameToolStripMenuItem.Name = "classNameToolStripMenuItem";
            classNameToolStripMenuItem.Size = new Size(136, 22);
            classNameToolStripMenuItem.Text = "Class Name";
            classNameToolStripMenuItem.Click += classNameToolStripMenuItem_Click;
            // 
            // titleToolStripMenuItem
            // 
            titleToolStripMenuItem.CheckOnClick = true;
            titleToolStripMenuItem.Name = "titleToolStripMenuItem";
            titleToolStripMenuItem.Size = new Size(136, 22);
            titleToolStripMenuItem.Text = "Title";
            titleToolStripMenuItem.Click += titleToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(133, 6);
            // 
            // ascendingToolStripMenuItem
            // 
            ascendingToolStripMenuItem.Checked = true;
            ascendingToolStripMenuItem.CheckOnClick = true;
            ascendingToolStripMenuItem.CheckState = CheckState.Checked;
            ascendingToolStripMenuItem.Enabled = false;
            ascendingToolStripMenuItem.Name = "ascendingToolStripMenuItem";
            ascendingToolStripMenuItem.Size = new Size(136, 22);
            ascendingToolStripMenuItem.Text = "Ascending";
            ascendingToolStripMenuItem.Click += ascendingToolStripMenuItem_Click;
            // 
            // descendingToolStripMenuItem
            // 
            descendingToolStripMenuItem.CheckOnClick = true;
            descendingToolStripMenuItem.Enabled = false;
            descendingToolStripMenuItem.Name = "descendingToolStripMenuItem";
            descendingToolStripMenuItem.Size = new Size(136, 22);
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
            splitContainerMain.Size = new Size(1407, 675);
            splitContainerMain.SplitterDistance = 742;
            splitContainerMain.TabIndex = 1;
            splitContainerMain.SplitterMoved += splitContainerMain_SplitterMoved;
            // 
            // propertyGridMain
            // 
            propertyGridMain.Dock = DockStyle.Fill;
            propertyGridMain.HelpVisible = false;
            propertyGridMain.Location = new Point(0, 0);
            propertyGridMain.Name = "propertyGridMain";
            propertyGridMain.PropertySort = PropertySort.Alphabetical;
            propertyGridMain.Size = new Size(661, 675);
            propertyGridMain.TabIndex = 0;
            propertyGridMain.ToolbarVisible = false;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1407, 699);
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
        private ToolStripMenuItem noneToolStripMenuItem;
    }
}