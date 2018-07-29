using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ZXStudio.Controls
{
    public class TreeMapEx : PictureBox
    {
        TreeMapItem treemap;
        DiskReader diskReader;
        PIDL root;
        public TreeMapEx()
        {
            
            this.Image = new Bitmap(this.Width, this.Height);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();
        }
        TreeNode TreeNode;
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible)
                if(TreeNode!= null)
                    Load(TreeNode);
            base.OnVisibleChanged(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {

            
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            tooltip.Hide(this);
            base.OnMouseLeave(e);
        }
        public void Load(TreeNode parent)
        {
            this.TreeNode = parent;
            if (!Visible) return;
            currentObject = null;
            disk = null;
            treemap = null;
            if (root != null) root.Dispose();
            root = null;
            if (diskReader != null) diskReader.Cancel = true;
            diskReader = null;
            if (copy != null) copy.Dispose();
            copy = null;

            root = (PIDL)((PIDL)parent.Tag).Clone();
            if (root.PhysicalPath.Length > 0)
            {
                using (Graphics g = Graphics.FromImage(Image))
                {
                    g.Clear(Color.White);
                    WriteCaption(g, "Loading " + " " + root.DisplayName, new Rectangle(0, 0,this.Width, this.Height), Color.Black, true, 30f);
                    this.Invoke(new UpdateScreenCallback(this.Refresh), null);
                }
                    diskReader = new DiskReader(root.PhysicalPath, null);
                diskReader.DiskReadComplete += DiskReader_DiskReadComplete;
                diskReader.Progress += DiskReader_Progress;
            }
        }

        private void DiskReader_Progress(float value, ListViewItem item)
        {
            float percent = value;

            try
            {
                using (Graphics graphics = CreateGraphics())
                {
                    //graphics.Clear(Color.White);
                    Rectangle Bounds = new Rectangle(0, Height/2+50, Width-10, 30);
                    float width = Bounds.Width * percent / 100;
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;


                    Rectangle fillrect = new Rectangle(Bounds.X + 2, Bounds.Y + 5, (int)width - 2, Bounds.Height - 10);
                    Rectangle backrect = new Rectangle(Bounds.X + 2, Bounds.Y + 5, Bounds.Width - 2, Bounds.Height - 10);
                    Rectangle textrect = new Rectangle(Bounds.X, Bounds.Y + 5, Bounds.Width, Bounds.Height - 12);
                    Rectangle borderrect = new Rectangle(Bounds.X + 1, Bounds.Y + 4, Bounds.Width - 2, Bounds.Height - 8);

                    LinearGradientBrush lb = new LinearGradientBrush(fillrect, Color.FromArgb(255, Color.Green), Color.FromArgb(0, Color.Wheat), LinearGradientMode.Vertical);
                    graphics.FillRectangle(Brushes.Wheat, backrect);
                    graphics.FillRectangle(lb, fillrect);

                    float text = percent / 100;
                    graphics.DrawString(text.ToString("#,##0.0%"), progressFont, Brushes.Navy, textrect, sf);
                    DrawRoundedRectangle(graphics, borderrect, 8, Pens.Black);
                    lb.Dispose();

                }
            }
            catch { }
        }
        Font progressFont = new Font("Courier New", 7, FontStyle.Bold);

        void DrawRoundedRectangle(Graphics g, Rectangle r, int d, Pen p)
        {

            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(r.X, r.Y, d, d, 180, 90);
            gp.AddArc(r.X + r.Width - d, r.Y, d, d, 270, 90);
            gp.AddArc(r.X + r.Width - d, r.Y + r.Height - d, d, d, 0, 90);
            gp.AddArc(r.X, r.Y + r.Height - d, d, d, 90, 90);
            gp.AddLine(r.X, r.Y + r.Height - d, r.X, r.Y + d / 2);

            g.DrawPath(p, gp);
        }


        private void DiskReader_DiskReadComplete(DiskReader reader, long Elapsed, ListViewItem item)
        {
            RePlot(reader);
            //treemap = new TreeMapItem();
            //treemap.Plot(diskReader, new System.Drawing.RectangleF(0, 0, Width, Height));
            //using (Bitmap copy = new Bitmap(Width, Height))
            //{
                
            //    using (Graphics clientDC = Graphics.FromImage(Image))
            //    {
            //        using (Graphics offScreenDC = Graphics.FromImage(copy))
            //        {
            //            ReDraw(offScreenDC);
            //            clientDC.DrawImage(copy, 0, 0);
            //            this.Invoke(new UpdateScreenCallback(Refresh), null);
            //        }
            //    }

            //}
        }

        DiskItem currentObject;
        DiskItem disk;
        public void RePlot(DiskItem disk)
        {
            if (disk.Type ==DiskItem.NodeType.Disk)
                this.disk = (DiskItem)disk;
            currentObject = disk;
            RePlot();
        }


        public void RePlot()
        {

            treemap = null;
            treemap = new TreeMapItem();

            try
            {
                prm prm = new prm();
                prm.treemap = this;
                prm.treemapitem = treemap;
                prm.fileitem = currentObject;
                prm.rect = new RectangleF(0, 0, this.Width, this.Height);
                Thread myThread;
                myThread = new Thread(new ParameterizedThreadStart(Plotter), 10000000);
                myThread.IsBackground = false;

                myThread.Start(prm);
                //treemap.Plot(currentObject, new RectangleF(0, 0, this.Width, this.Height));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            GC.Collect();

        }
        Bitmap copy;
        void Plotter(object prms)
        {
            if (diskReader == null) return;
            prm p = (prm)prms;
            copy = new Bitmap(this.Width, this.Height);
            
            using (Graphics offScreenDC = Graphics.FromImage(copy))
            {
                using (Graphics g = Graphics.FromImage(Image))
                {
                    g.Clear(Color.White);
                    WriteCaption(g, "Plotting " + " " + root.DisplayName, new Rectangle(0, 0, this.Width, this.Height), Color.Black, true, 30f);
                    this.Invoke(new UpdateScreenCallback(this.Refresh), null);
                    p.treemapitem.Plot(p.fileitem, new RectangleF(0, 0, this.Width, this.Height));

                    g.Clear(Color.White);
                    WriteCaption(g, "Drawing " + " " + root.DisplayName, new Rectangle(0, 0, this.Width, this.Height), Color.Black, true, 30f);
                    this.Invoke(new UpdateScreenCallback(this.Refresh), null);
                    ReDraw(offScreenDC);

                    g.Clear(Color.White);
                    this.Image = new Bitmap(this.Width, this.Height);
                    WriteCaption(g, "Refreshing " + " " + root.DisplayName, new Rectangle(0, 0, this.Width, this.Height), Color.Black, true, 30f);
                    this.Invoke(new UpdateScreenCallback(this.Refresh), null);
                }
                using (Graphics clientDC = Graphics.FromImage(p.treemap.Image))
                {
                    clientDC.DrawImage(copy, 0, 0);
                }
                this.Invoke(new UpdateScreenCallback(this.Refresh), null);
            }
        }

        struct prm
        {
            public TreeMapEx treemap;
            public TreeMapItem treemapitem;
            public DiskItem fileitem;
            public RectangleF rect;
        }
        delegate void UpdateScreenCallback();

        public void ReDraw(Graphics offScreenDC)
        {
            if (currentObject == null) return;
            using (Brush brush = new SolidBrush(Color.Orange))
            {
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                offScreenDC.FillRectangle(brush, 0, 0, this.Width, this.Height);
                WriteCaption(offScreenDC, currentObject.Name + " " + FormatSize(currentObject.Size), new RectangleF(2, 0, this.Width, 13), GetContrast(Color.Orange), false, 7f);

                Draw(offScreenDC, treemap.TreeMapItems);

            }
        }

        void Draw(Graphics offScreenDC, List<TreeMapItem> displayObject)
        {
            for (int i = 0; i < displayObject.Count; i++)
            {
                float x = displayObject[i].Rectangle.X;
                float y = displayObject[i].Rectangle.Y;
                float width = displayObject[i].Rectangle.Width;
                float height = displayObject[i].Rectangle.Height;
                //if (displayObject[i].FileItem.Depth < DetailLevel)
                {
                    if (displayObject[i].DiskItem.Type== DiskItem.NodeType.File)
                    {
                        DrawFile(offScreenDC, displayObject[i].Rectangle, displayObject[i].DisplayName, false, displayObject[i].DiskItem);
                    }
                    else
                    {
                        DrawFolder(offScreenDC, displayObject[i].Rectangle, displayObject[i].DisplayName, false, displayObject[i].DiskItem);
                        Draw(offScreenDC, displayObject[i].TreeMapItems);
                    }
                }
            }
        }
        StringFormat stringFormat = new StringFormat();
        Color contrast = Color.FromArgb(255, 10, 33, 76);
        void DrawFile(Graphics offScreenDC, RectangleF rect, string text, bool highlight, DiskItem fileObject)
        {
            //if (fileObject.Depth > detailLevel) return;
            float x = rect.X;
            float y = rect.Y;
            float width = rect.Width;
            float height = rect.Height;

            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            stringFormat.Trimming = StringTrimming.Character;
            stringFormat.FormatFlags = StringFormatFlags.NoWrap;

            Color color = GetFileTypeColor(fileObject.Name);

            contrast = ControlPaint.Light(color);
            Color bright = ControlPaint.LightLight(Color.WhiteSmoke);
            Color dull = ControlPaint.Light(bright);
            //if (fileObject.IsHidden && fileObject.IsSystem)
                color = ControlPaint.Dark(color);
            //else
            //    if (fileObject.IsHidden)
            //    color = ControlPaint.Dark(color);
            //else
            //        if (fileObject.IsSystem)
            //    color = ControlPaint.Dark(color);

            if (rect.Height >= 2 && rect.Width >= 2)
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddRectangle(rect);
                    using (PathGradientBrush fileBrush = new PathGradientBrush(path))
                    {
                        Color[] colors = { highlight ? contrast : color };
                        fileBrush.SurroundColors = colors;
                        fileBrush.CenterColor = dull;// highlight ? bright : dull;
                        offScreenDC.FillPath(fileBrush, path);
                    }
                }
            else
            {
                if (rect.Height > 0 && rect.Width > 0)
                    using (Brush fileBrush = new SolidBrush(highlight ? contrast : color))
                        offScreenDC.FillRectangle(fileBrush, rect.X, rect.Y, rect.Width, rect.Height);
            }
            WriteCaption(offScreenDC, text + Environment.NewLine + FormatSize(fileObject.Size), new RectangleF(x + 2, y, width, height - 2), GetContrast(color), highlight, 7f);
            using (Pen pen = new Pen(highlight ? dull : Color.Gray))
                offScreenDC.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        void DrawFolder(Graphics offScreenDC, RectangleF rect, string text, bool highlight, DiskItem fileObject)
        {
            //if (fileObject.Depth > detailLevel) return;
            float x = rect.X;
            float y = rect.Y;
            float width = rect.Width;
            float height = rect.Height;
            Color color = Color.Orange;
            //if (fileObject.IsHidden || fileObject.IsSystem) color = ControlPaint.Dark(color);
            using (Brush folderBrush = new SolidBrush(highlight ? fileObject.Parent == null ? color : ControlPaint.LightLight(color) : color))
            {

                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Near;
                stringFormat.Trimming = StringTrimming.Character;
                stringFormat.FormatFlags = StringFormatFlags.NoWrap;
                using (Pen folderPen = new Pen(folderBrush))
                {
                    if (height > 15)
                        offScreenDC.FillRectangle(folderBrush, x, y, width, 15);
                    else
                        offScreenDC.FillRectangle(folderBrush, x, y, width, height);

                    using (Pen pen = new Pen(highlight ? Color.White : Color.Black))
                    {
                        if (height >= 15)
                            offScreenDC.DrawRectangle(pen, rect.X, rect.Y, rect.Width, 15);
                        offScreenDC.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                        //g.DrawRectangle(folderPen, x + 1, y + 1, width - 2, height - 2);
                        float h = 13;
                        if (height < 13) h = height;
                        WriteCaption(offScreenDC, text + " " + FormatSize(fileObject.Size), new RectangleF(x + 2, y, width, h), GetContrast(color), highlight, 7f);
                    }
                }
            }
        }

        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern long StrFormatByteSize(long fileSize,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);

        public static string FormatSize(long size)
        {
            StringBuilder sbBuffer = new StringBuilder(20);
            StrFormatByteSize(size, sbBuffer, 20);
            return sbBuffer.ToString();
        }


        public Color GetFileTypeColor(string Filename)
        {
            string ext = Path.GetExtension(Filename).ToUpper();
            Color color = Color.Wheat;
            int code = ext.GetHashCode();
            //code >>= 8;
            int B = code & 0xF7;
            code >>= 8;
            int G = code & 0xF7;
            code >>= 8;
            int R = code & 0xF7;

            return ControlPaint.Light(Color.FromArgb(R, G, B));
        }

        public Color GetContrast(Color color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;
            int A = color.A;
            //int yiq = ((r * 299) + (g * 587) + (b * 114)) / 1000;

            //if (yiq >= 131.5)
            //    return Color.Black;
            //else
            //    return Color.White;
            return Color.FromArgb(255, r & 0xF, g & 0xF, b & 0xF);
            
        }

        Brush textBrush = new SolidBrush(Color.Black);
        
        void WriteCaption(Graphics g, string text, RectangleF rect, Color color, bool highlight, float maxHeight)
        {

            if (rect.Height < maxHeight) return;
            if (rect.Width < maxHeight) return;

            
            using (Brush textBrush = new SolidBrush(color))
            {
                using (Font textFont = new Font("Sans Serif", maxHeight, highlight ? FontStyle.Bold : FontStyle.Regular))
                {
                    try
                    {
                        g.DrawString(text, textFont, textBrush, rect, stringFormat);
                    }
                    catch { }
                }
            }
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            try
            {
                if(this.Image!=null)                this.Image.Dispose();
                this.Image = null;
                this.Image = new Bitmap(this.Width, this.Height);
                if (this.disk != null)
                {
                    RePlot();
                    this.Invalidate();
                }
            }
            catch { }
        }

        string lastTooltip = "";
        int X, Y;
        internal TreeMapItem foundNode = null;
        internal TreeMapItem lastFoundNode = null;
        ImageToolTip tooltip = new ImageToolTip();

        public int TooltipImageSize
        {
            get
            {
                return tooltip.ImageSize;
            }

            set
            {
                tooltip.ImageSize = value;
            }
        }

        void FindNode(TreeMapItem node, float x, float y)
        {
            if (node == null) return;

            if (node.Rectangle.Contains(x, y))
            {
                foundNode = node;
            }
            foreach (TreeMapItem n in node.TreeMapItems)
                FindNode(n, x, y);
        }

        internal void Highlight(Graphics g, bool highlight, TreeMapItem next)
        {

            while (next != null)
            {
                if (next.DiskItem.Type == DiskItem.NodeType.File)
                    DrawFile(g, next.Rectangle, next.DiskItem.Name, highlight, next.DiskItem);
                else
                    DrawFolder(g, next.Rectangle, next.DiskItem.Name, highlight, next.DiskItem);
                next = next.Parent;
            }
        }



        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.X == e.X && this.Y == e.Y) return;
            this.X = e.X;
            this.Y = e.Y;
            base.OnMouseMove(e);

            try
            {
                Task t = Task.Run(() =>
                {
                    lastFoundNode = foundNode;
                    FindNode(treemap, X, Y);
                });

                t.Wait();

                if (foundNode != null)
                {
                    
                    tooltip.Filename = foundNode.DiskItem.FullPath;
                    if (foundNode.DiskItem.Type == DiskItem.NodeType.Folder || foundNode.DiskItem.Type == DiskItem.NodeType.Disk)
                        tooltip.Caption = foundNode.DisplayName + Environment.NewLine +
                            " Size:" + foundNode.DiskItem.Size.ToString("#,##0") + " bytes" + Environment.NewLine +
                            " Files:" +foundNode.DiskItem.Files.ToString("#,##0") + Environment.NewLine +
                            " Folders:"+ foundNode.DiskItem.Folders.ToString("#,##0");

                    else
                        tooltip.Caption = foundNode.DisplayName + Environment.NewLine + 
                            " Size:" + foundNode.DiskItem.Size.ToString("#,##0") + " bytes";
                    tooltip.SetToolTip(this, tooltip.Caption);
                    using (Graphics offScreenDC = Graphics.FromImage(this.Image))
                    {
                        using (Graphics clientDC = Graphics.FromImage(this.Image)) clientDC.DrawImage(copy, 0, 0);
                        Highlight(offScreenDC, true, foundNode);
                    }
                    tooltip.Show(tooltip.Caption, this);
                }
                using (Graphics clientDC = this.CreateGraphics()) clientDC.DrawImage(this.Image, 0, 0);
                //{
                //    // Restore background
                //    using (Graphics clientDC = Graphics.FromImage(this.Image)) clientDC.DrawImage(copy, 0, 0);
                //    if (foundNode != null)
                //    {

                //        //Highlight(offScreenDC, true, foundNode);
                //        using (PIDL pidl = new PIDL(foundNode.FileItem.FullPath))
                //        {
                //            string counters = "";
                //            string attributes = "";
                //            string ishidden = "";
                //            string issystem = "";
                //            if (foundNode.FileItem.IsHidden) ishidden = "Hidden";
                //            if (foundNode.FileItem.IsSystem) issystem = "System";
                //            if (ishidden.Length > 0 || issystem.Length > 0)
                //                attributes = Environment.NewLine + ishidden + " " + issystem;

                //            if (foundNode.FileItem.FileItemType != FileItem.FileItemTypes.File)
                //                counters = Environment.NewLine + "Contains: " + foundNode.FileItem.Files.ToString("#,##0") + " Files, " + foundNode.FileItem.Folders.ToString("#,##0") + " Folders";
                //            tooltip.Filename = foundNode.FileItem.FullPath;
                //            tooltip.Caption = foundNode.DisplayName + Environment.NewLine
                //                + pidl.TypeName + Environment.NewLine
                //                + foundNode.FileItem.ActualSize.ToString("#,##0") + " bytes" + counters
                //                + attributes
                //            ;
                //            if (lastTooltip != tooltip.Caption)
                //                tooltip.SetToolTip(this, tooltip.Caption);
                //            lastTooltip = tooltip.Caption;
                //        }
                //    }
                //    using (Graphics clientDC = this.CreateGraphics()) clientDC.DrawImage(this.Image, 0, 0);
                //}
            }
            catch { }
        }


    }
}
