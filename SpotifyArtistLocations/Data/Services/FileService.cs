using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using SpotifyArtistLocations.ExtensionMethods;
using System.Text;
using System.IO;

namespace SpotifyArtistLocations.Data
{
    public class FileService
    {
        const string filePath = @"C:\Users\Public\test.json";

        public class ArtistContainer
        {
            public List<SingleArtist> artists { get; set; }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                foreach(var artist in artists)
                {
                    sb.Append(artist).Append("\n");
                }
                sb.Remove(sb.Length - 1, 1); // Remove last linebreak
                return sb.ToString();
            }
        }

        public class SingleArtist
        {
            public string name { get; set; }
            public string country { get; set; }

            public override string ToString()
            {
                return name + ": " + country;
            }
        }

        public ArtistContainer artistData = new ArtistContainer { artists = new List<SingleArtist>() };
        public ArtistContainer artistData2 = new ArtistContainer { artists = new List<SingleArtist>() };

        public void AddArtist(string artistName, string artistCountry)
        {
            SingleArtist newArtist = new SingleArtist { name = artistName, country = artistCountry };
            artistData.artists.AddUnique(newArtist);

            if (artistData == null) { Console.WriteLine("ArtistData null"); return; }
            if (artistData.artists == null) { Console.WriteLine("ArtistData.artists null"); return; }

            Console.WriteLine("Artistdata contains " + artistData.artists.Count  + " artists: ");
            for(int i=0; i < artistData.artists.Count; i++)
            {
                Console.WriteLine(artistData.artists[i]);
            }
        }

        public void WriteToFile()
        {
            Console.WriteLine("Writing..");

            string s = WriteToString(artistData);

            // Open file, write this string, note that WriteAllText will overwrite existing text
            if (File.Exists(filePath))
            {
                File.WriteAllText(filePath, s);
            }
        }

        public string WriteToString(ArtistContainer a)
        {
            Console.WriteLine("Artist: " + a);

            string s = JsonSerializer.Serialize(a, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("Converted string: " + s);

            return s;
        }

        public void ReadFromFile()
        {
            Console.WriteLine("Reading..");
            string s = "";

            // Open file, read string, pass this string
            if (File.Exists(filePath))
            {
                s = File.ReadAllText(filePath);
            }
            //s = "{\"artists\": [{\"name\": \"AC/DC\",\"country\": \"Australia\"}]}".Replace("\\", "");
            artistData2 = ReadFromString(s);
            Console.WriteLine(artistData2);
        }

        public ArtistContainer ReadFromString(string s)
        {   
            Console.WriteLine("String: " + s);

            ArtistContainer a = JsonSerializer.Deserialize<ArtistContainer>(s);
            Console.WriteLine("Converted artist: " + a);

            return a;
        }

        public void Test()
        {
            Console.WriteLine("Writing..");
            string s = WriteToString(artistData);

            Console.WriteLine("Reading..");
            ReadFromString(s);
        }

    }
}
