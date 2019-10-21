using System;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;

namespace NUnitTestingSuite
{
    public class FunctionsToTest
    {
        public async System.Threading.Tasks.Task<HtmlNodeCollection> GetBingResultsAsync(string searchterm)
        {
            return await GetSearchEngineResultNodesAsync(searchterm, "https://www.bing.com/search?q=", "//li[@class='b_algo']");           
        }
        public async System.Threading.Tasks.Task<HtmlNodeCollection> GetYahooResultsAsync(string searchterm)
        {
            return await GetSearchEngineResultNodesAsync(searchterm, "https://uk.search.yahoo.com/search?q=", "//div[@class='dd algo algo-sr Sr']");
            //return null;
        }

        public async System.Threading.Tasks.Task<HtmlNodeCollection> GetSearchEngineResultNodesAsync(string searchterm, string url_stem, string NodeSelectionTerm)
        {
            var url = url_stem + searchterm;

            var httpClient = new HttpClient();

            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlAgilityPack.HtmlDocument();

            htmlDocument.LoadHtml(html);

            HtmlNodeCollection Nodes = htmlDocument.DocumentNode.SelectNodes(NodeSelectionTerm);

            return Nodes;
        }

        public async System.Threading.Tasks.Task<bool> LoadWebPageAsync(string url)
        {

            var httpClient = new HttpClient();

            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlAgilityPack.HtmlDocument();

            htmlDocument.LoadHtml(html);

            HtmlNodeCollection NodesFromYahoo = htmlDocument.DocumentNode.SelectNodes("//h1[@class='off-left']");
            HtmlNodeCollection NodesFromBing = htmlDocument.DocumentNode.SelectNodes("//span[@class='sb_count']");

            return (NodesFromYahoo!=null || NodesFromBing!=null);
        }

        public bool CheckForUrls (HtmlNodeCollection NodeList)
        {
            bool Urls = true;
            foreach (HtmlNode Node in NodeList)
            {
                var href = Node.Descendants("a").Select(node => node.GetAttributeValue("href", "")).ToList();
                if (href == null)
                {
                    Urls = false;
                }
            }
            return Urls;
        }
    }
}
