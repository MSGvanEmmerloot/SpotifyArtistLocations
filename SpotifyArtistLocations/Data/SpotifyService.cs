using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Text;

namespace SpotifyArtistLocations.Data
{
    public class SpotifyService
    {
        //const string clientID = "32c326c076814b6c926c8ca5fbb2619c";
        //const string clientSecret = "655b5f5a730d4d51b4e0209234abde23";

        const string playlistUrlBase = "https://api.spotify.com/v1/playlists/";
        const string artistUrlBase = "https://api.spotify.com/v1/artists/";
        const string audiofeaturesUrlBase = "https://api.spotify.com/v1/audio-features/";
        const string playlist_Allusz = "2zAl5HSdaBys9523aumKcY";
        const string playlist_FavoPerBand = "1QxfvjZypDekGWsiP4V5sX";
        const string playlist_Test = "4I6OL8vNs3yNigdhbP7T5H";

        SpotifyJSON.Token token;
        //SpotifyJSON.SpotifyPlaylist playlistFull;
        //SpotifyJSON.Paging playlistPageFull;
        
        SpotifyJSON.SpotifyPlaylistCompact playlist;
        SpotifyJSON.PagingCompact playlistPage;
        SpotifyJSON.Artist artistInfo;

        SpotifyJSON.AudioFeatures audioFeatures;

        public List<Music.SongInfo> songs;
        public List<string> artistList;        
        public List<Music.ArtistInfo> artists;

        public Dictionary<string, List<string>> artistAlbums;
        SpotifyJSON.AlbumPagingCompact albumsPage;

        #region Functions that use the full JSON classes
        /*
        public async Task GetArtistsFromPlaylistFull()
        {            
            string url = playlistUrlBase + playlist_FavoPerBand;

            Console.WriteLine("Using url " + url);
            Console.WriteLine("Using token " + token.access_token);
            playlist = await GetSpotifyPlaylistAsync(new Uri(url));
            Console.WriteLine("Playlist: " + playlist);
            for (int t = 0; t < playlist.tracks.items.Count; t++)
            {
                int c = songs.Count;
                SpotifyJSON.Track track = playlist.tracks.items[t].track;
                //Console.WriteLine(c + ") " + track.name + " by " + track.artists[0].name);
                await AddSong(track);
            }

            string next = playlist.tracks.next;
            Console.WriteLine(next);
            await GetNextPlaylistPage(next);

            Console.WriteLine("Found " + songs.Count + " songs in total");
            return;
        }

        public async Task GetNextPlaylistPageFull(string url)
        {
            Console.WriteLine("Using url " + url);
            playlistPage = await GetSpotifyPlaylistPageAsync(new Uri(url));
            for (int t = 0; t < playlistPage.items.Count; t++)
            {
                int c = songs.Count;
                SpotifyJSON.Track track = playlistPage.items[t].track;
                //Console.WriteLine(c + ") " + track.name + " by " + track.artists[0].name);
                await AddSong(track);
            }

            string next = playlistPage.next;
            Console.WriteLine(next);
            if(next != null)
            {
                await GetNextPlaylistPage(next);
            }            
        }

        public async Task AddSongFull(SpotifyJSON.Track track)
        {
            if (songs == null) { songs = new List<Music.SongInfo>(); }
            if (artistList == null) { artistList = new List<string>(); }
            if (artists == null) { artists = new List<Music.ArtistInfo>(); }

            string song = track.name;
            string artist = track.artists[0].name;
            string artistID = track.artists[0].id;

            await Task.Run(() =>
            {
                songs.Add(new Music.SongInfo(song, artist));

                if (!artists.Exists(a => a.artistName.Equals(artist)))
                {
                    Music.ArtistInfo newArtist = new Music.ArtistInfo(artist, artistID);
                    newArtist.SetAlbum(track.album.name, track.album.id, track.album.release_date);
                    artists.Add(newArtist);
                }

                if (!artistList.Contains(artist))
                {
                    artistList.Add(artist);
                }
            });
        }
        */
        #endregion Functions that use the full JSON classes

        public void Reset()
        {
            if(songs == null) { songs = new List<Music.SongInfo>(); }
            songs.Clear();
            if (artistList == null) { artistList = new List<string>(); }
            artistList.Clear();
            if (artists == null) { artists = new List<Music.ArtistInfo>(); }
            artists.Clear();
    }

