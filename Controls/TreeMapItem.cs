using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ZXStudio.Controls
{
    public class TreeMapItem
    {
        public TreeMapItem Add(TreeMapItem item)
        {
            TreeMapItems.Add(item);
            return item;
        }

        public TreeMapItem()
        {
            TreeMapItems = new List<TreeMapItem>();
        }

        ~TreeMapItem()
        {
            DiskItem = null;
            TreeMapItems = null;


        }

        public DiskItem diskitem;
        public RectangleF Rectangle;
        public float Area;
        public double Percentage;
        public float PanelSize;
        public DiskItem.NodeType DiskItemNodeType;
        public DiskItem DiskItem;

        float top;
        internal List<TreeMapItem> TreeMapItems;
        float brushX;
        float brushY;
        float workAreaHeight;
        float workAreaWidth;
        float currentHeight;
        bool vertical;
        float panelSize;
        public string Key;
        float maxWidth = 5f;
        float maxHeight = 19f;
        public string DisplayName;
        public float TreeeNodeSize;
        public TreeMapItem Parent;
        int maxDepth;

        public void Plot(DiskItem diskItem, RectangleF workArea)
        {
            depth = 0;
            Plot(diskItem, workArea, 0);
        }
        static int depth = 0;
        public void Plot(DiskItem diskItem, RectangleF workArea, int maxDepth)
        {
            this.maxDepth = maxDepth;
            Key = diskItem.FullPath;
            Rectangle = workArea;
            TreeeNodeSize = diskItem.Size;
            DiskItem = diskItem;
            DisplayName = this.DiskItem.Name;

            if (diskItem.Files== 0) return;
            if (workArea.Height <= maxHeight && workArea.Width <= maxWidth) return;

            Reset(workArea);
            if (workArea.Height <= maxHeight && workArea.Width <= maxWidth) return;

            Squarify(PrepareTreeMapItems(diskItem.Children), new List<TreeMapItem>(), GetWidth());
        }

        private void Reset(RectangleF workArea)
        {
            workArea.X = workArea.X + 2;
            workArea.Y = workArea.Y + 16;
            workArea.Width = workArea.Width - 4;
            workArea.Height = workArea.Height - 18;

            this.TreeMapItems = new List<TreeMapItem>();

            brushX = 0;
            brushY = 0;
            workAreaHeight = workArea.Height;
            workAreaWidth = workArea.Width;
            brushX = workArea.X;
            brushY = workArea.Y;
            top = workArea.Y;
            currentHeight = 0;
        }
        float totalArea;

        private TreeMapItem PrepareTreeMapItems(List<DiskItem> values)
        {
            TreeMapItem TreeMapItems = new TreeMapItem();
            try
            {

                values.Sort(delegate (DiskItem x, DiskItem y) { return y.Size.CompareTo(x.Size); });
            }
            catch
            {
                return TreeMapItems;
            }
            totalArea = workAreaWidth * workAreaHeight;

            float sumOfValues = this.TreeeNodeSize;
            for (int i = 0; i < values.Count; i++)
            {
                DiskItem diskItem = values[i];
                double percentage = (diskItem.Size / sumOfValues) * 100;
                float area = (float)((totalArea / 100) * percentage);
                if (area > 0)
                {
                    TreeMapItem tm = new TreeMapItem();
                    tm.Key = diskItem.FullPath;
                    tm.Area = area;
                    tm.Percentage = percentage;
                    tm.DiskItem = diskItem;
                    tm.TreeeNodeSize = diskItem.Size;
                    tm.Key = diskItem.FullPath;
                    tm.DisplayName = diskItem.Name;
                    TreeMapItems.Area += area;
                    TreeMapItems.Add(tm);
                }
            }
            return TreeMapItems;
        }

        private void Squarify(TreeMapItem values, List<TreeMapItem> currentRow, float width)
        {

            if (values.TreeMapItems.Count == 0) return;
            if (width <= 0) return;
            List<TreeMapItem> nextIterationPreview = new List<TreeMapItem>();
            try {
                nextIterationPreview.AddRange(currentRow.ToArray());
            }
            catch (System.StackOverflowException ex)
            {
                Console.WriteLine(ex.Message);
            }
            //foreach (TreeMapItem item in currentRow) nextIterationPreview.Add(item);
            try
            {
                if (values.TreeMapItems.Count > 1) nextIterationPreview.Add(values.TreeMapItems[0]);
            }catch(System.StackOverflowException ex)
            {
                Console.WriteLine(ex.Message);
            }
            double aspectRatio = CalculateAspectRatio(currentRow, width);
            double nextAspectRatio = CalculateAspectRatio(nextIterationPreview, width);

            if (aspectRatio == 0 || (nextAspectRatio < aspectRatio && nextAspectRatio >= 1))
            {
                currentRow.Add(values.TreeMapItems[0]);
                values.TreeMapItems.RemoveAt(0);
                currentHeight = CalculateHeight(currentRow, width);

                if (values.TreeMapItems.Count > 0)
                    Squarify(values, currentRow, width);
                else
                    LayoutRow(currentRow);
            }
            else
            {
                // Row has reached it's optimum size
                LayoutRow(currentRow);
                // Start the next row, by passing an empty list of row values and recalculating the current width
                List<TreeMapItem> newlist = new List<TreeMapItem>();
                Squarify(values, newlist, GetWidth());
            }
        }

        private void LayoutRow(List<TreeMapItem> rowTreeMapItems)
        {
            PointF brushStartingPoint = new PointF(brushX, brushY);

            if (!vertical)
                if (workAreaHeight != currentHeight)
                    brushY = workAreaHeight - currentHeight + top;

            // Draw each TreeMapItem in the current row
            float width;
            float height;
            foreach (TreeMapItem TreeMapItem in rowTreeMapItems)
            {
                if (TreeMapItem.Area > 0)
                {
                    // Calculate Width & Height
                    if (vertical)
                    {
                        width = currentHeight;
                        height = TreeMapItem.Area / currentHeight;
                    }
                    else
                    {
                        width = TreeMapItem.Area / currentHeight;
                        height = currentHeight;
                    }
                    TreeMapItem.Rectangle = new RectangleF(brushX, brushY, width, height);
                    // Recursivley Add Folder objects
                    TreeMapItem.Parent = this;
                    if (TreeMapItem.DiskItemNodeType != DiskItem.NodeType.File)
                    {
                        //if (height >= 20f && width >= 6f)
                        if (height > maxHeight && width > maxWidth)
                        {
                            TreeMapItem.Plot(TreeMapItem.DiskItem, TreeMapItem.Rectangle, this.maxDepth);
                        }
                    }
                    this.Add(TreeMapItem);

                    // Reposition brush for the next TreeMapItem
                    if (vertical)
                    {
                        brushY += height;
                    }
                    else
                    {
                        brushX += width;
                    }

                }
            }

            // Finished drawing all TreeMapItems in the row
            // Reposition the brush ready for the next row
            if (vertical)
            {
                // x increase by width (in vertical, currentHeight is width)
                // y reset to starting position
                brushX += currentHeight;
                brushY = brushStartingPoint.Y;
                workAreaWidth -= currentHeight;
            }
            else
            {
                brushX = brushStartingPoint.X;
                brushY = brushStartingPoint.Y;
                workAreaHeight -= currentHeight;
            }

            currentHeight = 0;
        }

        private double CalculateAspectRatio(List<TreeMapItem> currentRow, double width)
        {
            double sumOfAreas = 0;
            foreach (TreeMapItem TreeMapItem in currentRow) sumOfAreas += TreeMapItem.Area;
            double maxArea = int.MinValue;
            double minArea = int.MaxValue;
            foreach (TreeMapItem TreeMapItem in currentRow)
            {
                if (TreeMapItem.Area > maxArea) maxArea = TreeMapItem.Area;
                if (TreeMapItem.Area < minArea) minArea = TreeMapItem.Area;
            }

            double widthSquared = width * width;
            double sumOfAreasSqaured = sumOfAreas * sumOfAreas;

            double ratio1 = (widthSquared * maxArea) / sumOfAreasSqaured;
            double ratio2 = sumOfAreasSqaured / (widthSquared * minArea);
            if (ratio1 > ratio2) return ratio1;
            return ratio2;
        }

        private float CalculateHeight(List<TreeMapItem> currentRow, float width)
        {
            float sum = 0;
            foreach (TreeMapItem TreeMapItem in currentRow) sum += TreeMapItem.Area;
            return sum / width;
        }

        private float GetWidth()
        {
            if (workAreaHeight > workAreaWidth)
            {
                vertical = false;
                return workAreaWidth;
            }

            vertical = true;
            return workAreaHeight;
        }

        public override string ToString()
        {
            return this.diskitem.Name + " " + this.Rectangle.ToString();
        }

    }
}
