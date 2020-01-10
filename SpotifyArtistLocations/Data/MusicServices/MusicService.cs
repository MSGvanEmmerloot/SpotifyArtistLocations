using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SpotifyArtistLocations.Data.MusicServices
{
    public abstract class MusicService
    {        
        public Dictionary<string, Music.ArtistLocation> artistData;
        public abstract string baseUrl { get; }

        // This method will call GetArtistCountry for every artist in the parameter list
        public void GetArtistCountries(List<string> artists)
        {
            if (artistData == null)
            {
                artistData = new Dictionary<string, Music.ArtistLocation>();
            }

            if (artists == null) { Console.WriteLine("Artists is null");  return; }
            if (artists.Count == 0) { Console.WriteLine("0 Artists"); return; }
            Console.WriteLine(artists.Count + " artists to be found");

            Task[] tasks = new Task[artists.Count];
            Console.Write(".");
            for (int item = 0; item < artists.Count; item++)
            {
                string artist = artists[item];
                Console.WriteLine("Task " + item + " will handle " + artist);
                tasks[item] = Task.Factory.StartNew(() => GetArtistCountry(artist));
            }
            Console.Write(".");
            Task.WaitAll(tasks);

            Console.WriteLine("Found the origins of " + artistData.Count + " artists!\n");
        }

        // This method will get the location data for a single artist by calling GetArtistData (which has to be implemented in derived classes)
        public string GetArtistCountry(string artist)
        {
            if (artistData == null)
            {
                artistData = new Dictionary<string, Music.ArtistLocation>();
            }

            if (artistData.ContainsKey(artist)) {
                Console.WriteLine("Origins of " + artist + " already known: " + artistData[artist].country);
                return null; 
            }

            string url = FormatURL(artist);
            Console.WriteLine("Accessing " + url);

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            if (doc == null) { Console.WriteLine("Couldn't find page for " + artist); return null; }

            Music.ArtistLocation curArtist = GetArtistData(artist, doc);

            if(curArtist != null)
            {
                artistData.Add(artist, curArtist);
                return curArtist.country;
            }            

            return null;
        }

        // This method should return a url based on the baseURL and the formatted artist name
        public abstract string FormatURL(string artist);

        // This method should return an ArtistLocation object if the wanted data is found, otherwise it should return null
        public abstract Music.ArtistLocation GetArtistData(string artist, HtmlDocument doc);
    }
}
