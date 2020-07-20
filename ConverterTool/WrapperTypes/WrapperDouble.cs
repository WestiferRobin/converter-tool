using System;
using System.Collections.Generic;
using System.Text;

namespace ConverterTool.WrapperTypes
{
    public class WrapperDouble : WrapperType
    {
        public double Value { get; set; }

        public WrapperDouble(string wrapperName, double value) : base(wrapperName)
        {
            this.Value = value;
        }
    }
}
