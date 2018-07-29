using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio.Controls
{
    public class DiskItem : IEnumerable<DiskItem>
    {
        public enum NodeType
        {
            Disk,
            Folder,
            File
        }
        long size;
        public string Name { get; set; }

        public long Size
        {
            get
            {

                if (Type == NodeType.File)
                    return this.size;
                long size = 0;
                foreach (DiskItem c in children)
                    size += c.Size;
                return size;
            }
            set
            {
                size = value;
            }
        }

        public long Files
        {
            get
            {

                if (Type == NodeType.File)
                    return 1;
                long files = 0;
                foreach (DiskItem c in children)
                    files += c.Files;
                return files;
            }
        }

        public long Folders
        {
            get
            {

                if (Type == NodeType.File)
                    return 0;
                long folders = 0;
                foreach (DiskItem c in children)
                {
                    if (c.Type == NodeType.Folder)
                        folders++;
                    folders += c.Folders;
                }
                return folders;
            }
        }

        public DiskItem Parent { get; set; }
        public NodeType Type { get; set; }
        private List<DiskItem> children;

        public DiskItem(DiskItem parent, string name, long size)
        {
            this.Name = name;
            this.Size = size;
            this.Parent = parent;
            Type = NodeType.File;
            children = new List<DiskItem>();
        }
        public DiskItem(DiskItem parent, string name)
        {
            this.Name = name;
            this.Size = 0;
            this.Parent = parent;
            Type = NodeType.Folder;
            children = new List<DiskItem>();
        }
        public DiskItem(string name)
        {
            this.Name = name;
            this.Size = 0;
            this.Parent = null;
            Type = NodeType.Disk;
            children = new List<DiskItem>();
        }

        public DiskItem Add(DiskItem value)
        {
            children.Add(value);
            return value;
        }

        public DiskItem this[int index]
        {
            get
            {
                return children[index];
            }
        }
        public string FullPath
        {
            get
            {
                string s = Name;
                DiskItem p = Parent;
                while (p != null)
                {
                    if(p.Name.EndsWith(@"\"))
                        s = p.Name + s;
                    else
                        s = p.Name + @"\" + s;
                    p = p.Parent;
                }
                return s;
            }
        }

        public int Level
        {
            get
            {
                int l = 0;
                DiskItem p = Parent;
                while (p != null)
                {
                    l++;
                    p = p.Parent;
                }
                return l;
            }
        }

        public DiskItem Find(string path)
        {
            if (FullPath == path)
                return this;
            foreach (DiskItem k in children)
            {
                DiskItem ts = k.Find(path);
                if (ts != null) return ts;
            }
            return null;
        }

        public override string ToString()
        {
            return Name.ToString() + ", " + Size.ToString("#,##0") + ", " + Type.ToString();
        }

        public bool HasFolders
        {
            get
            {
                foreach (DiskItem i in children)
                    if (i.Type == NodeType.Folder)
                        return true;
                return false;
            }

        }

        public List<DiskItem> Children
        {
            get
            {
                return children;
            }

            set
            {
                children = value;
            }
        }

        public IEnumerator<DiskItem> GetEnumerator()
        {
            return this.children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

}
