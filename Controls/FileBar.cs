using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;

namespace ZXStudio.Controls
{
    public delegate PIDL FolderSelectedEvent(object sender, FolderSelectedEventArgs e);
    public delegate void ItemSelectedEventHandler(object sender, ItemSelectedEventArgs e);
    class FileBar : Panel
    {
        public FileBar() : base()
        {
            DoubleBuffered = true;
            TabIndex = 0;
        }

        public event FolderSelectedEvent FolderSelectedEventHandler;
        public event ItemSelectedEventHandler ItemSelectedEvent;
        Items Items = new Items();
        object selectedPIDL;
        public object SelectedDIPL
        {
            get { return selectedPIDL; }
            set { selectedPIDL = value; }
        }
        SysImageList sysimagelist = new SysImageList(SysImageListSize.smallIcons);
        private PIDL data;
        public PIDL Data
        {
            get { return data; }
            set
            {
                data = value;
                Invalidate();

            }
        }

        protected PIDL OnFolderSelected(FolderSelectedEventArgs e)
        {
            click = false;
            if (FolderSelectedEventHandler != null)
                return FolderSelectedEventHandler(this, e);
            return null;
        }

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);
        private const Int32 CB_SETITEMHEIGHT = 0x153;
        int height;

        public new int Height
        {
            set
            {
                height = value;
            }
            get
            {
                return height;
            }
        }


        PIDL PIDL;
        Item selectedItem = null;
        Item lastselectedItem = null;
        int x, y;
        bool click = false;

        string lastDisplayName;

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                if (selected != null)
                {
                    if (selectable)
                    {
                        ItemSelectedEvent?.Invoke(this, new ItemSelectedEventArgs(selected.PIDL));
                        return;
                    }
                    if (!isopen && dropdown)
                    {
                        SelectedDIPL = null;
                        if (selectedIndex +1< Items.Count)
                            SelectedDIPL = Items[selectedIndex + 1].PIDL;
                        OnFolderSelected(new FolderSelectedEventArgs(selected.PIDL, dropdown, selected.Hotspot.X, SelectedDIPL));
                        isopen = true;
                    }
                    else
                    {
                        OnFolderSelected(new FolderSelectedEventArgs(null, false, 0, null));
                        dropdown = true;
                        selected = null;
                        isopen = false;

                    }
                }
                else
                {
                    OnFolderSelected(new FolderSelectedEventArgs(null, false, 0, null));
                    dropdown = true;
                    selected = null;
                    isopen = false;
                }
            lastselected = selected;

