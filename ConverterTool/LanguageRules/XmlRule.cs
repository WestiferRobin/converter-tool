using ConverterTool.Logger;
using ConverterTool.WrapperTypes;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConverterTool.LanguageRules
{
    internal class XmlRule : LanguageRule
    {
        private const string XML_HEADER = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

        public XmlRule(string filename) : base(LanguageType.MARKUP_LANG, ProgramType.XML, filename)
        {

        }

        public override void ScanFile()
        {
            var fileContents = File.ReadAllText(this.FullFile);
            bool isLocked = false;
            int index = 0;
            string hold = string.Empty;

            // Read and add header of XML
            foreach (var character in fileContents)
            {
                index++;
                if (isLocked)
                {
                    if (character == '>')
                    {
                        hold += character.ToString();
                        this.TokenList.Add(hold);
                        hold = string.Empty;
                        isLocked = false;
                        break;
                    }
                    hold += character.ToString();
                }
                else
                {
                    hold += character.ToString();
                    isLocked = true;
                }
            }

            hold = string.Empty;
            isLocked = false;

            for (;index < fileContents.Length; index++)
            {
                var character = fileContents[index];
                if (character == ' ' || character == '\n' || character == '\r' || character == '\t') continue;
                else if (isLocked)
                {
                    if (fileContents[index] == '-' && fileContents[index + 1] == '-' && fileContents[index + 2] == '>')
                    {
                        index += 2;
                        isLocked = false;
                    }
                    continue;
                }
                else if (character == '<')
                {
                    if (fileContents[index + 1] == '!')
                    {
                        isLocked = true;
                        index += 1;
                        continue;
                    }
                    this.TokenList.Add(character.ToString());
                    index++;
                    while (fileContents[index] != '>')
                    {
                        hold += fileContents[index++].ToString();
                    }
                    this.TokenList.Add(hold);
                    this.TokenList.Add(fileContents[index].ToString());
                    hold = string.Empty;
                    continue;
                }
                else 
                {
                    while (fileContents[index] != '<')
                    {
                        hold += fileContents[index++].ToString();
                    }
                    index--;
                    this.TokenList.Add(hold);
                    hold = string.Empty;
                }
            }
        }

        private int VerifyHeader(int index)
        {
            for (int i = 0; i < XML_HEADER.Length; i++)
            {
                if (this.TokenList[index][i] != XML_HEADER[i])
                    throw new Exception("This is a bad header.");
            }
            return ++index;
        }

        public override void ParseFile()
        {
            Log.Info("Starting to Parse XML file.");
            WrapperType mainNode;
            int index = 0;
            index = VerifyHeader(index);
            if (this.TokenList[index] == "<")
            {
                index++;
                if (this.TokenList[index].Contains("Array"))
                {
                    mainNode = new WrapperArray(this.TokenList[index++], null);
                    if (this.TokenList[index++] != ">")
                        throw new Exception("Invalid Token. Need first double quote for name of value");
                    index = ParseArray(index, mainNode as WrapperArray);

                    if (this.TokenList[index++] != "<")
                        throw new Exception("Invalid Token. Need \'<\' for name of value");
                    if (this.TokenList[index++] != $"/{mainNode.WrapperName + "Array"}")
                        throw new Exception($"Invalid Token. Need closing name {mainNode.WrapperName} for name of value");
                    if (this.TokenList[index++] != ">")
                        throw new Exception("Invalid Token. Need \'>\' for name of value");
                }
                else
                {
                    mainNode = new WrapperObject(this.TokenList[index++], null);
                    if (this.TokenList[index++] != ">")
                        throw new Exception("Invalid Token. Need first double quote for name of value");
                    index = ParseObject(index, mainNode as WrapperObject);

                    if (this.TokenList[index++] != "<")
                        throw new Exception("Invalid Token. Need \'<\' for name of value");
                    if (this.TokenList[index++] != $"/{mainNode.WrapperName}")
                        throw new Exception($"Invalid Token. Need closing name {mainNode.WrapperName} for name of value");
                    if (this.TokenList[index++] != ">")
                        throw new Exception("Invalid Token. Need \'>\' for name of value");
                }
            }
            else
            {
                throw new Exception("Invalid start to XML Parsing.");
            }

            this.Structure.Add(mainNode);
            Log.Success("XML Parse Successfully Completed.");
        }

        private int ParseValue(int index, string valueName, WrapperObject parentNode)
        {
            WrapperType actualValue;
            if (this.TokenList[index].Contains("."))
            {
                var proposedDouble = this.TokenList[index].Split(".");
                if (int.TryParse(proposedDouble[0], out _) && int.TryParse(proposedDouble[1], out _))
                {
                    actualValue = new WrapperDouble(valueName, double.Parse(this.TokenList[index++]));
                }
                else
                {
                    throw new Exception("This is an invlid Double Value");
                }
            }
            else if (int.TryParse(this.TokenList[index], out _))
            {
                actualValue = new WrapperInt(valueName, int.Parse(this.TokenList[index++]));
            }
            else if (this.TokenList[index].ToLower() == "true" || this.TokenList[index].ToLower() == "false")
            {
                bool boolVal = bool.Parse(this.TokenList[index++]);
                actualValue = new WrapperBool(valueName, boolVal);
            }
            else if (this.TokenList[index] == "<")
            {
                if (valueName.ToLower().Contains("array"))
                {
                    actualValue = new WrapperArray(valueName, null);
                    index = ParseArray(index, actualValue as WrapperArray);
                }
                else
                {
                    actualValue = new WrapperObject(valueName, null);
                    index = ParseObject(index, actualValue as WrapperObject);
                }
            }
            else
            {
                actualValue = new WrapperString(valueName, this.TokenList[index++]);
            }

            if (this.TokenList[index++] != "<")
                throw new Exception("Invalid Token. Need \'<\' for name of value");
            if (valueName.ToLower().Contains("array"))
            {
                if (this.TokenList[index].ToLower() != $"/{valueName.ToLower()}")
                    throw new Exception($"Invalid Token. Need closing name {valueName} for name of value");
            }
            else
            {
                if (this.TokenList[index] != $"/{valueName}")
                    throw new Exception($"Invalid Token. Need closing name {valueName} for name of value");
            }
            index++;
            if (this.TokenList[index++] != ">")
                throw new Exception("Invalid Token. Need \'>\' for name of value");

            parentNode.Value.Add(actualValue);
            return index;
        }

        private int ParseObject(int index, WrapperObject parentNode)
        {
            parentNode.Value = new List<WrapperType>();
            while (index < this.TokenList.Count)
            {
                if (this.TokenList[index++] != "<")
                    throw new Exception("Invalid Token. Need \'<\' for name of value");
                if (this.TokenList[index] == $"/{parentNode.WrapperName}")
                    break;
                string valueName = this.TokenList[index++];
                if (this.TokenList[index++] != ">")
                    throw new Exception("Invalid Token. Need \'>\' for name of value");

                index = ParseValue(index, valueName, parentNode);
            }
            return index - 1;
        }

        private int ParseArray(int index, WrapperArray parentNode)
        {
            parentNode.Value = new List<WrapperObject>();
            string jsonName = parentNode.WrapperName.Split("Array")[0];
            int id = 0;
            while (index < this.TokenList.Count)
            {
                if (this.TokenList[index++] != "<")
                    throw new Exception("Invalid Token. Need \'<\' for name of value");
                if (this.TokenList[index] == $"/{parentNode.WrapperName}")
                {
                    parentNode.WrapperName = jsonName;
                    break;
                }
                if (this.TokenList[index++] != jsonName)
                    throw new Exception("Invalid Token. Need \'<\' for name of value");
                if (this.TokenList[index++] != ">")
                    throw new Exception("Invalid Token. Need \'>\' for name of value");

                WrapperObject childNode = new WrapperObject(jsonName, null);
                index = ParseObject(index, childNode);
                childNode.Value.Add(new WrapperInt("id", id));
                childNode.WrapperName = "ID-" + id++;
                parentNode.Value.Add(childNode);

                if (this.TokenList[index++] != "<")
                    throw new Exception("Invalid Token. Need \'<\' for name of value");
                if (this.TokenList[index++] != $"/{jsonName}")
                    throw new Exception("Invalid Token. Need \'<\' for name of value");
                if (this.TokenList[index++] != ">")
                    throw new Exception("Invalid Token. Need \'>\' for name of value");
            }
            return index - 1;
        }

        private void BuildObject(WrapperObject mainNode, string tabs)
        {
            this.Results += $"{tabs}<{mainNode.WrapperName}>\n";
            foreach (var node in mainNode.Value)
            {
                if (node.WrapperName.ToLower() == "id")
                    continue;
                switch (node)
                {
                    case WrapperArray wrapperArray:
                        BuildArray(wrapperArray, tabs + "\t");
                        break;
                    case WrapperObject wrapperObject:
                        BuildObject(wrapperObject, tabs + "\t");
                        break;
                    case WrapperBool wrapperBool:
                        this.Results += $"{tabs + "\t"}<{wrapperBool.WrapperName}>{wrapperBool.Value}</{wrapperBool.WrapperName}>\n";
                        break;
                    case WrapperDouble wrapperDouble:
                        this.Results += $"{tabs + "\t"}<{wrapperDouble.WrapperName}>{wrapperDouble.Value}</{wrapperDouble.WrapperName}>\n";
                        break;
                    case WrapperInt wrapperInt:
                        this.Results += $"{tabs + "\t"}<{wrapperInt.WrapperName}>{wrapperInt.Value}</{wrapperInt.WrapperName}>\n";
                        break;
                    case WrapperString wrapperString:
                        this.Results += $"{tabs + "\t"}<{wrapperString.WrapperName}>{wrapperString.Value}</{wrapperString.WrapperName}>\n";
                        break;
                    default:
                        throw new Exception("This type is invalid for build the file.");
                }
            }
            this.Results += $"{tabs}</{mainNode.WrapperName}>\n";
        }

        private void BuildArray(WrapperArray mainNode, string tabs)
        {
            this.Results += $"{tabs}<{mainNode.WrapperName}Array>\n";
            foreach (var node in mainNode.Value)
            {
                switch (node)
                {
                    case WrapperObject wrapperObject:
                        wrapperObject.WrapperName = mainNode.WrapperName;
                        BuildObject(wrapperObject, tabs + "\t");
                        break;
                        throw new Exception("This type is invalid for build the file.");
                }
            }
            this.Results += $"{tabs}</{mainNode.WrapperName}Array>\n";
        }

        public override void BuildFile()
        {
            this.Results = $"{XML_HEADER}\n";
            switch (this.Structure[0])
            {
                case WrapperArray wrapperArray:
                    BuildArray(wrapperArray, "");
                    break;
                case WrapperObject wrapperObject:
                    BuildObject(wrapperObject, "");
                    break;
            }

            if (!Directory.Exists(Path.GetDirectoryName(this.FullFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(this.FullFile));

            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(this.FullFile))
            {
                string results = this.Results;
                foreach (var character in results)
                {
                    sw.Write(character);
                }
            }
        }

        protected override void CreateKeywords()
        {
            throw new Exception("XML Rules do not need to create Keywords.");
        }
    }
}
