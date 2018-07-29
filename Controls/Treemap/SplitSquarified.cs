using System;
using System.Collections;
using System.Windows.Forms;
using System.Text;

namespace TMFileManager
{
    public class SplitSquarified : SplitStrategy
    {
        private double w1, h1;
        private double x, y, w, h;
        private double x2, y2, w2, h2;

        public override void splitElements(ArrayList v, ArrayList v1, ArrayList v2)
        {
            int mid = 0;
            double weight0 = sumWeight(v);
            double a = ((TreeMapNode)(v[mid])).getWeight() / weight0;
            double b = a;

            if (this.w < this.h)
            {
                // height/width
                while (mid < v.Count)
                {
                    double aspect = normAspect(this.h, this.w, a, b);
                    double q = ((TreeMapNode)(v[mid])).getWeight() / weight0;
                    if (normAspect(this.h, this.w, a, b + q) > aspect)
                        break;
                    mid++;
                    b += q;
                }
                int i = 0;
                for (; i <= mid && i < v.Count; i++)
                {
                    v1.Add(v[i]);
                }
                for (; i < v.Count; i++)
                {
                    v2.Add(v[i]);
                }
                this.h1 = this.h * b;
                this.w1 = this.w;
                this.x2 = this.x;
                this.y2 = this.y + this.h * b;
                this.w2 = this.w;
                this.h2 = this.h - this.h1;
            }
            else {
                // width/height
                while (mid < v.Count)
                {
                    double aspect = normAspect(this.w, this.h, a, b);
                    double q = ((TreeMapNode)(v[mid])).getWeight() / weight0;
                    if (normAspect(this.w, this.h, a, b + q) > aspect)
                        break;
                    mid++;
                    b += q;
                }
                int i = 0;
                for (; i <= mid && i < v.Count; i++)
                {
                    v1.Add(v[i]);
                }
                for (; i < v.Count; i++)
                {
                    v2.Add(v[i]);
                }
                this.h1 = this.h;
                this.w1 = this.w * b;
                this.x2 = this.x + this.w * b;
                this.y2 = this.y;
                this.w2 = this.w - this.w1;
                this.h2 = this.h;
            }
        }

        /*
         * (non-Javadoc)
         * 
         * @see org.jense.swing.jtreemap.SplitStrategy#calculatePositionsRec(int, int,
         *      int, int, double, java.util.Vector)
         */
        protected override void calculatePositionsRec(double x0, double y0, double w0, double h0, double weight0, ArrayList v)
        {
            ArrayList vClone = new ArrayList();
            vClone.AddRange(v);
            //foreach (object n in v) {
            //    vClone.Add(n);
            //}

            vClone.Sort();
            //sortVector(vClone);

            if (vClone.Count <= 2)
            {
                SplitBySlice.splitInSlice(x0, y0, w0, h0, vClone, sumWeight(vClone));
                calculateChildren(vClone);
            }
            else {
                ArrayList v1 = new ArrayList();
                ArrayList v2 = new ArrayList();
                this.x = x0;
                this.y = y0;
                this.w = w0;
                this.h = h0;
                splitElements(vClone, v1, v2);
                //before the recurence, we have to "save" the values for the 2nd Vector
                double x2 = this.x2;
                double y2 = this.y2;
                double w2 = this.w2;
                double h2 = this.h2;
                SplitBySlice.splitInSlice(x0, y0, this.w1, this.h1, v1, sumWeight(v1));
                calculateChildren(v1);
                calculatePositionsRec(x2, y2, w2, h2, sumWeight(v2), v2);
            }

        }

        private double aspect(double big, double small, double a, double b)
        {
            return (big * b) / (small * a / b);
        }

        /**
         * Execute the recurence for the children of the elements of the vector<BR>
         * Add also the borders if necessary
         * 
         * @param v Vector with the elements to calculate
         */
        private void calculateChildren(ArrayList v)
        {
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

        private double normAspect(double big, double small, double a, double b)
        {
            double x = aspect(big, small, a, b);
            if (x < 1)
                return 1 / x;
            return x;
        }

    }
}