        #region Compact functions
        public async Task GetArtistsFromPlaylist(string playlistLink = null)
        {
            Reset();

            string url;

            if (playlistLink == null || playlistLink == "")
            {
                //url = playlistUrlBase + playlist_Test;
                url = playlistUrlBase + playlist_FavoPerBand;
            }
            else
            {
                string[] urlparts = playlistLink.Split("/");
                url = playlistUrlBase + urlparts[^1];
                //url = playlistUrlBase + playlist;
            }

            url += "?fields=tracks(items.track(album(id,name,release_date),artists,id,name),next)";

            Console.WriteLine("Using url " + url);
            Console.WriteLine("Using token " + token.access_token);
            playlist = await GetSpotifyPlaylistAsync(new Uri(url));
            Console.WriteLine("Playlist: " + playlist);
            for (int t = 0; t < playlist.tracks.items.Count; t++)
            {
                int c = songs.Count;
                SpotifyJSON.TrackCompact track = playlist.tracks.items[t].track;
                //Console.WriteLine(c + ") " + track.name + " by " + track.artists[0].name);
                await AddSong(track);
            }

            string next = playlist.tracks.next;
            Console.WriteLine(next);

            if (next != null)
            {
                await GetNextPlaylistPage(next);
            }            

            Console.WriteLine("Found " + songs.Count + " songs in total");
            return;
        }

        public async Task GetNextPlaylistPage(string url)
        {
            url += "&fields=items.track(album(id,name,release_date),artists,id,name),next";

            Console.WriteLine("Using url " + url);
            playlistPage = await GetSpotifyPlaylistPageAsync(new Uri(url));
            for (int t = 0; t < playlistPage.items.Count; t++)
            {
                int c = songs.Count;
                SpotifyJSON.TrackCompact track = playlistPage.items[t].track;
                //Console.WriteLine(c + ") " + track.name + " by " + track.artists[0].name);
                await AddSong(track);
            }

            string next = playlistPage.next;
            Console.WriteLine(next);
            if (next != null)
            {
                await GetNextPlaylistPage(next);
            }
        }

        public async Task AddSong(SpotifyJSON.TrackCompact track)
        {
            if (songs == null) { songs = new List<Music.SongInfo>(); }
            if (artistList == null) { artistList = new List<string>(); }
            if (artists == null) { artists = new List<Music.ArtistInfo>(); }

            string song = track.name;
            string artist = track.artists[0].name;
            string artistID = track.artists[0].id;
            string songID = track.id;

            await Task.Run(() =>
            {
                songs.Add(new Music.SongInfo(song, artist, songID));

                if (!artists.Exists(a => a.artistName.Equals(artist)))
                {
                    Music.ArtistInfo newArtist = new Music.ArtistInfo(artist, artistID);
                    newArtist.SetAlbum(track.album.name, track.album.id, track.album.release_date);
                    artists.Add(newArtist);
                }

                if (!artistList.Contains(artist))
                {
                    artistList.Add(artist);
                }
            });
        }
        #endregion Compact functions

        public async Task GetInfoFromArtist(string artist)
        {
            Music.ArtistInfo a = artists.Find(a => a.artistName.Equals(artist));
            if(a == null) { return; }

            string url = artistUrlBase + a.artistId;

            Console.WriteLine("Using url " + url);
            artistInfo = await GetSpotifyArtistAsync(new Uri(url));
            a.SetGenres(artistInfo.genres);

            Console.WriteLine("Complete artist info:" + a);

            return;
        }

        public async Task GetAlbumsFromArtists(List<string> artists)
        {
            if (artistAlbums == null)
            {
                artistAlbums = new Dictionary<string, List<string>>();
            }
            artistAlbums.Clear();

            for (int i=0; i<artists.Count; i++)
            {
                await GetAlbumsFromArtist(artists[i]);
            }
        }

