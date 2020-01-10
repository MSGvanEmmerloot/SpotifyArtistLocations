using Microsoft.AspNetCore.Components;
using SpotifyArtistLocations.Data;
using SpotifyArtistLocations.Data.MusicServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SpotifyArtistLocations.ExtensionMethods;

namespace SpotifyArtistLocations.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static void ClearIfExists<T>(this List<T> list)
        {
            if (list != null)
            {
                Console.WriteLine("Clearing list of type " + list.GetType());
                list.Clear();
            }
        }

        public static void ClearIfExists<T, T2>(this Dictionary<T, T2> dictionary)
        {
            if (dictionary != null)
            {
                Console.WriteLine("Clearing dictionary of type " + dictionary.GetType());
                dictionary.Clear();
            }
        }

        public static bool ExistsAndContainsKey<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key)
        {
            if (dictionary != null)
            {
                return dictionary.ContainsKey(key);
            }
            return false;
        }
    }
}

namespace SpotifyArtistLocations.Pages
{  
    public partial class SpotifyClass : ComponentBase
    {
        [Inject]
        SpotifyService Spotify { get; set; }
        [Inject]
        LastFMService LastFM { get; set; }
        [Inject]
        MetalArchivesService MetalArchives { get; set; }
        [Inject]
        MusicBrainzService MusicBrainz { get; set; }

        public List<Data.Music.SongInfo> songs;
        public List<string> artistList;
        public List<Data.Music.ArtistInfo> artistInfo;

        public Dictionary<string, Data.Music.ArtistLocation> artistLocationsLastFM;
        public Dictionary<string, Data.Music.ArtistLocation> artistLocationsMetalArchives;
        public Dictionary<string, Data.Music.ArtistLocation> artistLocationsMusicBrainz;

        List<string> artistsLeft;

        public string userInputPlaylist = "";
        public string userInputArtist = "";
        public string userInputSong = "";

        private int curArtistNum = -1;
        private int curSongNum = -1;

        public Dictionary<string, string> artistLocationsAfterExtensiveSearch;

        protected override async Task OnInitializedAsync()
        {
            await Task.Run(() => Spotify.Initialize());

            songs = Spotify.songs;
            artistList = Spotify.artistList;
            artistInfo = Spotify.artists;
        }
        
        private void Reset()
        {            
            songs.ClearIfExists();

            artistList.ClearIfExists();
            artistInfo.ClearIfExists();

            artistLocationsLastFM.ClearIfExists();
            artistLocationsMetalArchives.ClearIfExists();
            artistLocationsMusicBrainz.ClearIfExists();

            artistLocationsAfterExtensiveSearch.ClearIfExists();
        }

        protected async Task GetSongsFromPlaylist()
        {
            Reset();
            Console.WriteLine("User input playlist: " + userInputPlaylist);

            await Spotify.GetArtistsFromPlaylist(userInputPlaylist);

            songs = Spotify.songs;
            artistList = Spotify.artistList;
            artistInfo = Spotify.artists;
            artistList.Sort();

            Console.WriteLine("Set songs");
        }

        protected async Task SequentialScrape()
        {
            if (artistList == null) { return; }
            if (artistList.Count == 0) { return; }

            artistsLeft = new List<string>();
            artistsLeft.AddRange(artistList);
            Console.WriteLine(artistsLeft.Count + " artists left");

            // First step: try to find all artists on MusicBrainz by searching for the album name in combination with the artist name
            Console.WriteLine("MusicBrainz:");
            await Task.Run(() => MusicBrainz.GetArtistCountries(artistInfo, MusicService.SearchOptions.ArtistAndAlbum));
            artistLocationsMusicBrainz = MusicBrainz.artistData;
            Console.WriteLine(artistLocationsMusicBrainz.Count + " artist origins found on MusicBrainz");
            this.StateHasChanged();
            foreach(string k in artistLocationsMusicBrainz.Keys)
            {
                artistsLeft.Remove(k);
            }
            Console.WriteLine(artistsLeft.Count + " artists left");
            if(artistsLeft.Count == 0) { return; }

            // Second step: try to find the remaining artists on Metal Archives by searching for the artist name
            Console.WriteLine("Metal Archives:");
            await Task.Run(() => MetalArchives.GetArtistCountries(artistsLeft));
            artistLocationsMetalArchives = MetalArchives.artistData;
            Console.WriteLine(artistLocationsMetalArchives.Count + " artist origins found on Metal Archives");
            this.StateHasChanged();
            foreach (string k in artistLocationsMetalArchives.Keys)
            {
                artistsLeft.Remove(k);
            }
            Console.WriteLine(artistsLeft.Count + " artists left");
            if (artistsLeft.Count == 0) { return; }

            // Third step: try to find the remaining artists on Metal Archives by searching for the artist name
            Console.WriteLine("LastFM:");
            await Task.Run(() => LastFM.GetArtistCountries(artistsLeft));
            artistLocationsLastFM = LastFM.artistData;
            Console.WriteLine(artistLocationsLastFM.Count + " artist origins found on LastFM");
            this.StateHasChanged();
            foreach (string k in artistLocationsLastFM.Keys)
            {
                artistsLeft.Remove(k);
            }
            Console.WriteLine(artistsLeft.Count + " artists left");

            Console.WriteLine("Artists left:");
            for(int i=0; i< artistsLeft.Count; i++)
            {
                Console.WriteLine(artistsLeft[i]);
            }

            // Last step: try to find the remaining artists on Metal Archives by searching for the artist name
            Console.WriteLine("Executing extensive search!");
            await GetRemainingArtistAlbums();
        }

