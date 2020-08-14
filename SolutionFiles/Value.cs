using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HelloWorldSharp.Values
{
    public class Values<T>
    {
        public string Value1 {get;set;}

        private string _value2;
        public string Value2 { 
            get {
                return _value2;
            } 
            set {
                this._value2 = value;
            }
        }

        public string Value3 {set;get;}
        public Values(string datValue, T asdf)
        {
            _someT = asdf;
            this.Value1 = datValue;
            this.Value2 = "asdf";
            this.Value3 = "fdsa";
            this.SetRandomFunction();
        }

        private T _someT;

        private void SetRandomFunction()
        {
            string asdf = this.Value3 + "asdf";
            this.Value1 = this.Value2;
            List<T> fdsa = new List<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            Node current = head;

            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        public void AddHead(T t)
        {
            Node n = new Node(t);
            n.Next = head;
            head = n;
        }
    }
}