        public async Task GetAlbumsFromArtist(string artist)
        {
            Music.ArtistInfo a = artists.Find(a => a.artistName.Equals(artist));
            if (a == null) { return; }

            int albumsToFind = 10;
            int albumsToAdd = 5;

            string url = artistUrlBase + a.artistId + "/albums";
            url += "?include_groups=album,single&limit="+ albumsToFind;

            Console.WriteLine("Using url " + url);
            albumsPage = await GetSpotifyArtistAlbumsAsync(new Uri(url));
            if(albumsPage == null) { Console.WriteLine("Couldn't find album"); }
            Console.WriteLine(JsonSerializer.Serialize(albumsPage));

            if(artistAlbums == null)
            {
                artistAlbums = new Dictionary<string, List<string>>();
            }

            if (!artistAlbums.ContainsKey(artist))
            {
                artistAlbums.Add(artist, new List<string>());
            }
            if(artistAlbums[artist] == null)
            {
                artistAlbums[artist] = new List<string>();
            }
            artistAlbums[artist].Clear();

            for(int i=0; i<albumsPage.items.Count; i++)
            {
                if(artistAlbums[artist].Count == albumsToAdd) { return; }
                
                if (artistAlbums[artist].Contains(albumsPage.items[i].name)){ continue; }
                Console.WriteLine("Adding album " + albumsPage.items[i].name + " to artist " + artist);
                artistAlbums[artist].Add(albumsPage.items[i].name);
            }

            return;
        }

        public async Task GetAudioFeaturesFromSong(string song)
        {
            Music.SongInfo s = songs.Find(a => a.title.Equals(song));
            if (s == null) { Console.WriteLine("No song found"); return; }
            if (s.songId == null) { Console.WriteLine("No id found for song"); return; }

            string url = audiofeaturesUrlBase + s.songId;

            Console.WriteLine(s.GetFullString());
            Music.ArtistInfo a = artists.Find(a => a.artistName.Equals(s.artist));
            if (a != null)
            {
                Console.WriteLine(a.ToString());
            }

            Console.WriteLine("Using url " + url);

            audioFeatures = await GetSpotifyAudioFeaturesAsync(new Uri(url));
            Console.WriteLine("Complete artist info:" + audioFeatures);

            return;
        }

        public async Task GetAudioAnalysisFromSong(string song)
        {
            Music.SongInfo s = songs.Find(a => a.title.Equals(song));
            if (s == null) { Console.WriteLine("No song found"); return; }
            if (s.songId == null) { Console.WriteLine("No id found for song"); return; }

            string url = "https://api.spotify.com/v1/audio-analysis/" + s.songId;
            await GetSpotifyAudioAnalysisAsync(new Uri(url));

            return;
        }

        public void Initialize()
        {
            if (token == null)
            {
                token = GetSpotifyToken();
                Console.WriteLine("Received token " + token.access_token);
            }
            else Console.WriteLine("Already has token " + token.access_token);

            if (songs == null) { songs = new List<Music.SongInfo>(); }
            if (artists == null) { artists = new List<Music.ArtistInfo>(); }
            if (artistList == null) { artistList = new List<string>(); }
        }

        private SpotifyJSON.Token GetSpotifyToken()
        {
            string auth = System.Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(
                    Credentials.clientID + ':' + Credentials.clientSecret
            ));

            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded; charset=UTF-8");
                wc.Headers.Add(HttpRequestHeader.Authorization, "Basic " + auth);

