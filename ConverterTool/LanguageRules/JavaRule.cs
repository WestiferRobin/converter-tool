using ConverterTool.Logger;
using ConverterTool.WrapperTypes;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConverterTool.LanguageRules
{ 
    internal class JavaRule : LanguageRule
    {
        public JavaRule(string filename) : base(LanguageType.PROGRAM_LANG, ProgramType.JAVA, filename)
        {
            this.InitKeywords();
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
                else if (this.TokenList[index].ToLower() == "package")
                {
                    index = RulesUtility.ValidateToken(this.TokenList[index], "package", "This is not an accurate package.", index);
                    string packageName = string.Empty;
                    while (this.TokenList[index] != ";")
                    {
                        packageName += this.TokenList[index++];
                    }
                    otherHeaders.Value.Add(new WrapperString("PACKAGE", packageName));
                }
                index++;
            }

            this.Structure.Add(otherHeaders);

            return index;
        }

        private int AddClass(int index)
        {
            var classObject = new WrapperObject(string.Empty, new List<WrapperType>());

            if (RulesUtility.ValidAccessModifiers(this.ProgramTypeLanguage, this.TokenList[index]))
            {
                classObject.Value.Add(new WrapperString("ACCESS_MOD", this.TokenList[index++].ToLower()));
            }
            else
            {
                classObject.Value.Add(new WrapperString("ACCESS_MOD", "public"));
            }

            classObject.Value.Add(new WrapperBool("IS_STATIC", this.TokenList[index].Contains("static")));

            index = RulesUtility.ValidateToken(this.TokenList[index], "class", "This is not an accurate class.", index);

            while (this.TokenList[index] != "{")
            {
                classObject.WrapperName += this.TokenList[index++];
            }

            index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This is an invalid class opener.", index);

            index = BuildClassContent(index, classObject);

            index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This is an invalid class closer.", index);

            this.Structure.Add(classObject);
            return index;
        }

        private int BuildClassContent(int index, WrapperObject classObject)
        {
            while (index < this.TokenList.Count - 1)
            {
                WrapperObject contentObject = new WrapperObject(string.Empty, new List<WrapperType>());
                if (RulesUtility.ValidAccessModifiers(this.ProgramTypeLanguage, this.TokenList[index]))
                {
                    contentObject.Value.Add(new WrapperString("ACCESS_MOD", this.TokenList[index++].ToLower()));
                }
                else
                {
                    var classObjectAccessMod = (WrapperString)classObject.GetValue("ACCESS_MOD");
                    classObject.Value.Add(new WrapperString("ACCESS_MOD", classObjectAccessMod.Value));
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

                if (this.TokenList[index].ToLower() == "final")
                {
                    contentObject.Value.Add(new WrapperBool("IS_CONST", true));
                    index++;
                }
                else
                {
                    contentObject.Value.Add(new WrapperBool("IS_CONST", false));
                }

                if (this.TokenList[index + 1] != "(")
                {
                    if (RulesUtility.IsValidType(this.ProgramTypeLanguage, this.TokenList[index]))
                    {
                        string valueType = this.TokenList[index++];
                        if (this.TokenList[index] == "<")
                        {
                            while (this.TokenList[index] != ">")
                            {
                                valueType += this.TokenList[index++];
                            }
                            valueType += this.TokenList[index++];
                        }
                        contentObject.Value.Add(new WrapperString("VALUE_TYPE", valueType));
                    }
                    else
                    {
                        throw new Exception("This is an invalid return/set type.");
                    }
                }

                if (this.TokenList[index].ToUpper() == this.TokenList[index])
                {
                    string constName = string.Empty;
                    while (this.TokenList[index] != "=")
                    {
                        constName += this.TokenList[index++];
                    }
                    contentObject.WrapperName = constName;
                    index--;
                }
                else
                {
                    contentObject.WrapperName = this.TokenList[index];
                }

                if (this.TokenList[index][0] == '_' || (this.TokenList[index][0] >= 'A' && this.TokenList[index][0] <= 'Z' && this.TokenList[index + 1] != "{" && this.TokenList[index + 1] != "("))
                {
                    contentObject.WrapperName = this.TokenList[index++];
                    index = this.BuildClassProperty(index, contentObject);
                }
                else
                {
                    index++;
                    WrapperBool isConst = (WrapperBool)contentObject.GetValue("IS_CONST");
                    if (isConst.Value)
                        throw new Exception("This cannot use constant for auto property and function.");
                    index = this.BuildFunction(index, contentObject);
                }

                classObject.Value.Add(contentObject);
            }

            return index;
        }

        private int BuildClassProperty(int index, WrapperObject contentObject)
        {
            if (this.TokenList[index] == ";")
            {
                index = RulesUtility.ValidateToken(this.TokenList[index], ";", "This needs is a valid \';\'.", index);
                return index;
            }
            var type = (WrapperString)contentObject.GetValue("VALUE_TYPE");
            string statement = $"{type.Value} {contentObject.WrapperName} ";
            while (this.TokenList[index] != ";")
            {
                string lookAhead = this.TokenList[index + 1];
                statement += this.TokenList[index];
                if (lookAhead != "." && lookAhead != "(" && lookAhead != ")" && lookAhead != "\'" && lookAhead != ";"
                    && this.TokenList[index] != "." && this.TokenList[index] != "(" && this.TokenList[index] != ")"
                    && this.TokenList[index] != "\"" && this.TokenList[index] != "\'")
                    statement += " ";
                index++;
            }
            index = RulesUtility.ValidateToken(this.TokenList[index], ";", "This needs is a valid \';\'.", index);
            contentObject.Value.Add(new WrapperString("STATEMENT_1", statement));
            return index;
        }

        private int BuildFunction(int index, WrapperObject functionObject)
        {
            int holderValue = 1;
            WrapperObject parameters = new WrapperObject("PARAMETERS", new List<WrapperType>());

            index = RulesUtility.ValidateToken(this.TokenList[index], "(", "This needs is a valid \'(\'.", index);

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

        private int FillBracketStatement(int index, WrapperObject wrapperObject)
        {
            index = this.FillConditionalStatement(index, wrapperObject);

            index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This needs is a valid \'{\'.", index);
            index = this.FillFunctionContent(index, wrapperObject);
            index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This needs is a valid \'}\'.", index);

            return index;
        }

        private int FillSwitchStatement(int index, WrapperObject wrapperObject)
        {
            WrapperObject cases = new WrapperObject("CASES", new List<WrapperType>());
            int caseCount = 1;

            index = this.FillConditionalStatement(index, wrapperObject);
            index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This needs is a valid \'{\'.", index);

            while (this.TokenList[index] != "}")
            {
                switch (this.TokenList[index])
                {
                    case "case":
                        List<WrapperType> instantCases = new List<WrapperType>();
                        WrapperObject thisCase = new WrapperObject($"CASE_{caseCount++}", new List<WrapperType>());
                        WrapperObject caseContent = new WrapperObject("CASE_CONTENT", new List<WrapperType>());

                        index = RulesUtility.ValidateToken(this.TokenList[index], "case", "This needs is a valid \'case\'.", index);
                        string caseValue = string.Empty;
                        while (this.TokenList[index] != ":")
                        {
                            caseValue += this.TokenList[index++];
                        }
                        thisCase.Value.Add(new WrapperString("CASE_VALUE", caseValue));
                        index = RulesUtility.ValidateToken(this.TokenList[index], ":", "This needs is a valid \':\'.", index);
                        instantCases.Add(thisCase);

                        while (this.TokenList[index] == "case")
                        {
                            WrapperObject otherCase = new WrapperObject($"CASE_{caseCount++}", new List<WrapperType>());
                            index = RulesUtility.ValidateToken(this.TokenList[index], "case", "This needs is a valid \'case\'.", index);
                            string multiCaseValue = string.Empty;
                            while (this.TokenList[index] != ":")
                            {
                                multiCaseValue += this.TokenList[index++];
                            }
                            otherCase.Value.Add(new WrapperString("CASE_VALUE", multiCaseValue));
                            index = RulesUtility.ValidateToken(this.TokenList[index], ":", "This needs is a valid \':\'.", index);
                            instantCases.Add(otherCase);
                        }

                        index = this.FillFunctionContent(index, caseContent, false);
                        index = RulesUtility.ValidateToken(this.TokenList[index], "break", "This needs is a valid \'break\'.", index);
                        index = RulesUtility.ValidateToken(this.TokenList[index], ";", "This needs is a valid \';\'.", index);

                        foreach (var inst in instantCases)
                        {
                            (inst as WrapperObject).Value.Add(caseContent);
                            cases.Value.Add(inst);
                        }
                        break;
                    case "default":
                        WrapperObject defaultCase = new WrapperObject($"DEFAULT", new List<WrapperType>());
                        WrapperObject defaultContent = new WrapperObject("DEFAULT_CONTENT", new List<WrapperType>());

                        index = RulesUtility.ValidateToken(this.TokenList[index], "default", "This needs is a valid \'default\'.", index);
                        index = RulesUtility.ValidateToken(this.TokenList[index], ":", "This needs is a valid \':\'.", index);

                        index = this.FillFunctionContent(index, defaultContent, false);

                        index = RulesUtility.ValidateToken(this.TokenList[index], "break", "This needs is a valid \'break\'.", index);
                        index = RulesUtility.ValidateToken(this.TokenList[index], ";", "This needs is a valid \';\'.", index);

                        defaultCase.Value.Add(defaultContent);
                        cases.Value.Add(defaultCase);
                        break;
                    default:
                        throw new Exception("No valid case or default for switch statement");
                }
            }
            index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This needs is a valid \'}\'.", index);
            return index;
        }

        private int FillConditionalStatement(int index, WrapperObject wrapperObject)
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

            return index;
        }

        private int FillFunctionContent(int index, WrapperObject functionContent, bool isSwitchStatement = true)
        {
            int counter = 1;
            int whileLoopCount = 1;
            int forLoopCount = 1;
            int ifCount = 1;
            int ifElseCount = 1;
            int elseCount = 1;
            int switchCount = 1;

            string endingToken = isSwitchStatement ? "}" : "break";

            while (this.TokenList[index] != endingToken)
            {
                var values = string.Empty;
                string flag = this.TokenList[index].ToLower();

                switch (flag)
                {
                    case "for":
                        index = RulesUtility.ValidateToken(this.TokenList[index], "for", "This needs is a valid \'for\'.", index);
                        WrapperObject forObject = new WrapperObject($"FOR_{forLoopCount++}", new List<WrapperType>());

                        index = this.FillBracketStatement(index, forObject);

                        functionContent.Value.Add(forObject);
                        break;
                    case "while":
                        index = RulesUtility.ValidateToken(this.TokenList[index], "while", "This needs is a valid \'while\'.", index);
                        WrapperObject whileObject = new WrapperObject($"WHILE_{whileLoopCount++}", new List<WrapperType>());

                        index = this.FillBracketStatement(index, whileObject);

                        functionContent.Value.Add(whileObject);
                        break;
                    case "if":
                        index = RulesUtility.ValidateToken(this.TokenList[index], "if", "This needs is a valid \'if\'.", index);

                        WrapperObject ifObject = new WrapperObject($"IF_{ifCount++}", new List<WrapperType>());

                        index = this.FillBracketStatement(index, ifObject);

                        functionContent.Value.Add(ifObject);
                        break;
                    case "else":
                        index = RulesUtility.ValidateToken(this.TokenList[index], "else", "This needs is a valid \'else\'.", index);

                        if (this.TokenList[index].ToLower() == "if")
                        {
                            index = RulesUtility.ValidateToken(this.TokenList[index], "if", "This needs is a valid \'if\'.", index);
                            WrapperObject elseIfObject = new WrapperObject($"ELSE_IF_{ifElseCount++}", new List<WrapperType>());

                            index = this.FillBracketStatement(index, elseIfObject);

                            functionContent.Value.Add(elseIfObject);
                        }
                        else
                        {
                            WrapperObject elseObject = new WrapperObject($"ELSE_{elseCount++}", new List<WrapperType>());

                            index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This needs is a valid \'{\'.", index);
                            index = this.FillFunctionContent(index, elseObject);
                            index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This needs is a valid \'}\'.", index);

                            functionContent.Value.Add(elseObject);
                        }
                        break;
                    case "switch":
                        index = RulesUtility.ValidateToken(this.TokenList[index], "switch", "This needs is a valid \'switch\'.", index);

                        WrapperObject switchObject = new WrapperObject($"SWITCH_{switchCount++}", new List<WrapperType>());

                        index = this.FillSwitchStatement(index, switchObject);

                        functionContent.Value.Add(switchObject);
                        break;
                    default:
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
                        break;
                }
            }
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
                if (this.IsValidKeyword(hold.ToLower()))
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

        protected override void InitKeywords()
        {
            this.ValidKeywords = new List<string>()
            {
                "abstract",
                "boolean",
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
                "finally",
                "float",
                "for",
                "if",
                "implements",
                "import",
                "int",
                "interface",
                "instanceof",
                "long",
                "new",
                "package",
                "private",
                "protected",
                "public",
                "return",
                "short",
                "static",
                "super",
                "swtich",
                "this",
                "throw",
                "try",
                "void",
                "while"
            };
            this.WarningKeywords = new List<string>()
            {
                "byte",
                "enum",
                "extends",
                "final",
                "goto",
            };
            this.ErrorKeywords = new List<string>()
            {
                "assert",
                "native",
                "strictfp",
                "swtich",
                "synchronized",
                "throws",
                "transient",
            };
        }
    }
}
