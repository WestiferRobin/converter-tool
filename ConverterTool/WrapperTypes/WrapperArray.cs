using System;
using System.Collections.Generic;
using System.Text;

namespace ConverterTool.WrapperTypes
{
    public class WrapperArray : WrapperType
    {
        public List<WrapperObject> Value { get; set; }

        public WrapperArray(string wrapperName, List<WrapperObject> value) : base(wrapperName)
        {
            this.Value = value;
        }
    }
}
