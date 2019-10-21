using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using NUnitTestingSuite;
using HtmlAgilityPack;
using System.Linq;

namespace NUnitTestingSuite
{
    [TestFixture]
    public class FunctionTests
    {
        [TestCase("https://www.bing.com/search?q=test")]
        [TestCase("https://uk.search.yahoo.com/search?q=test")]    
        public void Test_url(string url)
        {
            FunctionsToTest Test = new FunctionsToTest();
            Assert.IsTrue(Test.LoadWebPageAsync(url).Result);
        }
        [TestCase("https://oldschool.runescape.com/")]
        public void Test_Wrongurl(string url)
        {
            FunctionsToTest Test = new FunctionsToTest();
            Assert.IsFalse(Test.LoadWebPageAsync(url).Result);
        }

        [TestCase("test", "https://www.bing.com/search?q=", "//li[@class='b_algo']")]
        [TestCase("test", "https://uk.search.yahoo.com/search?q=", "//div[@class='dd algo algo-sr Sr']")]
        public void Test_NodeCollection_String(string searchterm, string urlStem, string HtmlToCatch)
        {
            FunctionsToTest Test = new FunctionsToTest();
            Assert.IsNotNull(Test.GetSearchEngineResultNodesAsync(searchterm, urlStem, HtmlToCatch).Result);
        }

        [TestCase("test", "https://www.bing.com/search?q=", "//fake")]
        [TestCase("test", "https://uk.search.yahoo.com/search?q=", "//fake")]
        public void Test_NodeCollection_Failure(string searchterm, string urlStem, string HtmlToCatch)
        {
            FunctionsToTest Test = new FunctionsToTest();
            Assert.IsNull(Test.GetSearchEngineResultNodesAsync(searchterm, urlStem, HtmlToCatch).Result);
        }

        [TestCase("test","Bing")]
        [TestCase("test", "Yahoo")]
        public void Test_NodeCollectionContainsUrls(string searchterm, string SearchEngine)
        {
            FunctionsToTest Test = new FunctionsToTest();
            if (SearchEngine=="Bing")
            {                
                HtmlNodeCollection NodeList = Test.GetBingResultsAsync(searchterm).Result;                
                Assert.IsTrue(Test.CheckForUrls(NodeList));
            }
            if (SearchEngine == "Yahoo")
            {
                HtmlNodeCollection NodeList = Test.GetYahooResultsAsync(searchterm).Result;
                Assert.IsTrue(Test.CheckForUrls(NodeList));
            }            
        }

    }
}
