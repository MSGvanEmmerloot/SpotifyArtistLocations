using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SpotifyArtistLocations.Data
{
    public class ISRCSearchService
    {
        public List<string> GetArtistUPCs(string artistISRC)
        {
            string url = string.Format("https://isrcsearch.ifpi.org/#!/search?isrcCode={0}&tab=lookup&showReleases&start=0&number=10", artistISRC);
            Console.WriteLine("Accessing " + url);

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            if (doc == null) { Console.WriteLine("Couldn't find page for ISRC " + artistISRC); return null; }

            return GetUPCs(doc);
        }

        private List<string> GetUPCs(HtmlDocument doc)
        {
            Console.WriteLine(doc.Text);
            HtmlNode resultDiv = doc.DocumentNode.SelectSingleNode("//div[@id='results']");
            if (resultDiv == null) { Console.WriteLine("Couldn't find resultDiv"); return null; }

            //HtmlNode code = table.SelectSingleNode(".//tbody']");

            List<HtmlNode> tr = resultDiv.SelectSingleNode(".//table']").SelectSingleNode(".//tbody']").SelectNodes(".//tr").ToList();

            List<string> UPCs = new List<string>();

            for (int i = 0; i < tr.Count; i++)
            {
                HtmlNode codeNode = tr[i].SelectSingleNode(".//td[@dedupe='upcCode']");
                if(codeNode == null) { continue; }
                Console.WriteLine("Adding code " + codeNode.InnerText);
                UPCs.Add(codeNode.InnerText);
            }

            return UPCs;
        }
    }
}
