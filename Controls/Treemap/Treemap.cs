using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TMFileManager
{
    public partial class TreeMap : UserControl
    {
        private TreeMapNode root = new TreeMapNode("");
        private TreeMapNode highLighted = null;
        private bool highLightedIsDir = false;
        private bool showFreeSpace = false;

        public event EventHandler RootPathChanged;

        private Hashtable extensions = new Hashtable();
        private System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
        private Pen red2 = new Pen(Color.Red, 2);
        private Pen yellow2 = new Pen(Color.Yellow, 2);
        private Image buffer = null;
        private Graphics bufferGraphics = null;
        private bool repaint = true;
        private Font f = new Font("Ms Sans Serif", 9);

        private int depthLevel = 0;
        private TreeMapNode clipBoard;
        private bool cutClipboard = false;

        private bool editing = false;

        public int DepthLevel
        {
            get
            {
                return depthLevel;
            }
            set
            {
                if (value != depthLevel)
                {
                    depthLevel = value;
                    RootPath = RootPath;
                }
            }
        }

        public bool ShowFreeSpace
        {
            get
            {
                return showFreeSpace;
            }
            set
            {
                if (value != showFreeSpace)
                {
                    showFreeSpace = value;
                    RootPath = RootPath;
                }
            }
        }

        public TreeMap()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            if (!DesignMode)
            {
                //ReloadColorScheme();
            }
            InitializeComponent();
        }

        //public void ReloadColorScheme()
        //{
        //    ColorScheme colorScheme = new ColorScheme();
        //    if (File.Exists(Path.Combine(Application.StartupPath, "ColorScheme.xml")))
        //    {
        //        colorScheme.ReadXml(Path.Combine(Application.StartupPath, "ColorScheme.xml"));
        //        foreach (ColorScheme.ColorSchemeRow cs in colorScheme._ColorScheme.Rows)
        //        {
        //            extensions[cs.Extension.ToLower()] = Color.FromArgb(cs.A, cs.R, cs.G, cs.B);
        //        }
        //    }
        //}

        public string RootPath
        {
            get
            {
                return root.getLabel();
            }
            set
            {
                Cursor c = Cursor;
                Cursor = Cursors.WaitCursor;
                highLighted = null;
                if (value != "")
                {
                    if (!DesignMode)
                    {
                        root = new TreeMapNode(value);
                        root.setWidth(Width - 1);
                        root.setHeight(Height - 1);
                        if (showFreeSpace && Path.GetPathRoot(value) == value)
                        {
                            //DriveInfo di = new DriveInfo(value);
                            TreeMapNode freeSpace = new TreeMapNode("Free space", 0, new DefaultValue(0));
                            root.add(freeSpace);
                            root.setWeight(populateTree(root) + freeSpace.getWeight());
                        }
                        else {
                            root.setWeight(populateTree(root));
                        }
                        root.setValue(new DefaultValue(root.getWeight()));
                        SplitSquarified ss = new SplitSquarified();
                        ss.setBorder(0);
                        ss.calculatePositions(root);
                        repaint = true;
                        Invalidate();
                    }
                    if (RootPathChanged != null)
                    {
                        RootPathChanged(this, new EventArgs());
                    }
                }
                Cursor = c;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (repaint)
            {
                if (buffer == null)
                {
                    buffer = new Bitmap(ClientSize.Width, ClientSize.Height, e.Graphics);
                    bufferGraphics = Graphics.FromImage(buffer);
                }
                paint(root, bufferGraphics, 0);
                repaint = false;
            }

            e.Graphics.DrawImageUnscaled(buffer, 0, 0);
            if (highLighted != null)
            {
                if (highLightedIsDir)
                {
                    e.Graphics.DrawRectangle(yellow2, highLighted.Bounds);
                }
                else if (highLighted.Parent != null)
                {
                    e.Graphics.DrawRectangle(yellow2, highLighted.Parent.Bounds);
                }
                if (!highLightedIsDir)
                {
                    e.Graphics.DrawRectangle(Pens.Red, (int)Math.Round(highLighted.getX()), (int)Math.Round(highLighted.getY()),
                        (int)Math.Round(highLighted.getWidth() - 1D), (int)Math.Round(highLighted.getHeight() - 1D));
                }
            }
        }

        private void paint(TreeMapNode n, Graphics g, int level)
        {
            if (n.IsLeaf)
            {
                RectangleF rect = new RectangleF((float)n.getX(), (float)n.getY(), (float)n.getWidth(), (float)n.getHeight());
                if (rect.Width > 0 && rect.Height > 0)
                {
                    gp.Reset();
                    gp.AddEllipse(rect.Left - rect.Width / 4, rect.Top - rect.Height / 4, rect.Width + 2 * rect.Width / 4,
                        rect.Height + 2 * rect.Height / 4);
                    PathGradientBrush pgb = new PathGradientBrush(gp);
                    pgb.CenterColor = Color.White;
                    string ext = Path.GetExtension(n.getLabel()).TrimStart('.');
                    if (extensions.ContainsKey(ext.ToLower()))
                    {
                        pgb.SurroundColors = new Color[] { (Color)extensions[ext.ToLower()] };
                    }
                    else {
                        Color c = Color.FromKnownColor((KnownColor)((int)KnownColor.ForestGreen + extensions.Count));
                        extensions[ext.ToLower()] = c;
                        pgb.SurroundColors = new Color[] { c };
                    }
                    pgb.CenterPoint = new PointF(rect.Left + rect.Width / 4, rect.Top + rect.Height / 2);

                    g.FillRectangle(pgb, rect);
                    pgb.Dispose();

                    string label = Path.GetFileName(n.getLabel());
                    SizeF labelSize = g.MeasureString(label, f, (int)rect.Width);

                    if (rect.Width > 1 && rect.Height > 1 && File.Exists(n.getLabel()))
                    {
                        if (n.Icon == null)
                        {
                            n.Icon = Icon.ExtractAssociatedIcon(n.getLabel());
                        }
                        if (rect.Width >= n.Icon.Width && rect.Height >= n.Icon.Height + labelSize.Height)
                        {
                            Rectangle iconRect = new Rectangle((int)rect.Left + (int)rect.Width / 2 - n.Icon.Width / 2,
                                (int)rect.Top + (int)rect.Height / 2 - n.Icon.Height / 2 - (int)labelSize.Height / 2, n.Icon.Width, n.Icon.Height);
                            if (clipBoard == n && cutClipboard)
                            {
                                ImageAttributes ia = new ImageAttributes();
                                ia.SetGamma(0.3F);
                                g.DrawImage(n.Icon.ToBitmap(), iconRect, 0, 0, n.Icon.Width, n.Icon.Height, GraphicsUnit.Pixel, ia);
                            }
                            else {
                                g.DrawIconUnstretched(n.Icon, iconRect);
                            }
                            Region clip = g.Clip;
                            g.Clip = new Region(rect);
                            g.DrawString(label, f, Brushes.Black, new RectangleF(rect.Left + rect.Width / 2 - labelSize.Width / 2,
                                rect.Top + rect.Height / 2 - labelSize.Height / 2 + iconRect.Height / 2, labelSize.Width, labelSize.Height));
                            g.Clip = clip;
                        }
                        else {
                            double ratio = Math.Min(rect.Width / n.Icon.Width, rect.Height / n.Icon.Height);
                            if (clipBoard == n && cutClipboard)
                            {
                                ImageAttributes ia = new ImageAttributes();
                                ia.SetGamma(0.3F);
                                g.DrawImage(n.Icon.ToBitmap(), new Rectangle((int)Math.Round(rect.Left + rect.Width / 2 - n.Icon.Width * ratio / 2),
                                    (int)Math.Round(rect.Top + rect.Height / 2 - n.Icon.Height * ratio / 2),
                                    (int)Math.Round(n.Icon.Width * ratio), (int)Math.Round(n.Icon.Height * ratio)), 0, 0, n.Icon.Width, n.Icon.Height,
                                    GraphicsUnit.Pixel, ia);
                            }
                            else {
                                g.DrawIcon(n.Icon, new Rectangle((int)Math.Round(rect.Left + rect.Width / 2 - n.Icon.Width * ratio / 2),
                                    (int)Math.Round(rect.Top + rect.Height / 2 - n.Icon.Height * ratio / 2),
                                    (int)Math.Round(n.Icon.Width * ratio), (int)Math.Round(n.Icon.Height * ratio)));
                            }
                        }
                    }
                }
            }
            else if (level >1)
            {
                RectangleF rect = new RectangleF((float)n.getX(), (float)n.getY(), (float)n.getWidth(), (float)n.getHeight());
                if (rect.Width > 0 && rect.Height > 0)
                {
                    gp.Reset();
                    gp.AddEllipse(rect.Left - rect.Width / 4, rect.Top - rect.Height / 4, rect.Width + 2 * rect.Width / 4,
                        rect.Height + 2 * rect.Height / 4);
                    PathGradientBrush pgb = new PathGradientBrush(gp);
                    pgb.CenterColor = Color.White;
                    pgb.SurroundColors = new Color[] { Color.YellowGreen };
                    pgb.CenterPoint = new PointF(rect.Left + rect.Width / 4, rect.Top + rect.Height / 2);

                    g.FillRectangle(pgb, rect);
                    pgb.Dispose();

                    string label = Path.GetFileName(n.getLabel());
                    SizeF labelSize = g.MeasureString(label, f);
                    Region clip = g.Clip;
                    g.Clip = new Region(rect);
                    g.DrawString(label, f, Brushes.Black, new RectangleF(rect.Left + rect.Width / 2 - labelSize.Width / 2,
                        rect.Top + rect.Height / 2 - labelSize.Height / 2, labelSize.Width, labelSize.Height));
                    g.Clip = clip;
                }
            }
            if (depthLevel == 0 || level < depthLevel)
            {
                foreach (TreeMapNode child in n.Nodes)
                {
                    paint(child, g, level + 1);
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (buffer != null)
            {
                buffer.Dispose();
            }
            if (Width > 0 && Height > 0)
            {
                buffer = new Bitmap(Width, Height, CreateGraphics());
                if (bufferGraphics != null)
                {
                    bufferGraphics.Dispose();
                }
                bufferGraphics = Graphics.FromImage(buffer);
                Cursor c = Cursor;
                Cursor = Cursors.WaitCursor;
                SplitSquarified ss = new SplitSquarified();
                ss.setBorder(0);
                root.setWidth(Width);
                root.setHeight(Height);
                if (!DesignMode)
                {
                    ss.calculatePositions(root);
                }
                repaint = true;
                Cursor = c;
                Invalidate();
            }
            base.OnSizeChanged(e);
        }

        private long populateTree(TreeMapNode root)
        {
            DirectoryInfo d = new DirectoryInfo(root.getLabel());
            try
            {
                long size = 0L;
                foreach (DirectoryInfo dir in d.GetDirectories())
                {
                    TreeMapNode node = new TreeMapNode(dir.FullName);
                    long weight = populateTree(node);
                    node.setWeight(weight);
                    node.setValue(new DefaultValue(weight));
                    root.add(node);
                    size += weight;
                }
                foreach (FileInfo file in d.GetFiles())
                {
                    size += file.Length;
                    TreeMapNode node = new TreeMapNode(file.FullName, file.Length, new DefaultValue(file.Length));
                    root.add(node);
                }
                return size;
            }
            catch
            {
                return 0;
            }
        }

        private TreeMapNode FindNodeAt(TreeMapNode root, int x, int y, int level)
        {
            if (root.Bounds.Contains(x, y))
            {
                if (depthLevel == 0 || level < depthLevel)
                {
                    foreach (TreeMapNode node in root.Nodes)
                    {
                        TreeMapNode n = FindNodeAt(node, x, y, level + 1);
                        if (n != null)
                        {
                            return n;
                        }
                    }
                    return root;
                }
                else {
                    return root;
                }
            }
            else {
                return null;
            }
        }

    }
}
