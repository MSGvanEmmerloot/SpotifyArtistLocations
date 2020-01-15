using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SpotifyArtistLocations.Data.MusicServices
{
    public class MusicBrainzService : MusicService
    {
        public override string baseUrl { get { return "https://musicbrainz.org/search?type=release&limit=25&method=indexed&query="; } }

        public override string FormatURL(string artist)
        {
            return baseUrl + Uri.EscapeDataString(artist.Replace(" ", "+"));
        }

        public override string FormatURL(string artist, string album)
        {
            return baseUrl + "\"" + Uri.EscapeDataString(album.Replace(" ", "+")) + "\"" + "+AND+" + Uri.EscapeDataString("artist:"+artist.Replace(" ", "+"));
        }
        
        public string GetArtistCountryFromAlbums(string artist, List<string> albums)
        {
            for (int i = 0; i < albums.Count; i++) {
                string url = FormatURL(artist, albums[i]);
                Console.WriteLine("Accessing " + url);

                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);
                if (doc == null) { Console.WriteLine("Couldn't find page for " + artist); return null; }

                Music.ArtistLocation curArtist = GetArtistDataFromRelease(artist, doc);

                if (curArtist != null)
                {
                    Console.WriteLine("Found country " + curArtist.country);
                    return curArtist.country;
                }
            }

            return null;
        }

        public string GetArtistCountryFromBarcode(string artist, string barCode)
        {
            string url = "https://musicbrainz.org/search?type=release&limit=25&method=advanced&query=barcode%3A" + barCode;
            Console.WriteLine("Accessing " + url);

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            if (doc == null) { Console.WriteLine("Couldn't find page for " + artist); return null; }

            AddProductCode(artist, barCode);
            Music.ArtistLocation curArtist = GetArtistData(artist, doc);

            if (curArtist != null)
            {
                Console.WriteLine("Found country " + curArtist.country);
                return curArtist.country;
            }

            return null;
        }

        public override Music.ArtistLocation GetArtistData(string artist, HtmlDocument doc)
        {
            //Music.ArtistLocation curArtist = new Music.ArtistLocation(artist);

            //string artistLink = FindArtistLink(artist, doc);
            //if (artistLink == "") { return null; }
            //Console.WriteLine("Found link: " + artistLink);

            //return ParseArtistPage(curArtist, artistLink);
            return GetArtistDataFromRelease(artist, doc);
        }

        public override Music.ArtistLocation GetArtistDataFromRelease(string artist, HtmlDocument doc)
        {
            Music.ArtistLocation curArtist = new Music.ArtistLocation(artist);

            string[] artistInfo = FindArtistInfo(artist, doc);
            if(artistInfo == null) { Console.WriteLine("Rip"); return null; }
            Console.WriteLine(artistInfo);
            Console.WriteLine(artistInfo.Length);
            string artistLink = artistInfo[0];
            curArtist.country = artistInfo[1];
            if (artistLink == "") { return null; }
            Console.WriteLine("Found link: " + artistLink);

            return ParseArtistPage(curArtist, artistLink);
        }        

        //public override Music.ArtistLocation GetArtistData(string artist, HtmlDocument doc)
        //{
        //    Music.ArtistLocation curArtist = new Music.ArtistLocation(artist);
        //    string artistLink = "";
        //    string country = "";
        //    string location = "";
        //    Music.ArtistLocation tempArtist;

        //    #region Parsing search page
        //    HtmlNode searchResultTable = doc.DocumentNode.SelectSingleNode("//table[@class='tbl']");
        //    if (searchResultTable == null) { Console.WriteLine("Couldn't find search results for " + artist); return null; }

        //    List<HtmlNode> tableRows = searchResultTable.SelectSingleNode("//tbody").SelectNodes(".//tr").ToList();

        //    foreach (HtmlNode row in tableRows)
        //    {
        //        List<HtmlNode> tableColumns = row.SelectNodes(".//td").ToList();
        //        //Console.WriteLine("Sort name: " + tableColumns[1].InnerText);
        //        if (tableColumns[1].InnerText == artist)
        //        {
        //            artistLink = tableColumns[0].SelectSingleNode(".//a").Attributes["href"].Value;

        //            if (tableColumns[4].InnerText != "")
        //            {
        //                if (tableColumns[4].SelectSingleNode(".//span") != null)
        //                {
        //                    country = tableColumns[4].InnerText.Trim();
        //                    curArtist.country = country;
        //                    //Console.WriteLine("Country: " + country);
        //                }
        //                //else Console.WriteLine("Area: " + tableColumns[4].InnerText);
        //            }

        //            break;
        //        }
        //    }

        //    //Console.WriteLine("Found link: " + artistLink);
        //    //Console.WriteLine("Found country: " + country);
        //    #endregion

        //    string artistURL = "https://musicbrainz.org" + artistLink;

        //    #region Parsing artist page
        //    HtmlWeb web = new HtmlWeb();
        //    HtmlDocument doc2 = web.Load(artistURL);
        //    if (doc2 == null) { Console.WriteLine("Couldn't find page for " + artist); return null; }

        //    Console.WriteLine("Succesfully navigated to " + artistURL);

        //    HtmlNode propertiesContainer = doc2.DocumentNode.SelectSingleNode("//dl[@class='properties']");
        //    if (propertiesContainer == null) { Console.WriteLine("Couldn't find information for " + artist); return null; }

        //    HtmlNode areaInfo = propertiesContainer.SelectSingleNode(".//dd[@class='area']");
        //    HtmlNode beginAreaInfo = propertiesContainer.SelectSingleNode(".//dd[@class='begin_area']");

        //    if (areaInfo != null)
        //    {
        //        tempArtist = ParseNode(areaInfo, country);
        //        if (country == "" && tempArtist.country != "") { country = tempArtist.country; curArtist.country = tempArtist.country; }
        //        if (tempArtist.location.Length > location.Length) { location = tempArtist.location; curArtist.location = tempArtist.location; }
        //    }

        //    if (beginAreaInfo != null)
        //    {
        //        tempArtist = ParseNode(beginAreaInfo, country);
        //        if (country == "" && tempArtist.country != "") { curArtist.country = tempArtist.country; }
        //        if (tempArtist.location.Length > location.Length) { curArtist.location = tempArtist.location; }
        //    }
        //    #endregion

        //    if (curArtist.country != "")
        //    {
        //        Console.WriteLine("CurArtist at end: " + curArtist.GetLocation());
        //        return curArtist;
        //    }

        //    return null;
        //}

        public string[] FindArtistInfo(string artist, HtmlDocument doc)
        {
            string artistLink = "";
            string country = "";

            HtmlNode searchResultTable = doc.DocumentNode.SelectSingleNode("//table[@class='tbl']");
            if (searchResultTable == null) { Console.WriteLine("Couldn't find search results for " + artist); return null; }

            List<HtmlNode> tableRows = searchResultTable.SelectSingleNode("//tbody").SelectNodes(".//tr").ToList();
            List<HtmlNode> tableColumns = new List<HtmlNode>();

            foreach (HtmlNode row in tableRows)
            {
                tableColumns = row.SelectNodes(".//td").ToList();
                HtmlNode artistNode = tableColumns[1].SelectSingleNode(".//a");
                if(artistNode == null) { continue; }

                if (tableColumns[1].InnerText == artist)
                {
                    artistLink = artistNode.Attributes["href"].Value;
                    break;
                }
                else if (CheckEqual(artist, tableColumns[1].InnerText))
                {
                    artistLink = artistNode.Attributes["href"].Value;
                    break;
                }
                else if (CheckBarcodeMatch(artist, tableColumns[7].InnerText))
                {
                    artistLink = artistNode.Attributes["href"].Value;
                    break;
                }
                else Console.WriteLine("\"" + tableColumns[1].InnerText + "\" does not equal \"" + artist + "\"");
            }

            if(artistLink != "")
            {
                if (tableColumns.Count > 4 && tableColumns[4].InnerText != "")
                {
                    if (tableColumns[4].SelectSingleNode(".//span") != null)
                    {
                        HtmlNode linkNode = tableColumns[4].SelectSingleNode(".//a");
                        if(linkNode != null) {
                            HtmlNode abbrNode = linkNode.SelectSingleNode(".//abbr");
                            if(abbrNode != null)
                            {
                                HtmlAttribute titleAttr = abbrNode.Attributes["title"];
                                if(titleAttr != null)
                                {
                                    country = titleAttr.Value;
                                }
                            }
                        }
                        //country = tableColumns[4].SelectSingleNode(".//a").SelectSingleNode(".//abbr").Attributes["title"].Value;
                    }
                }
            }

            return new string[] { artistLink, country };
        }

        private bool CheckEqual(string string1, string string2)
        {
            if (string1 == null || string2 == null)
            {
                Console.WriteLine("null");
                return false;
            }

            if (string1.ToLower().Equals(string2.ToLower()))
            {
                Console.WriteLine("equals");
                return true;
            }
            if (string1.ToLower().Equals(string2.ToLower(), StringComparison.InvariantCulture))
            {
                Console.WriteLine("equals invariant");
                return true;
            }

            return false;
        }

        private bool CheckBarcodeMatch(string artist, string barcode)
        {
            if (artistUPCs == null || !artistUPCs.ContainsKey(artist)) { return false; }

            if (artistUPCs[artist].Contains(barcode)) {
                Console.WriteLine("Found barcode match for " + artist);
                return true; 
            }

            return false;
        }

        public Music.ArtistLocation ParseArtistPage(Music.ArtistLocation curArtist, string artistLink)
        {
            string country = "";
            string location = "";
            Music.ArtistLocation tempArtist;

            string artistUrl = "https://musicbrainz.org" + artistLink;

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(artistUrl);
            if (doc == null) { Console.WriteLine("Couldn't find page for " + curArtist.name); return null; }
            Console.WriteLine("Succesfully navigated to " + artistUrl);

            HtmlNode propertiesContainer = doc.DocumentNode.SelectSingleNode("//dl[@class='properties']");
            if (propertiesContainer == null) { Console.WriteLine("Couldn't find information for " + curArtist.name); return null; }

            HtmlNode areaInfo = propertiesContainer.SelectSingleNode(".//dd[@class='area']");
            HtmlNode beginAreaInfo = propertiesContainer.SelectSingleNode(".//dd[@class='begin_area']");

            // Parse begin area first to check where the band was founded
            if (beginAreaInfo != null)
            {
                tempArtist = ParseNode(beginAreaInfo, country);
                if (country == "" && tempArtist.country != "") { country = tempArtist.country; curArtist.country = tempArtist.country; }
                if (tempArtist.location.Length > location.Length) { location = tempArtist.location; curArtist.location = tempArtist.location; }
            }

            if (areaInfo != null)
            {
                tempArtist = ParseNode(areaInfo, country);
                if (country == "" && tempArtist.country != "") { curArtist.country = tempArtist.country; }
                //Only update location if it matches the origin country
                if (tempArtist.location.Length > location.Length && tempArtist.country == curArtist.country) { curArtist.location = tempArtist.location; }
            }

            if (curArtist.country != "" && curArtist.country != null)
            {
                Console.WriteLine("CurArtist at end: " + curArtist.GetLocation());
                return curArtist;
            }

            return null;
        }

        public Music.ArtistLocation ParseNode(HtmlNode node, string knownCountry)
        {
            Music.ArtistLocation tempArtist = new Music.ArtistLocation("dummy")
            {
                country = knownCountry,
                location = ""
            };

            HtmlNode flag = node.SelectSingleNode(".//span");
            if (flag != null)
            {
                Console.WriteLine("Country from flag: " + flag.InnerText);
                tempArtist.country = flag.InnerText.Trim();
            }

            string countryBuffer = "";
            string innerText = node.InnerText;
            string[] originParts = innerText.Split(",");
            if (originParts.Length > 1)
            {
                for (int i = 1; 1 < originParts.Length - 1; i++)
                {
                    countryBuffer = originParts[^i];                    

                    if ((tempArtist.country != "" && countryBuffer.Trim() == tempArtist.country) || tempArtist.country == "")
                    {
                        tempArtist.location = innerText.Substring(0, innerText.Length - countryBuffer.Length - 1);
                        break;
                    }
                }
            }

            return tempArtist;
        }
    }
}
