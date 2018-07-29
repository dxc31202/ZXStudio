using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Diagnostics;
namespace ZXStudio.Controls
{
    class FileListView : ListView
    {
        public FolderTreeView FolderTreeView;
        List<DiskReader> diskReader;
        public PIDL Root;
        List<ListViewItem> ListViewItems;
        ImageList smallimagelist = new ImageList();
        ImageList largeimagelist = new ImageList();
        SysImageList sysimagelist = new SysImageList(SysImageListSize.largeIcons);
        TreeNode parent;
        public FileListView()
        {
            //SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            largeimagelist.Dispose();
            smallimagelist.Dispose();
            smallimagelist = new ImageList();
            largeimagelist = new ImageList();

            smallimagelist.ImageSize = new Size(16, 22);
            smallimagelist.ColorDepth = ColorDepth.Depth32Bit;
            SmallImageList = smallimagelist;

            largeimagelist.ImageSize = new Size(96, 96);
            largeimagelist.ColorDepth = ColorDepth.Depth32Bit;
            LargeImageList = largeimagelist;
            View = View.LargeIcon;
            Columns.Add("Name", 200);
            Columns.Add("Type", 100);
            Columns.Add("Size", 100);
            Columns.Add("Bytes", 120);
            Columns.Add("Files", 70);
            Columns.Add("Folders", 70);
            Columns[2].TextAlign = HorizontalAlignment.Right;
            Columns[3].TextAlign = HorizontalAlignment.Right;
            Columns[4].TextAlign = HorizontalAlignment.Right;
            Columns[5].TextAlign = HorizontalAlignment.Right;
            FullRowSelect = true;
            View = View.Details;

        }
        int x, y;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            x = e.X;
            y = e.Y;
            base.OnMouseMove(e);
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                ListViewItem item = GetItemAt(e.X, e.Y);
                if (item != null)
                {
                    item.Selected = true;

                    ShellContextMenu contextMenu = new ShellContextMenu();
                    using (PIDL pidl = ((PIDL)((PIDL)item.Tag).Clone()))
                    {

                        string path = pidl.PhysicalPath;

                            if (path.EndsWith("\\")) path = path.Substring(0, path.Length - 1);
                            FileInfo[] fileinfo = { new FileInfo(pidl.PhysicalPath) };
                            contextMenu.ShowContextMenu(Handle, fileinfo, Cursor.Position);
                    }
                }

            }

            base.OnMouseDown(e);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            //if (SelectedIndices.Count != 1) return;

