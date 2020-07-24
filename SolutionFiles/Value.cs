using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HelloWorldSharp.Values
{
    public class Values
    {
        public string Value {get;set;}
        public Values(string datValue)
        {
            this.Value = datValue;
        }
    }
}