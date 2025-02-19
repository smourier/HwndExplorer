//NOSONAR
/*
MIT License

Copyright (c) 2023-2025 Simon Mourier

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms.VisualStyles;
using System.Windows.Forms;
using System;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace ListTreeView
{
	public class Cell(object? value)
	    {
	        public virtual object? Value { get; set; } = value;
	        public string ValueAsString => Value?.ToString() ?? string.Empty;
	
	        public override string ToString() => ValueAsString;
	
	        protected internal virtual void Draw(Graphics graphics, Row row, Column column, Rectangle layout, StringFormat format)
	        {
	            ArgumentNullException.ThrowIfNull(graphics);
	            ArgumentNullException.ThrowIfNull(row);
	            ArgumentNullException.ThrowIfNull(column);
	            ArgumentNullException.ThrowIfNull(format);
	
	            var e = new DrawCellEventArgs(graphics, row, column, this, layout, format);
	            row.Owner?.OnDrawingCell(this, e);
	            if (e.Handled)
	                return;
	
	            if (row.Owner == null)
	                return;
	
	            if (layout.Width == 0 || layout.Height == 0)
	                return;
	
	            layout.X += column.HorizontalPadding;
	            layout.Y += column.VerticalPadding;
	            layout.Width -= column.HorizontalPadding * 2;
	            layout.Height -= column.VerticalPadding * 2;
	
	            // sending width = 0 causes the string to be displayed (!)
	            if (layout.Width == 0 || layout.Height == 0)
	                return;
	
	            graphics.DrawString(Value?.ToString() ?? string.Empty, column.Font ?? row.Owner.Font, column.TextBrush ?? SystemBrushes.ControlText, layout, format);
	            row.Owner?.OnDrawnCell(this, new DrawCellEventArgs(graphics, row, column, this, layout, format));
	        }
	    }
	
	public class Column : INotifyPropertyChanged
	    {
	        private int _width = ListTreeViewControl.DefaultDefaultColumnWidth;
	        private int _minWidth = ListTreeViewControl.DefaultDefaultMinColumnWidth;
	        private int _verticalPadding = ListTreeViewControl.DefaultDefaultColumnPadding;
	        private int _horizontalPadding = ListTreeViewControl.DefaultDefaultColumnPadding;
	        private int _index = -1;
	        private StringAlignment _headerVerticalAlignment = StringAlignment.Center;
	        private StringAlignment _headerHorizontalAlignment = StringAlignment.Center;
	        private StringFormatFlags _headerFormatFlags = StringFormatFlags.FitBlackBox;
	        private StringTrimming _headerTrimming = StringTrimming.None;
	        private StringAlignment _verticalAlignment = StringAlignment.Center;
	        private StringAlignment _horizontalAlignment = StringAlignment.Center;
	        private StringFormatFlags _formatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.LineLimit;
	        private StringTrimming _trimming = StringTrimming.EllipsisCharacter;
	        private Font? _font;
	        private Font? _headerFont;
	        private Brush? _textBrush;
	        private Brush? _headerTextBrush;
	        private ListTreeViewControl? _owner;
	        private Lazy<Rectangle?> _bounds;
	
	        public event PropertyChangedEventHandler? PropertyChanged;
	
	        public Column(string text)
	        {
	            ArgumentNullException.ThrowIfNull(text);
	            Text = text;
	            ResetBounds();
	        }
	
	        public string Text { get; }
	        public virtual object? Tag { get; set; }
	        public virtual Rectangle? Bounds => _bounds.Value;
	
	        public virtual Rectangle? HeaderBounds
	        {
	            get
	            {
	                if (Owner == null || !Bounds.HasValue)
	                    return null;
	
	                var bounds = Bounds.Value;
	                bounds.Height = Owner.HeaderHeight;
	                return bounds;
	            }
	        }
	
	        public ListTreeViewControl? Owner
	        {
	            get => _owner;
	            internal set
	            {
	                if (_owner == value)
	                    return;
	
	                _owner = value;
	                OnPropertyChanged();
	                ResetBounds();
	            }
	        }
	
	        public int Index
	        {
	            get => _index;
	            internal set
	            {
	                if (_index == value)
	                    return;
	
	                _index = value;
	                OnPropertyChanged();
	                ResetBounds();
	            }
	        }
	
	        public virtual int Width
	        {
	            get => _width;
	            set
	            {
	                if (_width == value)
	                    return;
	
	                ArgumentOutOfRangeException.ThrowIfNegative(value);
	
	                _width = Math.Max(MinWidth, value);
	                OnPropertyChanged();
	                ResetBounds();
	            }
	        }
	
	        public virtual int MinWidth
	        {
	            get => _minWidth;
	            set
	            {
	                if (_minWidth == value)
	                    return;
	
	                ArgumentOutOfRangeException.ThrowIfNegative(value);
	
	                _minWidth = value;
	                OnPropertyChanged();
	                ResetBounds();
	            }
	        }
	
	        public virtual int VerticalPadding
	        {
	            get => _verticalPadding;
	            set
	            {
	                if (_verticalPadding == value)
	                    return;
	
	                ArgumentOutOfRangeException.ThrowIfNegative(value);
	
	                _verticalPadding = value;
	                OnPropertyChanged();
	            }
	        }
	
	        public virtual int HorizontalPadding
	        {
	            get => _horizontalPadding;
	            set
	            {
	                if (_horizontalPadding == value)
	                    return;
	
	                ArgumentOutOfRangeException.ThrowIfNegative(value);
	
	                _horizontalPadding = value;
	                OnPropertyChanged();
	            }
	        }
	
	        public virtual Brush? HeaderTextBrush
	        {
	            get => _headerTextBrush;
	            set
	            {
	                if (_headerTextBrush == value)
	                    return;
	
	                try
	                {
	                    _headerTextBrush?.Dispose();
	                }
	                catch
	                {
	                    // continue
	                }
	
	                _headerTextBrush = value;
	                OnPropertyChanged();
	                var bounds = HeaderBounds;
	                if (bounds.HasValue && Owner != null)
	                {
	                    Owner.Invalidate(bounds.Value);
	                }
	            }
	        }
	
	        public virtual Font? HeaderFont
	        {
	            get => _headerFont;
	            set
	            {
	                if (_headerFont == value)
	                    return;
	
	                try
	                {
	                    _headerFont?.Dispose();
	                }
	                catch
	                {
	                    // continue
	                }
	
	                _headerFont = value;
	                OnPropertyChanged();
	                var bounds = HeaderBounds;
	                if (bounds.HasValue && Owner != null)
	                {
	                    Owner.Invalidate(bounds.Value);
	                }
	            }
	        }
	
	        public virtual Brush? TextBrush
	        {
	            get => _textBrush;
	            set
	            {
	                if (_textBrush == value)
	                    return;
	
	                try
	                {
	                    _textBrush?.Dispose();
	                }
	                catch
	                {
	                    // continue
	                }
	
	                _textBrush = value;
	                OnPropertyChanged();
	                var bounds = Bounds;
	                if (bounds.HasValue && Owner != null)
	                {
	                    Owner.Invalidate(bounds.Value);
	                }
	            }
	        }
	
	        public virtual Font? Font
	        {
	            get => _font;
	            set
	            {
	                if (_font == value)
	                    return;
	
	                try
	                {
	                    _font?.Dispose();
	                }
	                catch
	                {
	                    // continue
	                }
	
	                _font = value;
	                OnPropertyChanged();
	                var bounds = Bounds;
	                if (bounds.HasValue && Owner != null)
	                {
	                    Owner.Invalidate(bounds.Value);
	                }
	            }
	        }
	
	        public virtual StringAlignment HeaderVerticalAlignment
	        {
	            get => _headerVerticalAlignment;
	            set
	            {
	                if (_headerVerticalAlignment == value)
	                    return;
	
	                _headerVerticalAlignment = value;
	                OnPropertyChanged();
	                var bounds = HeaderBounds;
	                if (bounds.HasValue && Owner != null)
	                {
	                    Owner.Invalidate(bounds.Value);
	                }
	            }
	        }
	
	        public virtual StringAlignment HeaderHorizontalAlignment
	        {
	            get => _headerHorizontalAlignment;
	            set
	            {
	                if (_headerHorizontalAlignment == value)
	                    return;
	
	                _headerHorizontalAlignment = value;
	                OnPropertyChanged();
	                var bounds = HeaderBounds;
	                if (bounds.HasValue && Owner != null)
	                {
	                    Owner.Invalidate(bounds.Value);
	                }
	            }
	        }
	
	        public virtual StringFormatFlags HeaderFormatFlags
	        {
	            get => _headerFormatFlags;
	            set
	            {
	                if (_headerFormatFlags == value)
	                    return;
	
	                _headerFormatFlags = value;
	                OnPropertyChanged();
	                var bounds = HeaderBounds;
	                if (bounds.HasValue && Owner != null)
	                {
	                    Owner.Invalidate(bounds.Value);
	                }
	            }
	        }
	
	        public virtual StringTrimming HeaderTrimming
	        {
	            get => _headerTrimming;
	            set
	            {
	                if (_headerTrimming == value)
	                    return;
	
	                _headerTrimming = value;
	                OnPropertyChanged();
	                var bounds = HeaderBounds;
	                if (bounds.HasValue && Owner != null)
	                {
	                    Owner.Invalidate(bounds.Value);
	                }
	            }
	        }
	
	        public virtual StringAlignment VerticalAlignment
	        {
	            get => _verticalAlignment;
	            set
	            {
	                if (_verticalAlignment == value)
	                    return;
	
	                _verticalAlignment = value;
	                OnPropertyChanged();
	                var bounds = Bounds;
	                if (bounds.HasValue && Owner != null)
	                {
	                    Owner.Invalidate(bounds.Value);
	                }
	            }
	        }
	
	        public virtual StringAlignment HorizontalAlignment
	        {
	            get => _horizontalAlignment;
	            set
	            {
	                if (_horizontalAlignment == value)
	                    return;
	
	                _horizontalAlignment = value;
	                OnPropertyChanged();
	                var bounds = Bounds;
	                if (bounds.HasValue && Owner != null)
	                {
	                    Owner.Invalidate(bounds.Value);
	                }
	            }
	        }
	
	        public virtual StringFormatFlags FormatFlags
	        {
	            get => _formatFlags;
	            set
	            {
	                if (_formatFlags == value)
	                    return;
	
	                _formatFlags = value;
	                OnPropertyChanged();
	                var bounds = Bounds;
	                if (bounds.HasValue && Owner != null)
	                {
	                    Owner.Invalidate(bounds.Value);
	                }
	            }
	        }
	
	        public virtual StringTrimming Trimming
	        {
	            get => _trimming;
	            set
	            {
	                if (_trimming == value)
	                    return;
	
	                _trimming = value;
	                OnPropertyChanged();
	                var bounds = Bounds;
	                if (bounds.HasValue && Owner != null)
	                {
	                    Owner.Invalidate(bounds.Value);
	                }
	            }
	        }
	
	        public IEnumerable<Column> FollowingColumns
	        {
	            get
	            {
	                if (Index < 0)
	                    yield break;
	
	                var nextIndex = Index + 1;
	                if (Owner != null)
	                {
	                    for (var i = nextIndex; i < Owner.Columns.Count; i++)
	                    {
	                        yield return Owner.Columns[i];
	                    }
	                }
	            }
	        }
	
	        [MemberNotNull(nameof(_bounds))]
	        protected internal void ResetBounds()
	        {
	            _bounds = new Lazy<Rectangle?>(ComputeBounds);
	
	            // TODO: needs to be tuned to what's needed
	            Owner?.Invalidate();
	            OnPropertyChanged(nameof(Bounds));
	            OnPropertyChanged(nameof(HeaderBounds));
	        }
	
	        protected virtual Rectangle? ComputeBounds()
	        {
	            if (Owner == null || Index < 0 || Index >= Owner.Columns.Count)
	                return null;
	
	            var left = Owner.LineWidth;
	            for (var i = 0; i < Index; i++)
	            {
	                left += Owner.Columns[i].Width + Owner.LineWidth;
	            }
	            return new Rectangle(left, Owner.LineWidth, Width, Owner.Height - Owner.LineWidth);
	        }
	
	        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => OnPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
	        protected virtual void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(sender, e);
	        protected internal virtual void DrawHeader(Graphics graphics)
	        {
	            ArgumentNullException.ThrowIfNull(graphics);
	            if (Owner == null)
	                return;
	
	            var layout = HeaderBounds;
	            if (layout == null)
	                return;
	
	            var format = new StringFormat(StringFormatFlags.FitBlackBox)
	            {
	                LineAlignment = HeaderVerticalAlignment,
	                Alignment = HeaderHorizontalAlignment,
	                Trimming = HeaderTrimming,
	                FormatFlags = HeaderFormatFlags,
	            };
	
	            var e = new DrawColumnEventArgs(graphics, this, layout.Value, format);
	            Owner.OnDrawingColumnHeader(this, e);
	            if (e.Handled)
	                return;
	
	            graphics.DrawString(Text, HeaderFont ?? Owner.Font, HeaderTextBrush ?? SystemBrushes.ControlText, layout.Value, format);
	            Owner.OnDrawnColumnHeader(this, new DrawColumnEventArgs(graphics, this, layout.Value, format));
	        }
	
	        public override string ToString() => Text;
	    }
	
	public class DrawCellEventArgs : DrawEventArgs
	    {
	        public DrawCellEventArgs(Graphics graphics, Row row, Column column, Cell cell, Rectangle layout, StringFormat format)
	            : base(graphics)
	        {
	            ArgumentNullException.ThrowIfNull(graphics);
	            ArgumentNullException.ThrowIfNull(row);
	            ArgumentNullException.ThrowIfNull(column);
	            ArgumentNullException.ThrowIfNull(cell);
	            ArgumentNullException.ThrowIfNull(format);
	            Row = row;
	            Column = column;
	            Cell = cell;
	            Layout = layout;
	            Format = format;
	        }
	
	        public Row Row { get; }
	        public Column Column { get; }
	        public Cell Cell { get; }
	        public Rectangle Layout { get; }
	        public StringFormat Format { get; }
	    }
	
	public class DrawColumnEventArgs : DrawEventArgs
	    {
	        public DrawColumnEventArgs(Graphics graphics, Column column, Rectangle layout, StringFormat format)
	            : base(graphics)
	        {
	            ArgumentNullException.ThrowIfNull(graphics);
	            ArgumentNullException.ThrowIfNull(column);
	            ArgumentNullException.ThrowIfNull(format);
	            Column = column;
	            Layout = layout;
	            Format = format;
	        }
	
	        public Column Column { get; }
	        public Rectangle Layout { get; }
	        public StringFormat Format { get; }
	    }
	
	public class DrawEventArgs : HandledEventArgs
	    {
	        public DrawEventArgs(Graphics graphics)
	        {
	            ArgumentNullException.ThrowIfNull(graphics);
	            Graphics = graphics;
	        }
	
	        public Graphics Graphics { get; }
	    }
	
	public class DrawRowEventArgs : DrawEventArgs
	    {
	        public DrawRowEventArgs(Graphics graphics, Row row)
	            : base(graphics)
	        {
	            ArgumentNullException.ThrowIfNull(graphics);
	            ArgumentNullException.ThrowIfNull(row);
	            Row = row;
	        }
	
	        public Row Row { get; }
	        public virtual bool DrawExpander { get; set; } = true;
	        public virtual bool DrawSelection { get; set; } = true;
	        public virtual int FirstCellRightOffset { get; set; }
	    }
	
	public class ListTreeViewControl : Control, INotifyPropertyChanged
	    {
	        public static int DefaultDefaultColumnWidth { get; set; } = 200;
	        public static int DefaultDefaultMinColumnWidth { get; set; } = 40;
	        public static int DefaultDefaultColumnPadding { get; set; } = 5;
	        public static int DefaultRowHeight { get; set; } = 26;
	        public static int DefaultRowOverhang { get; set; } = 11;
	        public static int DefaultLineWidth { get; set; } = 1;
	        public static int MouseTolerance { get; set; } = 4;
	        public static Color DefaultLineColor { get; set; } = SystemColors.Control;
	
	        private int _rowHeight;
	        private int _headerHeight;
	        private int _rowOverhang;
	        private int _extentHeight;
	        private int _defaultColumnWidth;
	        private int _defaultMinColumnWidth;
	        internal Pen _linePen;
	        private Brush? _rowSelectedBrush;
	        private Brush? _backColorBrush;
	        private SelectionMode _selectionMode;
	        private bool _drawColumnsHeader;
	        private bool _drawLastColumnRightLine;
	        private bool _drawLastRowBottomLine;
	        private Row? _rowUnderMouse;
	        private Row? _selectedRow; // always null with multiple selection
	        private MovingColumnHeader? _movingColumnHeader;
	        private int _firstVisibleRowIndex;
	        private readonly List<Row> _rowsCache = [];
	
	        public event PropertyChangedEventHandler? PropertyChanged;
	        public event EventHandler<DrawRowEventArgs>? DrawingRow;
	        public event EventHandler<DrawRowEventArgs>? DrawnRow;
	        public event EventHandler<DrawColumnEventArgs>? DrawingColumnHeader;
	        public event EventHandler<DrawColumnEventArgs>? DrawnColumnHeader;
	        public event EventHandler<DrawCellEventArgs>? DrawingCell;
	        public event EventHandler<DrawCellEventArgs>? DrawnCell;
	        public event EventHandler<EventArgs>? SelectionChanged;
	        public event EventHandler<RowExpandedEventArgs>? RowExpanded;
	        public event EventHandler<RowCollapsedEventArgs>? RowCollapsed;
	
	        public ListTreeViewControl()
	        {
	            DefaultColumnWidth = DefaultDefaultColumnWidth;
	            DefaultMinColumnWidth = DefaultDefaultMinColumnWidth;
	            RowHeight = DefaultRowHeight;
	            HeaderHeight = RowHeight;
	            RowOverhang = DefaultRowOverhang;
	            LineWidth = DefaultLineWidth;
	            LineColor = DefaultLineColor;
	            DrawColumnsHeader = true;
	            DrawLastColumnRightLine = true;
	            DrawLastRowBottomLine = true;
	            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
	            HorizontalScrollBar.Visible = true;
	            HorizontalScrollBar.ValueChanged += (s, e) => OnHorizontalScrollBarValueChanged();
	            Controls.Add(HorizontalScrollBar);
	            VerticalScrollBar.Visible = true;
	            VerticalScrollBar.ValueChanged += (s, e) => OnVerticalScrollBarValueChanged();
	            Controls.Add(VerticalScrollBar);
	            UpdateScrollBars();
	            BackColor = Color.White;
	
	            Columns.CollectionChanged += (s, e) => OnColumnsCollectionChanged(e);
	            Rows.CollectionChanged += (s, e) => OnRowsCollectionChanged(e);
	        }
	
	        protected HScrollBar HorizontalScrollBar { get; } = new();
	        protected VScrollBar VerticalScrollBar { get; } = new();
	
	        public ObservableList<Column> Columns { get; } = [];
	        public ObservableList<Row> Rows { get; } = [];
	
	        public virtual SelectionMode SelectionMode
	        {
	            get => _selectionMode;
	            set
	            {
	                if (_selectionMode == value)
	                    return;
	
	                var old = _selectionMode;
	                _selectionMode = value;
	                OnPropertyChanged();
	
	                if (_selectionMode == SelectionMode.None)
	                {
	                    if (_selectedRow != null)
	                    {
	                        _selectedRow.IsSelected = false;
	                    }
	
	                    if (old == SelectionMode.MultiSimple || old == SelectionMode.MultiExtended)
	                    {
	                        foreach (var row in AllRows)
	                        {
	                            row.IsSelected = false;
	                        }
	                    }
	                }
	            }
	        }
	
	        public virtual bool DrawColumnsHeader
	        {
	            get => _drawColumnsHeader;
	            set
	            {
	                if (_drawColumnsHeader == value)
	                    return;
	
	                _drawColumnsHeader = value;
	                OnPropertyChanged();
	            }
	        }
	
	        public virtual bool DrawLastColumnRightLine
	        {
	            get => _drawLastColumnRightLine;
	            set
	            {
	                if (_drawLastColumnRightLine == value)
	                    return;
	
	                _drawLastColumnRightLine = value;
	                OnPropertyChanged();
	            }
	        }
	
	        public virtual bool DrawLastRowBottomLine
	        {
	            get => _drawLastRowBottomLine;
	            set
	            {
	                if (_drawLastRowBottomLine == value)
	                    return;
	
	                _drawLastRowBottomLine = value;
	                OnPropertyChanged();
	            }
	        }
	
	        public virtual int VerticalOffset
	        {
	            get => VerticalScrollBar.Value;
	            set
	            {
	                if (VerticalScrollBar.Value == value)
	                    return;
	
	                value = Math.Max(0, Math.Min(value, ExtentHeight - Height));
	                VerticalScrollBar.Value = value;
	                OnPropertyChanged();
	            }
	        }
	
	        public virtual int HorizontalOffset
	        {
	            get => HorizontalScrollBar.Value;
	            set
	            {
	                if (HorizontalScrollBar.Value == value)
	                    return;
	
	                value = Math.Max(0, Math.Min(value, ExtentWidth - Width));
	                HorizontalScrollBar.Value = value;
	                OnPropertyChanged();
	            }
	        }
	
	        public virtual int RowHeight
	        {
	            get => _rowHeight;
	            set
	            {
	                if (_rowHeight == value)
	                    return;
	
	                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
	
	                _rowHeight = value;
	                OnPropertyChanged();
	            }
	        }
	
	        private int FinalHeaderHeight => DrawColumnsHeader ? HeaderHeight + 2 * LineWidth : 0;
	        public virtual int HeaderHeight
	        {
	            get => _headerHeight;
	            set
	            {
	                if (_headerHeight == value)
	                    return;
	
	                ArgumentOutOfRangeException.ThrowIfNegative(value);
	
	                _headerHeight = value;
	                OnPropertyChanged();
	            }
	        }
	
	        public virtual int RowOverhang
	        {
	            get => _rowOverhang;
	            set
	            {
	                if (_rowOverhang == value)
	                    return;
	
	                ArgumentOutOfRangeException.ThrowIfNegative(value);
	
	                _rowOverhang = value;
	                OnPropertyChanged();
	            }
	        }
	
	        public virtual int DefaultColumnWidth
	        {
	            get => _defaultColumnWidth;
	            set
	            {
	                if (_defaultColumnWidth == value)
	                    return;
	
	                ArgumentOutOfRangeException.ThrowIfNegative(value);
	
	                _defaultColumnWidth = value;
	                OnPropertyChanged();
	            }
	        }
	
	        public virtual int DefaultMinColumnWidth
	        {
	            get => _defaultMinColumnWidth;
	            set
	            {
	                if (_defaultMinColumnWidth == value)
	                    return;
	
	                ArgumentOutOfRangeException.ThrowIfNegative(value);
	
	                _defaultMinColumnWidth = value;
	                OnPropertyChanged();
	            }
	        }
	
	        public virtual Brush? RowSelectedBrush
	        {
	            get => _rowSelectedBrush;
	            set
	            {
	                if (_rowSelectedBrush == value)
	                    return;
	
	                try
	                {
	                    _rowSelectedBrush?.Dispose();
	                }
	                catch
	                {
	                    // continue
	                }
	
	                _rowSelectedBrush = value;
	                OnPropertyChanged();
	            }
	        }
	
	        public virtual Color LineColor
	        {
	            get => _linePen?.Color ?? DefaultLineColor;
	            [MemberNotNull(nameof(_linePen))]
	            set
	            {
	                if (_linePen != null && _linePen.Color == value)
	                    return;
	
	                var width = LineWidth;
	
	                _linePen?.Dispose();
	                _linePen = new Pen(value, width);
	                OnPropertyChanged();
	            }
	        }
	
	        public virtual int LineWidth
	        {
	            get => (int)(_linePen?.Width ?? DefaultLineWidth);
	            [MemberNotNull(nameof(_linePen))]
	            set
	            {
	                if (_linePen != null && _linePen.Width == value)
	                    return;
	
	                var color = LineColor;
	                _linePen?.Dispose();
	                _linePen = new Pen(color, value);
	                OnPropertyChanged();
	            }
	        }
	
	        public virtual int ExtentWidth => Columns.Count == 0 ? 0 : Columns[^1].Bounds!.Value.Right;
	        public virtual int ExtentHeight
	        {
	            get => _extentHeight;
	            protected set
	            {
	                if (_extentHeight == value)
	                    return;
	
	                ArgumentOutOfRangeException.ThrowIfNegative(value);
	
	                _extentHeight = value;
	                OnPropertyChanged();
	                UpdateScrollBarsVisibility();
	            }
	        }
	
	        public override Rectangle DisplayRectangle
	        {
	            get
	            {
	                var rc = ClientRectangle;
	                if (HorizontalScrollBar is not null && HorizontalScrollBar.Visible)
	                {
	                    rc.Height -= HorizontalScrollBar.Height;
	                }
	
	                if (VerticalScrollBar is not null && VerticalScrollBar.Visible)
	                {
	                    rc.Width -= VerticalScrollBar.Width;
	                }
	
	                return rc;
	            }
	        }
	
	        public IEnumerable<Row> AllRows
	        {
	            get
	            {
	                foreach (var row in Rows)
	                {
	                    yield return row;
	                    foreach (var child in row.AllChildRows)
	                    {
	                        yield return child;
	                    }
	                }
	            }
	        }
	
	        public IEnumerable<Row> VisibleRows
	        {
	            get
	            {
	                if (_rowsCache.Count <= _firstVisibleRowIndex)
	                    return [];
	
	                return _rowsCache.Skip(_firstVisibleRowIndex);
	            }
	        }
	
	        public IEnumerable<Row> SelectedRows
	        {
	            get
	            {
	                if (_selectedRow != null)
	                    yield return _selectedRow;
	
	                // TODO: improve using cache (more memory)
	                foreach (var row in AllRows)
	                {
	                    yield return row;
	                }
	            }
	        }
	
	        public virtual Row AddRow()
	        {
	            var row = new Row();
	            Rows.Add(row);
	            return row;
	        }
	
	        public virtual Column AddColumn(string text)
	        {
	            var column = new Column(text);
	            Columns.Add(column);
	            return column;
	        }
	
	        protected override void Dispose(bool disposing)
	        {
	            base.Dispose(disposing);
	            _linePen?.Dispose();
	            _rowSelectedBrush?.Dispose();
	            _backColorBrush?.Dispose();
	        }
	
	        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
	        {
	            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
	            RowHeight = RowHeight * deviceDpiNew / deviceDpiOld;
	            HeaderHeight = HeaderHeight * deviceDpiNew / deviceDpiOld;
	            LineWidth = LineWidth * deviceDpiNew / deviceDpiOld;
	            RowOverhang = RowOverhang * deviceDpiNew / deviceDpiOld;
	            DefaultColumnWidth = DefaultColumnWidth * deviceDpiNew / deviceDpiOld;
	            DefaultDefaultMinColumnWidth = DefaultDefaultMinColumnWidth * deviceDpiNew / deviceDpiOld;
	
	            foreach (var column in Columns)
	            {
	                column.Width = column.Width * deviceDpiNew / deviceDpiOld;
	                column.MinWidth = column.MinWidth * deviceDpiNew / deviceDpiOld;
	                column.HorizontalPadding = column.HorizontalPadding * deviceDpiNew / deviceDpiOld;
	                column.VerticalPadding = column.VerticalPadding * deviceDpiNew / deviceDpiOld;
	            }
	
	            ClearCache();
	        }
	
	        private void ClearCache()
	        {
	            _firstVisibleRowIndex = 0;
	            _rowsCache.Clear();
	
	            foreach (var row in AllRows)
	            {
	                row.FirstCellBounds = null;
	            }
	
	            // TODO: currently, if we change dynamically we lose position
	            VerticalOffset = 0;
	            HorizontalOffset = 0;
	        }
	
	        protected override void OnDpiChangedAfterParent(EventArgs e)
	        {
	            base.OnDpiChangedAfterParent(e);
	            if (Rows.Count > 0)
	            {
	                Rows[0].ComputeFirstCellBounds(true);
	            }
	        }
	
	        protected override void OnMouseWheel(MouseEventArgs e)
	        {
	            base.OnMouseWheel(e);
	            var scrollLines = SystemInformation.MouseWheelScrollLines;
	            if (scrollLines > 0)
	            {
	                // lines
	                var offset = (RowHeight + LineWidth) * e.Delta / 120 * SystemInformation.MouseWheelScrollLines;
	                VerticalOffset -= offset;
	            }
	            else if (scrollLines < 0)
	            {
	                // pages
	            }
	        }
	
	        protected override void OnMouseMove(MouseEventArgs e)
	        {
	            base.OnMouseMove(e);
	
	            if (Capture)
	            {
	                _movingColumnHeader?.MouseMove(e.Location);
	                return;
	            }
	
	            var left = GetSplitColumn(e.Location);
	            Cursor = left != null ? Cursors.VSplit : Cursors.Default;
	
	            _rowUnderMouse = GetRow(e.Location);
	            if (_rowUnderMouse != null && _rowUnderMouse.IsExpandable == true && _rowUnderMouse.ExpanderBoundsWithOffsets?.Contains(e.Location) == true)
	            {
	                Cursor = Cursors.Hand;
	            }
	        }
	
	        protected override void OnMouseDown(MouseEventArgs e)
	        {
	            base.OnMouseDown(e);
	
	            if (e.Button == MouseButtons.Left)
	            {
	                var left = GetSplitColumn(e.Location);
	                if (left != null)
	                {
	                    Capture = true;
	                    _movingColumnHeader = new MovingColumnHeader(left, e.Location);
	                    return;
	                }
	            }
	
	            if (_rowUnderMouse != null)
	            {
	                if (_rowUnderMouse.IsExpandable == true && _rowUnderMouse.ExpanderBoundsWithOffsets?.Contains(e.Location) == true)
	                {
	                    _rowUnderMouse.IsExpanded = !_rowUnderMouse.IsExpanded;
	                }
	                else if (SelectionMode != SelectionMode.None)
	                {
	                    _rowUnderMouse.IsSelected = !_rowUnderMouse.IsSelected;
	                    OnSelectionChanged(this, EventArgs.Empty);
	                    if (SelectionMode == SelectionMode.One)
	                    {
	                        if (_selectedRow != null && _selectedRow != _rowUnderMouse)
	                        {
	                            _selectedRow.IsSelected = false;
	                        }
	
	                        if (_rowUnderMouse.IsSelected)
	                        {
	                            _selectedRow = _rowUnderMouse;
	                        }
	                    }
	                }
	            }
	        }
	
	        protected override void OnMouseUp(MouseEventArgs e)
	        {
	            base.OnMouseUp(e);
	            if (Capture)
	            {
	                if (_movingColumnHeader != null)
	                {
	                    _movingColumnHeader = null;
	                }
	            }
	        }
	
	        protected override void OnPaint(PaintEventArgs e)
	        {
	            ArgumentNullException.ThrowIfNull(e);
	
	            DrawRows(e.Graphics);
	
	            if (DrawColumnsHeader)
	            {
	                DrawHeader(e.Graphics);
	            }
	
	            DrawLines(e.Graphics);
	            DrawScrollBarsCorner(e.Graphics);
	        }
	
	        protected virtual void DrawLines(Graphics graphics)
	        {
	            ArgumentNullException.ThrowIfNull(graphics);
	            if (Columns.Count == 0)
	                return;
	
	            var width = ExtentWidth;
	            // rows
	            graphics.DrawLine(_linePen, new Point(0, 0), new Point(width, 0));
	            for (var i = _firstVisibleRowIndex; i < _rowsCache.Count; i++)
	            {
	                var bounds = _rowsCache[i].FirstCellBounds;
	                if (!bounds.HasValue)
	                    continue;
	
	                var rect = bounds.Value;
	                var top = rect.Top - LineWidth - VerticalOffset;
	
	                // after header if header drawn
	                if (!DrawColumnsHeader || top > FinalHeaderHeight)
	                {
	                    graphics.DrawLine(_linePen, new Point(0, top), new Point(width, top));
	                }
	
	                if (DrawLastRowBottomLine && i == _rowsCache.Count - 1)
	                {
	                    graphics.DrawLine(_linePen, new Point(0, top + LineWidth + RowHeight), new Point(width, top + LineWidth + RowHeight));
	                }
	            }
	
	            // columns
	            graphics.DrawLine(_linePen, new Point(0, 0), new Point(0, ExtentHeight - 1));
	            var x = Columns[0].Width + LineWidth;
	            for (var i = 1; i < Columns.Count; i++)
	            {
	                graphics.DrawLine(_linePen, new Point(x, 0), new Point(x, ExtentHeight - 1));
	                x += Columns[i].Width + LineWidth;
	            }
	
	            if (DrawLastColumnRightLine)
	            {
	                graphics.DrawLine(_linePen, new Point(width, 0), new Point(width, ExtentHeight - 1));
	            }
	
	            if (DrawColumnsHeader)
	            {
	                graphics.DrawLine(_linePen, new Point(0, HeaderHeight + LineWidth), new Point(width, HeaderHeight + LineWidth));
	            }
	        }
	
	        protected virtual void DrawRows(Graphics graphics)
	        {
	            ArgumentNullException.ThrowIfNull(graphics);
	            if (_rowsCache.Count == 0)
	            {
	                if (Rows.Count == 0)
	                    return;
	
	                ensureRow(Rows[0], 0);
	            }
	
	            var index = _firstVisibleRowIndex;
	            var firstRow = _rowsCache[index];
	            ensureRow(firstRow, index++);
	            firstRow.Draw(graphics);
	            foreach (var row in firstRow.ExpandedFollowingRows)
	            {
	                if (!row.FirstCellBounds.HasValue)
	                    break;
	
	                if (row.FirstCellBounds.Value.Top > row.OwnerBottom)
	                    break;
	
	                row.Draw(graphics);
	                ensureRow(row, index++);
	                //Trace.WriteLine("draw row:" + row);
	            }
	
	            if (_rowsCache.Count > index)
	            {
	                _rowsCache.RemoveRange(index, _rowsCache.Count - index);
	            }
	
	            //Trace.WriteLine("draw visible:" + _visibleRows.Count);
	            //Trace.WriteLine("draw first row:" + firstRow);
	            //Trace.WriteLine("draw last row:" + _visibleRows.Last());
	
	            void ensureRow(Row row, int idx)
	            {
	                if (idx >= _rowsCache.Count)
	                {
	                    _rowsCache.Add(row);
	                    return;
	                }
	
	                _rowsCache[idx] = row;
	            }
	        }
	
	        private void UpdateRows()
	        {
	            if (_rowsCache.Count == 0)
	                return;
	
	            var first = _rowsCache[_firstVisibleRowIndex];
	            var last = _rowsCache[^1];
	            if (!first.FirstCellBounds.HasValue)
	                return;
	
	            var firstBounds = first.FirstCellBounds.Value;
	            var pos = firstBounds.Top - (_firstVisibleRowIndex * (RowHeight + LineWidth)) + VerticalOffset - FinalHeaderHeight;
	            var newFirstRowIndex = pos / (RowHeight + LineWidth);
	            var mod = pos % (RowHeight + LineWidth);
	            var diff = newFirstRowIndex - _firstVisibleRowIndex;
	            if (diff > 0 || mod != 0)
	            {
	                if (last.FirstCellBounds.HasValue)
	                {
	                    last.ComputeFollowings(last.FirstCellBounds.Value);
	                }
	                else
	                    throw new NotImplementedException();
	            }
	
	            if (newFirstRowIndex != _firstVisibleRowIndex)
	            {
	                if (newFirstRowIndex < _rowsCache.Count)
	                {
	                    _firstVisibleRowIndex = newFirstRowIndex;
	                }
	                else
	                {
	                    // use last computed bounds
	                    _firstVisibleRowIndex = _rowsCache.Count - 1;
	                }
	            }
	
	            // compute what's needed below
	            if (last.FirstCellBounds.HasValue)
	            {
	                var lastBounds = last.FirstCellBounds.Value;
	                foreach (var following in last.ExpandedFollowingRows)
	                {
	                    if (lastBounds.Bottom >= last.OwnerBottom)
	                        break;
	
	                    lastBounds.Y += RowHeight + LineWidth;
	                    following.FirstCellBounds = lastBounds;
	                }
	            }
	
	            Invalidate();
	        }
	
	        protected virtual void DrawHeader(Graphics graphics)
	        {
	            ArgumentNullException.ThrowIfNull(graphics);
	            _backColorBrush ??= new SolidBrush(BackColor);
	            graphics.FillRectangle(_backColorBrush, new Rectangle(LineWidth, LineWidth, Width - 2 * LineWidth, HeaderHeight));
	            foreach (var column in Columns)
	            {
	                column.DrawHeader(graphics);
	            }
	        }
	
	        protected virtual void OnSelectionChanged(object? sender, EventArgs e) => SelectionChanged?.Invoke(sender, e);
	        protected internal virtual void OnRowExpanded(object? sender, RowExpandedEventArgs e) => RowExpanded?.Invoke(sender, e);
	        protected internal virtual void OnRowCollapsed(object? sender, RowCollapsedEventArgs e) => RowCollapsed?.Invoke(sender, e);
	        protected internal virtual void OnDrawingColumnHeader(object? sender, DrawColumnEventArgs e) => DrawingColumnHeader?.Invoke(sender, e);
	        protected internal virtual void OnDrawnColumnHeader(object? sender, DrawColumnEventArgs e) => DrawnColumnHeader?.Invoke(sender, e);
	        protected internal virtual void OnDrawingCell(object? sender, DrawCellEventArgs e) => DrawingCell?.Invoke(sender, e);
	        protected internal virtual void OnDrawnCell(object? sender, DrawCellEventArgs e) => DrawnCell?.Invoke(sender, e);
	        protected internal virtual void OnDrawingRow(object? sender, DrawRowEventArgs e) => DrawingRow?.Invoke(sender, e);
	        protected internal virtual void OnDrawnRow(object? sender, DrawRowEventArgs e) => DrawnRow?.Invoke(sender, e);
	        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => OnPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
	        protected virtual void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(sender, e);
	        protected override void OnClientSizeChanged(EventArgs e)
	        {
	            base.OnClientSizeChanged(e);
	            UpdateScrollBarsVisibility();
	            UpdateScrollBars();
	            UpdateRows();
	        }
	
	        protected override void OnVisibleChanged(EventArgs e)
	        {
	            base.OnVisibleChanged(e);
	            UpdateScrollBars();
	        }
	
	        protected override void OnBackColorChanged(EventArgs e)
	        {
	            base.OnBackColorChanged(e);
	            _backColorBrush?.Dispose();
	            _backColorBrush = new SolidBrush(BackColor);
	        }
	
	        public virtual Row? GetRow(Point point)
	        {
	            if (_rowsCache.Count == 0)
	                return null;
	
	            for (var i = _firstVisibleRowIndex; i < _rowsCache.Count; i++)
	            {
	                var bounds = _rowsCache[i].BoundsWithOffsets;
	                if (!bounds.HasValue)
	                    continue;
	
	                if (bounds.Value.Contains(point))
	                    return _rowsCache[i];
	            }
	            return null;
	        }
	
	        private Column? GetSplitColumn(Point point)
	        {
	            if (Columns.Count == 0)
	                return null;
	
	            var width = Columns[0].Width;
	            for (var i = 1; i < Columns.Count; i++)
	            {
	                var delta = point.X - width;
	                if (Math.Abs(delta) <= MouseTolerance)
	                    return Columns[i - 1];
	
	                width += Columns[i].Width + LineWidth;
	            }
	
	            if (DrawLastColumnRightLine)
	            {
	                var delta = point.X - width;
	                if (Math.Abs(delta) <= MouseTolerance)
	                    return Columns[^1];
	            }
	            return null;
	        }
	
	        internal void ComputeExtentHeight()
	        {
	            var height = 0;
	            if (DrawColumnsHeader)
	            {
	                height += HeaderHeight + LineWidth;
	            }
	
	            foreach (var row in Rows)
	            {
	                height += RowHeight + LineWidth;
	                if (row.IsExpanded)
	                {
	                    foreach (var child in row.ExpandedChildRows)
	                    {
	                        height += RowHeight + LineWidth;
	                    }
	                }
	            }
	
	            if (DrawLastRowBottomLine)
	            {
	                height += LineWidth;
	            }
	
	            ExtentHeight = height;
	        }
	
	        private void UpdateScrollBarsVisibility()
	        {
	            var invisibleWidth = Math.Max(0, ExtentWidth - Width);
	            HorizontalScrollBar.Visible = invisibleWidth > 0;
	            HorizontalScrollBar.LargeChange = ClientSize.Width;
	            HorizontalScrollBar.Maximum = ExtentWidth;
	            HorizontalOffset = Math.Min(invisibleWidth, HorizontalOffset);
	
	            var invisibleHeight = Math.Max(0, ExtentHeight - Height);
	            VerticalScrollBar.Visible = invisibleHeight > 0;
	            VerticalScrollBar.LargeChange = ClientSize.Height;
	            VerticalScrollBar.Maximum = ExtentHeight;
	            VerticalOffset = Math.Min(invisibleHeight, VerticalOffset);
	        }
	
	        private void UpdateScrollBars()
	        {
	            var size = ClientSize;
	            if (size.Width <= VerticalScrollBar.Width || size.Height < HorizontalScrollBar.Height)
	                return;
	
	            HorizontalScrollBar.Width = size.Width - (VerticalScrollBar.Visible ? VerticalScrollBar.Width : 0);
	            HorizontalScrollBar.Top = size.Height - HorizontalScrollBar.Height;
	            VerticalScrollBar.Height = size.Height - (HorizontalScrollBar.Visible ? HorizontalScrollBar.Height : 0);
	            VerticalScrollBar.Left = size.Width - VerticalScrollBar.Width;
	        }
	
	        private void DrawScrollBarsCorner(Graphics graphics)
	        {
	            var dr = DisplayRectangle;
	            var cr = ClientRectangle;
	            var w = cr.Width - dr.Width;
	            if (w <= 0)
	                return;
	
	            var h = cr.Height - dr.Height;
	            if (h <= 0)
	                return;
	
	            graphics.FillRectangle(SystemBrushes.Control, new Rectangle(dr.Right, dr.Bottom, w, h));
	        }
	
	        protected virtual void OnHorizontalScrollBarValueChanged()
	        {
	        }
	
	        protected virtual void OnVerticalScrollBarValueChanged()
	        {
	            UpdateRows();
	        }
	
	        protected virtual void OnRowsCollectionChanged(NotifyCollectionChangedEventArgs e)
	        {
	            ArgumentNullException.ThrowIfNull(e);
	            switch (e.Action)
	            {
	                case NotifyCollectionChangedAction.Add:
	                    if (e.NewItems != null)
	                    {
	                        var index = e.NewStartingIndex >= 0 ? e.NewStartingIndex : Rows.Count;
	                        foreach (var item in e.NewItems.OfType<Row>())
	                        {
	                            if (item == null)
	                                throw new InvalidOperationException("Item is invalid.");
	
	                            if (item.Owner != null)
	                                throw new InvalidOperationException($"Item {item} is already owned by a control.");
	
	                            item.Owner = this;
	                            item.Index = index++;
	                        }
	
	                        for (var i = index; i < Rows.Count; i++)
	                        {
	                            Rows[i].Index = i;
	                        }
	                    }
	                    break;
	
	                case NotifyCollectionChangedAction.Remove:
	                    if (e.OldItems != null)
	                    {
	                        foreach (var item in e.OldItems.OfType<Row>())
	                        {
	                            if (item == null)
	                                throw new InvalidOperationException("Item is invalid.");
	
	                            if (item.Owner == null)
	                                throw new InvalidOperationException($"Item {item} is not owned by a control.");
	
	                            item.Owner = null;
	                            item.Index = -1;
	                            item.ClearAllRows();
	                        }
	
	                        reIndex();
	                    }
	                    break;
	
	                case NotifyCollectionChangedAction.Replace:
	                    if (e.OldItems != null)
	                    {
	                        OnRowsCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems, e.OldStartingIndex));
	                    }
	
	                    if (e.NewItems != null)
	                    {
	                        OnRowsCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems, e.NewStartingIndex));
	                    }
	                    break;
	
	                case NotifyCollectionChangedAction.Move:
	                    if (e.NewItems != null && e.OldItems != null)
	                    {
	                        reIndex();
	                    }
	                    break;
	
	                case NotifyCollectionChangedAction.Reset:
	                    ClearCache();
	                    Invalidate();
	                    break;
	            }
	
	            ComputeExtentHeight();
	
	            void reIndex()
	            {
	                var index = 0;
	                foreach (var item in Rows)
	                {
	                    item.Index = index++;
	                }
	            }
	        }
	
	        protected virtual void OnColumnsCollectionChanged(NotifyCollectionChangedEventArgs e)
	        {
	            ArgumentNullException.ThrowIfNull(e);
	            switch (e.Action)
	            {
	                case NotifyCollectionChangedAction.Add:
	                    if (e.NewItems != null)
	                    {
	                        var index = e.NewStartingIndex >= 0 ? e.NewStartingIndex : Columns.Count;
	                        foreach (var item in e.NewItems.OfType<Column>())
	                        {
	                            if (item == null)
	                                throw new InvalidOperationException("Item is invalid.");
	
	                            if (item.Owner != null)
	                                throw new InvalidOperationException($"Item {item} is already owned by a control.");
	
	                            item.Owner = this;
	                            item.Index = index++;
	                        }
	
	                        // reindex following columns
	                        for (var i = index; i < Columns.Count; i++)
	                        {
	                            Columns[i].Index = i;
	                            Columns[i].ResetBounds();
	                        }
	                    }
	                    break;
	
	                case NotifyCollectionChangedAction.Remove:
	                    if (e.OldItems != null)
	                    {
	                        foreach (var item in e.OldItems.OfType<Column>())
	                        {
	                            if (item == null)
	                                throw new InvalidOperationException("Item is invalid.");
	
	                            if (item.Owner == null)
	                                throw new InvalidOperationException($"Item {item} is not owned by a control.");
	
	                            item.Owner = null;
	                            item.Index = -1;
	                        }
	
	                        // reindex all columns
	                        reIndex();
	                    }
	                    break;
	
	                case NotifyCollectionChangedAction.Replace:
	                    if (e.OldItems != null)
	                    {
	                        OnColumnsCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems, e.OldStartingIndex));
	                    }
	
	                    if (e.NewItems != null)
	                    {
	                        OnColumnsCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems, e.NewStartingIndex));
	                    }
	                    break;
	
	                case NotifyCollectionChangedAction.Move:
	                    if (e.NewItems != null && e.OldItems != null)
	                    {
	                        reIndex();
	                    }
	                    break;
	
	                case NotifyCollectionChangedAction.Reset:
	                    foreach (var item in Columns)
	                    {
	                        item.Owner = null;
	                        item.Index = -1;
	                    }
	                    break;
	            }
	
	            void reIndex()
	            {
	                var index = 0;
	                foreach (var item in Columns)
	                {
	                    item.Index = index++;
	                }
	            }
	        }
	
	        private sealed class MovingColumnHeader(Column column, Point position)
	        {
	            public int Width = column.Width;
	            public Column Column = column;
	            public Point Position = position;
	
	            public void MouseMove(Point position)
	            {
	                var newWidth = Width + position.X - Position.X;
	                if (newWidth <= 0)
	                    return;
	
	                Column.Width = newWidth;
	                foreach (var column in Column.FollowingColumns)
	                {
	                    column.ResetBounds();
	                }
	
	                if (Column.Owner != null)
	                {
	                    if (Column.Index == 0)
	                    {
	                        foreach (var row in Column.Owner._rowsCache)
	                        {
	                            var bounds = row.FirstCellBounds!.Value;
	                            bounds.Width = Column.Width;
	                            row.FirstCellBounds = bounds;
	                        }
	                        Column.Owner.Invalidate();
	                    }
	                    Column.Owner.UpdateScrollBarsVisibility();
	                }
	            }
	        }
	    }
	
	public class ObservableCreatingEventArgs<T> : EventArgs
	    {
	        public ObservableCreatingEventArgs()
	        {
	        }
	
	        public virtual IList<T>? BaseList { get; set; }
	    }
	
	public class ObservableInsertedEventArgs<T> : EventArgs
	    {
	        public ObservableInsertedEventArgs(int index, T item)
	        {
	            ArgumentOutOfRangeException.ThrowIfNegative(index);
	
	            Index = index;
	            Item = item;
	        }
	
	        public int Index { get; }
	        public T Item { get; }
	    }
	
	public class ObservableInsertingEventArgs<T> : CancelEventArgs
	    {
	        public ObservableInsertingEventArgs(int index, T item)
	        {
	            ArgumentOutOfRangeException.ThrowIfNegative(index);
	
	            Index = index;
	            Item = item;
	        }
	
	        public int Index { get; }
	        public T Item { get; }
	    }
	
	public class ObservableList<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
	    {
	        public event EventHandler<ObservableCreatingEventArgs<T>>? Creating;
	        public event EventHandler<CancelEventArgs>? Clearing;
	        public event EventHandler<EventArgs>? Cleared;
	        public event EventHandler<ObservableInsertingEventArgs<T>>? Inserting;
	        public event EventHandler<ObservableInsertedEventArgs<T>>? Inserted;
	        public event EventHandler<ObservableRemovingEventArgs<T>>? Removing;
	        public event EventHandler<ObservableRemovedEventArgs<T>>? Removed;
	        public event EventHandler<ObservableListReplacingEventArgs<T>>? Replacing;
	        public event EventHandler<ObservableReplacedEventArgs<T>>? Replaced;
	        public event PropertyChangedEventHandler? PropertyChanged;
	        public event NotifyCollectionChangedEventHandler? CollectionChanged;
	
	        public ObservableList()
	        {
	            var e = new ObservableCreatingEventArgs<T>();
	            OnCreating(this, e);
	            BaseList = e.BaseList ?? [];
	        }
	
	        protected IList<T> BaseList { get; }
	        public T this[int index] { get => BaseList[index]; set => Replace(index, value); }
	        public int Count => BaseList.Count;
	
	        public int IndexOf(T item) => BaseList.IndexOf(item);
	        public bool Contains(T item) => BaseList.Contains(item);
	        public IEnumerator<T> GetEnumerator() => BaseList.GetEnumerator();
	
	        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => BaseList.CopyTo(array, arrayIndex);
	        bool ICollection<T>.IsReadOnly => false;
	        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	
	        public void Add(T item) => Insert(Count, item);
	        public virtual void Insert(int index, T item)
	        {
	            var e = new ObservableInsertingEventArgs<T>(index, item);
	            OnInserting(this, e);
	            if (e.Cancel)
	                return;
	
	            BaseList.Insert(index, item);
	            OnInserted(this, new ObservableInsertedEventArgs<T>(index, item));
	            OnCountPropertyChanged();
	            OnIndexerPropertyChanged();
	            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
	        }
	
	        public virtual void RemoveAt(int index) => RemoveAt(index, true);
	        protected virtual void RemoveAt(int index, bool sendRemovingEvent)
	        {
	            var item = BaseList[index];
	            if (sendRemovingEvent)
	            {
	                var e = new ObservableRemovingEventArgs<T>(index, item);
	                OnRemoving(this, e);
	                if (e.Cancel)
	                    return;
	            }
	
	            BaseList.RemoveAt(index);
	            OnRemoved(this, new ObservableRemovedEventArgs<T>(index, item));
	            OnCountPropertyChanged();
	            OnIndexerPropertyChanged();
	            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
	        }
	
	        public virtual void Clear()
	        {
	            var e = new CancelEventArgs();
	            OnClearing(this, e);
	            if (e.Cancel)
	                return;
	
	            BaseList.Clear();
	            OnCleared(this, EventArgs.Empty);
	            OnCountPropertyChanged();
	            OnIndexerPropertyChanged();
	            OnCollectionReset();
	        }
	
	        public virtual bool Remove(T item)
	        {
	            var e = new ObservableRemovingEventArgs<T>(null, item);
	            OnRemoving(this, e);
	            if (e.Cancel)
	                return false;
	
	            var index = IndexOf(item);
	            if (index < 0)
	                return false;
	
	            RemoveAt(index, false);
	            return true;
	        }
	
	        protected virtual void Replace(int index, T item)
	        {
	            var existing = BaseList[index];
	            var e = new ObservableListReplacingEventArgs<T>(index, item, existing);
	            OnReplacing(this, e);
	            if (e.Cancel)
	                return;
	
	            BaseList[index] = item;
	            OnReplaced(this, new ObservableReplacedEventArgs<T>(index, item, existing));
	            OnIndexerPropertyChanged();
	            OnCollectionChanged(NotifyCollectionChangedAction.Replace, existing, item, index);
	        }
	
	        private void OnCollectionReset() => OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
	        private void OnCollectionChanged(NotifyCollectionChangedAction action, object? item, int index) => OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
	        private void OnCollectionChanged(NotifyCollectionChangedAction action, object? oldItem, object? newItem, int index) => OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
	        private void OnIndexerPropertyChanged() => OnPropertyChanged(this, new PropertyChangedEventArgs("Item[]"));
	        private void OnCountPropertyChanged() => OnPropertyChanged(this, new PropertyChangedEventArgs(nameof(Count)));
	        protected virtual void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(sender, e);
	        protected virtual void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(sender, e);
	        protected virtual void OnCreating(object? sender, ObservableCreatingEventArgs<T> e) => Creating?.Invoke(sender, e);
	        protected virtual void OnClearing(object? sender, CancelEventArgs e) => Clearing?.Invoke(sender, e);
	        protected virtual void OnCleared(object? sender, EventArgs e) => Cleared?.Invoke(sender, e);
	        protected virtual void OnInserting(object? sender, ObservableInsertingEventArgs<T> e) => Inserting?.Invoke(sender, e);
	        protected virtual void OnInserted(object? sender, ObservableInsertedEventArgs<T> e) => Inserted?.Invoke(sender, e);
	        protected virtual void OnRemoving(object? sender, ObservableRemovingEventArgs<T> e) => Removing?.Invoke(sender, e);
	        protected virtual void OnRemoved(object? sender, ObservableRemovedEventArgs<T> e) => Removed?.Invoke(sender, e);
	        protected virtual void OnReplacing(object? sender, ObservableListReplacingEventArgs<T> e) => Replacing?.Invoke(sender, e);
	        protected virtual void OnReplaced(object? sender, ObservableReplacedEventArgs<T> e) => Replaced?.Invoke(sender, e);
	    }
	
	public class ObservableListReplacingEventArgs<T> : CancelEventArgs
	    {
	        public ObservableListReplacingEventArgs(int index, T item, T oldItem)
	        {
	            ArgumentOutOfRangeException.ThrowIfNegative(index);
	
	            Index = index;
	            Item = item;
	            OldItem = oldItem;
	        }
	
	        public int Index { get; }
	        public T Item { get; }
	        public T OldItem { get; }
	    }
	
	public class ObservableRemovedEventArgs<T> : EventArgs
	    {
	        public ObservableRemovedEventArgs(int index, T item)
	        {
	            ArgumentOutOfRangeException.ThrowIfNegative(index);
	
	            Index = index;
	            Item = item;
	        }
	
	        public int Index { get; }
	        public T Item { get; }
	    }
	
	public class ObservableRemovingEventArgs<T> : CancelEventArgs
	    {
	        public ObservableRemovingEventArgs(int? index, T item)
	        {
	            if (index < 0)
	                throw new ArgumentOutOfRangeException(nameof(index));
	
	            Index = index;
	            Item = item;
	        }
	
	        public int? Index { get; }
	        public T Item { get; }
	    }
	
	public class ObservableReplacedEventArgs<T> : EventArgs
	    {
	        public ObservableReplacedEventArgs(int index, T item, T oldItem)
	        {
	            ArgumentOutOfRangeException.ThrowIfNegative(index);
	
	            Index = index;
	            Item = item;
	            OldItem = oldItem;
	        }
	
	        public int Index { get; }
	        public T Item { get; }
	        public T OldItem { get; }
	    }
	
	public class Row : INotifyPropertyChanged
	    {
	        private bool _isExpanded;
	        private bool _isSelected;
	        private bool? _isExpandable;
	        private int _index = -1;
	        private ListTreeViewControl? _owner;
	        private Row? _parentRow;
	
	        public event PropertyChangedEventHandler? PropertyChanged;
	
	        public Row()
	        {
	            ChildRows.CollectionChanged += (sender, e) => OnChildRowsCollectionChanged(e);
	            Cells.CollectionChanged += (sende, e) => OnCellsCollectionChanged(e);
	        }
	
	        public ObservableList<Row> ChildRows { get; } = [];
	        public ObservableList<Cell> Cells { get; } = [];
	        public string Key { get; private set; } = string.Empty;
	        public virtual int ExpanderSize => Owner?.LogicalToDeviceUnits(9) ?? 9;
	        public virtual int ExpanderPadding => Owner?.LogicalToDeviceUnits(4) ?? 4;
	        public virtual object? Tag { get; set; }
	        public int Level => ParentRow != null ? ParentRow.Level + 1 : 0;
	        public Rectangle? FirstCellBounds { get; internal protected set; }
	        public string FirstCellText => Cells.Count > 0 ? Cells[0].ValueAsString : string.Empty;
	
	        internal int OwnerBottom => OwnerTop + OwnerHeight;
	        internal int OwnerTop
	        {
	            get
	            {
	                if (Owner == null)
	                    return 0;
	
	                var top = Owner.VerticalOffset;
	                if (Owner.DrawColumnsHeader)
	                {
	                    top += Owner.HeaderHeight + Owner.LineWidth * 2;
	                }
	                return top;
	            }
	        }
	
	        private int OwnerHeight
	        {
	            get
	            {
	                if (Owner == null)
	                    return 0;
	
	                var height = Owner.Height;
	                if (Owner.DrawColumnsHeader)
	                {
	                    height -= Owner.HeaderHeight + Owner.LineWidth * 2;
	                }
	                return Math.Max(0, height);
	            }
	        }
	
	        internal Rectangle? BoundsWithOffsets
	        {
	            get
	            {
	                if (!Bounds.HasValue)
	                    return null;
	
	                var bounds = Bounds.Value;
	                bounds.X -= Owner!.HorizontalOffset;
	                bounds.Y -= Owner!.VerticalOffset;
	                return bounds;
	            }
	        }
	
	        public virtual Rectangle? Bounds
	        {
	            get
	            {
	                if (Owner == null || !FirstCellBounds.HasValue)
	                    return null;
	
	                var bounds = FirstCellBounds.Value;
	                bounds.Width = Owner.Width;
	                return bounds;
	            }
	        }
	
	        internal Rectangle? ExpanderBoundsWithOffsets
	        {
	            get
	            {
	                if (!ExpanderBounds.HasValue)
	                    return null;
	
	                var bounds = ExpanderBounds.Value;
	                bounds.X -= Owner!.HorizontalOffset;
	                bounds.Y -= Owner!.VerticalOffset;
	                return bounds;
	            }
	        }
	
	        protected internal virtual Rectangle? ExpanderBounds
	        {
	            get
	            {
	                if (Owner == null || !FirstCellBounds.HasValue)
	                    return null;
	
	                var bounds = FirstCellBounds.Value;
	                bounds.X += ExpanderPadding + Level * Owner.RowOverhang - ListTreeViewControl.MouseTolerance / 2;
	                bounds.Width = ExpanderSize + ListTreeViewControl.MouseTolerance;
	                bounds.Y += (Owner.RowHeight - ExpanderSize) / 2 - ListTreeViewControl.MouseTolerance / 2;
	                bounds.Height = ExpanderSize + ListTreeViewControl.MouseTolerance;
	                return bounds;
	            }
	        }
	
	        public Row? NextRow
	        {
	            get
	            {
	                if (Index < 0 || Owner == null)
	                    return null;
	
	                if (ParentRow != null)
	                {
	                    if (Index + 1 < ParentRow.ChildRows.Count)
	                        return ParentRow.ChildRows[Index + 1];
	                }
	                else if (Index + 1 < Owner.Rows.Count)
	                    return Owner.Rows[Index + 1];
	
	                return null;
	            }
	        }
	
	        public Row? PreviousRow
	        {
	            get
	            {
	                if (Index <= 0 || Owner == null)
	                    return null;
	
	                if (ParentRow != null)
	                    return ParentRow.ChildRows[Index - 1];
	
	                return Owner.Rows[Index - 1];
	            }
	        }
	
	        public int Index
	        {
	            get => _index;
	            internal set
	            {
	                if (_index == value)
	                    return;
	
	                _index = value;
	                SetKey();
	                ComputeFirstCellBounds(true);
	                OnPropertyChanged();
	            }
	        }
	
	        public ListTreeViewControl? Owner
	        {
	            get => _owner;
	            internal set
	            {
	                if (_owner == value)
	                    return;
	
	                _owner = value;
	                ComputeFirstCellBounds(true);
	                OnPropertyChanged();
	            }
	        }
	
	        public Row? ParentRow
	        {
	            get => _parentRow;
	            internal set
	            {
	                if (_parentRow == value)
	                    return;
	
	                _parentRow = value;
	                SetKey();
	                ComputeFirstCellBounds(true);
	                OnPropertyChanged();
	            }
	        }
	
	        public virtual bool? IsExpandable
	        {
	            get => _isExpandable ?? ChildRows.Count > 0;
	            set
	            {
	                if (_isExpandable == value)
	                    return;
	
	                _isExpandable = value;
	                OnPropertyChanged();
	            }
	        }
	
	        public bool IsVisible => Owner != null && Index >= 0 && (ParentRow == null || ParentRow.IsHierarchyExpanded);
	        public bool IsRoot => Owner != null && Index >= 0 && ParentRow == null;
	        private bool IsHierarchyExpanded
	        {
	            get
	            {
	                if (Owner == null || Index < 0)
	                    return false;
	
	                if (ParentRow == null)
	                    return IsExpanded;
	
	                return IsExpanded && ParentRow.IsExpanded;
	            }
	        }
	
	        public virtual bool IsExpanded
	        {
	            get => _isExpanded;
	            set
	            {
	                if (_isExpanded == value)
	                    return;
	
	                _isExpanded = value;
	                OnPropertyChanged();
	                if (value)
	                {
	                    DoExpand();
	                    OnExpanded(this, EventArgs.Empty);
	                }
	                else
	                {
	                    DoCollapse();
	                    OnCollapsed(this, EventArgs.Empty);
	                }
	
	                var owner = Owner;
	                if (owner != null)
	                {
	                    owner.ComputeExtentHeight();
	                    owner.Invalidate();
	                }
	            }
	        }
	
	        public virtual bool IsSelected
	        {
	            get => _isSelected;
	            set
	            {
	                if (_isSelected == value)
	                    return;
	
	                _isSelected = value;
	                OnPropertyChanged();
	                Owner?.Invalidate();
	            }
	        }
	
	        // within same parent
	        public IEnumerable<Row> PreviousRows
	        {
	            get
	            {
	                var previous = PreviousRow;
	                do
	                {
	                    if (previous == null)
	                        yield break;
	
	                    yield return previous;
	                    previous = previous.PreviousRow;
	                }
	                while (true);
	            }
	        }
	
	        // presumes this.IsHierarchyExpanded is true
	        internal IEnumerable<Row> ExpandedChildRows
	        {
	            get
	            {
	#if DEBUG
	                if (!IsHierarchyExpanded)
	                    throw new InvalidOperationException();
	#endif
	                foreach (var child in ChildRows)
	                {
	                    yield return child;
	                    if (child.IsExpanded)
	                    {
	                        foreach (var gchild in child.ExpandedChildRows)
	                        {
	                            yield return gchild;
	                        }
	                    }
	                }
	            }
	        }
	
	        public IEnumerable<Row> FollowingRows
	        {
	            get
	            {
	                foreach (var child in ChildRows)
	                {
	                    yield return child;
	                }
	
	                if (ParentRow != null)
	                {
	                    foreach (var next in NextRows)
	                    {
	                        yield return next;
	                        foreach (var child in next.ChildRows)
	                        {
	                            yield return child;
	                        }
	                    }
	
	                    var parentNext = ParentRows.FirstOrDefault(p => p.NextRow != null)?.NextRow;
	                    if (parentNext != null)
	                    {
	                        yield return parentNext;
	                        foreach (var following in parentNext.FollowingRows)
	                        {
	                            yield return following;
	                        }
	                    }
	                }
	                else
	                {
	                    var nextRow = NextRow;
	                    if (nextRow != null)
	                    {
	                        yield return nextRow;
	                        foreach (var following in nextRow.FollowingRows)
	                        {
	                            yield return following;
	                        }
	                    }
	                }
	            }
	        }
	
	        public IEnumerable<Row> ExpandedFollowingRows
	        {
	            get
	            {
	                if (IsHierarchyExpanded)
	                {
	                    foreach (var child in ExpandedChildRows)
	                    {
	                        yield return child;
	                    }
	                }
	
	                if (ParentRow != null)
	                {
	                    if (ParentRow.IsHierarchyExpanded)
	                    {
	                        foreach (var next in NextRows)
	                        {
	                            yield return next;
	                            if (next.IsExpanded)
	                            {
	                                foreach (var child in next.ExpandedChildRows)
	                                {
	                                    yield return child;
	                                }
	                            }
	                        }
	                    }
	
	                    var parentNext = ParentRows.FirstOrDefault(p => p.NextRow != null)?.NextRow;
	                    if (parentNext != null && parentNext.IsVisible)
	                    {
	                        yield return parentNext;
	                        foreach (var following in parentNext.ExpandedFollowingRows)
	                        {
	                            yield return following;
	                        }
	                    }
	                }
	                else
	                {
	                    var nextRow = NextRow;
	                    if (nextRow != null && nextRow.IsVisible)
	                    {
	                        yield return nextRow;
	                        foreach (var following in nextRow.ExpandedFollowingRows)
	                        {
	                            yield return following;
	                        }
	                    }
	                }
	            }
	        }
	
	        // within same parent
	        public IEnumerable<Row> NextRows
	        {
	            get
	            {
	                var next = NextRow;
	                do
	                {
	                    if (next == null)
	                        yield break;
	
	                    yield return next;
	                    next = next.NextRow;
	                }
	                while (true);
	            }
	        }
	
	        public IEnumerable<Row> ParentRows
	        {
	            get
	            {
	                var parent = ParentRow;
	                do
	                {
	                    if (parent == null)
	                        yield break;
	
	                    yield return parent;
	                    parent = parent.ParentRow;
	                }
	                while (true);
	            }
	        }
	
	        public IEnumerable<Row> AllChildRows
	        {
	            get
	            {
	                foreach (var child in ChildRows)
	                {
	                    yield return child;
	                    foreach (var grandChild in child.AllChildRows)
	                    {
	                        yield return grandChild;
	                    }
	                }
	            }
	        }
	
	        protected virtual void DoCollapse()
	        {
	            if (Owner == null || Owner.Columns.Count == 0 || Index < 0)
	                return;
	
	            if (!FirstCellBounds.HasValue)
	                return;
	
	            var height = 0;
	            foreach (var child in AllChildRows)
	            {
	                if (child.FirstCellBounds.HasValue)
	                {
	                    var bounds = child.FirstCellBounds.Value;
	                    height += bounds.Height + Owner.LineWidth;
	                    child.FirstCellBounds = null;
	                    //Trace.WriteLine("collapse row " + child + " set fcb null");
	                    if (bounds.Top >= OwnerBottom)
	                        break;
	                }
	            }
	
	            var clearMode = false;
	            foreach (var following in ExpandedFollowingRows)
	            {
	                if (clearMode)
	                {
	                    // TODO: task?
	                    following.FirstCellBounds = null;
	                    //Trace.WriteLine("expand row " + following + " set fcb null (clear mode)");
	                    continue;
	                }
	
	                if (following.FirstCellBounds.HasValue)
	                {
	                    var bounds = following.FirstCellBounds.Value;
	                    bounds.Y -= height;
	                    if (bounds.Top >= OwnerBottom)
	                    {
	                        following.FirstCellBounds = null;
	                        clearMode = true;
	                        continue;
	                    }
	
	                    following.FirstCellBounds = bounds;
	                }
	                else
	                {
	                    following.ComputeFirstCellBounds(true);
	                    break;
	                }
	            }
	        }
	
	        protected virtual int DoExpand()
	        {
	            if (Owner == null || Owner.Columns.Count == 0 || Index < 0)
	                return 0;
	
	            // what was this previous position?
	            if (!FirstCellBounds.HasValue)
	            {
	                // none, we need to get to first previously visible row
	                var computed = PreviousRows.FirstOrDefault(r => r.FirstCellBounds.HasValue);
	                if (computed == null)
	                    return 0; // do nothing at this point (keep non expanded)
	
	                // compute all visible ones until this one
	                foreach (var following in computed.ExpandedFollowingRows)
	                {
	                    if (following == this)
	                        break;
	
	                    if (!following.FirstCellBounds.HasValue && !following.ComputeFirstCellBounds(false))
	                        return 0;
	                }
	
	                if (!FirstCellBounds.HasValue && !ComputeFirstCellBounds(false))
	                    return 0;
	            }
	
	            var fcb = FirstCellBounds!.Value;
	            if (fcb.Top >= OwnerBottom)
	            {
	                // we're below the view don't need to compute
	                FirstCellBounds = null;
	                //Trace.WriteLine("expand row " + this + " set fcb null (top to large)");
	                return 0;
	            }
	
	            var expandedHeight = 0;
	            if (fcb.Bottom < OwnerTop)
	            {
	                // we're above the view, but expanding us (by api call) can change the view
	                return 0;
	                //throw new NotImplementedException();
	            }
	
	            // we're in the view
	            var clearMode = false;
	            foreach (var following in ExpandedFollowingRows)
	            {
	                if (clearMode)
	                {
	                    // TODO: task?
	                    following.FirstCellBounds = null;
	                    //Trace.WriteLine("expand row " + following + " set fcb null (clear mode)");
	                    continue;
	                }
	
	                expandedHeight += Owner.RowHeight + Owner.LineWidth;
	                var top = fcb.Top + expandedHeight;
	
	                // if we're below the view, stop
	                if (top >= OwnerBottom)
	                {
	                    following.FirstCellBounds = null;
	                    clearMode = true;
	                    continue;
	                }
	
	                following.FirstCellBounds = new Rectangle(fcb.Left, top, Owner.Columns[0].Width, Owner.RowHeight);
	                //Trace.WriteLine("set row " + following + " fcb:" + FirstCellBounds);
	            }
	            return expandedHeight;
	        }
	
	        internal bool ComputeFirstCellBounds(bool withFollowings)
	        {
	            if (Owner == null || Owner.Columns.Count == 0 || Index < 0)
	            {
	                FirstCellBounds = null;
	                return false;
	            }
	
	            int left;
	            int top;
	            var height = 0;
	            if (ParentRow == null)
	            {
	                // root rows
	                left = Owner.LineWidth - Owner.HorizontalOffset;
	                top = (Owner.RowHeight + Owner.LineWidth) * Index - Owner.VerticalOffset;
	                if (Owner.DrawColumnsHeader)
	                {
	                    top += Owner.HeaderHeight + Owner.LineWidth;
	                }
	
	                // how many rows between this previous sibling and this
	                var previous = PreviousRow;
	                if (previous != null)
	                {
	                    foreach (var following in previous.ExpandedFollowingRows)
	                    {
	                        if (following == this)
	                            break;
	
	                        if (following.FirstCellBounds.HasValue)
	                        {
	                            height += following.FirstCellBounds.Value.Height + Owner.LineWidth;
	                        }
	
	                        if (top + height > OwnerBottom)
	                        {
	                            FirstCellBounds = null;
	                            //Trace.WriteLine("compute row " + this + " set fcb null (top+height)");
	                            return false;
	                        }
	                    }
	                }
	
	                top += height;
	
	                FirstCellBounds = new Rectangle(left, top + Owner.LineWidth, Owner.Columns[0].Width, Owner.RowHeight);
	                //Trace.WriteLine("compute row " + this + " set fcb:" + FirstCellBounds);
	
	                if (withFollowings)
	                {
	                    // TODO: task?
	                    ComputeFollowings(FirstCellBounds.Value);
	                }
	                return true;
	            }
	
	            // level N (with N >= 1)
	            if (!ParentRow.IsExpanded)
	            {
	                FirstCellBounds = null;
	                //Trace.WriteLine("compute row " + this + " set fcb null (parent not expanded)");
	                return false;
	            }
	
	            var parentBounds = ParentRow.FirstCellBounds;
	            if (!parentBounds.HasValue)
	            {
	                FirstCellBounds = null;
	                //Trace.WriteLine("compute row " + this + " set fcb null (parent no bounds)");
	                return false;
	            }
	
	            // how many rows between this parent and this
	            foreach (var following in ParentRow.ExpandedFollowingRows)
	            {
	                if (following == this)
	                    break;
	
	                if (following.FirstCellBounds.HasValue)
	                {
	                    height += following.FirstCellBounds.Value.Height + Owner.LineWidth;
	                }
	
	                if (parentBounds.Value.Bottom + height > OwnerBottom)
	                {
	                    FirstCellBounds = null;
	                    //Trace.WriteLine("compute row " + this + " set fcb null (top+parentBounds.Bottom)");
	                    return false;
	                }
	            }
	
	            top = parentBounds.Value.Bottom + height;
	            left = parentBounds.Value.Left;
	
	            FirstCellBounds = new Rectangle(left, top + Owner.LineWidth, Owner.Columns[0].Width, Owner.RowHeight);
	            //Trace.WriteLine("compute row " + this + " set fcb:" + FirstCellBounds);
	            if (withFollowings)
	            {
	                // TODO: task?
	                ComputeFollowings(FirstCellBounds.Value);
	            }
	            return true;
	        }
	
	        internal int ComputeFollowings(Rectangle fcb)
	        {
	            var added = 0;
	            var clearMode = false;
	            var expandedHeight = 0;
	            foreach (var following in ExpandedFollowingRows)
	            {
	                if (clearMode)
	                {
	                    // TODO: task?
	                    following.FirstCellBounds = null;
	                    //Trace.WriteLine("computef row " + following + " set fcb null (clear mode)");
	                    continue;
	                }
	
	                expandedHeight += Owner!.RowHeight + Owner.LineWidth;
	                var top = fcb.Top + expandedHeight;
	
	                // if we're below the view, stop
	                if (top >= OwnerBottom)
	                {
	                    following.FirstCellBounds = null;
	                    clearMode = true;
	                    continue;
	                }
	
	                following.FirstCellBounds = new Rectangle(fcb.Left, top, Owner.Columns[0].Width, Owner.RowHeight);
	                added++;
	                //Trace.WriteLine("computef row " + following + " set fcb:" + following.FirstCellBounds);
	            }
	            return added;
	        }
	
	        public virtual Rectangle? GetCellBounds(int columnIndex)
	        {
	            ArgumentOutOfRangeException.ThrowIfNegative(columnIndex);
	
	            if (Owner == null || columnIndex >= Owner.Columns.Count)
	                return null;
	
	            var fcb = FirstCellBounds;
	            if (columnIndex == 0)
	                return fcb;
	
	            if (!fcb.HasValue)
	                return null;
	
	            var columnBounds = Owner.Columns[columnIndex].Bounds;
	            if (!columnBounds.HasValue)
	                return null;
	
	            return new Rectangle(columnBounds.Value.Left, fcb.Value.Top, Owner.Columns[columnIndex].Width, fcb.Value.Height);
	        }
	
	        private void SetKey()
	        {
	            if (ParentRow == null)
	            {
	                Key = Index.ToString();
	                return;
	            }
	
	            Key = ParentRow.Key + "." + Index;
	        }
	
	        public virtual Row AddChildRow()
	        {
	            var row = new Row();
	            ChildRows.Add(row);
	            return row;
	        }
	
	        public virtual Cell AddCell(object? value)
	        {
	            var cell = new Cell(value);
	            Cells.Add(cell);
	            return cell;
	        }
	
	        public virtual void ExpandHierarchy()
	        {
	            foreach (var parent in ParentRows)
	            {
	                parent.Expand();
	            }
	        }
	
	        public virtual void Expand() => IsExpanded = true;
	        public virtual void Collapse() => IsExpanded = false;
	        public virtual void ExpandAll()
	        {
	            IsExpanded = true;
	            foreach (var row in ChildRows)
	            {
	                row.ExpandAll();
	            }
	        }
	
	        public virtual void CollapseAll()
	        {
	            IsExpanded = false;
	            foreach (var row in ChildRows)
	            {
	                row.CollapseAll();
	            }
	        }
	
	        public override string ToString()
	        {
	            var str = new string('-', Level) + " " + Index;
	            if (Cells.Count > 0)
	            {
	                str += " " + Cells[0].ValueAsString;
	            }
	            return str;
	        }
	
	        public virtual void ClearAllRows()
	        {
	            foreach (var row in ChildRows)
	            {
	                row.ClearAllRows();
	                row.ChildRows.Clear();
	            }
	        }
	
	        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => OnPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
	        protected virtual void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(sender, e);
	        protected virtual void OnExpanded(object? sender, EventArgs e) => Owner?.OnRowExpanded(Owner, new RowExpandedEventArgs(this));
	        protected virtual void OnCollapsed(object? sender, EventArgs e) => Owner?.OnRowCollapsed(Owner, new RowCollapsedEventArgs(this));
	        protected internal virtual void Draw(Graphics graphics)
	        {
	            ArgumentNullException.ThrowIfNull(graphics);
	            if (Owner == null)
	                return;
	
	            var e = new DrawRowEventArgs(graphics, this);
	            Owner.OnDrawingRow(this, e);
	            if (e.Handled)
	                return;
	
	            //Trace.WriteLine("draw row " + this + " fcb:" + FirstCellBounds);
	            var fcb = FirstCellBounds;
	            if (!fcb.HasValue)
	                return;
	
	            if (IsSelected && Owner.RowSelectedBrush != null && e.DrawSelection)
	            {
	                DrawSelection(graphics);
	            }
	
	            var cellCount = Math.Min(Owner.Columns.Count, Cells.Count);
	            for (var i = 0; i < cellCount; i++)
	            {
	                var cell = Cells[i];
	                var col = Owner.Columns[i];
	
	                Rectangle layout;
	                if (i == 0)
	                {
	                    layout = fcb.Value;
	                }
	                else
	                {
	                    layout = GetCellBounds(i)!.Value;
	                }
	
	                layout.X -= Owner.HorizontalOffset;
	                layout.Y -= Owner.VerticalOffset;
	
	                if (i == 0 && e.DrawExpander)
	                {
	                    if (IsExpandable == true)
	                    {
	                        DrawExpander(graphics, layout);
	                    }
	
	                    var offset = ExpanderSize + ExpanderPadding + Level * Owner.RowOverhang;
	                    layout.X += offset;
	                    layout.Width = Math.Max(0, layout.Width - offset);
	                }
	
	                if (i == 0)
	                {
	                    layout.X += e.FirstCellRightOffset;
	                    layout.Width = Math.Max(0, layout.Width - e.FirstCellRightOffset);
	                }
	
	                // note at this point, layout width or height can be 0, but we want the events to be sent anyways
	
	                var format = (StringFormat)StringFormat.GenericTypographic.Clone();
	                if (i == 0)
	                {
	                    format.LineAlignment = StringAlignment.Center;
	                    format.Alignment = StringAlignment.Near;
	                }
	                else
	                {
	                    format.LineAlignment = col.VerticalAlignment;
	                    format.Alignment = col.HorizontalAlignment;
	                }
	
	                format.FormatFlags = col.FormatFlags;
	                format.Trimming = col.Trimming;
	
	                cell.Draw(graphics, this, col, layout, format);
	            }
	
	            Owner.OnDrawnRow(this, new DrawRowEventArgs(graphics, this));
	        }
	
	        protected internal virtual void DrawSelection(Graphics graphics)
	        {
	            ArgumentNullException.ThrowIfNull(graphics);
	            if (Owner == null || Owner.RowSelectedBrush == null)
	                return;
	
	            Region? clip = null;
	            if (!graphics.IsClipEmpty)
	            {
	                clip = graphics.Clip;
	                graphics.ResetClip();
	            }
	
	            var layout = Bounds!.Value;
	            layout.X -= Owner.HorizontalOffset;
	            layout.Y -= Owner.VerticalOffset;
	            layout.Width = Owner.ExtentWidth;
	            graphics.FillRectangle(Owner.RowSelectedBrush, layout);
	
	            if (clip != null)
	            {
	                graphics.Clip = clip;
	            }
	        }
	
	        protected internal virtual void DrawExpander(Graphics graphics, Rectangle layout)
	        {
	            ArgumentNullException.ThrowIfNull(graphics);
	            if (Owner == null || Owner.Columns.Count == 0)
	                return;
	
	            var bounds = new Rectangle(layout.Left + ExpanderPadding + Level * Owner.RowOverhang, layout.Top + (Owner.RowHeight - ExpanderSize) / 2, ExpanderSize, ExpanderSize);
	            if (bounds.Left >= Owner.Columns[0].Width)
	                return;
	
	            if (bounds.Width == 0 || bounds.Height == 0)
	                return;
	
	            var clip = bounds;
	            clip.X = layout.X;
	            clip.Width = Owner.Columns[0].Width;
	            var renderer = new VisualStyleRenderer(IsExpanded ? VisualStyleElement.TreeView.Glyph.Opened : VisualStyleElement.TreeView.Glyph.Closed);
	            renderer.DrawBackground(graphics, bounds, clip);
	        }
	
	        protected virtual void OnChildRowsCollectionChanged(NotifyCollectionChangedEventArgs e)
	        {
	            ArgumentNullException.ThrowIfNull(e);
	            switch (e.Action)
	            {
	                case NotifyCollectionChangedAction.Add:
	                    if (e.NewItems != null)
	                    {
	                        if (Owner == null)
	                            throw new InvalidOperationException($"Parent row {this} must be owned by a control.");
	
	                        var index = e.NewStartingIndex >= 0 ? e.NewStartingIndex : ChildRows.Count;
	                        foreach (var item in e.NewItems.OfType<Row>())
	                        {
	                            if (item == null)
	                                throw new InvalidOperationException("Item is invalid.");
	
	                            if (item.ParentRow != null)
	                                throw new InvalidOperationException($"Item {item} is already owned by a parent row.");
	
	                            item.ParentRow = this;
	                            item.Owner = Owner;
	                            item.Index = index++;
	                        }
	
	                        for (var i = index; i < ChildRows.Count; i++)
	                        {
	                            ChildRows[i].Index = i;
	                        }
	                    }
	                    break;
	
	                case NotifyCollectionChangedAction.Remove:
	                    if (e.OldItems != null)
	                    {
	                        foreach (var item in e.OldItems.OfType<Row>())
	                        {
	                            if (item == null)
	                                throw new InvalidOperationException("Item is invalid.");
	
	                            if (item.Owner == null)
	                                throw new InvalidOperationException($"Item {item} is not owned by a control.");
	
	                            item.ParentRow = null;
	                            item.Owner = null;
	                            item.Index = -1;
	                            item.ClearAllRows();
	                        }
	
	                        reIndex();
	                    }
	                    break;
	
	                case NotifyCollectionChangedAction.Replace:
	                    if (e.OldItems != null)
	                    {
	                        OnChildRowsCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems, e.OldStartingIndex));
	                    }
	
	                    if (e.NewItems != null)
	                    {
	                        OnChildRowsCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems, e.NewStartingIndex));
	                    }
	                    break;
	
	                case NotifyCollectionChangedAction.Move:
	                    if (e.NewItems != null && e.OldItems != null)
	                    {
	                        reIndex();
	                    }
	                    break;
	
	                case NotifyCollectionChangedAction.Reset:
	                    throw new NotImplementedException();
	            }
	
	            void reIndex()
	            {
	                var index = 0;
	                foreach (var item in ChildRows)
	                {
	                    item.Index = index++;
	                }
	            }
	        }
	
	        protected virtual void OnCellsCollectionChanged(NotifyCollectionChangedEventArgs e)
	        {
	            ArgumentNullException.ThrowIfNull(e);
	        }
	    }
	
	public class RowCollapsedEventArgs(Row row) : RowEventArgs(row)
	    {
	    }
	
	public abstract class RowEventArgs : HandledEventArgs
	    {
	        protected RowEventArgs(Row row)
	        {
	            ArgumentNullException.ThrowIfNull(row);
	            Row = row;
	        }
	
	        public Row Row { get; }
	    }
	
	public class RowExpandedEventArgs(Row row) : RowEventArgs(row)
	    {
	    }
}

#pragma warning restore IDE0130 // Namespace does not match folder structure
#pragma warning restore IDE0079 // Remove unnecessary suppression
