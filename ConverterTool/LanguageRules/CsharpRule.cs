using System;
using System.Collections.Generic;
using System.IO;
using ConverterTool.Logger;
using ConverterTool.WrapperTypes;

namespace ConverterTool.LanguageRules
{
    internal class CsharpRule : LanguageRule
    {
        private readonly List<string> _autoPropertyList; 

        public CsharpRule(string filename) : base(LanguageType.PROGRAM_LANG, ProgramType.C_SHARP, filename)
        {
            this.InitKeywords();
            this._autoPropertyList = new List<string>();
        }

        public override void BuildFile()
        {
            Log.Warn("Csharp BuildFile is not ready.");
        }

        public override void ParseFile()
        {
            Log.Info("Staring to Parse Csharp file.");

            int index = 0;
            index = this.AddHeader(index);
            _ = this.AddNamespace(index);

            Log.Success("Parsing Csharp is Completed.");
        }

        private int AddHeader(int index)
        {
            var otherHeaders = new WrapperObject("HEADERS");
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

        private int AddNamespace(int index)
        {
            // TODO: FIGURE THIS WITH CLASS AND STRUCTURE AND SHIT
            var namespaceObject = new WrapperObject();

            index = RulesUtility.ValidateToken(this.TokenList[index], "namespace", "This is not an accurate namespace.", index);

            while (this.TokenList[index] != "{")
            {
                namespaceObject.WrapperName += this.TokenList[index++];
            }

            index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This is an invalid class opener.", index);

            while (this.TokenList[index] != "}")
            {
                var potentialObject = new WrapperObject();

                if (RulesUtility.ValidAccessModifiers(this.ProgramTypeLanguage, this.TokenList[index]))
                {
                    potentialObject.Value.Add(new WrapperString("ACCESS_MOD", this.TokenList[index++].ToLower()));
                }
                else
                {
                    potentialObject.Value.Add(new WrapperString("ACCESS_MOD", "public"));
                }

                if (this.TokenList[index].ToLower() == "abstract"
                    || this.TokenList[index].ToLower() == "override"
                    || this.TokenList[index].ToLower() == "virtual")
                {
                    potentialObject.Value.Add(new WrapperString("UNIQUE_MOD", this.TokenList[index++].ToLower()));
                }
                else
                {
                    potentialObject.Value.Add(new WrapperString("UNIQUE_MOD", this.TokenList[index++].ToLower()));
                }

                if (this.TokenList[index].ToLower() == "static")
                {
                    potentialObject.Value.Add(new WrapperBool("IS_STATIC", true));
                    index++;
                }
                else if (this.TokenList[index].ToLower() == "struct")
                {
                    index = this.AddClassLikeType(index, potentialObject, namespaceObject, "struct");
                    continue;
                }
                else if (this.TokenList[index].ToLower() == "enum")
                {
                    index = this.AddClassLikeType(index, potentialObject, namespaceObject, "enum");
                    continue;
                }
                else
                {
                    potentialObject.Value.Add(new WrapperBool("IS_STATIC", false));
                }

                index = this.AddClassLikeType(index, potentialObject, namespaceObject, "class");
            }

            index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This is an invalid class closer.", index);

            this.Structure.Add(namespaceObject);

            return index;
        }

        private int AddClassLikeType(int index, WrapperObject wrapperObject, WrapperObject parentObject, string flag)
        {
            index = RulesUtility.ValidateToken(this.TokenList[index], flag, $"This is not an accurate {flag}.", index);

            while (this.TokenList[index] != "{")
            {
                wrapperObject.WrapperName += this.TokenList[index++];
            }

            index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This is an invalid class opener.", index);

            switch (flag)
            {
                case "enum":
                    index = BuildEnumContent(index, wrapperObject);
                    break;
                case "struct":
                case "class":
                    index = BuildClassOrStructContent(index, wrapperObject, flag == "struct");
                    break;
                default:
                    throw new Exception($"Unable to make a enum, struct, or class with this token {flag}.");
            }

            index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This is an invalid class closer.", index);

            parentObject.Value.Add(wrapperObject);

            return index;
        }

        private int BuildEnumContent(int index, WrapperObject enumObject)
        {
            int times = 0;
            WrapperObject enumContent = new WrapperObject("OBJECT_CONTENT");

            while (this.TokenList[index] != "}")
            {
                WrapperString enumEntry = new WrapperString($"ENUM_VALUE_{times++}", string.Empty);

                while (this.TokenList[index] != "}" && this.TokenList[index] != ",")
                {
                    enumEntry.Value += this.TokenList[index++];
                }

                enumContent.Value.Add(enumEntry);

                if (this.TokenList[index] != "}")
                {
                    index = RulesUtility.ValidateToken(this.TokenList[index], ",", $"{enumObject.WrapperName} enum object needs a valid divider.", index);
                }
            }

            enumObject.Value.Add(enumContent);

            return index;
        }

        private int BuildClassOrStructContent(int index, WrapperObject classObject, bool isStruct)
        {
            WrapperObject classContent = new WrapperObject("OBJECT_CONTENT");
            while (this.TokenList[index] != "}")
            {
                WrapperObject contentObject = new WrapperObject();

                if (RulesUtility.ValidAccessModifiers(this.ProgramTypeLanguage, this.TokenList[index]))
                {
                    contentObject.Value.Add(new WrapperString("ACCESS_MOD", this.TokenList[index++].ToLower()));
                }
                else
                {
                    var classObjectAccessMod = (WrapperString)classObject.GetValue("ACCESS_MOD");
                    contentObject.Value.Add(new WrapperString("ACCESS_MOD", classObjectAccessMod.Value));
                }

                if (this.TokenList[index].ToLower() == "abstract"
                    || this.TokenList[index].ToLower() == "override"
                    || this.TokenList[index].ToLower() == "virtual")
                {
                    contentObject.Value.Add(new WrapperString("UNIQUE_MOD", this.TokenList[index++].ToLower()));
                }
                else
                {
                    contentObject.Value.Add(new WrapperString("UNIQUE_MOD", this.TokenList[index++].ToLower()));
                }

                if (this.TokenList[index].ToLower() == "static")
                {
                    contentObject.Value.Add(new WrapperBool("IS_STATIC", true));
                    index++;
                }
                else if (this.TokenList[index].ToLower() == "struct")
                {
                    index = this.AddClassLikeType(index, contentObject, classContent, "struct");
                    continue;
                }
                else if (this.TokenList[index].ToLower() == "enum")
                {
                    index = this.AddClassLikeType(index, contentObject, classContent, "enum");
                    continue;
                }
                else
                {
                    contentObject.Value.Add(new WrapperBool("IS_STATIC", false));
                }

                if (this.TokenList[index].ToLower() == "const")
                {
                    contentObject.Value.Add(new WrapperBool("IS_CONST", true));
                    index++;
                }
                else
                {
                    contentObject.Value.Add(new WrapperBool("IS_CONST", false));
                }

                if (this.TokenList[index] == "class")
                {
                    index = this.AddClassLikeType(index, contentObject, classContent, "class");
                    continue;
                }

                // Need to take into account for this being a constructor or a return type.
                if (this.TokenList[index + 1] != "(")
                {
                    if (RulesUtility.IsValidType(this.ProgramTypeLanguage, this.TokenList[index]))
                    {
                        string valueType = this.TokenList[index++];
                        if (this.TokenList[index] == "<")
                        {
                            while(this.TokenList[index] != ">")
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
                    index++;
                    index = this.BuildClassProperty(index, contentObject);
                }
                else
                {
                    index++;
                    WrapperBool isConst = (WrapperBool)contentObject.GetValue("IS_CONST");
                    if (isConst.Value)
                        throw new Exception("This cannot use constant for auto property and function.");
                    if (this.TokenList[index] == "(")
                    {
                        index = this.BuildFunction(index, contentObject, isStruct && classObject.WrapperName == contentObject.WrapperName);
                    }
                    else if (this.TokenList[index] == "{")
                    {
                        index = this.BuildAutoProperty(index, contentObject, classContent);
                        continue;
                    }
                    else
                    {
                        throw new Exception("This is not a function or an auto property.");
                    }
                }

                classContent.Value.Add(contentObject);
            }

            classObject.Value.Add(classContent);
            
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

        private int BuildAuxMethod(int index, WrapperObject auxObject, string variableName)
        {
            string flagName = auxObject.WrapperName.ToString().ToLower();
            index = RulesUtility.ValidateToken(this.TokenList[index], flagName,
                $"This needs is a valid \'{flagName}\'.", index);
            WrapperObject functionContent = new WrapperObject("FUNCTION_CONTENT");

            if (this.TokenList[index] == ";")
            {
                index = RulesUtility.ValidateToken(this.TokenList[index], ";", "This needs is a valid \';\'.", index);
                if (flagName == "get")
                {
                    functionContent.Value.Add(new WrapperString("STATEMENT_1", $"return {variableName}"));
                }
                else if (flagName == "set")
                {
                    functionContent.Value.Add(new WrapperString("STATEMENT_1", $"this.{variableName} = value"));
                }
                else
                {
                    throw new Exception($"This flag name {flagName} is not valid for autoproperty getter/setter");
                }
            }
            else if (this.TokenList[index] == "{")
            {
                index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This needs is a valid \'{\'.", index);

                index = this.FillFunctionContent(index, functionContent);

                index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This needs is a valid \'}\'.", index);
            }
            else
            {
                throw new Exception("This is not a valid set function of this auto property.");
            }
            auxObject.Value.Add(functionContent);

            return index;
        }

        private int BuildAutoProperty(int index, WrapperObject contentObject, WrapperObject parentObject)
        {
            index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This needs is a valid \'{\'.", index);
            WrapperObject setObject = new WrapperObject("SET");
            WrapperObject getObject = new WrapperObject("GET");

            contentObject.CopyData(getObject);
            contentObject.CopyData(setObject);

            string compVariableName = "_" + char.ToLower(contentObject.WrapperName[0]).ToString() + contentObject.WrapperName.Substring(1);
            string holdOlderName = contentObject.WrapperName;
            if (!parentObject.GetKeys().Contains(compVariableName))
            {
                contentObject.WrapperName = compVariableName;
                contentObject.UpdateStringValue("ACCESS_MOD", "private");
                parentObject.Value.Add(contentObject);
            }

            WrapperString valueType = setObject.GetValue("VALUE_TYPE") as WrapperString;
            WrapperObject parameters = new WrapperObject("PARAMETERS");
            WrapperObject parameter = new WrapperObject($"PARAMETER_1");
            parameter.Value.Add(new WrapperString("VALUE_TYPE", valueType.Value));
            parameter.Value.Add(new WrapperString("PARAM_NAME", "value"));
            setObject.UpdateStringValue("VALUE_TYPE", "void");
            parameters.Value.Add(parameter);
            setObject.Value.Add(parameters);

            if (this.TokenList[index] == "get")
            {
                index = this.BuildAuxMethod(index, getObject, compVariableName);
                index = this.BuildAuxMethod(index, setObject, compVariableName);
            }
            else if (this.TokenList[index] == "set")
            {
                index = this.BuildAuxMethod(index, setObject, compVariableName);
                index = this.BuildAuxMethod(index, getObject, compVariableName);
            }
            else
            {
                throw new Exception("This auto property needs an explicet get and set keywords.");
            }

            index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This needs is a valid \'}\'.", index);
            getObject.WrapperName = $"Get{holdOlderName}";
            setObject.WrapperName = $"Set{holdOlderName}";
            parentObject.Value.Add(getObject);
            parentObject.Value.Add(setObject);
            this._autoPropertyList.Add(holdOlderName);
            this._autoPropertyList.Add($"this.{holdOlderName}");

            return index;
        }

        private int BuildFunction(int index, WrapperObject functionObject, bool isStruct)
        {
            int holderValue = 1;
            WrapperObject parameters = new WrapperObject("PARAMETERS");

            index = RulesUtility.ValidateToken(this.TokenList[index], "(", "This needs is a valid \'(\'.", index);

            while (this.TokenList[index] != ")")
            {
                WrapperObject parameter = new WrapperObject($"PARAMETER_{holderValue++}");
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

            if (isStruct && parameters.Value.Count < 0)
                throw new Exception("You cannot have a default constructor for a struct object.");
            else if (parameters.Value.Count > 0)
                functionObject.Value.Add(parameters);

            index = RulesUtility.ValidateToken(this.TokenList[index], ")", "This needs is a valid \')\'.", index);
            index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This needs is a valid \'{\'.", index);
            WrapperObject functionContent = new WrapperObject("FUNCTION_CONTENT");

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
            WrapperObject cases = new WrapperObject("CASES");
            int caseCount = 1;

            index = this.FillConditionalStatement(index, wrapperObject);
            index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This needs is a valid \'{\'.", index);
            
            while (this.TokenList[index] != "}")
            {
                switch (this.TokenList[index])
                {
                    case "case":
                        List<WrapperType> instantCases = new List<WrapperType>();
                        WrapperObject thisCase = new WrapperObject($"CASE_{caseCount++}");
                        WrapperObject caseContent = new WrapperObject("CASE_CONTENT");

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
                            WrapperObject otherCase = new WrapperObject($"CASE_{caseCount++}");
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
                        WrapperObject defaultCase = new WrapperObject($"DEFAULT");
                        WrapperObject defaultContent = new WrapperObject("DEFAULT_CONTENT");

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

        private int FillFunctionContent(int index, WrapperObject functionContent, bool isNormalStatement = true)
        {
            int counter = 1;
            int whileLoopCount = 1;
            int forLoopCount = 1;
            int ifCount = 1;
            int ifElseCount = 1;
            int elseCount = 1;
            int switchCount = 1;
            int doCount = 1;
            int tryCount = 1;

            string endingToken = isNormalStatement ? "}" : "break";

            while (this.TokenList[index] != endingToken)
            {
                var values = string.Empty;
                string flag = this.TokenList[index].ToLower();
                switch (flag)
                {
                    case "for":
                        index = RulesUtility.ValidateToken(this.TokenList[index], "for", "This needs is a valid \'for\'.", index);
                        WrapperObject forObject = new WrapperObject($"FOR_{forLoopCount++}");

                        index = this.FillBracketStatement(index, forObject);

                        functionContent.Value.Add(forObject);
                        break;
                    case "while":
                        index = RulesUtility.ValidateToken(this.TokenList[index], "while", "This needs is a valid \'while\'.", index);
                        WrapperObject whileObject = new WrapperObject($"WHILE_{whileLoopCount++}");

                        index = this.FillBracketStatement(index, whileObject);

                        functionContent.Value.Add(whileObject);
                        break;
                    case "if":
                        index = RulesUtility.ValidateToken(this.TokenList[index], "if", "This needs is a valid \'if\'.", index);

                        WrapperObject ifObject = new WrapperObject($"IF_{ifCount++}");

                        index = this.FillBracketStatement(index, ifObject);

                        functionContent.Value.Add(ifObject);
                        break;
                    case "else":
                        index = RulesUtility.ValidateToken(this.TokenList[index], "else", "This needs is a valid \'else\'.", index);

                        if (this.TokenList[index].ToLower() == "if")
                        {
                            index = RulesUtility.ValidateToken(this.TokenList[index], "if", "This needs is a valid \'if\'.", index);
                            WrapperObject elseIfObject = new WrapperObject($"ELSE_IF_{ifElseCount++}");

                            index = this.FillBracketStatement(index, elseIfObject);

                            functionContent.Value.Add(elseIfObject);
                        }
                        else
                        {
                            WrapperObject elseObject = new WrapperObject($"ELSE_{elseCount++}");

                            index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This needs is a valid \'{\'.", index);
                            index = this.FillFunctionContent(index, elseObject);
                            index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This needs is a valid \'}\'.", index);

                            functionContent.Value.Add(elseObject);
                        }
                        break;
                    case "switch":
                        index = RulesUtility.ValidateToken(this.TokenList[index], "switch", "This needs is a valid \'switch\'.", index);

                        WrapperObject switchObject = new WrapperObject($"SWITCH_{switchCount++}");

                        index = this.FillSwitchStatement(index, switchObject);

                        functionContent.Value.Add(switchObject);
                        break;
                    case "do":
                        index = RulesUtility.ValidateToken(this.TokenList[index], "do", "This needs is a valid \'do\'.", index);

                        WrapperObject doObject = new WrapperObject($"DO_{doCount++}");

                        index = this.FillDoWhile(index, doObject);

                        functionContent.Value.Add(doObject);
                        break;
                    case "try":
                        index = RulesUtility.ValidateToken(this.TokenList[index], "try", "This needs is a valid \'try\'.", index);

                        WrapperObject tryObject = new WrapperObject($"TRY_{tryCount++}");

                        index = this.FillTryCatch(index, tryObject);

                        functionContent.Value.Add(tryObject);
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
                        values = this.CleanStatement(values);
                        values += ";";
                        functionContent.Value.Add(new WrapperString($"STATEMENT_{counter++}", values));
                        break;
                }
            }
            return index;
        }

        private int FillDoWhile(int index, WrapperObject doObject)
        {
            WrapperObject doContentObject = new WrapperObject("DO_CONTENT");
            index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This needs is a valid \'{\'.", index);
            index = this.FillFunctionContent(index, doContentObject);
            index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This needs is a valid \'}\'.", index);
            
            WrapperString whileCond = new WrapperString("WHILE_COND", string.Empty);
            index = RulesUtility.ValidateToken(this.TokenList[index], "while", "This needs is a valid \'while\'.", index);
            while (this.TokenList[index] != ")")
            {
                string lookAhead = this.TokenList[index + 1];
                whileCond.Value += this.TokenList[index];
                if (lookAhead != "." && lookAhead != "(" && lookAhead != ")" && lookAhead != "\'" && lookAhead != ";"
                    && this.TokenList[index] != "." && this.TokenList[index] != "(" && this.TokenList[index] != ")"
                    && this.TokenList[index] != "\"" && this.TokenList[index] != "\'")
                    whileCond.Value += " ";
                index++;
            }

            index = RulesUtility.ValidateToken(this.TokenList[index], ")", "This needs is a valid \')\'.", index);
            whileCond.Value += ")";

            index = RulesUtility.ValidateToken(this.TokenList[index], ";", "This needs is a valid \';\'.", index);
            whileCond.Value = this.CleanStatement(whileCond.Value);
            whileCond.Value += ";";

            doObject.Value.Add(doContentObject);
            doObject.Value.Add(whileCond);
            return index;
        }

        private int FillTryCatch(int index, WrapperObject mainTryObject)
        {
            WrapperObject tryBlockObject = new WrapperObject("TRY_CONTENT");

            index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This needs is a valid \'{\'.", index);
            index = this.FillFunctionContent(index, tryBlockObject);
            index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This needs is a valid \'}\'.", index);

            mainTryObject.Value.Add(tryBlockObject);

            if (this.TokenList[index] == "catch")
            {
                WrapperObject catchBlockObject = new WrapperObject("CATCH_CONTENT");

                index = RulesUtility.ValidateToken(this.TokenList[index], "catch", "This needs is a valid \'catch\'.", index);

                if (this.TokenList[index] == "(")
                {
                    string exStatement = string.Empty;

                    while (this.TokenList[index] != ")")
                    {
                        string lookAhead = this.TokenList[index + 1];
                        exStatement += this.TokenList[index];
                        if (lookAhead != "." && lookAhead != "(" && lookAhead != ")" && lookAhead != "\'" && lookAhead != ";"
                            && this.TokenList[index] != "." && this.TokenList[index] != "(" && this.TokenList[index] != ")"
                            && this.TokenList[index] != "\"" && this.TokenList[index] != "\'")
                            exStatement += " ";
                        index++;
                    }

                    index = RulesUtility.ValidateToken(this.TokenList[index], ")", "This needs is a valid \')\'.", index);
                    exStatement += ")";

                    catchBlockObject.Value.Add(new WrapperString("CATCH_COND", exStatement));
                }

                index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This needs is a valid \'{\'.", index);
                index = this.FillFunctionContent(index, catchBlockObject);
                index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This needs is a valid \'}\'.", index);

                mainTryObject.Value.Add(catchBlockObject);
            }

            if (this.TokenList[index] == "finally")
            {
                WrapperObject finallyObject = new WrapperObject("FINALLY_CONTENT");

                index = RulesUtility.ValidateToken(this.TokenList[index], "finally", "This needs is a valid \'finally\'.", index);
                index = RulesUtility.ValidateToken(this.TokenList[index], "{", "This needs is a valid \'{\'.", index);
                index = this.FillFunctionContent(index, finallyObject);
                index = RulesUtility.ValidateToken(this.TokenList[index], "}", "This needs is a valid \'}\'.", index);

                mainTryObject.Value.Add(finallyObject);
            }

            return index;
        }

        private string CleanStatement(string values)
        {
            string[] splitAssignment = values.Split('=');
            string assignValue = string.Empty;

            if (splitAssignment.Length > 2)
            {
                throw new Exception($"Clean Statement didn't work for line {values}");
            }
            else if (splitAssignment.Length == 2)
            {
                foreach (var autoProp in this._autoPropertyList)
                {
                    if (splitAssignment[1].Contains(autoProp) && !autoProp.Contains(".this"))
                    {
                        string result = autoProp.Replace("this.", string.Empty);
                        splitAssignment[1] = splitAssignment[1].Replace($"this.{autoProp}", $"this.Get{result}()");
                    }
                }
                assignValue = splitAssignment[1];
                if (this._autoPropertyList.Contains(splitAssignment[0].Replace(" ", string.Empty)))
                {
                    string result = splitAssignment[0].Replace("this.", string.Empty);
                    values = $"this.Set{result}({assignValue})";
                }
                else
                {
                    values = $"{splitAssignment[0]} = {assignValue}";
                }
            }

            return values;
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
                if (fileContents[index] == ' ')
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
            Log.Success("Scanning Csharp file is Completed.");
        }

        protected override void InitKeywords()
        {
            this.ValidKeywords = new List<string>()
            {
                "abstract",
                "base",
                "bool",
                "break",
                "byte",
                "case",
                "catch",
                "char",
                "class",
                "const",
                "continue",
                "decimal",
                "default",
                "do",
                "double",
                "else",
                "enum",
                "false",
                "finally",
                "float",
                "for",
                "if",
                "int",
                "interface",
                "is",
                "long",
                "namespace",
                "new",
                "null",
                "override",
                "private",
                "protected",
                "public",
                "return",
                "short",
                "static",
                "string",
                "struct",
                "switch",
                "this",
                "throw",
                "true",
                "try",
                "using",
                "void",
                "while"
            };
            this.WarningKeywords = new List<string>()
            {
                //"as",
                "foreach",
                "goto",
                //"in",
                //"internal",
                "object",
                "readonly",
                "sbyte",
                "sizeof",
                "typeof",
                "unit",
                "ulong",
                "ushort",
                "virtual",
            };
            this.ErrorKeywords = new List<string>()
            {
                "checked",
                "delegate",
                "event",
                "explicit",
                "extern",
                "fixed",
                "implicit",
                "lock",
                "operator",
                "out",
                "params",
                "ref",
                "sealed",
                "stackalloc",
                "unchecked",
                "unsafe",
            };
        }
    }
}
