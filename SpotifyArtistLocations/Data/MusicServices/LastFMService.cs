using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace SpotifyArtistLocations.Data.MusicServices
{
    public class LastFMService : MusicService
    {
        public override string baseUrl { get { return "https://www.last.fm/music/"; } }

        public override string FormatURL(string artist)
        {
            return baseUrl + Uri.EscapeDataString(artist.Replace(" ", "+"));
        }

        public override Music.ArtistLocation GetArtistData(string artist, HtmlDocument doc)
        {
            Music.ArtistLocation curArtist = new Music.ArtistLocation(artist);

            HtmlNode metadataContainer = doc.DocumentNode.SelectSingleNode("//div[@class='metadata-column']");
            if (metadataContainer == null) { Console.WriteLine("Couldn't find information for " + artist); return null; }

            HtmlNode metadata = metadataContainer.SelectSingleNode(".//dl[@class='catalogue-metadata']");

            List<HtmlNode> dt = metadata.SelectNodes(".//dt").ToList();
            List<HtmlNode> dd = metadata.SelectNodes(".//dd").ToList();

            for (int i = 0; i < dt.Count; i++)
            {
                if (dt[i].InnerText == "Founded In")
                {
                    string founded = dd[i].InnerText;

                    string[] originParts = founded.Split(",");
                    string country = originParts[^1];

                    curArtist.country = country.Trim();
                    curArtist.location = founded.Substring(0, founded.Length - country.Length - 1);

                    Console.WriteLine(artist + " comes from " + curArtist.GetLocation());

                    return curArtist;
                }
            }

            return null;
        }
    }
}
