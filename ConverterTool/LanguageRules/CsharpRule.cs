using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ConverterTool.Logger;

namespace ConverterTool.LanguageRules
{
    internal class CsharpRule : LanguageRule
    {
        public CsharpRule(string filename) : base(LanguageType.PROGRAM_LANG, ProgramType.C_SHARP, filename)
        {

        }

        public override void BuildFile()
        {
            Log.Warn("Csharp BuildFile is not ready.");
        }

        public override void ParseFile()
        {
            Log.Info("Staring to Parse Csharp file.");
            foreach(var keyword in this.TokenList)
            {
                //Log.Info(keyword);
            }
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
                else if (this.ValidSymbol(fileContents[index]))
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

        private bool ValidSymbol(char symbol)
        {
            switch (symbol)
            {
                case '{':
                case '}':
                case '(':
                case ')':
                case '[':
                case ']':
                case '-':
                case '+':
                case '%':
                case '^':
                case '*':
                case '\\':
                case '\"':
                case '\'':
                case '<':
                case '>':
                case '&':
                case '|':
                case '!':
                case '@':
                case '=':
                case ':':
                case '?':
                case ',':
                case '.':
                    return true;
                default:
                    return false;
            }
        }

        protected override void CreateKeywords()
        {
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
                "in",
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
