using System;
using System.Collections.Generic;
using System.Text;

namespace ConverterTool.WrapperTypes
{
    public class WrapperObject : WrapperType
    {
        public List<WrapperType> Value { get; set; }

        public WrapperObject(string wrapperName, List<WrapperType> value) : base(wrapperName)
        {
            this.Value = value;
        }
    }
}
