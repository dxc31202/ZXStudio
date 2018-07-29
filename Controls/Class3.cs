using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZXStudio.Controls
{
    class Class3:Form
    {
        FolderListBox folderListBox;

        public Class3() {
            folderListBox = new FolderListBox();
            Controls.Add(folderListBox);
            folderListBox.Dock = DockStyle.Fill;
            folderListBox.ItemSelectedEvent += FolderListBox_ItemSelectedEvent;
                }

        private void FolderListBox_ItemSelectedEvent(object sender, ItemSelectedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
