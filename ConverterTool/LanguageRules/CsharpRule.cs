using System;
using System.Collections.Generic;
using System.IO;
using ConverterTool.Logger;
using ConverterTool.WrapperTypes;

namespace ConverterTool.LanguageRules
{
    internal class CsharpRule : LanguageRule
    {
        public CsharpRule(string filename) : base(LanguageType.PROGRAM_LANG, ProgramType.C_SHARP, filename)
        {
            this.CreateKeywords();
        }

        public override void BuildFile()
        {
            Log.Warn("Csharp BuildFile is not ready.");
        }

        public int AddHeader(int index)
        {
            var otherHeaders = new WrapperObject("HEADERS", new List<WrapperType>());
            while (this.TokenList[index].ToLower() != "namespace")
            {
                if (this.TokenList[index].ToLower() == "using")
                {
                    index++;
                    if (this.TokenList[index].ToLower() == "system")
                    {
                        while (this.TokenList[index] != ";")
                            index++;
                    }
                    else
                    {
                        string location = string.Empty;
                        while (this.TokenList[index] != ";")
                            location += this.TokenList[index++];
                        otherHeaders.Value.Add(new WrapperString("IMPORT", location));
                    }
                }
                index++;
            }
            this.Structure.Add(otherHeaders);
            return index;
        }

        private int AddClass(int index)
        {
            var classObject = new WrapperObject("TEMP_NAME", new List<WrapperType>());

            RulesUtility.ValidateToken(this.TokenList[index++], "namespace", "This is not an accurate namespace.");

            // TODO: When doing multifiles conversions PLEASE! add this back in.
            //classObject.VariableName = this.TokenList[index++];
            index++;    // for now just ignore the name.

            RulesUtility.ValidateToken(this.TokenList[index++], "{", "This is an invalid class opener.");

            if (RulesUtility.ValidAccessModifiers(this.ProgramTypeLanguage, this.TokenList[index]))
            {
                classObject.Value.Add(new WrapperString("ACCESS_MOD", this.TokenList[index++].ToLower()));
            }
            else
            {
                classObject.Value.Add(new WrapperString("ACCESS_MOD", "public"));
            }

            if (this.TokenList[index].ToLower() == "static")
            {
                classObject.Value.Add(new WrapperBool("IS_STATIC", true));
                index++;
            }
            else
            {
                classObject.Value.Add(new WrapperBool("IS_STATIC", false));
            }

            RulesUtility.ValidateToken(this.TokenList[index++], "class", "This is not an accurate class.");

            classObject.WrapperName = this.TokenList[index++];

            RulesUtility.ValidateToken(this.TokenList[index++], "{", "This is an invalid class opener.");

            index = BuildClassContent(index, classObject);

            RulesUtility.ValidateToken(this.TokenList[index++], "{", "This is an invalid class closer.");
            RulesUtility.ValidateToken(this.TokenList[index++], "{", "This is an invalid class closer.");

            this.Structure.Add(classObject);
            return index;
        }

        private int BuildClassContent(int index, WrapperObject classObject)
        {
            while (index < this.TokenList.Count - 2)
            {
                WrapperObject contentObject = new WrapperObject("TEMP_NAME", new List<WrapperType>());
                if (RulesUtility.ValidAccessModifiers(this.ProgramTypeLanguage, this.TokenList[index]))
                {
                    contentObject.Value.Add(new WrapperString("ACCESS_MOD", this.TokenList[index++].ToLower()));
                }
                else
                {
                    throw new Exception("This is an invalid function or variable opener.");
                }

                if (this.TokenList[index].ToLower() == "static")
                {
                    contentObject.Value.Add(new WrapperBool("IS_STATIC", true));
                    index++;
                }
                else
                {
                    contentObject.Value.Add(new WrapperBool("IS_STATIC", false));
                }

                if (RulesUtility.IsValidType(this.ProgramTypeLanguage, this.TokenList[index]))
                {
                    contentObject.Value.Add(new WrapperString("RETURN_TYPE", this.TokenList[index++].ToLower()));
                }
                else
                {
                    throw new Exception("This is an invalid return/set type.");
                }

                if (this.TokenList[index][0] == '_')
                {
                    contentObject.WrapperName = this.TokenList[index++];
                    if (this.TokenList[index++] != ";")
                        throw new Exception("This needs is a valid \';\'.");
                }
                else
                {
                    contentObject.WrapperName = this.TokenList[index++];
                    index = this.BuildFunction(index, contentObject);
                }

                classObject.Value.Add(contentObject);
            }
            
            return index;
        }

        private int BuildFunction(int index, WrapperObject functionObject)
        {
            RulesUtility.ValidateToken(this.TokenList[index++], "(", "This needs is a valid \'(\'.");

            WrapperObject parameters = new WrapperObject("PARAMETERS", new List<WrapperType>());
            int holderValue = 1;
            while (this.TokenList[index] != ")")
            {
                WrapperObject parameter = new WrapperObject($"PARAMETER_{holderValue++}", new List<WrapperType>());
                if (RulesUtility.IsValidType(this.ProgramTypeLanguage, this.TokenList[index]))
                {
                    string valueName = this.TokenList[index++];
                    if (this.TokenList[index] == "[")
                    {
                        valueName += "[]";
                        index += 2;
                    }
                    parameter.Value.Add(new WrapperString("VALUE_TYPE", valueName));
                    parameter.Value.Add(new WrapperString("PARAM_NAME", this.TokenList[index++]));
                    if (this.TokenList[index] == ")")
                    {
                        parameters.Value.Add(parameter);
                        break;
                    }
                    RulesUtility.ValidateToken(this.TokenList[index++], ",", "This needs is a valid \',\'.");
                }
                else
                {
                    throw new Exception("This is an invalid parameter type.");
                }
                parameters.Value.Add(parameter);
            }
            functionObject.Value.Add(parameters);

            RulesUtility.ValidateToken(this.TokenList[index++], ")", "This needs is a valid \')\'.");
            RulesUtility.ValidateToken(this.TokenList[index++], "{", "This needs is a valid \'{\'.");
            RulesUtility.ValidateToken(this.TokenList[index++], "}", "This needs is a valid \'}\'.");

            return index;
        }

