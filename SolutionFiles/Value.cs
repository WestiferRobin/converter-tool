using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HelloWorldSharp.Values
{
    public class Values
    {
        public string Value1 {get;set;}

        // private string _value2;
        // public string Value2 { 
        //     get {
        //         return _value2;
        //     } 
        //     set {
        //         this._value2 = value;
        //     }
        // }

        public string Value3 {set;get;}
        public Values(string datValue)
        {
            this.Value = datValue;
            this.Value2 = "asdf";
        }
    }
}