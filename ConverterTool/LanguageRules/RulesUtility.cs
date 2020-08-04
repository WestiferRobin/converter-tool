using System;
namespace ConverterTool.LanguageRules
{
    public static class RulesUtility
    {
        public static int ValidateToken(string source, string target, string errorMsg, int index)
        {
            if (source.ToLower() != target.ToLower())
                throw new Exception($"This is not an accurate {target}.");
            return ++index;
        }

        public static bool ValidAccessModifiers(ProgramType programType, string target)
        {
            switch (programType)
            {
                case ProgramType.C_SHARP:
                    return (target.ToLower() == "public" ||
                            target.ToLower() == "private" ||
                            target.ToLower() == "protected" ||
                            target.ToLower() == "internal");
                case ProgramType.JAVA:
                    return (target.ToLower() == "public" ||
                            target.ToLower() == "private" ||
                            target.ToLower() == "protected");
                default:
                    return false;
            }
        }

        public static bool ValidSymbol(ProgramType programType, char symbol)
        {
            switch (programType)
            {
                case ProgramType.C_SHARP:
                case ProgramType.JAVA:
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
                default:
                    return false;
            }
        }

        public static bool IsValidType(ProgramType programType, string target)
        {
            bool isBasicType;
            switch (programType)
            {
                case ProgramType.C_SHARP:
                    switch (target.ToLower())
                    {
                        case "void":
                        case "char":
                        case "short":
                        case "int":
                        case "long":
                        case "float":
                        case "double":
                        case "signed":
                        case "unsigned":
                        case "string":
                            isBasicType = true;
                            break;
                        default:
                            isBasicType = false;
                            break;
                    }
                    return isBasicType || char.IsUpper(target[0]); ;
                case ProgramType.JAVA:
                    switch (target.ToLower())
                    {
                        case "void":
                        case "char":
                        case "short":
                        case "int":
                        case "long":
                        case "float":
                        case "double":
                        case "signed":
                        case "unsigned":
                            isBasicType = true;
                            break;
                        default:
                            isBasicType = false;
                            break;
                    }

                    return isBasicType || char.IsUpper(target[0]);
                default:
                    return false;
            }
        }
    }
}
