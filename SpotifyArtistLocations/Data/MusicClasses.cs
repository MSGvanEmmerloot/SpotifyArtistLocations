using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyArtistLocations.Data.Music
{    
    public class ArtistLocation
    {
        public string name { get; set; }
        public string country { get; set; }
        public string location { get; set; }

        public ArtistLocation(string artistName)
        {
            name = artistName;
        }

        public string GetLocation()
        {
            return country + " (" + location + ")";
        }
    }

    public class SongInfo
    {
        public string title;
        public string artist;
        public string songId;
        public string ISRC; // International Standard Recording Code

        public SongInfo(string _title, string _artist, string _songId = null, string _ISRC = null)
        {
            title = _title;
            artist = _artist;
            songId = _songId;
            ISRC = _ISRC;
        }

        public override string ToString()
        {
            return title + " by " + artist;
        }

        public string GetFullString()
        {
            return title + " (" + songId + ") by " + artist;
        }
    }

    public class ArtistInfo
    {
        public string artistName;
        public string artistId;

        public string albumName;
        public string albumId;
        public string albumDate;
        //public string UPC; // Universal Product Code
        public List<string> UPCs;

        public List<string> genres;
        public ArtistInfo(string _artistName, string _artistId)
        {
            artistName = _artistName;
            artistId = _artistId;
        }

        public void SetAlbum(string _albumName, string _albumId, string _albumDate)
        {
            albumName = _albumName;
            albumId = _albumId;
            albumDate = _albumDate;
        }

        public void SetGenres(List<string> _genres)
        {
            genres = _genres;
        }

        public void SetProductCode(string _UPC)
        {
            if(UPCs == null){
                UPCs = new List<string>();
            }

            UPCs.Add(_UPC);
        }

        public void SetProductCodes(List<string> _UPCs)
        {
            UPCs = _UPCs;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\nArtist: " + artistName + " (id = " + artistId + ")\n");
            sb.Append("Album: " + albumName + " (id = " + albumId + ", released on " + albumDate + ")\n");            

            if (genres != null)
            {
                if (genres.Count > 0)
                {
                    sb.Append("Genres:");
                    sb.Append(genres[0]);

                    if (genres.Count > 1)
                    {
                        for (int i = 1; i < genres.Count; i++)
                        {
                            sb.Append(", ").Append(genres[i]);
                        }
                    }
                }
            }

            return sb.ToString();
        }
    }
}
