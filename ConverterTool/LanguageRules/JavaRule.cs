using ConverterTool.Logger;
using ConverterTool.WrapperTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConverterTool.LanguageRules
{
    internal class JavaRule : LanguageRule
    {
        public JavaRule(string filename) : base(LanguageType.PROGRAM_LANG, ProgramType.JAVA, filename)
        {
            this.CreateKeywords();
        }

        public override void BuildFile()
        {
            Log.Warn("Java BuildFile is not ready.");
        }

        private int AddHeader(int index)
        {
            var otherHeaders = new WrapperObject("HEADERS", new List<WrapperType>());
            // TODO: make sure to implement this while multi file task
            while (this.TokenList[index].ToLower() != "class" &&
                !RulesUtility.ValidAccessModifiers(this.ProgramTypeLanguage, this.TokenList[index]))
            {
                if (this.TokenList[index].ToLower() == "import")
                {
                    index++;
                    if (this.TokenList[index].ToLower() == "java")
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

            if (RulesUtility.ValidAccessModifiers(this.ProgramTypeLanguage, this.TokenList[index]))
            {
                classObject.Value.Add(new WrapperString("ACCESS_MOD", this.TokenList[index++].ToLower()));
            }
            else
            {
                classObject.Value.Add(new WrapperString("ACCESS_MOD", "public"));
            }

            classObject.Value.Add(new WrapperBool("IS_STATIC", this.TokenList[index].Contains("static")));

            RulesUtility.ValidateToken(this.TokenList[index++], "class", "This is not an accurate class.");

            classObject.WrapperName = this.TokenList[index++];

            RulesUtility.ValidateToken(this.TokenList[index++], "{", "This is an invalid class opener.");

            index = BuildClassContent(index, classObject);

            RulesUtility.ValidateToken(this.TokenList[index++], "}", "This is an invalid class closer.");

            this.Structure.Add(classObject);
            return index;
        }

        private int BuildClassContent(int index, WrapperObject classObject)
        {
            while (index < this.TokenList.Count - 1)
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
                    RulesUtility.ValidateToken(this.TokenList[index++], ";", "This needs is a valid \';\'.");
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
            Log.Info("Staring to Parse Java file.");
            int index = 0;
            index = this.AddHeader(index);
            _ = this.AddClass(index);
            Log.Success("Parsing Java file is Completed.");
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
            Log.Info("Scanning Java file.");
            for (int index = 0; index < fileContents.Length; index++)
            {
                if (fileContents[index] == ' ' || fileContents[index] == '\n' || fileContents[index] == '\r')
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
            Log.Success("Scanning Java file is Completed.");
        }


        protected override void CreateKeywords()
        {
            this.Keywords = new List<string>()
            {
                "abstract",
                "assert",
                "boolean",
                "byte",
                "case",
                "catch",
                "char",
                "class",
                "const",
                "continue",
                "default",
                "do",
                "double",
                "else",
                "enum",
                "extends",
                "final",
                "finally",
                "float",
                "for",
                "goto",
                "if",
                "implements",
                "import",
                "instanceof",
                "int",
                "interface",
                "long",
                "native",
                "new",
                "package",
                "private",
                "protected",
                "public",
                "return",
                "short",
                "static",
                "strictfp",
                "super",
                "swtich",
                "synchronized",
                "this",
                "throw",
                "throws",
                "transient",
                "try",
                "void"
            };
        }
    }
}
