using System;
using System.Text;

namespace TMFileManager
{
    public abstract class Value : IComparable
    {
        /**
         * get the double value
         *
         *@return the double value
         */
        public abstract double getValue();

        /**
         * get the formatedValue
         *
         *@return the label of the value
         */
        public abstract String getLabel();

        /**
         * set the double value
         *
         *@param value the new double value
         */
        public abstract void setValue(double value);

        /**
         * set the new label
         *
         *@param newLabel the new label
         */
        public abstract void setLabel(String newLabel);

        /* (non-Javadoc)
         * @see java.lang.Object#equals(java.lang.Object)
         */
        public bool equals(Object value)
        {
            if (value != null && value is Value)
            {
                Value value2 = (Value)value;
                return value2.getValue() == this.getValue();
            }
            return false;
        }

        /* (non-Javadoc)
         * @see java.lang.Comparable#compareTo(java.lang.Object)
         */
        public int CompareTo(Object value)
        {
            if (value != null && value is Value)
            {
                Value value2 = (Value)value;
                if (this.getValue() < value2.getValue())
                    return -1;
                else if (this.getValue() > value2.getValue())
                    return 1;
                else
                    return 0;
            }
            throw new ArgumentException();
        }
    }
}

