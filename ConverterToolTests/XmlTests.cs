using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using ConverterTool;
using Newtonsoft.Json.Linq;

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
            string outputFile = Path.Combine(IsolatedPath, "SimpleObject.json");

            File.Copy(Path.Combine(SolutionPath, "SimpleObject.xml"), startFile);

            Converter.RunTool(startFile, outputFile);

            this.SetBaselinePath("JsonFiles");
            this.BaselinePath = Path.Combine(this.BaselinePath, "SimpleObject.json");

            JObject outputObject = JObject.Parse(File.ReadAllText(outputFile));
            JObject baselineObject = JObject.Parse(File.ReadAllText(this.BaselinePath));

            Assert.IsTrue(JToken.DeepEquals(outputObject, baselineObject));

            Cleanup();
        }

        [TestMethod]
        public void ArrayObject()
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, "ArrayObject.xml");
            string outputFile = Path.Combine(IsolatedPath, "ArrayObject.json");

            File.Copy(Path.Combine(SolutionPath, "ArrayObject.xml"), startFile);

            Converter.RunTool(startFile, outputFile);

            this.SetBaselinePath("JsonFiles");
            this.BaselinePath = Path.Combine(this.BaselinePath, "ArrayObject.json");

            JObject outputObject = JObject.Parse(File.ReadAllText(outputFile));
            JObject baselineObject = JObject.Parse(File.ReadAllText(this.BaselinePath));

            Assert.IsTrue(JToken.DeepEquals(outputObject, baselineObject));

            Cleanup();
        }

        [TestMethod]
        public void NestedObject()
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, "NestedObject.xml");
            string outputFile = Path.Combine(IsolatedPath, "NestedObject.json");

            File.Copy(Path.Combine(SolutionPath, "NestedObject.xml"), startFile);

            Converter.RunTool(startFile, outputFile);

            this.SetBaselinePath("JsonFiles");
            this.BaselinePath = Path.Combine(this.BaselinePath, "NestedObject.json");

            JObject outputObject = JObject.Parse(File.ReadAllText(outputFile));
            JObject baselineObject = JObject.Parse(File.ReadAllText(this.BaselinePath));

            Assert.IsTrue(JToken.DeepEquals(outputObject, baselineObject));

            Cleanup();
        }

        [TestMethod]
        public void ReversalObjects()
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, "SimpleObject.xml");
            string rawFile = Path.Combine(IsolatedPath, "RawOutput.json");
            string outputFile = Path.Combine(IsolatedPath, "Results.xml");

            File.Copy(Path.Combine(SolutionPath, "SimpleObject.xml"), startFile);

            Converter.RunTool(startFile, rawFile);
            Converter.RunTool(rawFile, outputFile);

            var startObject = File.ReadAllBytes(startFile);
            var outputObject = File.ReadAllBytes(outputFile);

            bool isEqual = startObject.Length == outputObject.Length;
            for (int index = 0; index < outputObject.Length; index++)
            {
                if (startObject[index] != outputObject[index])
                {
                    isEqual = false;
                    break;
                }
            }

            Assert.IsTrue(isEqual);

            Cleanup();
        }

        [TestMethod]
        public void MultiFiles()
        {
            Assert.Inconclusive("Not Ready for Unit Testing");
        }
    }
}