        protected async Task GetArtistInfo()
        {
            if (artistList == null) { return; }
            if (artistList.Count == 0) { return; }

            Console.WriteLine("User input artist: " + userInputArtist);
            string a;

            if (userInputArtist == null || userInputArtist == "")
            {
                curArtistNum++;
                if (curArtistNum > artistList.Count - 1) { curArtistNum = 0; }

                a = artistList[curArtistNum];
            }
            else a = userInputArtist;

            Console.WriteLine("Getting extra info for " + a);

            await Spotify.GetInfoFromArtist(a);
        }

        protected async Task GetRemainingArtistAlbums()
        {
            if (artistsLeft == null) { return; }
            if (artistsLeft.Count == 0) { return; }

            for(int i=0; i< artistsLeft.Count; i++)
            {
                await GetArtistAlbums(artistsLeft[i]);
            }
        }

        protected async Task GetArtistAlbums(string artistName = null)
        {
            string chosenArtist;
            chosenArtist = artistName;

            if (artistName == null)
            {
                if (artistList == null) { return; }
                if (artistList.Count == 0) { return; }

                Console.WriteLine("User input artist: " + userInputArtist);            

                if (userInputArtist == null || userInputArtist == "")
                {
                    curArtistNum++;
                    if (curArtistNum > artistList.Count - 1) { curArtistNum = 0; }

                    chosenArtist = artistList[curArtistNum];
                }
                else chosenArtist = userInputArtist;
            }

            Console.WriteLine("Getting albums for " + chosenArtist);

            await Spotify.GetAlbumsFromArtist(chosenArtist);

            //for(int i=0; i< Spotify.artistAlbums.Count; i++)
            //{
            //    Console.WriteLine(Spotify.artistAlbums.Keys[i])
            //}
            foreach(string artist in Spotify.artistAlbums.Keys)
            {
                Console.WriteLine("Artist: " + artist);
                for(int i=0; i< Spotify.artistAlbums[artist].Count; i++)
                {
                    Console.WriteLine("Album " + i + ") " + Spotify.artistAlbums[artist][i]);
                }
            }

            if (Spotify.artistAlbums.ContainsKey(chosenArtist)){
                string foundCountry = await Task.Run(() => MusicBrainz.GetArtistCountryFromAlbums(chosenArtist, Spotify.artistAlbums[chosenArtist]));

                if(foundCountry != null)
                {
                    if(artistLocationsAfterExtensiveSearch == null)
                    {
                        artistLocationsAfterExtensiveSearch = new Dictionary<string, string>();
                    }

                    artistLocationsAfterExtensiveSearch.Add(chosenArtist, foundCountry);
                }
            }
            
        }

        protected async Task GetAudioFeatures()
        {
            if (songs == null) { return; }
            if (songs.Count == 0) { return; }

            Console.WriteLine("User input song: " + userInputSong);
            string s;

            if (userInputSong == null || userInputSong == "")
            {
                curSongNum++;
                if (curSongNum > songs.Count - 1) { curSongNum = 0; }

                s = songs[curSongNum].title;
            }
            else s = userInputSong;


            Console.WriteLine("Getting audio features for " + s);

            await Spotify.GetAudioFeaturesFromSong(s);
        }

        protected async Task GetAudioAnalysis()
        {
            if (songs == null) { return; }
            if (songs.Count == 0) { return; }            

            Console.WriteLine("User input song: " + userInputSong);
            string s;

            if (userInputSong == null || userInputSong == "")
            {
                if (curSongNum < 0) { return; }
                s = songs[curSongNum].title;
            }
            else s = userInputSong;

            Console.WriteLine("Getting audio analysis for " + s);

            await Spotify.GetAudioAnalysisFromSong(s);
        }

    }
}
