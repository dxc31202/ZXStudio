using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ZXStudio
{
    public partial class OpenDialog : Form
    {
        public OpenDialog()
        {
            InitializeComponent();
            folderTreeView1.ComboFilebar = comboFilebar1;
            folderTreeView1.ListViewEx = fileListView1;
            fileListView1.FolderTreeView = folderTreeView1;
            foreach (Control control in Controls)
            {
                AddEvent(control);
            }

            comboFilebar1.ItemSelectedEvent += ComboFilebar1_ItemSelectedEvent;
            folderListBox1.ItemSelectedEvent += ComboFilebar1_ItemSelectedEvent;
        }

        private void ComboFilebar1_ItemSelectedEvent(object sender, Controls.ItemSelectedEventArgs e)
        {
            if (e.PIDL == null) return;
            folderTreeView1.BeginUpdate();

            if (folderTreeView1.CurrentNode.Nodes.Count == 1)
                if (folderTreeView1.CurrentNode.Nodes[0].Text == "")
                    folderTreeView1.Load(folderTreeView1.CurrentNode);

            TreeNode[] nodes = folderTreeView1.Nodes.Find(e.PIDL.DisplayName, true);
            if (nodes.Length == 0)
                nodes = folderTreeView1.Nodes.Find(e.PIDL.PhysicalPath, true);
            if (nodes.Length == 0)
            {
                string[] parts = e.PIDL.PhysicalPath.Split('\\');
                string path = parts[0] + @"\";
                TreeNode parent = null;
                for (int i = 1; i < parts.Length; i++)
                {

                    nodes = folderTreeView1.Nodes.Find(path, true);
                    if (nodes.Length > 0)
                        parent = nodes[0];
                    if (nodes.Length > 0)
                        folderTreeView1.Load(parent);
                    path = Path.Combine(path, parts[i]);
                }

                nodes = folderTreeView1.Nodes.Find(e.PIDL.PhysicalPath, true);
            }

            if (nodes.Length > 0)
                folderTreeView1.Load(nodes[0]);


            //TreeNode treeNode = SelectTreeNode(folderTreeView1.CurrentNode, e.PIDL);
            //if (treeNode == null)
            //    treeNode = SelectTreeNode(folderTreeView1.SelectedNode, e.PIDL);
            //if (treeNode == null)
            //    treeNode = SelectTreeNode(folderTreeView1.Nodes[0], e.PIDL);

            folderTreeView1.EndUpdate();

            fileListView1.Load(folderTreeView1.CurrentNode);

        }

        TreeNode SelectTreeNode(TreeNode treenode, PIDL pidl)
        {
            if (treenode == null) return null;
            if (treenode.Tag == null) return null;
            if (pidl != null)
                if (pidl.Equals(treenode.Tag))
                    return treenode;
            if (pidl != null)
                if (pidl.DisplayName== ((PIDL)treenode.Tag).DisplayName && pidl.PhysicalPath== ((PIDL)treenode.Tag).PhysicalPath)
                return treenode;
            foreach (TreeNode node in treenode.Nodes)
            {
                TreeNode found = SelectTreeNode(node, pidl);
                if (found != null)
                    return found;
            }
            return null;

        }
        void AddEvent(Control control)
        {
            if (control is Controls.ComboFilebar) return;
            control.Click += Control_Click;
            foreach (Control c in control.Controls)
            {
                AddEvent(c);
            }

        }
        private void Control_Click(object sender, EventArgs e)
        {
            comboFilebar1.CloseFileList();
        }

        private void fileListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fileListView1.SelectedItems.Count != 1) return;
            PIDL pidl ;
            if (fileListView1.SelectedItems[0].Tag == null)
            {
                if (fileListView1.ParentPIDL == null) return;
                pidl = (PIDL)fileListView1.ParentPIDL.Clone();

                return;

            }
            pidl=(PIDL) fileListView1.Root.Clone();// (PIDL)((PIDL)fileListView1.SelectedItems[0].Tag).Clone();


            if (!File.Exists(pidl.PhysicalPath))
                comboFilebar1.Data = pidl;
            //using (OpenFileDialog ofd = new OpenFileDialog())
            //{
            //    ofd.ShowDialog();
            //}
        }

        private void folderTreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;
            if (e.Node.Tag == null) return;
            PIDL pidl;
            pidl = (PIDL)((PIDL)e.Node.Tag).Clone();

            

        }

        private void OpenDialog_Load(object sender, EventArgs e)
        {
            comboFilebar1.FolderSelectedEventHandler += ComboFilebar1_FolderSelectedEventHandler;
        }

        private PIDL ComboFilebar1_FolderSelectedEventHandler(object sender, Controls.FolderSelectedEventArgs e)
        {
            if (e.PIDL == null)
            {
                folderListBox1.Visible = false;
                return null;
            }
            if (e.PIDL == null) return null;

            SizeF size;
            using (Graphics g = Graphics.FromHwnd(Handle))
            {
                size = g.MeasureString(e.PIDL.DisplayName, comboFilebar1.Font);
            }
                folderListBox1.Location = new Point((int)(e.Location -16), comboFilebar1.Top + comboFilebar1.Height-1);
            //if (e.Hotspot)
            {
                LoadFiles((PIDL)e.PIDL.Clone(), (PIDL)e.SelectedPIDL);
                folderListBox1.Focus();
            }
            //if (iconListBox1.Items.Count == 0)
            //    iconListBox1.Visible = false;
            folderListBox1.Visible = true;
                return null;
            //return (PIDL)((PIDL)folderListBox1.SelectedPIDL).Clone();
        }
        string lastDisplayName;
        public void LoadFiles(PIDL pidl,PIDL selectedPIDL)
        {
            folderListBox1.IsDesktop = (pidl.DisplayName == "My Computer");
            folderListBox1.SelectedPIDL =selectedPIDL;
            if (lastDisplayName == pidl.DisplayName) return;
            bool showHiddenObjects = false;
            bool showNonFolders = false;
            bool sortResults = true;

            ShellFolder folder;
            if (pidl.DisplayName == "My Computer")
            {
                folder = new ShellFolder(Environment.SpecialFolder.Desktop);
            }
            else
                folder = new ShellFolder(pidl);
            {
                ArrayList children = folder.GetChildren(showHiddenObjects, showNonFolders, sortResults);
                {
                    folderListBox1.AddFolders(children);
                }

            }
            folder.Dispose();
            lastDisplayName = pidl.DisplayName;
        }

    }
}
