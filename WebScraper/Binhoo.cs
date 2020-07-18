using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Net.Mail;

namespace WebScraper
{
    public partial class Binhoo : Form
    {
        public Binhoo()
        {
            InitializeComponent();
            pictureBox1.Image = Properties.Resources.Binhoo;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            GetBingResults(textBox1.Text);
            GetYahooResults(textBox1.Text);
            richTextBox2.Clear();
            richTextBox2.Text += "Showing search results for: " + textBox1.Text;
            textBox1.Clear();
            }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void GetBingResults(string searchterm)
        {
            GetSearchEngineResultsAsync(searchterm, "https://www.bing.com/search?q=", "//li[@class='b_algo']", "Bing");
        }
        private void GetYahooResults(string searchterm)
        {
            List<string> ClassLabels = new List<string> { "//div[@class='dd algo algo-sr relsrch Sr']", "//div[@class='dd algo algo-sr relsrch fst Sr']", "//div[@class='dd algo algo-sr relsrch lst Sr']" };
            foreach (string S in ClassLabels)
            {
                GetSearchEngineResultsAsync(searchterm, "https://uk.search.yahoo.com/search?q=", S, "Yahoo");
            }
            //GetSearchEngineResultsAsync(searchterm, "https://uk.search.yahoo.com/search?q=", "//div[@class='dd algo algo-sr Sr']", "Yahoo");
            //"dd algo algo-sr relsrch Sr"
            //"dd algo algo-sr relsrch fst Sr"
            //"dd algo algo-sr relsrch lst Sr"
        }

        private async void GetSearchEngineResultsAsync(string searchterm, string url_stem, string NodeSelectionTerm, string searchEngine)
        {
            var url = url_stem + searchterm;                        

            var httpClient = new HttpClient();

            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlAgilityPack.HtmlDocument();

            htmlDocument.LoadHtml(html);

            HtmlNodeCollection Nodes = htmlDocument.DocumentNode.SelectNodes(NodeSelectionTerm);

            if (searchEngine=="Bing")
            {
                if (Nodes != null)
                {
                    try
                    {
                        PopulateBingSearchResults(Nodes);
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                        ReportToAdmin("Bing", textBox1.Text, "StringLengthError");
                        richTextBox1.Text += "\r\n";
                        addsearchresult("Something went wrong when getting search results from Bing! The authors are investigating");
                        richTextBox1.Text += "\r\n";
                    }
                }
                else
                {
                    HtmlNodeCollection CheckNodes = htmlDocument.DocumentNode.SelectNodes("//li[@class='b_no']");
                    if (CheckNodes!=null)
                    {
                        addsearchresult("Bing found no results for this search term. This is most unfortunate.");
                    }
                    else
                    {
                        ReportToAdmin(searchEngine, searchterm, "NodeError");
                        addsearchresult(string.Format("Something went wrong when getting search results from {0}! The authors are investigating",searchEngine));
                    }
                }
            }
            if (searchEngine=="Yahoo")
            {
                if (Nodes != null)
                {
                    PopulateYahooSearchResults(Nodes);
                }
                else
                {
                    HtmlNodeCollection CheckNodes = htmlDocument.DocumentNode.SelectNodes("//li[@class='first last']");
                    if(CheckNodes!=null)
                    {
                        addsearchresult("Yahoo found no results for this search term. How upsetting.");
                    }
                    else
                    {
                        ReportToAdmin(searchEngine, searchterm, "NodeError");
                        addsearchresult(string.Format("Something went wrong when getting search results from {0}! The authors are investigating", searchEngine));                     
                    }
                }
            }
        }

        private void PopulateBingSearchResults(HtmlNodeCollection Nodes)
        {

            foreach (HtmlAgilityPack.HtmlNode Node in Nodes) //System.NullReferenceException
            {
                
                addsearchresult("Result from Bing");

                var href = Node.Descendants("a").Select(node => node.GetAttributeValue("href", "")).ToList();
                if (href.Count > 0)
                {
                    addsearchresult(href[0]);
                }
                addsearchresult(Node.ChildNodes.ToList()[0].InnerText);
                if (Node.ChildNodes.ToList().Count > 1)
                {
                    if (Node.ChildNodes.ToList()[1].InnerText.Substring(0, 5) != "https")
                    {
                        addsearchresult(System.Net.WebUtility.HtmlDecode(Node.ChildNodes.ToList()[1].InnerText));
                    }
                    else
                    {
                        addsearchresult(System.Net.WebUtility.HtmlDecode(Node.ChildNodes.ToList()[1].InnerText.Substring(href[0].Length, Node.ChildNodes.ToList()[1].InnerText.Length - href[0].Length)));
                    }
                }
                
                richTextBox1.Text += "\r\n";
            }
        }    

        private void PopulateYahooSearchResults(HtmlNodeCollection Nodes)
        {

            foreach (HtmlAgilityPack.HtmlNode Node in Nodes)
            {
                addsearchresult("Result from Yahoo");

                var href = Node.Descendants("a").Select(node => node.GetAttributeValue("href", "")).ToList();
                if (href.Count > 0)
                {
                    addsearchresult(href[0]);
                }

                var SearchOutput = Node.ChildNodes.ToList();
                var LinkAndTitle = SearchOutput[0];

                if (LinkAndTitle.InnerText.Contains("www.") == false)
                {
                    addsearchresult(System.Net.WebUtility.HtmlDecode(LinkAndTitle.InnerText));
                }
                else
                {
                    if (href[0].Length < LinkAndTitle.InnerText.Length)
                    {
                        addsearchresult(System.Net.WebUtility.HtmlDecode(LinkAndTitle.InnerText.Substring(0, LinkAndTitle.InnerText.IndexOf("www."))));
                    }
                }

                HtmlNode Description;
                if (SearchOutput.Count > 1)
                {
                    if (SearchOutput[1].InnerText == "Cached")
                    {
                        Description = SearchOutput[2];
                    }
                    else
                    {
                        Description = SearchOutput[1];
                    }

                    addsearchresult(System.Net.WebUtility.HtmlDecode(Description.InnerText));
                }
                richTextBox1.Text += "\r\n";

            }
        }

        private void addsearchresult(string searchresult)
        {
            richTextBox1.Text += "\r\n";
            richTextBox1.Text += searchresult;           
        }

        private void ReportToAdmin(string SearchEngine, string searchterm, string errorType)
        {
            string emailFrom = "emaileraddresshere";
            string emailTo = "adminaddresshere";
            string ErrorReport = (errorType == "NodeError" ? 
                string.Format("The following search term produced a {0} webpage avoiding current node selection: {1}", SearchEngine, searchterm)
                : string.Format("The following search term produced {0} search results which subvert the current processing: {1}", SearchEngine, searchterm));
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.UseDefaultCredentials = true;
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("emaileradresshere", "emailerpasswordhere");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(emailFrom, emailTo, string.Format("{0} search error", SearchEngine), ErrorReport);
        }
    }
}
