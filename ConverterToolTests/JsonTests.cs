using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ConverterTool;
using Newtonsoft.Json.Linq;

namespace ConverterToolTests
{
    [TestClass]
    public class JsonTests : TestGeneral
    {
        public JsonTests() : base("JsonFiles")
        {
        }

        [TestMethod]
        public void SimpleObject()
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, "SimpleObject.json");
            string outputFile = Path.Combine(IsolatedPath, "SimpleObject.xml");

            File.Copy(Path.Combine(SolutionPath, "SimpleObject.json"), startFile);

            Converter.RunTool(startFile, outputFile);

            this.SetBaselinePath("XmlFiles");
            this.BaselinePath = Path.Combine(this.BaselinePath, "SimpleObject.xml");

            var startObject = File.ReadAllText(outputFile);
            var outputObject = File.ReadAllText(this.BaselinePath);

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
        public void ArrayObject()
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, "ArrayObject.json");
            string outputFile = Path.Combine(IsolatedPath, "ArrayObject.xml");

            File.Copy(Path.Combine(SolutionPath, "ArrayObject.json"), startFile);

            Converter.RunTool(startFile, outputFile);

            this.SetBaselinePath("XmlFiles");
            this.BaselinePath = Path.Combine(this.BaselinePath, "SimpleObject.xml");

            var startObject = File.ReadAllBytes(outputFile);
            var outputObject = File.ReadAllBytes(this.BaselinePath);

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
        public void NestedObject()
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, "NestedObject.json");
            string outputFile = Path.Combine(IsolatedPath, "NestedObject.xml");

            File.Copy(Path.Combine(SolutionPath, "NestedObject.json"), startFile);

            Converter.RunTool(startFile, outputFile);

            this.SetBaselinePath("XmlFiles");
            this.BaselinePath = Path.Combine(this.BaselinePath, "NestedObject.xml");

            var startObject = File.ReadAllBytes(startFile);
            var outputObject = File.ReadAllBytes(this.BaselinePath);

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
        public void ReversalObjects()
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, "SimpleObject.json");
            string rawFile = Path.Combine(IsolatedPath, "RawOutput.xml");
            string outputFile = Path.Combine(IsolatedPath, "Results.json");

            File.Copy(Path.Combine(SolutionPath, "SimpleObject.json"), startFile);

            Converter.RunTool(startFile, rawFile);
            Converter.RunTool(rawFile, outputFile);

            JObject startObject = JObject.Parse(File.ReadAllText(startFile));
            JObject outputObject = JObject.Parse(File.ReadAllText(outputFile));

            Assert.IsTrue(JToken.DeepEquals(startObject, outputObject));

            Cleanup();
        }

        [TestMethod]
        public void MultiFiles()
        {
            Assert.Inconclusive("Not Ready for Unit Testing");
        }
    }
}
