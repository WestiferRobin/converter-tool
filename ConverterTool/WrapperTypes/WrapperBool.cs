using System;
using System.Collections.Generic;
using System.Text;

namespace ConverterTool.WrapperTypes
{
    public class WrapperBool : WrapperType
    {
        public bool Value { get; set; }

        public WrapperBool(string wrapperName, bool value) : base(wrapperName)
        {
            this.Value = value;
        }
    }
}
