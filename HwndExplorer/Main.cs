using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using HwndExplorer.Utilities;
using ListTreeView;

namespace HwndExplorer
{
    public partial class Main : Form
    {
        private readonly ListTreeViewControl _ltv = new();

        public Main()
        {
            InitializeComponent();
            Icon = Resources.HwndExplorer;
            splitContainerMain.Panel1.Controls.Add(_ltv);
            _ltv.Dock = DockStyle.Fill;
            _ltv.Font = new Font("Consolas", 10);
            _ltv.AddColumn("Hwnd").Width = 100;
            _ltv.AddColumn("Class");
            _ltv.AddColumn("Title");
            _ltv.AddColumn("Process");
            _ltv.SelectionMode = SelectionMode.One;
            _ltv.RowSelectedBrush = Brushes.LightGray;
            _ltv.SelectionChanged += OnSelectionChanged;
            _ltv.RowExpanded += OnRowExpanded;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => Close();
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e) => this.ShowMessage(Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyTitleAttribute>()!.Title + " - " + (IntPtr.Size == 4 ? "32" : "64") + "-bit" + Environment.NewLine + "Copyright (C) 2021-" + DateTime.Now.Year + " Simon Mourier. All rights reserved.");

        private IEnumerable<Win32Window> EnumerateWindows(Win32Window? parent)
        {
            IEnumerable<Win32Window> e;

            if (parent != null)
            {
                e = parent.ChildWindows;
            }
            else
            {
                e = Win32Window.TopLevelWindows;
            }

            Func<Win32Window, object>? f = null;

            if (handleToolStripMenuItem.Checked)
            {
                f = w => w.Handle;
            }
            else if (classNameToolStripMenuItem.Checked)
            {
                f = w => w.ClassName;
            }
            else if (titleToolStripMenuItem.Checked)
            {
                f = w => w.Text;
            }

            if (f != null)
            {
                if (ascendingToolStripMenuItem.Checked)
                {
                    e = e.OrderBy(f);
                }
                else
                {
                    e = e.OrderByDescending(f);
                }
            }
            return e;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            LoadTopLevelWindows();
        }

        private void LoadTopLevelWindows()
        {
            foreach (var window in EnumerateWindows(null))
            {
                AddWindow(window, null);
            }
        }

        private void AddWindow(Win32Window window, Row? parent)
        {
            Row row;
            if (parent == null)
            {
                row = _ltv.AddRow();
            }
            else
            {
                row = parent.AddChildRow();
            }

            row.AddCell(window.HandleAsHex);
            row.AddCell(window.ClassName);
            row.AddCell(window.Text);
            row.AddCell(window.Process.ProcessName);
            row.Tag = new Model { Window = window };
            if (window.ChildWindows.Any())
            {
                row.IsExpandable = true;
            }
        }

        private void OnSelectionChanged(object? sender, EventArgs e)
        {
            propertyGridMain.SelectedObject = (_ltv.SelectedRows.FirstOrDefault()?.Tag as Model)?.Window;
        }

        private void OnRowExpanded(object? sender, RowExpandedEventArgs e)
        {
            if (e.Row.Tag is not Model model || model.Loaded)
                return;

            foreach (var window in EnumerateWindows(model.Window))
            {
                AddWindow(window, e.Row);
            }
        }

        private sealed class Model
        {
            public bool Loaded;
            public required Win32Window Window { get; init; }
        }

        private void UpdateControls()
        {
            _ltv.Rows.Clear();
            LoadTopLevelWindows();
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            noneToolStripMenuItem.Checked = true;
            handleToolStripMenuItem.Checked = false;
            titleToolStripMenuItem.Checked = false;
            classNameToolStripMenuItem.Checked = false;

            descendingToolStripMenuItem.Enabled = false;
            ascendingToolStripMenuItem.Enabled = false;
            UpdateControls();
        }

        private void handleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleToolStripMenuItem.Checked = true;
            noneToolStripMenuItem.Checked = false;
            titleToolStripMenuItem.Checked = false;
            classNameToolStripMenuItem.Checked = false;

            descendingToolStripMenuItem.Enabled = true;
            ascendingToolStripMenuItem.Enabled = true;
            UpdateControls();
        }

        private void classNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            classNameToolStripMenuItem.Checked = true;
            noneToolStripMenuItem.Checked = false;
            titleToolStripMenuItem.Checked = false;
            handleToolStripMenuItem.Checked = false;

            descendingToolStripMenuItem.Enabled = true;
            ascendingToolStripMenuItem.Enabled = true;
            UpdateControls();
        }

        private void titleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            titleToolStripMenuItem.Checked = true;
            noneToolStripMenuItem.Checked = false;
            classNameToolStripMenuItem.Checked = false;
            handleToolStripMenuItem.Checked = false;

            descendingToolStripMenuItem.Enabled = true;
            ascendingToolStripMenuItem.Enabled = true;
            UpdateControls();
        }

        private void ascendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            descendingToolStripMenuItem.Checked = !ascendingToolStripMenuItem.Checked;
            UpdateControls();
        }

        private void descendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ascendingToolStripMenuItem.Checked = !descendingToolStripMenuItem.Checked;
            UpdateControls();
        }

        private void splitContainerMain_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }
    }
}