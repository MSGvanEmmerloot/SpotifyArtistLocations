using Microsoft.AspNetCore.Components;
using SpotifyArtistLocations.Data;
using SpotifyArtistLocations.Data.MusicServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SpotifyArtistLocations.ExtensionMethods;

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
        [Inject]
        FileService FileHandler { get; set; }

        public List<Data.Music.SongInfo> songs = new List<Data.Music.SongInfo>();
        public List<string> artistList;
        public List<Data.Music.ArtistInfo> artistInfo;

        public Dictionary<string, Data.Music.ArtistLocation> artistLocationsLastFM;
        public Dictionary<string, Data.Music.ArtistLocation> artistLocationsMetalArchives;
        public Dictionary<string, Data.Music.ArtistLocation> artistLocationsMusicBrainz;

        List<string> artistsLeft;

        List<string> knownArtists;

        public string userInputPlaylist = "";
        public string userInputArtist = "";
        public string userInputSong = "";

        private int curArtistNum = -1;
        private int curSongNum = -1;

        public Dictionary<string, string> artistLocationsAfterExtensiveSearch;

        protected override async Task OnInitializedAsync()
        {
            Spotify.Initialize();

            songs = Spotify.songs;
            artistList = Spotify.artistList;
            artistInfo = Spotify.artists;

            //knownArtists = FileHandler.GetKnownArtistNames();
        }

        protected async Task Test()
        {
            //List<string> upcs = ISRCSearch.GetArtistUPCs("DEY421100018");
            //if(upcs == null) { return; }

            //for(int l=0; l< upcs.Count; l++)
            //{
            //    Console.WriteLine("Code " + l + " = " + upcs[l]);
            //}

            //string barCode = await Spotify.GetAlbumByID("4GXGLj7ga17bYylaLbICr0");
            //MusicBrainz.AddProductCode("Fenriz Red Planet", barCode);
            //MusicBrainz.GetArtistCountryFromBarcode("Fenriz Red Planet", barCode);

            //FileHandler.AddArtist("AC/DC", "Australia");
            //FileHandler.AddArtist("Iron Maiden", "United Kingdom");
            //FileHandler.AddArtist("Motorhead", "United Kingdom");
            //FileHandler.CheckFormatting();
            //File.Test();
            //FileHandler.WriteToFile();
            //FileHandler.ReadFromFile();
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

            int musicBrainzCount = 0;
            int metalArchivesCount = 0;
            int lastFMCount = 0;
            int barCodeCount = 0;
            int otherAlbumCount = 0;

            artistsLeft = new List<string>();
            artistsLeft.AddRange(artistList);
            Console.WriteLine(artistsLeft.Count + " artists left");

            if(knownArtists != null)
            {
                Console.WriteLine(knownArtists.Count + " artist origins already known");

                foreach (string a in knownArtists)
                {
                    artistsLeft.Remove(a);
                }
                Console.WriteLine(artistsLeft.Count + " artists left");
            }

            // First step: try to find all artists on MusicBrainz by searching for the album name in combination with the artist name
            Console.WriteLine("MusicBrainz:");
            await Task.Run(() => MusicBrainz.GetArtistCountries(artistInfo, MusicService.SearchOptions.ArtistAndAlbum));
            artistLocationsMusicBrainz = MusicBrainz.artistData;
            musicBrainzCount = artistLocationsMusicBrainz.Count;
            Console.WriteLine(artistLocationsMusicBrainz.Count + " artist origins found on MusicBrainz");
            this.StateHasChanged();
            foreach (string k in artistLocationsMusicBrainz.Keys)
            {
                FileHandler.AddArtist(k, artistLocationsMusicBrainz[k].country);
                artistsLeft.Remove(k);
            }
            Console.WriteLine(artistsLeft.Count + " artists left");
            if(artistsLeft.Count == 0) { return; }

            // Second step: try to find the remaining artists on Metal Archives by searching for the artist name
            Console.WriteLine("Metal Archives:");
            await Task.Run(() => MetalArchives.GetArtistCountries(artistsLeft));
            artistLocationsMetalArchives = MetalArchives.artistData;
            metalArchivesCount = artistLocationsMetalArchives.Count;
            Console.WriteLine(artistLocationsMetalArchives.Count + " artist origins found on Metal Archives");
            this.StateHasChanged();
            foreach (string k in artistLocationsMetalArchives.Keys)
            {
                FileHandler.AddArtist(k, artistLocationsMetalArchives[k].country);
                artistsLeft.Remove(k);
            }
            Console.WriteLine(artistsLeft.Count + " artists left");
            if (artistsLeft.Count == 0) { return; }

            // Third step: try to find the remaining artists on Metal Archives by searching for the artist name
            Console.WriteLine("LastFM:");
            await Task.Run(() => LastFM.GetArtistCountries(artistsLeft));
            artistLocationsLastFM = LastFM.artistData;
            lastFMCount = artistLocationsLastFM.Count;
            Console.WriteLine(artistLocationsLastFM.Count + " artist origins found on LastFM");
            this.StateHasChanged();
            foreach (string k in artistLocationsLastFM.Keys)
            {
                FileHandler.AddArtist(k, artistLocationsLastFM[k].country);
                artistsLeft.Remove(k);
            }
            Console.WriteLine(artistsLeft.Count + " artists left");

            Console.WriteLine("Artists left:");
            for(int i=0; i< artistsLeft.Count; i++)
            {
                Console.WriteLine(artistsLeft[i]);
            }

            // Last step: try to find the remaining artists on MusicBrainz by searching for multiple album names in combination with the artist name
            Console.WriteLine("Executing extensive search!");
            await GetRemainingArtistsByBarcodes();
            if (artistLocationsAfterExtensiveSearch != null)
            {
                barCodeCount = artistLocationsAfterExtensiveSearch.Count;
                Console.WriteLine(barCodeCount + " artist origins found by barcode");
                this.StateHasChanged();
                foreach (string k in artistLocationsAfterExtensiveSearch.Keys)
                {
                    artistsLeft.Remove(k);
                }
            }
            
            await GetRemainingArtistAlbums();
            if (artistLocationsAfterExtensiveSearch != null)
            {
                otherAlbumCount = artistLocationsAfterExtensiveSearch.Count - barCodeCount;
                //Console.WriteLine(otherAlbumCount + " artist origins found by other albums");
                foreach (string k in artistLocationsAfterExtensiveSearch.Keys)
                {
                    FileHandler.AddArtist(k, artistLocationsAfterExtensiveSearch[k]);
                }
            }
            //await GetRemainingArtistsByBarcodes();            

            Console.WriteLine(musicBrainzCount + " artist origins found on MusicBrainz");
            Console.WriteLine(metalArchivesCount + " artist origins found on Metal Archives");
            Console.WriteLine(lastFMCount + " artist origins found on LastFM");
            Console.WriteLine(barCodeCount + " artist origins found by barcode");
            Console.WriteLine(otherAlbumCount + " artist origins found by other albums");
            FileHandler.WriteToFile();
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

        protected async Task GetRemainingArtistsByBarcodes()
        {
            //if (artistsLeft == null) { return; }
            //if (artistsLeft.Count == 0) { return; }

            //Dictionary<string, List<string>> artistBarcodes = new Dictionary<string, List<string>>();

            //for (int i = 0; i < artistsLeft.Count; i++)
            //{
            //    List<string> barcodes = await Task.Run(() => ISRCSearch.GetArtistUPCs(songs.Find(s => s.artist == artistsLeft[i]).ISRC));
            //}
            if (artistsLeft == null) { return; }
            if (artistsLeft.Count == 0) { return; }

            for (int i = 0; i < artistsLeft.Count; i++)
            {
                string chosenArtist = artistsLeft[i];
                await Spotify.GetAlbumByArtistName(chosenArtist);
                Data.Music.ArtistInfo a = artistInfo.Find(a => a.artistName == chosenArtist);
                if (a == null || a.UPCs == null || a.UPCs.Count == 0) { continue; }

                string UPC = a.UPCs[0];
                Console.WriteLine("UPC for " + chosenArtist + ": " + UPC);

                string foundCountry = MusicBrainz.GetArtistCountryFromBarcode(chosenArtist, UPC);

                if (foundCountry != null)
                {
                    if (artistLocationsAfterExtensiveSearch == null)
                    {
                        artistLocationsAfterExtensiveSearch = new Dictionary<string, string>();
                    }

                    artistLocationsAfterExtensiveSearch.Add(chosenArtist, foundCountry);
                }
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
