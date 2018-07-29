using System;
using System.Collections;
using System.Windows.Forms;
using System.Text;

namespace TMFileManager
{
    public class SplitBySlice : SplitStrategy
    {

        /**
         * Calculate the dimension of the elements of the Vector.
         * 
         * @param x0 x-coordinate
         * @param y0 y-coordinate
         * @param w0 width
         * @param h0 height
         * @param v elements to split in the dimensions before
         * @param sumWeight sum of the weights
         */
        public static void splitInSlice(double x0, double y0, double w0, double h0, ArrayList v, double sumWeight)
        {
            double offset = 0;
            bool vertical = h0 > w0;

            int nodeIndex = 0;
            foreach (TreeMapNode node in v)
            {
                if (vertical)
                {
                    node.setX(x0);
                    node.setWidth(w0);
                    node.setY(y0 + offset);
                    node.setHeight(h0 * node.getWeight() / sumWeight);
                    offset = offset + node.getHeight();
                }
                else {
                    node.setX(x0 + offset);
                    node.setWidth(w0 * node.getWeight() / sumWeight);
                    node.setY(y0);
                    node.setHeight(h0);
                    offset = offset + node.getWidth();
                }
                //Because of the Math.round(), we adjust the last element to fit the
                // correctly the JTreeMap
                if (nodeIndex == v.Count - 1)
                {
                    if (vertical && h0 != offset)
                    {
                        node.setHeight(node.getHeight() - offset + h0);
                    }
                    else if (!vertical && w0 != offset)
                    {
                        node.setWidth(node.getWidth() - offset + w0);
                    }
                }
                nodeIndex++;
            }
        }

        /*
         * (non-Javadoc)
         * 
         * @see org.jense.swing.jtreemap.SplitStrategy#splitElements(java.util.Vector,
         *      java.util.Vector, java.util.Vector)
         */
        public override void splitElements(ArrayList v, ArrayList v1, ArrayList v2)
        {
            // ignore
        }

        /*
         * (non-Javadoc)
         * 
         * @see org.jense.swing.jtreemap.SplitStrategy#calculatePositionsRec(int, int,
         *      int, int, double, java.util.Vector)
         */
        protected override void calculatePositionsRec(double x0, double y0, double w0, double h0, double weight0, ArrayList v)
        {

            SplitBySlice.splitInSlice(x0, y0, w0, h0, v, weight0);

            foreach (TreeMapNode node in v)
            {
                if (node.IsLeaf)
                {
                    node.setX(node.getX() + getBorder());
                    node.setY(node.getY() + getBorder());
                    node.setHeight(node.getHeight() - getBorder());
                    node.setWidth(node.getWidth() - getBorder());
                }
                else {
                    //if this is not a leaf, calculation for the children
                    if (getBorder() > 1)
                    {
                        setBorder(getBorder() - 2);
                        calculatePositionsRec(node.getX() + 2, node.getY() + 2, node.getWidth() - 2, node.getHeight() - 2, node.getWeight(),
                            node.getChildren());
                        setBorder(getBorder() + 2);
                    }
                    else if (getBorder() == 1)
                    {
                        setBorder(0);
                        calculatePositionsRec(node.getX() + 1, node.getY() + 1, node.getWidth() - 1, node.getHeight() - 1, node.getWeight(),
                            node.getChildren());
                        setBorder(1);
                    }
                    else {
                        calculatePositionsRec(node.getX(), node.getY(), node.getWidth(), node.getHeight(), node.getWeight(), node.getChildren());
                    }
                }
            }

        }
    }
}

