using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Text;

namespace TMFileManager
{
    public class TreeMapNode : IComparable, IDisposable
    {
        private double height;
        private Value value;
        private double weight = 0.0;
        private double width;
        private double x;
        private double y;
        private string label;
        private ArrayList nodes = new ArrayList();
        private TreeMapNode parent;
        private Icon icon;

        public ArrayList Nodes
        {
            get
            {
                return nodes;
            }
        }

        public TreeMapNode Parent
        {
            get
            {
                return parent;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Math.Round(x), (int)Math.Round(y), (int)Math.Round(width), (int)Math.Round(height));
            }
        }

        public Icon Icon
        {
            get
            {
                return icon;
            }
            set
            {
                icon = value;
            }
        }

        /**
         * Constructor for a branch.
         * 
         * @param label label of the branch.
         */
        public TreeMapNode(String label)
        {
            this.label = label;
        }

        /**
         * Constructor for a leaf.
         * 
         * @param label label of the leaf.
         * @param weight weight of the leaf (if negative, we take the absolute value).
         * @param value Value associ?e ? la feuille
         */
        public TreeMapNode(String label, double weight, Value value)
        {
            //the weight must be positive
            this.label = label;
            this.weight = Math.Abs(weight);
            this.value = value;
        }

        /**
         * add a new child to the node
         * 
         * @param newChild new child
         */
        public void add(TreeMapNode newChild)
        {
            Nodes.Add(newChild);
            newChild.parent = this;
            this.setWeight(this.weight + newChild.getWeight());
        }

        public bool IsLeaf
        {
            get
            {
                return Nodes.Count == 0;
            }
        }

        /**
         * get the active leaf.<BR>
         * null if the passed position is not in this tree.
         * 
         * @param x x-coordinate
         * @param y y-coordinate
         * @return active leaf
         */
        public TreeMapNode getActiveLeaf(double x, double y)
        {

            if (this.IsLeaf)
            {
                if ((x >= this.getX()) && (x <= this.getX() + this.getWidth())
                    && (y >= this.getY()) && (y <= this.getY() + this.getHeight()))
                {
                    return this;
                }
            }
            else {
                foreach (TreeMapNode node in Nodes)
                {
                    if ((x >= node.getX()) && (x <= node.getX() + node.getWidth())
                        && (y >= node.getY()) && (y <= node.getY() + node.getHeight()))
                    {
                        return node.getActiveLeaf(x, y);
                    }
                }
            }
            return null;
        }

        /**
         * get the first child which fits the position.<BR>
         * null if the passed position is not in this tree.
         * 
         * @param x x-coordinate
         * @param y y-coordinate
         * @return the first child which fits the position.
         */
        public TreeMapNode getChild(double x, double y)
        {
            if (!this.IsLeaf)
            {
                foreach (TreeMapNode node in Nodes)
                {
                    if ((x >= node.getX()) && (x <= node.getX() + node.getWidth())
                        && (y >= node.getY()) && (y <= node.getY() + node.getHeight()))
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        /**
         * get a Vector with the children
         * 
         * @return Vector with the children
         */
        public ArrayList getChildren()
        {
            return this.Nodes;
        }

        /**
         * get the height
         * 
         * @return the height
         */
        public double getHeight()
        {
            return this.height;
        }

        /**
         * get the label
         * 
         * @return the label
         */
        public String getLabel()
        {
            return label;
        }

        /**
         * get the label of the Value
         * 
         * @return the label of the Value
         */
        public String getLabelValue()
        {
            return this.value.getLabel();
        }

        /**
         * get the Value
         * 
         * @return the value
         */
        public Value getValue()
        {
            return this.value;
        }

        /**
         * get the double Value
         * 
         * @return the double value
         */
        public double getDoubleValue()
        {
            return this.value.getValue();
        }

        /**
         * get the weight
         * 
         * @return the weight
         */
        public double getWeight()
        {
            return this.weight;
        }

        /**
         * get the width
         * 
         * @return the width
         */
        public double getWidth()
        {
            return this.width;
        }

        /**
         * get the x-coordinate
         * 
         * @return the x-coordinate
         */
        public double getX()
        {
            return this.x;
        }

        /**
         * get the y-coordinate
         * 
         * @return the y-coordinate
         */
        public double getY()
        {
            return this.y;
        }

        /**
         * set the position and the size
         * 
         * @param x x-coordinate
         * @param y y-coordinate
         * @param width the new width
         * @param height the new height
         */
        public void setDimension(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        /**
         * set the height
         * 
         * @param height la nouvelle valeur de height
         */
        public void setHeight(double height)
        {
            this.height = height;
        }

        /**
         * set the label
         * 
         * @param label the new label
         */
        public void setLabel(String label)
        {
            this.label = label;
        }

        /**
         * set the position
         * 
         * @param x x-coordinate
         * @param y y-coordinate
         */
        public void setPosition(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /**
         * set size
         * 
         * @param width the new width
         * @param height the new height
         */
        public void setSize(double width, double height)
        {
            this.width = width;
            this.height = height;
        }

        /**
         * set the Value
         * 
         * @param value the new Value
         */
        public void setValue(Value value)
        {
            this.value = value;
        }

        /**
         * set the weight of the node and update the parents
         * 
         * @param weight the new weight
         */
        public void setWeight(double weight)
        {
            double newWeight = Math.Abs(weight);
            if (this.Parent != null)
            {
                ((TreeMapNode)this.Parent).setWeight(((TreeMapNode)this.Parent).weight - this.weight + newWeight);
            }
            this.weight = newWeight;
        }

        /**
         * set the width
         * 
         * @param width la nouvelle valeur de width
         */
        public void setWidth(double width)
        {
            this.width = width;
        }

        /**
         * set the x-coordinate
         * 
         * @param x the new x-coordinate
         */
        public void setX(double x)
        {
            this.x = x;
        }

        /**
         * set the y-coordinate
         * 
         * @param y the new y-coordinate
         */
        public void setY(double y)
        {
            this.y = y;
        }

        public int CompareTo(object obj)
        {
            TreeMapNode n = (TreeMapNode)obj;
            if (weight < n.weight)
            {
                return 1;
            }
            else if (weight > n.weight)
            {
                return -1;
            }
            else {
                return 0;
            }
        }

        public void Dispose()
        {
            if (icon != null)
            {
                icon.Dispose();
                icon = null;
            }
        }
    }
}
