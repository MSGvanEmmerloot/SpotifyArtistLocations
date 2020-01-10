using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SpotifyArtistLocations.Data.MusicServices
{
    public class MusicBrainzService : MusicService
    {
        public override string baseUrl { get { return "https://musicbrainz.org/search?type=artist&limit=25&method=direct&query="; } }
        public string baseUrlIndexedSearch { get { return "https://musicbrainz.org/search?type=release&limit=25&method=indexed&query="; } }

        public override string FormatURL(string artist)
        {
            return baseUrl + Uri.EscapeDataString(artist.Replace(" ", "+"));
        }

        public string FormatURL(string artist, string album)
        {
            return baseUrlIndexedSearch + "\"" + Uri.EscapeDataString(album.Replace(" ", "+")) + "\"" + "+AND+" +Uri.EscapeDataString("artist:"+artist.Replace(" ", "+"));
        }

        public void GetArtistCountries(List<Data.Music.ArtistInfo> artists)
        {
            if (artistData == null)
            {
                artistData = new Dictionary<string, Music.ArtistLocation>();
            }

            if (artists == null) { Console.WriteLine("Artists is null"); return; }
            if (artists.Count == 0) { Console.WriteLine("0 Artists"); return; }
            Console.WriteLine(artists.Count + " artists to be found");

            Task[] tasks = new Task[artists.Count];
            Console.Write(".");
            for (int item = 0; item < artists.Count; item++)
            {
                string artist = artists[item].artistName;
                string album = artists[item].albumName;
                Console.WriteLine("Task " + item + " will handle " + artist + " album " + album);
                tasks[item] = Task.Factory.StartNew(() => GetArtistCountry(artist,album));
            }
            Console.Write(".");
            Task.WaitAll(tasks);

            Console.WriteLine("Found the origins of " + artistData.Count + " artists!\n");
        }

        public string GetArtistCountry(string artist, string album)
        {
            if (artistData == null)
            {
                artistData = new Dictionary<string, Music.ArtistLocation>();
            }

            if (artistData.ContainsKey(artist))
            {
                Console.WriteLine("Origins of " + artist + " already known: " + artistData[artist].country);
                return null;
            }

            string url = FormatURL(artist, album);
            Console.WriteLine("Accessing " + url);

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            if (doc == null) { Console.WriteLine("Couldn't find page for " + artist); return null; }

            Music.ArtistLocation curArtist = GetArtistDataFromRelease(artist, doc);

            if (curArtist != null)
            {
                artistData.Add(artist, curArtist);
                return curArtist.country;
            }

            return null;
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

        public Music.ArtistLocation GetArtistDataFromRelease(string artist, HtmlDocument doc)
        {
            Music.ArtistLocation curArtist = new Music.ArtistLocation(artist);
            string artistLink = "";
            string country = "";
            string location = "";
            Music.ArtistLocation tempArtist;

            #region Parsing search page
            HtmlNode searchResultTable = doc.DocumentNode.SelectSingleNode("//table[@class='tbl']");
            if (searchResultTable == null) { Console.WriteLine("Couldn't find search results for " + artist); return null; }

            List<HtmlNode> tableRows = searchResultTable.SelectSingleNode("//tbody").SelectNodes(".//tr").ToList();

            foreach (HtmlNode row in tableRows)
            {
                List<HtmlNode> tableColumns = row.SelectNodes(".//td").ToList();
                //Console.WriteLine("Sort name: " + tableColumns[1].InnerText);
                if (tableColumns[1].InnerText == artist)
                {
                    artistLink = tableColumns[1].SelectSingleNode(".//a").Attributes["href"].Value;
                    break;
                }
                else if(CheckEqual(artist, tableColumns[1].InnerText)){
                    artistLink = tableColumns[1].SelectSingleNode(".//a").Attributes["href"].Value;
                    break;
                }
                else Console.WriteLine("\"" + tableColumns[1].InnerText + "\" does not equal \"" + artist + "\"");
            }

            if(artistLink == "") { return null; }

            Console.WriteLine("Found link: " + artistLink);
            Console.WriteLine("Found country: " + country);
            #endregion

            string artistURL = "https://musicbrainz.org" + artistLink;

            #region Parsing artist page
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc2 = web.Load(artistURL);
            if (doc2 == null) { Console.WriteLine("Couldn't find page for " + artist); return null; }

            Console.WriteLine("Succesfully navigated to " + artistURL);

            HtmlNode propertiesContainer = doc2.DocumentNode.SelectSingleNode("//dl[@class='properties']");
            if (propertiesContainer == null) { Console.WriteLine("Couldn't find information for " + artist); return null; }

            HtmlNode areaInfo = propertiesContainer.SelectSingleNode(".//dd[@class='area']");
            HtmlNode beginAreaInfo = propertiesContainer.SelectSingleNode(".//dd[@class='begin_area']");

            // Parse begin area first to check where the band was founded
            if (beginAreaInfo != null)
            {
                //Console.WriteLine("Parse areaInfo");
                tempArtist = ParseNode(beginAreaInfo, country);
                if (country == "" && tempArtist.country != "") { country = tempArtist.country; curArtist.country = tempArtist.country; }
                if (tempArtist.location.Length > location.Length) { location = tempArtist.location; curArtist.location = tempArtist.location; }
            }

            if (areaInfo != null)
            {
                //Console.WriteLine("Parse beginAreaInfo");
                tempArtist = ParseNode(areaInfo, country);
                if (country == "" && tempArtist.country != "") { curArtist.country = tempArtist.country; }
                //Only update location if it matches the origin country
                if (tempArtist.location.Length > location.Length && tempArtist.country == curArtist.country) { curArtist.location = tempArtist.location; }
            }
            #endregion

            if (curArtist.country != "" && curArtist.country != null)
            {
                //Console.WriteLine("Country at end: " + curArtist.country);

                if (curArtist.location != "")
                {
                    //Console.WriteLine("Location at end: " + curArtist.location);
                }
                Console.WriteLine("CurArtist at end: " + curArtist.GetLocation());
                return curArtist;
            }

            return null;
        }

        private bool CheckEqual(string string1, string string2)
        {
            if(string1 == null || string2 == null) {
                Console.WriteLine("null");
                return false; 
            }

            if (string1.ToLower().Equals(string2.ToLower())) {
                Console.WriteLine("equals");
                return true; 
            }
            if (string1.ToLower().Equals(string2.ToLower(), StringComparison.InvariantCulture)) {
                Console.WriteLine("equals invariant");
                return true;
            }

            return false;
        }

        public override Music.ArtistLocation GetArtistData(string artist, HtmlDocument doc)
        {
            Music.ArtistLocation curArtist = new Music.ArtistLocation(artist);
            string artistLink = "";
            string country = "";
            string location = "";
            Music.ArtistLocation tempArtist;

            #region Parsing search page
            HtmlNode searchResultTable = doc.DocumentNode.SelectSingleNode("//table[@class='tbl']");
            if (searchResultTable == null) { Console.WriteLine("Couldn't find search results for " + artist); return null; }

            List<HtmlNode> tableRows = searchResultTable.SelectSingleNode("//tbody").SelectNodes(".//tr").ToList();
            
            foreach(HtmlNode row in tableRows)
            {
                List<HtmlNode> tableColumns = row.SelectNodes(".//td").ToList();
                //Console.WriteLine("Sort name: " + tableColumns[1].InnerText);
                if(tableColumns[1].InnerText == artist)
                {
                    artistLink = tableColumns[0].SelectSingleNode(".//a").Attributes["href"].Value;

                    if (tableColumns[4].InnerText != "")
                    {
                        if(tableColumns[4].SelectSingleNode(".//span") != null)
                        {
                            country = tableColumns[4].InnerText.Trim();
                            curArtist.country = country;
                            //Console.WriteLine("Country: " + country);
                        }
                        //else Console.WriteLine("Area: " + tableColumns[4].InnerText);
                    }

                    break;
                }
            }

            //Console.WriteLine("Found link: " + artistLink);
            //Console.WriteLine("Found country: " + country);
            #endregion

            string artistURL = "https://musicbrainz.org" + artistLink;            

            #region Parsing artist page
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc2 = web.Load(artistURL);
            if (doc2 == null) { Console.WriteLine("Couldn't find page for " + artist); return null; }

            Console.WriteLine("Succesfully navigated to " + artistURL);
            
            HtmlNode propertiesContainer = doc2.DocumentNode.SelectSingleNode("//dl[@class='properties']");
            if (propertiesContainer == null) { Console.WriteLine("Couldn't find information for " + artist); return null; }

            HtmlNode areaInfo = propertiesContainer.SelectSingleNode(".//dd[@class='area']");
            HtmlNode beginAreaInfo = propertiesContainer.SelectSingleNode(".//dd[@class='begin_area']");            

            if (areaInfo != null)
            {
                //Console.WriteLine("Parse areaInfo");
                tempArtist = ParseNode(areaInfo, country);
                if (country == "" && tempArtist.country != "") { country = tempArtist.country;  curArtist.country = tempArtist.country; }
                if (tempArtist.location.Length > location.Length) { location = tempArtist.location; curArtist.location = tempArtist.location; }             
            }

            if (beginAreaInfo != null)
            {
                //Console.WriteLine("Parse beginAreaInfo");
                tempArtist = ParseNode(beginAreaInfo, country);
                if (country == "" && tempArtist.country != "") { curArtist.country = tempArtist.country; }
                if (tempArtist.location.Length > location.Length) { curArtist.location = tempArtist.location; }
            }
            #endregion

            if (curArtist.country != "")
            {
                //Console.WriteLine("Country at end: " + curArtist.country);

                if (curArtist.location != "")
                {
                    //Console.WriteLine("Location at end: " + curArtist.location);
                }
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

            string loc = "";
            string con = "";
            string innerText = node.InnerText;
            string[] originParts = innerText.Split(",");
            if (originParts.Length > 1)
            {
                for (int i = 1; 1 < originParts.Length - 1; i++)
                {
                    con = originParts[^i];
                    loc = innerText.Substring(0, innerText.Length - con.Length - 1);
                    innerText = loc;

                    if ((tempArtist.country != "" && con.Trim() == tempArtist.country) || tempArtist.country == "")
                    {
                        if (loc.Length > tempArtist.location.Length)
                        {
                            tempArtist.location = loc;
                        }
                        break;
                    }
                }
            }
            else con = innerText;
            //Console.WriteLine("Con: " + con + ", loc: " + loc);
            //Console.WriteLine("Inner text: " + innerText);

            //Console.WriteLine("Returned artist: " + tempArtist.GetLocation());

            return tempArtist;
        }
    }
}
