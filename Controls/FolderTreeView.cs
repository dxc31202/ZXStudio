using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace ZXStudio.Controls
{
    class FolderTreeView : TreeView
    {

        public FileListView ListViewEx;
        public TreeMapEx TreeMapEx;
        public ComboFilebar ComboFilebar;
        ImageList smallimagelist = new ImageList();
        SysImageList sysimagelist = new SysImageList(SysImageListSize.smallIcons);
        public FolderTreeView()
        {
            DoubleBuffered = true;
            HideSelection = false;
            Nodes.Clear();
            if (!isDesignMode)
            {

                smallimagelist.ImageSize = new Size(16, 16);
                smallimagelist.TransparentColor = Color.Black;
                smallimagelist.ColorDepth = ColorDepth.Depth32Bit;
                this.ImageList = smallimagelist;
                using (ShellFolder folder = new ShellFolder(Environment.SpecialFolder.MyComputer))
                {
                    string key = folder.Pidl.PhysicalPath;
                    if (key == "")
                        key = folder.Pidl.DisplayName;
                    TreeNode node = Nodes.Add(key, folder.Pidl.DisplayName);
                    node.Tag = folder.Pidl.Clone();
                    node.Nodes.Add("");
                    node.ImageIndex = -1;
                    node.SelectedImageIndex = -1;
                    node.Expand();
                    SelectedNode = node;
                }
                SelectedNode=CurrentNode = Nodes[0];
                
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!this.DesignMode && Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6)
                NativeMethods.SetWindowTheme(this.Handle, "explorer", null);
            ItemHeight = 24;
            ShowRootLines = false;
            ShowLines = false;
        }

        void ClearNodes(TreeNode parent)
        {
            foreach (TreeNode node in parent.Nodes)
            {
                ClearNodes(node);
                if (node.Tag is PIDL) ((PIDL)node.Tag).Dispose();
                node.Tag = null;
            }
            parent.Nodes.Clear();
        }

        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            OnAfterSelect(new TreeViewEventArgs(SelectedNode));
            base.OnNodeMouseClick(e);
        }
        public TreeNode CurrentNode;
        List<TreeNode> TreeNodes;
        public void Load(TreeNode treenode)
        {
            if (treenode == null) return;
            if (treenode.Tag == null) return;
            GC.Collect();
            BeginUpdate();
            CurrentNode = treenode;
            SuspendLayout();
            if (ComboFilebar != null) ComboFilebar.Data = (PIDL)treenode.Tag;
            AddImage(Thumbnail.GetIcon(((PIDL)treenode.Tag).Pidl, 16), (PIDL)((PIDL)treenode.Tag).Clone(), treenode);
            treenode.Nodes.Clear();
            TreeNodes = new List<TreeNode>();
            using (PIDL pidl = ((PIDL)treenode.Tag).Clone() as PIDL)
            {
                treenode.Name = pidl.PhysicalPath == "" ? pidl.DisplayName : pidl.PhysicalPath;
                Cursor = Cursors.WaitCursor;
                using (ShellFolder folder = new ShellFolder(pidl))
                {
                    List<PIDL> children = folder.GetFolders(false);
                    foreach (PIDL child in children)
                    {
                        if (child.IsCompressed) continue;
                        string key = child.PhysicalPath;
                        if (key == "")
                            key = child.DisplayName;
                        TreeNode node = new TreeNode(child.DisplayName);
                        node=treenode.Nodes.Add(key, child.DisplayName,node.ImageIndex,node.SelectedImageIndex);
                        node.Name = key;
                        node.Tag = child;
                        if (child.DisplayName.ToLower() == "network" || child.HasChildren)
                            node.Nodes.Add("");

                        AddImage(Thumbnail.GetIcon(((PIDL)node.Tag).Pidl, 16), (PIDL)((PIDL)node.Tag).Clone(), node);
                    }
                }
            }
            if (ComboFilebar != null)
                ComboFilebar.SelectedPIDL = treenode.Tag;
            ResumeLayout();
            EndUpdate();
            
            Cursor = Cursors.Default;
        }
        bool ThreadRunning = false;
        bool Cancel = false;
        public void StopThread()
        {
            if (ThreadRunning)
            {
                Cancel = true;
                while (Cancel) { Globals.DoEvents(); }
            }

        }


        public void UpdateIcons(Object threadContext)
        {
            if (TreeNodes.Count == 0) return;
            if (Cancel) return;
            ThreadRunning = true;
            Cancel = false;
            foreach (TreeNode item in TreeNodes)
            {
                if (Cancel) break;

                Invoke(new AddImageDelegate(AddImage), new object[] { Thumbnail.GetIcon(((PIDL)item.Tag).Pidl, 16), (PIDL)((PIDL)item.Tag).Clone(), item });
            }
            Cancel = false;
            ThreadRunning = false;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (ListViewEx != null) ListViewEx.Load(GetNodeAt(e.X, e.Y));
            
            base.OnMouseClick(e);
        }
        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            CurrentNode = e.Node;
            
            if (ComboFilebar != null) ComboFilebar.Data = (PIDL)e.Node.Tag;
            if (ListViewEx != null) ListViewEx.Load(e.Node);
            if (TreeMapEx != null) TreeMapEx.Load(e.Node);
            base.OnBeforeSelect(e);
        }
        protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
        {
            if (e.Node == Nodes[0]) { e.Cancel = true; return; }
            if (e.Node.Nodes.Count > 0)
            {
                ClearNodes(e.Node);
                e.Node.Nodes.Add("");
            }
            else
                ClearNodes(e.Node);
            GC.Collect();
            base.OnBeforeCollapse(e);
        }

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            base.OnBeforeExpand(e);
            Load(e.Node);
        }

        private static bool isDesignMode
        {
            get
            {
                bool bProcCheck = false;
                using (System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess())
                {
                    bProcCheck = process.ProcessName.ToLower().Trim() == "devenv";
                }
                bool bModeCheck = (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime);
                return bProcCheck || bModeCheck;
            }
        }

        delegate void AddImageDelegate(IntPtr himage, PIDL pidl, TreeNode item);

        void AddImage(IntPtr himage, PIDL pidl, TreeNode item)
        {
            int index = smallimagelist.Images.IndexOfKey(item.Name);
            if (index < 0)
            {
                index = sysimagelist.IconIndex(pidl.Pidl);
                smallimagelist.Images.Add(sysimagelist.Icon(index).ToBitmap());
                index = smallimagelist.Images.Count - 1;
            }
            item.ImageIndex = index;
            item.SelectedImageIndex = index;

        }


    }
}
