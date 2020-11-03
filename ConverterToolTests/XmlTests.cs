using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using ConverterTool;

namespace ConverterToolTests
{
    [TestClass]
    public class XmlTests : TestGeneral
    {
        public XmlTests() : base("XmlFiles")
        {
        }

        [TestMethod]
        public void SimpleObject()
        {
            Cleanup();
            string startFile = Path.Combine(IsolatedPath, "SimpleObject.xml");
            string targetFile = Path.Combine(IsolatedPath, "SimpleObject.json");
            File.Copy(Path.Combine(SolutionPath, "SimpleObject.xml"), startFile);
            Converter.RunTool(startFile, targetFile);
        }

        [TestMethod]
        public void ArrayObject()
        {
            Cleanup();
            File.Copy(Path.Combine(SolutionPath, "ArrayObject.xml"), Path.Combine(IsolatedPath, "ArrayObject.xml"));
        }

        [TestMethod]
        public void NestedObject()
        {
            Cleanup();
            Console.WriteLine("Nested Object Test");
        }

        [TestMethod]
        public void ReversalObjects()
        {
            Cleanup();
            Console.WriteLine("Revert Object Test");
        }

        [TestMethod]
        public void MultiFiles()
        {
            Cleanup();
            Console.WriteLine("Multi File Test");
        }
    }
}
