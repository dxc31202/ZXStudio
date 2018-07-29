using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZXStudio.Controls
{
    class ComboFilebar : Panel
    {
        public event ItemSelectedEventHandler ItemSelectedEvent;
        public object SelectedPIDL
        {
            get { return fileBar.SelectedDIPL; }
            set { fileBar.SelectedDIPL = value; }
        }
        public event FolderSelectedEvent FolderSelectedEventHandler;
        public PIDL Data
        {
            get { return fileBar.Data; }
            set { SelectedPIDL= fileBar.Data = value; }
        }
        SysImageList sysimagelist = new SysImageList(SysImageListSize.smallIcons);
        FileBar fileBar;
        Label label1;
        Label label2;
        Label label3;
        Label label4;
        Label leftButton;
        Label rightButton;
        Label upButton;
        public ComboFilebar():base()
        {
            leftButton = new Label();
            rightButton = new Label();
            upButton = new Label();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label2.Width = 7;
            label2.BackColor = Color.White;
            label3.Width = 6;
            label4.Width = 1;

            label3.BackColor = Color.White;
            upButton.MouseEnter += UpButton_MouseEnter;
            upButton.MouseLeave += UpButton_MouseLeave;
            leftButton.BackColor = rightButton.BackColor = upButton.BackColor = Color.White;
            //leftButton.FlatAppearance.BorderSize = rightButton.FlatAppearance.BorderSize = upButton.FlatAppearance.BorderSize = 0;
            leftButton.FlatStyle = rightButton.FlatStyle = upButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            leftButton.Image = ZXStudio.Properties.Resources.LeftArrow;
            rightButton.Image = ZXStudio.Properties.Resources.RightArrow;
            upButton.Image = ZXStudio.Properties.Resources.UpArrow;
            leftButton.Width = rightButton.Width = upButton.Width = 22;
            label1.Text = "  ";
            fileBar = new FileBar();
            fileBar.ItemSelectedEvent += FileBar_ItemSelectedEvent;
            //Font = new Font("Microsoft Sans Serif", 9);
            Height = fileBar.Height = 24;

            label1.Dock = DockStyle.Right;
            label4.Dock = DockStyle.Left;
            label2.Dock = DockStyle.Left;
            leftButton.Dock = DockStyle.Left;
            label3.Dock = DockStyle.Left;
            rightButton.Dock = DockStyle.Left;
            upButton.Dock = DockStyle.Left;
            fileBar.Dock = DockStyle.Fill;
            Controls.Add(fileBar);
            Controls.Add(label1);
            Controls.Add(label4);
            Controls.Add(upButton);
            Controls.Add(rightButton);
            Controls.Add(label3);
            Controls.Add(leftButton);
            Controls.Add(label2);
            fileBar.FolderSelectedEventHandler += Combobox_FolderSelectedEventHandler;
        }

        private void FileBar_ItemSelectedEvent(object sender, ItemSelectedEventArgs e)
        {
            ItemSelectedEvent?.Invoke(sender, e);
        }

        ListBox listbox;

        public void CloseFileList()
        {
            fileBar.CloseFileList();
        }
        private PIDL Combobox_FolderSelectedEventHandler(object sender, FolderSelectedEventArgs e)
        {
            if(e.Location>0)
                e.Location += fileBar.Left;
            if (FolderSelectedEventHandler == null) return null;
            return FolderSelectedEventHandler(sender, e);
        }

        private void UpButton_MouseLeave(object sender, EventArgs e)
        {
            upButton.Image = ZXStudio.Properties.Resources.UpArrow;
        }

        private void UpButton_MouseEnter(object sender, EventArgs e)
        {
            upButton.Image = ZXStudio.Properties.Resources.UpArrowHilight;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
    }
}