        public override void ParseFile()
        {
            Log.Info("Staring to Parse Csharp file.");
            int index = 0;
            index = this.AddHeader(index);
            _ = this.AddClass(index);
            Log.Success("Parsing Csharp is Completed.");
        }

        private string CleanSourceCode()
        {
            var fileList = File.ReadAllLines(this.FullFile);
            var ans = string.Empty;
            bool isMultiLine = false;
            Log.Info($"Removing all comments in {this.Filename}");
            foreach (var line in fileList)
            {
                string temp = line;
                string result = string.Empty;
                for (int index = 0; index < line.Length; index++)
                {
                    if (isMultiLine)
                    {
                        if (line[index] == '*' && line[index + 1] == '/')
                        {
                            index++;
                            isMultiLine = false;
                        }
                        continue;
                    }
                    else if (line[index] == '/')
                    {
                        if (line[index + 1] == '*')
                        {
                            isMultiLine = true;
                            result += line.Substring(0, index++);
                            continue;
                        }
                        if (line[index + 1] == '/')
                        {
                            result += line.Substring(0, index++);
                            break;
                        }
                    }
                    else if (index == line.Length - 1)
                    {
                        result += line.Substring(0, index + 1);
                    }
                }
                ans += result;
            }
            return ans;
        }

        public override void ScanFile()
        {
            var fileContents = this.CleanSourceCode();
            string hold = string.Empty;
            Log.Info("Scanning Csharp file.");
            for (int index = 0; index < fileContents.Length; index++)
            {
                if (fileContents[index] == ' ') //|| fileContents[index] == '\n' || fileContents[index] == '\r')
                {
                    if (!string.IsNullOrEmpty(hold) && !string.IsNullOrWhiteSpace(hold))
                    {
                        this.TokenList.Add(hold);
                        hold = string.Empty;
                    }
                    continue;
                }
                else if (RulesUtility.ValidSymbol(this.ProgramTypeLanguage, fileContents[index]))
                {
                    if (fileContents[index] == '\"')
                    {
                        this.TokenList.Add(fileContents[index].ToString());
                        int subIndex = index + 1;
                        string content = string.Empty;
                        while (fileContents[subIndex] != '\"')
                        {
                            content += fileContents[subIndex++].ToString();
                        }
                        this.TokenList.Add(content);
                        index = subIndex;
                    }
                    else if (!string.IsNullOrEmpty(hold) && !string.IsNullOrWhiteSpace(hold))
                    {
                        this.TokenList.Add(hold);
                        hold = string.Empty;
                    }
                    this.TokenList.Add(fileContents[index].ToString());
                    continue;
                }
                else if (fileContents[index] == ';')
                {
                    if (!string.IsNullOrEmpty(hold) && !string.IsNullOrWhiteSpace(hold))
                    {
                        this.TokenList.Add(hold);
                        hold = string.Empty;
                    }
                    this.TokenList.Add(fileContents[index].ToString());
                    hold = string.Empty;
                    continue;
                }
                hold += fileContents[index];
                if (this.Keywords.Contains(hold.ToLower()))
                {
                    if (!string.IsNullOrEmpty(hold) && !string.IsNullOrWhiteSpace(hold))
                    {
                        this.TokenList.Add(hold);
                        hold = string.Empty;
                    }
                    continue;
                }
            }
            Log.Success("Scanning Csharp file is Completed.");
        }

        protected override void CreateKeywords()
        {
            // TODO: create this.Keywords to be valid Keywords, Error Keywords, and Warning Keywords.
            this.Keywords = new List<string>()
            {
                "abstract",
                "as",
                "base",
                "bool",
                "break",
                "byte",
                "case",
                "catch",
                "char",
                "checked",
                "class",
                "const",
                "continue",
                "decimal",
                "default",
                "delegate",
                "do",
                "double",
                "else",
                "enum",
                "event",
                "explicit",
                "extern",
                "false",
                "finally",
                "fixed",
                "float",
                "for",
                "foreach",
                "goto",
                "if",
                "implicit",
                //"in",
                "int",
                "interface",
                "internal",
                "is",
                "lock",
                "long",
                "namespace",
                "new",
                "null",
                "object",
                "operator",
                "out",
                "override",
                "params",
                "private",
                "protected",
                "public",
                "readonly",
                "ref",
                "return",
                "sbyte",
                "sealed",
                "short",
                "sizeof",
                "stackalloc",
                "static",
                "string",
                "struct",
                "switch",
                "this",
                "throw",
                "true",
                "try",
                "typeof",
                "unit",
                "ulong",
                "unchecked",
                "unsafe",
                "ushort",
                "using",
                "virtual",
                "void",
                "volatile",
                "while"
            };
        }
    }
}