                string tokenString = wc.UploadString(new Uri("https://accounts.spotify.com/api/token"), "POST", "grant_type=client_credentials");
                Console.WriteLine("Token: " + tokenString);
                return JsonSerializer.Deserialize<SpotifyJSON.Token>(tokenString);
            }
        }

        #region REST calls that use the full JSON classes
        /*
        private async Task<SpotifyJSON.SpotifyPlaylist> GetSpotifyPlaylistAsync(Uri uri)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                wc.Headers.Add(HttpRequestHeader.Accept, "application/json");
                wc.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token.access_token);

                string data = await wc.DownloadStringTaskAsync(uri);
                Console.WriteLine("GET Playlist received " + data.Length + " characters of data");
                SpotifyJSON.SpotifyPlaylist s = await Task.FromResult(JsonSerializer.Deserialize<SpotifyJSON.SpotifyPlaylist>(data));
                return s;
            }
        }

        private async Task<SpotifyJSON.Paging> GetSpotifyPlaylistPageAsync(Uri uri)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                wc.Headers.Add(HttpRequestHeader.Accept, "application/json");
                wc.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token.access_token);

                string data = await wc.DownloadStringTaskAsync(uri);
                Console.WriteLine("GET Page received " + data.Length + " characters of data");
                SpotifyJSON.Paging s = await Task.FromResult(JsonSerializer.Deserialize<SpotifyJSON.Paging>(data));
                return s;
            }
        }
        */
        #endregion REST calls that use the full JSON classes

        #region Compact REST calls
        private async Task<SpotifyJSON.SpotifyPlaylistCompact> GetSpotifyPlaylistAsync(Uri uri)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                wc.Headers.Add(HttpRequestHeader.Accept, "application/json");
                wc.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token.access_token);

                string data = await wc.DownloadStringTaskAsync(uri);
                Console.WriteLine("GET Playlist Received " + data.Length + " characters of data");
                SpotifyJSON.SpotifyPlaylistCompact s = await Task.FromResult(JsonSerializer.Deserialize<SpotifyJSON.SpotifyPlaylistCompact>(data));
                return s;
            }
        }

        private async Task<SpotifyJSON.PagingCompact> GetSpotifyPlaylistPageAsync(Uri uri)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                wc.Headers.Add(HttpRequestHeader.Accept, "application/json");
                wc.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token.access_token);

                string data = await wc.DownloadStringTaskAsync(uri);
                Console.WriteLine("GET Page Received " + data.Length + " characters of data");
                SpotifyJSON.PagingCompact s = await Task.FromResult(JsonSerializer.Deserialize<SpotifyJSON.PagingCompact>(data));
                return s;
            }
        }

        private async Task<SpotifyJSON.AlbumPagingCompact> GetSpotifyArtistAlbumsAsync(Uri uri)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                wc.Headers.Add(HttpRequestHeader.Accept, "application/json");
                wc.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token.access_token);

                string data = await wc.DownloadStringTaskAsync(uri);
                SpotifyJSON.AlbumPagingCompact s = await Task.FromResult(JsonSerializer.Deserialize<SpotifyJSON.AlbumPagingCompact>(data));
                //Console.WriteLine(s);
                //Console.WriteLine(s.items);
                //Console.WriteLine(s.items[0].name);
                //Console.WriteLine(JsonSerializer.Serialize(s));
                return s;
            }
        }
        #endregion Compact REST calls

        private async Task<SpotifyJSON.Artist> GetSpotifyArtistAsync(Uri uri)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                wc.Headers.Add(HttpRequestHeader.Accept, "application/json");
                wc.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token.access_token);

                string data = await wc.DownloadStringTaskAsync(uri);
                Console.WriteLine("Artist:" + data);
                SpotifyJSON.Artist s = await Task.FromResult(JsonSerializer.Deserialize<SpotifyJSON.Artist>(data));
                return s;
            }
        }

        private async Task<SpotifyJSON.AudioFeatures> GetSpotifyAudioFeaturesAsync(Uri uri)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                wc.Headers.Add(HttpRequestHeader.Accept, "application/json");
                wc.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token.access_token);

                string data = await wc.DownloadStringTaskAsync(uri);
                Console.WriteLine("Audio features:" + data);
                SpotifyJSON.AudioFeatures s = await Task.FromResult(JsonSerializer.Deserialize<SpotifyJSON.AudioFeatures>(data));
                return s;
            }
        }

        private async Task<SpotifyJSON.AudioFeatures> GetSpotifyAudioAnalysisAsync(Uri uri)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                wc.Headers.Add(HttpRequestHeader.Accept, "application/json");
                wc.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token.access_token);

                //string data = await wc.DownloadStringTaskAsync(uri);
                //SpotifyJSON.AudioFeatures s = await Task.FromResult(JsonSerializer.Deserialize<SpotifyJSON.AudioFeatures>(data));
                //return s;

                char[] charBuffer = new char[800];
                using (StreamReader st = new StreamReader(wc.OpenRead(uri)))
                {
                    st.Read(charBuffer, 0, 800);
                }

                Console.WriteLine("First characters of audio analysis stream:");
                for (int c = 0; c< charBuffer.Length; c++)
                {
                    Console.Write(charBuffer[c]);
                }
                
                return null;
            }
        }
    }
}
