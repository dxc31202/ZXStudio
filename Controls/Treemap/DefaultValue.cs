using System;
using System.Text;

namespace TMFileManager
{
    public class DefaultValue : Value
    {
        private double value;

        /**
         * Constructor
         */
        public DefaultValue()
        {
            //nothing to do
        }

        /**
         * Constructor
         * 
         * @param value double value
         */
        public DefaultValue(double value)
        {
            this.value = value;
        }

        /*
         * (non-Javadoc)
         * 
         * @see org.jense.swing.jtreemap.Value#getValue()
         */
        public override double getValue()
        {
            return this.value;
        }

        /*
         * (non-Javadoc)
         * 
         * @see org.jense.swing.jtreemap.Value#getLabel()
         */
        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        /// <returns></returns>
        public override String getLabel()
        {
            return "" + this.value;
        }

        /*
         * (non-Javadoc)
         * 
         * @see org.jense.swing.jtreemap.Value#setValue(double)
         */
        public override void setValue(double value)
        {
            this.value = value;
        }

        /*
         * (non-Javadoc)
         * 
         * @see org.jense.swing.jtreemap.Value#setLabel(java.lang.String)
         */
        public override void setLabel(String newLabel)
        {
            // ignore

        }

    }
}