            base.OnSelectedIndexChanged(e);
        }
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            if (SelectedItems.Count != 1) return;
            if (SelectedItems[0].Tag == null)
            {
                if (parent.Parent != null)
                {
                    FolderTreeView.Load(parent);
                    FolderTreeView.SelectedNode = parent;
                }
                return;
            }

            if(parent.Nodes.Count==1)
            {
                if (parent.Nodes[0].Name == "")
                {
                    FolderTreeView.Load(parent);
                    if (SelectedItems.Count > 0) {
                        for (int i = 0; i < parent.Nodes.Count; i++)
                        {
                            if (((PIDL)parent.Nodes[i].Tag).PhysicalPath == ((PIDL)SelectedItems[0].Tag).PhysicalPath)
                            {
                                parent = parent.Nodes[i];
                                FolderTreeView.Load(parent);
                                Load(parent);
                                if (Items.Count > 0)
                                    Items[0].Selected = true;
                                return;
                            }
                        }

                    }

                }


            }
            if (SelectedItems.Count > 0)
            {
                foreach (TreeNode node in parent.Nodes)
                {
                    if (((PIDL)node.Tag).PhysicalPath == ((PIDL)SelectedItems[0].Tag).PhysicalPath)
                    {
                        Load(node);
                        if (Items.Count == 0) return;
                        if (Items[0].Tag != null)
                            Items[0].Selected = true;

                        return;
                    }
                }
                Process myProcess = new Process();
                myProcess.StartInfo.FileName = ((PIDL)SelectedItems[0].Tag).PhysicalPath;
                myProcess.Start();
            }

        }

        public bool ShowHidden = false;

        public void Load(TreeNode parent)
        {
            if (parent == null) return;
            if (parent.Tag == null) return;
            if (Root!=null)
                if (Root.Pidl == ((PIDL)parent.Tag).Pidl)
                    return;
            if (diskReader != null)
                foreach (DiskReader reader in diskReader)
                    reader.Cancel = true;
            diskReader = new List<DiskReader>();
            this.parent = parent;
            BeginUpdate();
            SuspendLayout();
            Items.Clear();
            largeimagelist.Images.Clear();
            smallimagelist.Images.Clear();
            Root = (PIDL)((PIDL)parent.Tag).Clone();
            {

                ListViewItems = new List<ListViewItem>();
                //if (parent.Parent != null)
                //{
                //    using (PIDL owner = (PIDL)((PIDL)parent.Parent.Tag).Clone())
                //        ListViewItems.Add(new ListViewItem(owner.DisplayName));
                //}
                Cursor = Cursors.WaitCursor;
                using (ShellFolder folder = new ShellFolder(Root))
                {
                    List<PIDL> children = folder.GetFolders(ShowHidden);
                    if (children == null)
                    {
                        ResumeLayout();
                        EndUpdate();
                        Cursor = Cursors.Default;
                        return;
                    }
                    foreach (PIDL child in children)
                    {

                        ListViewItem item = new ListViewItem(child.DisplayName);
                        item.ImageIndex = -1;
                        item.Tag = child;
                        item.SubItems.Add(child.TypeName);

                        ListViewItems.Add(item);

                        item.Name = child.PhysicalPath + child.DisplayName;
                        if (child.IsFileSystem)
                        {
                            if (child.IsCompressed)
                            {
                                FileInfo fi = new FileInfo(child.PhysicalPath);
                                try
                                {
                                    item.SubItems.Add(StrFormatByteSize(fi.Length));
                                }
                                catch (Exception ex)
                                {
                                    item.SubItems.Add(ex.Message);
                                }
                            }
                            else
                            {
                                diskReader.Add(new DiskReader(child.PhysicalPath, item));
                                diskReader[diskReader.Count - 1].DiskReadComplete += DiskReader_DiskReadComplete;
                                diskReader[diskReader.Count - 1].Progress += ListViewEx_Progress;
                                item.SubItems.Add("");
                                item.BackColor = Color.Wheat;
                                item.UseItemStyleForSubItems = true;
                                item.SubItems.Add("");
                                item.SubItems.Add("");
                            }
                        }
                        else
                            item.SubItems.Add("");
                        item.SubItems.Add("");
                        item.Name = child.PhysicalPath + child.DisplayName;

                    }

                    children = folder.GetFiles(ShowHidden);
                    if (children == null) return;
                    foreach (PIDL child in children)
                    {
                        if (!child.IsFolder)
                        {
                            ListViewItem item = new ListViewItem(child.DisplayName);
                            item.ImageIndex = -1;
                            item.Tag = child;
                            item.SubItems.Add(child.TypeName);

                            ListViewItems.Add(item);
                            if (child.PhysicalPath.Length > 0)
                            {
                                FileInfo fi = new FileInfo(child.PhysicalPath);
                                item.SubItems.Add(StrFormatByteSize(fi.Length));
                            }
                            else
                                item.SubItems.Add("");

                            item.SubItems.Add("");
                        }

                    }
                    Items.AddRange(ListViewItems.ToArray());
                    ResumeLayout();
                    EndUpdate();
                    Cursor = Cursors.Default;
                    ThreadPool.QueueUserWorkItem(UpdateIcons);
                }
            }
        }

        private void ListViewEx_Progress(float value, ListViewItem item)
        {
            Invoke(new DrawSubProgressDelegate(DrawProgressBar), new object[] { item, value });
        }

        delegate void DrawSubProgressDelegate(ListViewItem sender, float value);
        private void DiskReader_DiskReadComplete(DiskReader reader, long Elapsed, ListViewItem item)
        {
            Invoke(new SetTextDelegate(SetSize), new object[] { StrFormatByteSize(reader.Size), item });
            Invoke(new SetTextDelegate(SetBytes), new object[] { reader.Size.ToString("#,##0"), item });
            Invoke(new SetTextDelegate(SetFiles), new object[] { reader.Files.ToString("#,##0"), item });
            Invoke(new SetTextDelegate(SetFolders), new object[] { reader.Folders.ToString("#,##0"), item });
        }

        void DrawProgressBar(ListViewItem sender, float value)
        {
            if (View != View.Details) return;
            sender.BackColor = Color.White;
            sender.UseItemStyleForSubItems = true;
            float percent = value;

            using (Graphics graphics = CreateGraphics())
            {
                Rectangle Bounds = sender.SubItems[2].Bounds;
                float width = Bounds.Width * percent / 100;
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;

                if ((int)width - 2 <= 0) return;
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



        delegate void SetTextDelegate(string text, ListViewItem item);
        void SetSize(string text, ListViewItem item)
        {
            //BeginUpdate();
            item.SubItems[2].Text = text;
            item.BackColor = Color.White;
            item.UseItemStyleForSubItems = true;
            //EndUpdate();
        }
        void SetBytes(string text, ListViewItem item)
        {
            //BeginUpdate();
            item.SubItems[3].Text = text;
            item.BackColor = Color.White;
            item.UseItemStyleForSubItems = true;
            //EndUpdate();
        }
        void SetFiles(string text, ListViewItem item)
        {
            //BeginUpdate();
            item.SubItems[4].Text = text;
            item.BackColor = Color.White;
            item.UseItemStyleForSubItems = true;
            //EndUpdate();
        }
        void SetFolders(string text, ListViewItem item)
        {
            //BeginUpdate();
            item.SubItems[5].Text = text;
            item.BackColor = Color.White;
            item.UseItemStyleForSubItems = true;
            //EndUpdate();
        }

        public void UpdateIcons(Object threadContext)
        {
            if (Items.Count == 0) return;
            if (Cancel) return;
            ThreadRunning = true;
            Cancel = false;
            foreach (ListViewItem item in ListViewItems)
            {
                if (Cancel) break;
                if (item.Tag != null)
                    Invoke(new AddImageDelegate(AddImage), new object[] { Thumbnail.GetThumbNail(((PIDL)item.Tag).Pidl, largeimagelist.ImageSize.Width), (PIDL)((PIDL)item.Tag).Clone(), item });
                else
                    Invoke(new AddImageDelegate2(AddImage2), new object[] { item });
            }
            Cancel = false;
            ThreadRunning = false;
        }
        bool ThreadRunning = false;
        bool Cancel = false;
        public void StopThread()
        {
            if (diskReader != null)
                foreach (DiskReader reader in diskReader)
                    reader.Cancel = true;

            if (ThreadRunning)
            {
                Cancel = true;
                while (Cancel) { Globals.DoEvents(); }
            }

        }

        delegate void AddImageDelegate(IntPtr himage, PIDL pidl, ListViewItem item);
        delegate void AddImageDelegate2(ListViewItem item);

        void AddImage2(ListViewItem item)
        {
            int imageSize = largeimagelist.ImageSize.Width;
            Bitmap bmp = ZXStudio.Properties.Resources.up_folder_icon_54527;
            if (largeimagelist.Images.IndexOfKey(item.Name) < 0)
            {
                switch (imageSize)
                {
                    case 16: sysimagelist = new SysImageList(SysImageListSize.smallIcons); break;
                    case 48: sysimagelist = new SysImageList(SysImageListSize.largeIcons); break;
                    case 96: sysimagelist = new SysImageList(SysImageListSize.extraLargeIcons); break;
                    case 256: sysimagelist = new SysImageList(SysImageListSize.JumboIcons); break;
                }
                LargeImageList.Images.Add(item.Name, Thumbnail.CreateAlphaBitmap(bmp, System.Drawing.Imaging.PixelFormat.Format32bppArgb));

                smallimagelist.Images.Add(item.Name, Thumbnail.CreateAlphaBitmap(bmp, System.Drawing.Imaging.PixelFormat.Format32bppArgb));
                item.ImageIndex = LargeImageList.Images.Count - 1;
            }
            else
            {

                smallimagelist.Images.Add(item.Name, Thumbnail.CentreImage(bmp, smallimagelist.ImageSize.Width, smallimagelist.ImageSize.Height));
                if ((bmp.Width != imageSize && bmp.Height != imageSize))// && pidl.PhysicalPath != "")
                    bmp = Thumbnail.CentreImage(bmp, imageSize, imageSize);
                else
                {
                    if (bmp.Width != imageSize || bmp.Height != imageSize)
                        bmp = Thumbnail.ScaleImage(bmp, imageSize, imageSize);
                }

                //Thumbnail.SaveThumb(pidl.Pidl, bmp);
                largeimagelist.Images.Add(item.Name, bmp);
            }

            item.ImageIndex = largeimagelist.Images.Count - 1;

        }
        void AddImage(IntPtr himage, PIDL pidl, ListViewItem item)
        {
            int imageSize = largeimagelist.ImageSize.Width;
            Bitmap bmp = null;

            //if (thumbDB.HasThumbnail(System.IO.Path.GetFileName(pidl.PhysicalPath)))
            //{
            //    using (Image img = thumbDB.GetThumbnailImage(System.IO.Path.GetFileName(pidl.PhysicalPath)))
            //    {
            //        largeimagelist.Images.Add(item.Name, Thumbnail.ScaleImage((Bitmap)img, imageSize, imageSize));
            //        item.ImageIndex = largeimagelist.Images.Count - 1;

            //        return;
            //    }

            //}
            try
            {
                if (largeimagelist.Images.IndexOfKey(item.Name) < 0)
                {
                    if (himage == IntPtr.Zero)
                    {
                        switch (imageSize)
                        {
                            case 16: sysimagelist = new SysImageList(SysImageListSize.smallIcons); break;
                            case 48: sysimagelist = new SysImageList(SysImageListSize.largeIcons); break;
                            case 96: sysimagelist = new SysImageList(SysImageListSize.extraLargeIcons); break;
                            case 256: sysimagelist = new SysImageList(SysImageListSize.JumboIcons); break;
                        }
                        int index = sysimagelist.IconIndex(pidl.Pidl);
                        LargeImageList.Images.Add(item.Name, Thumbnail.CreateAlphaBitmap(sysimagelist.Icon(index).ToBitmap(), System.Drawing.Imaging.PixelFormat.Format32bppArgb));

                        smallimagelist.Images.Add(item.Name, Thumbnail.CreateAlphaBitmap(sysimagelist.Icon(index).ToBitmap(), System.Drawing.Imaging.PixelFormat.Format32bppArgb));
                        item.ImageIndex = LargeImageList.Images.Count - 1;
                    }
                    else
                    {
                        //bmp = Thumbnail.GetCachedImage(pidl.Pidl, imageSize);
                        //if (bmp == null)
                        {
                            bmp = Thumbnail.GetBitmapFromHBitmap(himage);
                            {

                                smallimagelist.Images.Add(item.Name, Thumbnail.CentreImage(bmp, smallimagelist.ImageSize.Width, smallimagelist.ImageSize.Height));
                                if ((bmp.Width != imageSize && bmp.Height != imageSize))// && pidl.PhysicalPath != "")
                                    bmp = Thumbnail.CentreImage(bmp, imageSize, imageSize);
                                else
                                {
                                    if (bmp.Width != imageSize || bmp.Height != imageSize)
                                        bmp = Thumbnail.ScaleImage(bmp, imageSize, imageSize);
                                }
                            }
                            //Thumbnail.SaveThumb(pidl.Pidl, bmp);
                        }
                        largeimagelist.Images.Add(item.Name, bmp);

                        item.ImageIndex = largeimagelist.Images.Count - 1;
                    }
                    return;
                }
                item.ImageIndex = largeimagelist.Images.IndexOfKey(item.Name);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (bmp != null) bmp.Dispose();
                Thumbnail.DeleteObject(himage);
            }
        }

        public new View View
        {
            get { return base.View; }
            set
            {
                //if (base.View == value) return;
                //base.View = value;

                switch (value)
                {
                    //case View.Tile: base.View = View.Tile; largeimagelist.ImageSize = new Size(256, 256); break;
                    case View.LargeIcon: base.View = View.LargeIcon; largeimagelist.ImageSize = new Size(96, 96); break;
                    case View.SmallIcon: base.View = View.SmallIcon; largeimagelist.ImageSize = new Size(16, 22); break;
                    case View.Details: base.View = View.Details; largeimagelist.ImageSize = new Size(16, 22); break;
                }
                Load(parent);

            }
        }
        public static string StrFormatByteSize(long filesize)
        {
            StringBuilder sb = new StringBuilder(255);
            NativeMethods.StrFormatByteSize(filesize, sb, sb.Capacity);
            return sb.ToString();
        }

       public PIDL ParentPIDL
        {
            get
            {

                foreach (TreeNode node in parent.Nodes)
                {
                    if (SelectedItems[0].Tag == null)
                        break;
                    if (((PIDL)node.Tag).PhysicalPath == ((PIDL)SelectedItems[0].Tag).PhysicalPath)
                    {
                        return (PIDL)((PIDL)node.Tag).Clone();
                    }
                }
                return null;
            }
        }
    }

}



