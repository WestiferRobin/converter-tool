using System;
using System.Collections.Generic;
using System.Text;

namespace ConverterTool.WrapperTypes
{
    public class WrapperInt : WrapperType
    {
        public int Value { get; set; }

        public WrapperInt(string wrapperName, int value) : base(wrapperName)
        {
            this.Value = value;
        }
    }
}
