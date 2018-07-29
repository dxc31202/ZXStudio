using System;
using System.Collections;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace ZXStudio.Controls
{
    class FolderListBox : Panel
    {
        public event ItemSelectedEventHandler ItemSelectedEvent;
        SysImageList sysimagelist = new SysImageList(SysImageListSize.smallIcons);
        public bool IsDesktop
        {
            get { return desktopPanel.Visible; }
            set { desktopPanel.Visible = value;
                if (value)
                {
                    DrawDesktop(BackColor);
                }
            }
        }

        public object SelectedPIDL
        {
            get {
                return iconListBox.SelectedItem;
            }
            set {
                iconListBox.SelectedItem= value;
            }
        }
        IconListBox iconListBox;
        Panel desktopPanel;
        PIDL desktop;
        public FolderListBox() : base()
        {
            Width = 267;
            BorderStyle = BorderStyle.FixedSingle;
            iconListBox = new IconListBox();
            iconListBox.ItemSelectedEvent += IconListBox_ItemSelectedEvent;
            desktopPanel = new Panel();
            desktopPanel.Height = 33;
            desktop = new PIDL(Environment.SpecialFolder.Desktop);
            iconListBox.BackColor = desktopPanel.BackColor = BackColor = Color.FromArgb(0xf2, 0xf2, 0xf2);
            Controls.Add(iconListBox);
            Controls.Add(desktopPanel);
            iconListBox.BringToFront();
            desktopPanel.Visible = false;
            desktopPanel.Dock = DockStyle.Top;
            iconListBox.Dock = DockStyle.Fill;
            desktopPanel.MouseEnter += DesktopPanel_MouseEnter;
            desktopPanel.MouseLeave += DesktopPanel_MouseLeave;
            desktopPanel.MouseClick += DesktopPanel_MouseClick;

        }

        private void DesktopPanel_MouseClick(object sender, MouseEventArgs e)
        {
            using (ShellFolder folder = new ShellFolder(desktop))
            {
                ArrayList children = folder.GetChildren(true, true, true);
                ItemSelectedEvent?.Invoke(this, new ItemSelectedEventArgs((PIDL)children[0]));
            }
        }

        private void IconListBox_ItemSelectedEvent(object sender, ItemSelectedEventArgs e)
        {
            ItemSelectedEvent?.Invoke(sender, e);
        }

        private void DesktopPanel_MouseLeave(object sender, EventArgs e)
        {
            DrawDesktop(BackColor);
        }

        Color Hilight = Color.FromArgb(0xC3, 0xDE, 0xF5);
        private void DesktopPanel_MouseEnter(object sender, EventArgs e)
        {
            iconListBox.MouseOver = -1;
            iconListBox.Invalidate();
            DrawDesktop(Hilight);
        }

        void DrawDesktop(Color color)
        {
            int index = sysimagelist.IconIndex(desktop.Pidl);
            using (Graphics g = Graphics.FromHwnd(desktopPanel.Handle))
            using (Brush brush = new SolidBrush(color))
            {
                Rectangle rect = desktopPanel.Bounds;
                rect.X += 2;
                rect.Y += 2;
                rect.Width -= 4;
                rect.Height = 20;
                g.FillRectangle(brush, rect);
                g.DrawImage(sysimagelist.Icon(index).ToBitmap(), new Rectangle(8, desktopPanel.Bounds.Y + 4, 16, 16));
                g.DrawString(desktop.DisplayName, Font, Brushes.Black, 35, desktopPanel.Bounds.Y + 6);
                g.DrawLine(SystemPens.ControlDark, 2, desktopPanel.Height - 5, desktopPanel.Width - 3, desktopPanel.Height - 5);
            }

        }

        public int ItemHeight
        {
            get { return iconListBox.ItemHeight; }
            set { iconListBox.ItemHeight = value; }
        }
        public void AddFolders(ArrayList children)
        {
            
            Height = (children.Count) * 22 + 5;
            if (Height > 400)
                Height = 400;
            iconListBox.BackColor = BackColor;
            iconListBox.BeginUpdate();
            iconListBox.Items.Clear();
            foreach (PIDL child in children)
            {
                if (child.IsCompressed) continue;
                iconListBox.Items.Add(child);
            }
            iconListBox.EndUpdate();
            if (IsDesktop)
            {
                DrawDesktop(BackColor);

            }
        }
    }
}
