using System;
using System.Collections.Generic;

namespace ConverterTool.WrapperTypes
{
    public class WrapperObject : WrapperType
    {
        public List<WrapperType> Value { get; set; }

        public WrapperObject(string wrapperName, List<WrapperType> value) : base(wrapperName)
        {
            this.Value = value;
        }

        public WrapperObject(string wrapperName) : base(wrapperName)
        {
            this.Value = new List<WrapperType>();
        }

        public WrapperObject() : base(string.Empty)
        {
            this.Value = new List<WrapperType>();
        }

        public List<string> GetKeys()
        {
            List<string> ans = new List<string>();
            foreach (var valueEntry in this.Value)
            {
                ans.Add(valueEntry.WrapperName);
            }
            return ans;
        }

        public void SetValue(WrapperType wrapperType)
        {
            this.Value.Add(wrapperType);
        }

        public WrapperType GetValue(string key)
        {
            foreach (var valueEntry in this.Value)
            {
                if (valueEntry.WrapperName.ToLower() == key.ToLower())
                {
                    return valueEntry;
                }
            }
            return null;
        }

        public void UpdateStringValue(string key, string value)
        {
            foreach (var valueEntry in this.Value)
            {
                if (valueEntry.WrapperName.ToLower() == key.ToLower())
                {
                    switch (valueEntry)
                    {
                        case WrapperString ws:
                            ws.Value = value;
                            break;
                        default:
                            throw new Exception($"Not a valid string type for {key}");
                    }
                    break;
                }
            }
        }

        public void CopyData(WrapperObject wrapperObject)
        {
            foreach (var entry in this.Value)
            {
                switch (entry)
                {
                    case WrapperBool wb:
                        wrapperObject.Value.Add(new WrapperBool(wb.WrapperName, wb.Value));
                        break;
                    case WrapperDouble wd:
                        wrapperObject.Value.Add(new WrapperDouble(wd.WrapperName, wd.Value));
                        break;
                    case WrapperInt wi:
                        wrapperObject.Value.Add(new WrapperInt(wi.WrapperName, wi.Value));
                        break;
                    case WrapperString ws:
                        wrapperObject.Value.Add(new WrapperString(ws.WrapperName, ws.Value));
                        break;
                    default:
                        throw new Exception($"This Fill Data for {wrapperObject.WrapperName}");
                }
            }
        }

        // TODO: Check this out on doing the other WrapperTypes
    }
}
