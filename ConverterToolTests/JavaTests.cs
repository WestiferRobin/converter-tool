using System.Collections.Generic;
using System.IO;
using ConverterTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConverterToolTests
{
    [TestClass]
    public class JavaTests : TestGeneral
    {
        private readonly List<string> targetExtensions;

        public JavaTests() : base("JavaFiles")
        {
            targetExtensions = new List<string> { "cs" };
        }

        private void generalTest(string fileName)
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, $"{fileName}.java");

            foreach (var extension in targetExtensions)
            {
                string outputFile = Path.Combine(IsolatedPath, $"{fileName}.{extension}");

                File.Copy(Path.Combine(SolutionPath, $"{fileName}.java"), startFile);

                Converter.RunTool(startFile, outputFile);

                Assert.Inconclusive("Not Ready for Unit Testing");

                Cleanup();
            }
        }


        [TestMethod]
        public void SimpleClasses()
        {
            generalTest("SimpleClass");
            generalTest("StaticClass");
            generalTest("AbstractClass");
        }

        [TestMethod]
        public void SimpleEnum()
        {
            generalTest("SimpleEnumOutter");
            generalTest("SimpleEnumInner");
        }

        [TestMethod]
        public void NestedClasses()
        {
            generalTest("");
        }

        [TestMethod]
        public void NestedEnums()
        {
            generalTest("");
        }

        [TestMethod]
        public void CombinedEnumsAndClasses()
        {
            generalTest("");
        }

        [TestMethod]
        public void SimpleMethods()
        {
            generalTest("");
        }

        [TestMethod]
        public void SimpleProperties()
        {
            generalTest("");
        }

        [TestMethod]
        public void SimpleConstructors()
        {
            generalTest("");
        }

        [TestMethod]
        public void NormalClass()
        {
            //-Test Combination of Methods, Properties, Constructors
            generalTest("");
        }

        [TestMethod]
        public void SimpleOperators()
        {
            generalTest("");
        }

        [TestMethod]
        public void MultiFiles()
        {
            generalTest("");
        }
    }
}
