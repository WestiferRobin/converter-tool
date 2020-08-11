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

        private int AddHeader(int index)
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

            index = RulesUtility.ValidateToken(this.TokenList[index], "namespace", "This is not an accurate namespace.", index);

            // TODO: When doing multifiles conversions PLEASE! add this back in.
            //classObject.VariableName = this.TokenList[index++];
            index++;    // for now just ignore the name.

            index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This is an invalid class opener.", index);

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

            index = RulesUtility.ValidateToken(this.TokenList[index], "class", "This is not an accurate class.", index);

            classObject.WrapperName = this.TokenList[index++];

            index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This is an invalid class opener.", index);

            index = BuildClassContent(index, classObject);

            index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This is an invalid class closer.", index);
            index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This is an invalid class closer.", index);

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
                    index = RulesUtility.ValidateToken(this.TokenList[index], ";", "This needs is a valid \';\'.", index);
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
            index = RulesUtility.ValidateToken(this.TokenList[index], "(", "This needs is a valid \'(\'.", index);

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
                    index = RulesUtility.ValidateToken(this.TokenList[index], ",", "This needs is a valid \',\'.", index);
                }
                else
                {
                    throw new Exception("This is an invalid parameter type.");
                }
                parameters.Value.Add(parameter);
            }
            functionObject.Value.Add(parameters);

            index = RulesUtility.ValidateToken(this.TokenList[index], ")", "This needs is a valid \')\'.", index);
            index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This needs is a valid \'{\'.", index);
            WrapperObject functionContent = new WrapperObject("FUNCTION_CONTENT", new List<WrapperType>());

            index = this.FillFunctionContent(index, functionContent);

            functionObject.Value.Add(functionContent);
            index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This needs is a valid \'}\'.", index);

            return index;
        }

        private int FillInnerBrackets(int index, WrapperObject wrapperObject)
        {
            string conditionStatement = string.Empty;
            var values = string.Empty;
            index = RulesUtility.ValidateToken(this.TokenList[index], "(", "This needs is a valid \'(\'.", index);
            conditionStatement += "(";
            while (this.TokenList[index] != ")")
            {
                string lookAhead = this.TokenList[index + 1];
                values += this.TokenList[index];
                if (lookAhead != "." && lookAhead != "(" && lookAhead != ")" && lookAhead != "\'" && lookAhead != ";"
                    && this.TokenList[index] != "." && this.TokenList[index] != "(" && this.TokenList[index] != ")"
                    && this.TokenList[index] != "\"" && this.TokenList[index] != "\'")
                    values += " ";
                index++;
            }
            conditionStatement += values;
            index = RulesUtility.ValidateToken(this.TokenList[index], ")", "This needs is a valid \')\'.", index);
            conditionStatement += ")";

            wrapperObject.Value.Add(new WrapperString("CONDITION_STATEMENT", conditionStatement));
            index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This needs is a valid \'{\'.", index);
            index = this.FillFunctionContent(index, wrapperObject);
            index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This needs is a valid \'}\'.", index);
            return index;
        }

        private int FillFunctionContent(int index, WrapperObject functionContent)
        {
            int counter = 1;
            int whileLoopCount = 1;
            int forLoopCount = 1;
            int ifCount = 1;
            int ifElseCount = 1;
            int elseCount = 1;
            while (this.TokenList[index] != "}")
            {
                var values = string.Empty;
                if (this.TokenList[index].ToLower() == "for")
                {
                    index = RulesUtility.ValidateToken(this.TokenList[index], "for", "This needs is a valid \'for\'.", index);
                    WrapperObject forObject = new WrapperObject($"FOR_{forLoopCount++}", new List<WrapperType>());

                    index = this.FillInnerBrackets(index, forObject);

                    functionContent.Value.Add(forObject);
                }
                else if (this.TokenList[index].ToLower() == "while")
                {
                    index = RulesUtility.ValidateToken(this.TokenList[index], "while", "This needs is a valid \'while\'.", index);
                    WrapperObject whileObject = new WrapperObject($"WHILE_{whileLoopCount++}", new List<WrapperType>());

                    index = this.FillInnerBrackets(index, whileObject);

                    functionContent.Value.Add(whileObject);
                }
                else if (this.TokenList[index].ToLower() == "if")
                {
                    index = RulesUtility.ValidateToken(this.TokenList[index], "if", "This needs is a valid \'if\'.", index);

                    WrapperObject ifObject = new WrapperObject($"IF_{ifCount++}", new List<WrapperType>());

                    index = this.FillInnerBrackets(index, ifObject);

                    functionContent.Value.Add(ifObject);
                }
                else if (this.TokenList[index].ToLower() == "else")
                {
                    index = RulesUtility.ValidateToken(this.TokenList[index], "else", "This needs is a valid \'else\'.", index);

                    if (this.TokenList[index].ToLower() == "if")
                    {
                        index = RulesUtility.ValidateToken(this.TokenList[index], "if", "This needs is a valid \'if\'.", index);
                        WrapperObject ifObject = new WrapperObject($"ELSE_IF_{ifElseCount++}", new List<WrapperType>());

                        index = this.FillInnerBrackets(index, ifObject);

                        functionContent.Value.Add(ifObject);
                    }
                    else
                    {
                        WrapperObject elseObject = new WrapperObject($"ELSE_{elseCount++}", new List<WrapperType>());

                        index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This needs is a valid \'{\'.", index);
                        index = this.FillFunctionContent(index, elseObject);
                        index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This needs is a valid \'}\'.", index);

                        functionContent.Value.Add(elseObject);
                    }
                }
                else
                {
                    while (this.TokenList[index] != ";")
                    {
                        string lookAhead = this.TokenList[index + 1];
                        values += this.TokenList[index];
                        if (lookAhead != "." && lookAhead != "(" && lookAhead != ")" && lookAhead != "\'" && lookAhead != ";"
                            && this.TokenList[index] != "." && this.TokenList[index] != "(" && this.TokenList[index] != ")"
                            && this.TokenList[index] != "\"" && this.TokenList[index] != "\'")
                            values += " ";
                        index++;
                    }
                    index = RulesUtility.ValidateToken(this.TokenList[index], ";", "This needs is a valid \';\'.", index);
                    values += ";";
                    functionContent.Value.Add(new WrapperString($"STATEMENT_{counter++}", values));
                }
            }
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
                //"as",
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