            base.OnMouseClick(e);
        }

        public void CloseFileList()
        {
            OnFolderSelected(new FolderSelectedEventArgs(null, false, 0, null));
            dropdown = true;
            selected = null;
            isopen = false;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            x = e.X;
            y = e.Y;
            Invalidate();
            if (isopen)
                if (selected != null)
                {
                    OnFolderSelected(new FolderSelectedEventArgs(selected.PIDL, dropdown, selected.Hotspot.X, SelectedDIPL));
                    isopen = true;
                }
            lastselected = selected;
            base.OnMouseMove(e);
        }

        ToolTip tooltip = new ToolTip();
        Brush Hilight = new SolidBrush(Color.FromArgb(0xCC, 0xE8, 0xFF));
        Pen Border = new Pen(Color.FromArgb(0x99, 0xD1, 0xFF));
        bool dropdown = true;
        bool selectable = false;
        protected override void OnPaint(PaintEventArgs e)
        {
            int count = 0;
            Items = new Items();
            if (data != null)
            {
                PIDL pidl = (PIDL)data.Clone();
                    while (!pidl.IsDesktop)
                    {
                        Items.Add(new Item(pidl));
                        pidl = (PIDL)pidl.GetParentFolder().Clone();
                        count++;
                    }
                    pidl.DisplayName = "My Computer";
                    Item item = Items.Add(new Item(pidl));
                item.Rectangle = new Rectangle(0, 3, 16, 16);
                item.X = 0;
                item.Y = 3;
                item.Hotspot = new Rectangle(17, 3, 17, 16);
                pidl.Dispose();

                int index = sysimagelist.IconIndex(data.Pidl);
                e.Graphics.DrawImage(sysimagelist.Icon(index).ToBitmap(), new Rectangle(3, 3, 16, 16));
                DrawItems(e.Graphics);
            }

            Rectangle rect = ClientRectangle;
            rect.Width--;
            rect.Height--;
            e.Graphics.DrawRectangle(RectPen, rect);
            base.OnPaint(e);
        }
        Pen RectPen = new Pen(Color.FromArgb(0xd9, 0xd9, 0xd9));
        Item selected = null;
        Item lastselected = null;
        int selectedIndex;
        bool isopen = false;
        void DrawItems(Graphics g)
        {
            selected = null;
            dropdown = false;
            selectable = false;
            SizeF size;
            int c = 21;
            bool skip = true;
            int left = c;
            int right = c + 16 + 3;
            Rectangle rect = new Rectangle(c, 0, 16, 20);
            if (left <= x && x <= c)
                selectable = true;
            if (left <= x && x <= right)
            {
                dropdown = true;
                g.FillRectangle(Hilight, rect);
                rect.Y++;
                rect.Height--;
                g.DrawRectangle(Pens.LightSkyBlue, rect);
                Items[Items.Count - 1].Rectangle = new Rectangle(0, 0, 35, 20);
                selected = Items[Items.Count - 1];
                selectedIndex = Items.Count - 1;
            }
            else
            {
                g.FillRectangle(Brushes.White, rect);
            }

            g.DrawImage(ZXStudio.Properties.Resources.Separator, new Point(c, 3));
            for (int i = Items.Count - 2; i >= 0; i--)
            {
                if (Items[i].DisplayName == "My Computer")
                {
                    if (skip)
                    {
                        skip = false;
                        continue;
                    }
                }
                size = g.MeasureString(Items[i].DisplayName, Font);
                c += 17;
                left = c;
                right = c + (int)size.Width;
                Items[i].X = c;
                Items[i].Y = 2;
                rect = new Rectangle(c, 0, (int)size.Width, 20);
                if (Items[i].PIDL.HasSubfolders)
                    rect.Width += 19;
                if (left <= x && x <= right)
                    selectable = true;
                if (left <= x && x <= right + 19)
                {
                    selected = Items[i];
                    selectedIndex = i;
                    g.FillRectangle(Hilight, rect);
                    rect.Y++;
                    rect.Height--;
                    g.DrawRectangle(Border, rect);
                    g.DrawString(Items[i].DisplayName, Font, System.Drawing.Brushes.Black, c, 2);
                }
                else
                {
                    g.FillRectangle(Brushes.White, rect);
                }
                g.DrawString(Items[i].DisplayName, Font, System.Drawing.Brushes.Black, c, 2);
                c += (int)size.Width + 3;
                Items[i].Hotspot = new Rectangle(c - 1, 3, 16, 16);
                left = c;
                right = c + 16;
                if (Items[i].PIDL.HasSubfolders)
                    if (left <= x && x <= right)
                    {
                        dropdown = true;
                        g.DrawLine(Border, Items[i].Hotspot.X, 2, Items[i].Hotspot.X, 19);
                        x = 0;
                    }
                    else
                    {
                        //g.FillRectangle(Brushes.White, ComboBoxExItems[i].Hotspot);
                    }
                if (Items[i].PIDL.HasSubfolders)
                    g.DrawImage(ZXStudio.Properties.Resources.Separator, new Point(c, 3));

            }
        }
    }

    class Items : IEnumerable
    {
        List<Item> items = new List<Item>();
        public Item Add(Item item)
        {
            items.Add(item);
            return item;
        }

        public int Count
        {
            get
            {
                return items.Count;
            }
        }
        public Item this[int index]
        {
            get
            {
                return items[index];
            }
        }

        // Implementation for the GetEnumerator method.
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (object o in items)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (o == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return o;
            }
        }

    }
    class Item
    {
        public bool Hilighted = false;
        public int X;
        public int Y;
        public PIDL PIDL;
        public Rectangle Rectangle;
        public Rectangle Hotspot;
        public Item(PIDL pidl)
        {
            PIDL = pidl;
        }

        public string DisplayName
        {
            get
            {
                if (PIDL != null)
                    return PIDL.DisplayName;
                return "";
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }
        public string PhysicalPath
        {
            get
            {
                return PIDL.PhysicalPath;
            }
        }
    }

    public class FolderSelectedEventArgs : EventArgs
    {
        public object SelectedPIDL;
        public PIDL PIDL;
        public bool Hotspot;
        public int Location;
        public FolderSelectedEventArgs(PIDL pidl, bool hotspot, int location, object selectedPIDL)
        {
            SelectedPIDL = selectedPIDL;
            PIDL = pidl;
            Hotspot = hotspot;
            Location = location;
        }
    }
    public class ItemSelectedEventArgs : EventArgs
    {
        public PIDL PIDL;
        public ItemSelectedEventArgs(PIDL pidl)
        {
            PIDL = pidl;
        }
    }
}
