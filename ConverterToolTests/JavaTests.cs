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

                //Assert.Inconclusive("Not Ready for Unit Testing");

                Cleanup();
            }
        }


        [TestMethod]
        public void SimpleClasses()
        {
            generalTest("SimpleClass");
            generalTest("StaticClass");
            generalTest("AbstractClass");
            generalTest("Interface");
        }

        [TestMethod]
        public void SimpleEnum()
        {
            generalTest("SimpleEnum");
        }

        [TestMethod]
        public void NestedClasses()
        {
            generalTest("NestedClasses");
        }

        [TestMethod]
        public void GeneralMethods()
        {
            generalTest("SimpleMethods");
            generalTest("StaticMethods");
            generalTest("AbstractMethods");
            generalTest("InterfaceMethods");
        }

        [TestMethod]
        public void Inheritance()
        {
            generalTest("StandardInheritance");
            generalTest("AbstractInheritance");
            generalTest("InterfaceInheritance");
        }

        [TestMethod]
        public void MultiFiles()
        {
            Assert.Inconclusive("Not Ready for Unit Testing");
        }
    }
}
