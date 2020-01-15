using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace SpotifyArtistLocations.Data.MusicServices
{
    public class MetalArchivesService : MusicService
    {
        public override string baseUrl { get { return "https://www.metal-archives.com/bands/"; } }

        public override string FormatURL(string artist)
        {
            return baseUrl + artist.Replace(" ", "_");
        }

        public override Music.ArtistLocation GetArtistData(string artist, HtmlDocument doc)
        {
            Music.ArtistLocation curArtist = new Music.ArtistLocation(artist);

            HtmlNode metadataContainer = doc.DocumentNode.SelectSingleNode("//div[@id='band_stats']");
            if (metadataContainer == null) { Console.WriteLine("Couldn't find information for " + artist); return null; }

            HtmlNode metadata = metadataContainer.SelectSingleNode(".//dl[@class='float_left']");

            List<HtmlNode> dt = metadata.SelectNodes(".//dt").ToList();
            List<HtmlNode> dd = metadata.SelectNodes(".//dd").ToList();

            string country = "";
            string location = "";

            for (int i = 0; i < dt.Count; i++)
            {
                if (dt[i].InnerText == "Country of origin:")
                {
                    HtmlNode link = dd[i].SelectSingleNode(".//a");
                    if (link != null)
                    {
                        Console.Write("Link. ");
                        country = link.InnerText;
                    }
                    else
                    {
                        Console.Write("Not a link. ");
                        country = dd[i].InnerText;
                    }
                    Console.WriteLine("Country: " + country);
                    //curArtist.country = country;
                }
                else if (dt[i].InnerText == "Location:")
                {
                    location = dd[i].InnerText;
                    Console.WriteLine("Location: " + location);
                    //curArtist.location = location;
                }
            }

            if (country != "")
            {
                curArtist.country = country;

                if(location != "")
                {
                    curArtist.location = location;
                }
                
                return curArtist;
            }

            return null;
        }
    }
}
