using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SpotifyArtistLocations.Data.MusicServices
{
    public abstract class MusicService
    {
        public enum SearchOptions { ArtistOnly, ArtistAndAlbum };

        public Dictionary<string, Music.ArtistLocation> artistData;
        public abstract string baseUrl { get; }

        // This method will call GetArtistCountry for every artist string in the parameter list
        public void GetArtistCountries(List<string> artists)
        {
            if (artistData == null)
            {
                artistData = new Dictionary<string, Music.ArtistLocation>();
            }

            if (artists == null) { Console.WriteLine("Artists is null"); return; }
            if (artists.Count == 0) { Console.WriteLine("0 Artists"); return; }
            Console.WriteLine(artists.Count + " artists to be found");

            Task[] tasks = new Task[artists.Count];

            for (int item = 0; item < artists.Count; item++)
            {
                string artist = artists[item];
                Console.WriteLine("Task " + item + " will handle " + artist);
                tasks[item] = Task.Factory.StartNew(() => GetArtistCountry(artist));
            }

            Task.WaitAll(tasks);

            Console.WriteLine("Found the origins of " + artistData.Count + " artists!\n");
        }

        // This method will call GetArtistCountry for every artist object in the parameter list, using the artist album as well when the option parameter is set to ArtistAndAlbum
        public void GetArtistCountries(List<Music.ArtistInfo> artists, SearchOptions option = SearchOptions.ArtistOnly)
        {
            if (artistData == null)
            {
                artistData = new Dictionary<string, Music.ArtistLocation>();
            }

            if (artists == null) { Console.WriteLine("Artists is null"); return; }
            if (artists.Count == 0) { Console.WriteLine("0 Artists"); return; }
            Console.WriteLine(artists.Count + " artists to be found");

            Task[] tasks = new Task[artists.Count];

            switch (option)
            {
                case SearchOptions.ArtistOnly:
                    for (int item = 0; item < artists.Count; item++)
                    {
                        string artist = artists[item].artistName;
                        Console.WriteLine("Task " + item + " will handle " + artist);
                        tasks[item] = Task.Factory.StartNew(() => GetArtistCountry(artist));
                    }
                    break;
                case SearchOptions.ArtistAndAlbum:
                    for (int item = 0; item < artists.Count; item++)
                    {
                        string artist = artists[item].artistName;
                        string album = artists[item].albumName;
                        Console.WriteLine("Task " + item + " will handle " + artist + " album " + album);
                        tasks[item] = Task.Factory.StartNew(() => GetArtistCountry(artist, album));
                    }
                    break;
            }

            Task.WaitAll(tasks);

            Console.WriteLine("Found the origins of " + artistData.Count + " artists!\n");
        }

        // This method will get the location data for a single artist by calling GetArtistData (which has to be implemented in derived classes)
        public string GetArtistCountry(string artist, string album = null)
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

            string url = HandleFormattingURL(artist, album);
            Console.WriteLine("Accessing " + url);

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            if (doc == null) { Console.WriteLine("Couldn't find page for " + artist); return null; }

            Music.ArtistLocation curArtist = HandleGettingArtistData(artist, album, doc);

            if (curArtist != null)
            {
                artistData.Add(artist, curArtist);
                return curArtist.country;
            }

            return null;
        }

        public Music.ArtistLocation HandleGettingArtistData(string artist, string album, HtmlDocument doc)
        {
            if (album != null)
            {
                return GetArtistDataFromRelease(artist, doc);
            }
            else return GetArtistData(artist, doc);
        }

        public string HandleFormattingURL(string artist, string album)
        {
            if (album != null)
            {
                return FormatURL(artist, album);
            }
            else return FormatURL(artist);
        }

        // This method should return a url based on the baseURL and the formatted artist name
        public abstract string FormatURL(string artist);
        // This method should return a url based on the baseURL and the formatted artist name, it is not mandatory for a class to implement this method
        public virtual string FormatURL(string artist, string album)
        {
            throw new NotImplementedException();
            //return null;
        }

        // This method should return an ArtistLocation object if the wanted data is found, otherwise it should return null
        public abstract Music.ArtistLocation GetArtistData(string artist, HtmlDocument doc);

        // This method should return an ArtistLocation object if the wanted data is found, otherwise it should return null, it is not mandatory for a class to implement this method
        public virtual Music.ArtistLocation GetArtistDataFromRelease(string artist, HtmlDocument doc)
        {
            throw new NotImplementedException();
            //return null;
        }
    }
}
