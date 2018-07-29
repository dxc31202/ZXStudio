using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace TMFileManager
{
    public abstract class SplitStrategy
    {
        //max border between two nodes of the same level
        private int border = 3;

        /**
         * calculate the positions for all the elements of the root.
         * 
         * @param root the root to calculate
         */
        public void calculatePositions(TreeMapNode root)
        {
            if (root == null)
                return;

            ArrayList v = root.Nodes;
            if (v != null)
            {
                calculatePositionsRec(root.getX(), root.getY(), root.getWidth(), root.getHeight(), this.sumWeight(v), v);
            }
        }

        /**
         * Get the max border
         * 
         * @return Returns the border.
         */
        public int getBorder()
        {
            return this.border;
        }

        /**
         * Set the max border
         * 
         * @param border The border to set.
         */
        public void setBorder(int border)
        {
            this.border = border;
        }

        /**
         * split the elements of a JTreeMap.
         * 
         * @param v Vector with the elements to split (arg IN)
         * @param v1 first Vector of the split (arg OUT)
         * @param v2 second Vector of the split (arg OUT)
         */
        public abstract void splitElements(ArrayList v, ArrayList v1, ArrayList v2);

        /**
         * Sum the weight of elements. <BR>
         * You can override this method if you want to apply a coef on the weights or
         * to cancel the effect of weight on the strategy.
         * 
         * @param v Vector with the elements to sum
         * @return the sum of the weight of elements
         */
        public double sumWeight(ArrayList v)
        {
            double d = 0D;
            if (v != null)
            {
                foreach (TreeMapNode n in v)
                {
                    d += n.getWeight();
                }
            }
            return d;
        }

        protected virtual void calculatePositionsRec(double x0, double y0, double w0, double h0, double weight0, ArrayList v)
        {
            if (w0 <= this.border || h0 <= this.border)
            {
                return;
            }

            //if the Vector contains only one element
            if (v.Count == 1)
            {
                TreeMapNode f = (TreeMapNode)v[0];
                if (f.IsLeaf)
                {
                    //if this is a leaf, we display with the border
                    f.setDimension(x0 + this.border, y0 + this.border, w0 - this.border, h0 - this.border);
                }
                else {
                    f.setDimension(x0, y0, w0, h0);
                    //if this is not a leaf, calculation for the children
                    if (this.border > 1)
                    {
                        this.border -= 2;
                        calculatePositionsRec(x0 + 2, y0 + 2, w0 - 2, h0 - 2, weight0, f.Nodes);
                        this.border += 2;
                    }
                    else if (this.border == 1)
                    {
                        this.border = 0;
                        calculatePositionsRec(x0 + 1, y0 + 1, w0 - 1, h0 - 1, weight0, f.Nodes);
                        this.border = 1;
                    }
                    else {
                        calculatePositionsRec(x0, y0, w0, h0, weight0, f.Nodes);
                    }
                }
            }
            else {
                //if there is more than one element
                //we split the Vector according to the selected strategy
                ArrayList v1 = new ArrayList();
                ArrayList v2 = new ArrayList();
                double weight1, weight2; //poids des 2 vecteurs
                this.splitElements(v, v1, v2);
                weight1 = this.sumWeight(v1);
                weight2 = this.sumWeight(v2);

                double w1, w2, h1, h2;
                double x2, y2;
                //if width is greater than height, we split the width
                if (w0 > h0)
                {
                    w1 = (int)(w0 * weight1 / weight0);
                    w2 = w0 - w1;
                    h1 = h0;
                    h2 = h0;
                    x2 = x0 + w1;
                    y2 = y0;
                }
                else {
                    //else we split the height
                    w1 = w0;
                    w2 = w0;
                    h1 = (int)(h0 * weight1 / weight0);
                    h2 = h0 - h1;
                    x2 = x0;
                    y2 = y0 + h1;
                }
                //calculation for the new two Vectors
                calculatePositionsRec(x0, y0, w1, h1, weight1, v1);
                calculatePositionsRec(x2, y2, w2, h2, weight2, v2);
            }
        }

        /**
         * sort the elements by descending weight
         * 
         * @param v Vector with the elements to be sorted
         */
        protected void sortVector(ArrayList v)
        {
            TreeMapNode tmn;
            //we use the bubble sort
            for (int i = 0; i < v.Count; i++)
            {
                for (int j = v.Count - 1; j > i; j--)
                {
                    if (((TreeMapNode)(v[j])).getWeight() > ((TreeMapNode)(v[j - 1])).getWeight())
                    {
                        tmn = (TreeMapNode)(v[j]);
                        v[j] = v[j - 1];
                        v[j - 1] = tmn;
                    }
                }
            }

        }

    }
}

