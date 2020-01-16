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
        const string filePath = @"C:\Users\Public\bands.json";
        const string testPath = @"C:\Users\Public\test.json";

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
        public ArtistContainer loadedArtistData = new ArtistContainer { artists = new List<SingleArtist>() };

        // Adds an artist to artistData if the artist is not yet
        public void AddArtist(string artistName, string artistCountry)
        {
            SingleArtist newArtist = new SingleArtist { name = artistName, country = artistCountry };
            //artistData.artists.AddUnique(newArtist);
            artistData.artists.AddUniqueByObjectProperty(newArtist, "name");

            //if (artistData == null) { Console.WriteLine("ArtistData null"); return; }
            //if (artistData.artists == null) { Console.WriteLine("ArtistData.artists null"); return; }
            //Console.WriteLine("Succesfully added artist");
        }

        public void CheckFormatting()
        {
            // Write Skálmöld to file to check encoding
            string band = "Sk\u00E1lm\u00F6ld";
            if (File.Exists(filePath))
            {
                File.WriteAllText(testPath, band, Encoding.Unicode);
            }
        }

        public List<string> GetArtistsFromFile()
        {
            ReadFromFile();
            return GetKnownArtistNames();
        }

        public List<string> GetKnownArtistNames()
        {
            if(loadedArtistData == null || loadedArtistData.artists == null) { return null; }
            return loadedArtistData.artists.Select(s => s.name).ToList();
        }

        public void WriteToFile()
        {
            string s = WriteToString(artistData);
            if(s == null) { return; }

            // Open file, write this string, note that WriteAllText will overwrite existing text
            if (File.Exists(filePath))
            {
                File.WriteAllText(filePath, s, Encoding.Unicode);
            }
        }

        // Converts the given ArtistContainer to a (nicely formatted) string
        public string WriteToString(ArtistContainer a)
        {
            try
            {
                return JsonSerializer.Serialize(a, new JsonSerializerOptions { WriteIndented = true });
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public void ReadFromFile()
        {
            string s = "";

            // Open file, read string, pass this string
            if (File.Exists(filePath))
            {
                s = File.ReadAllText(filePath, Encoding.Unicode);
            }
            //s = "{\"artists\": [{\"name\": \"AC/DC\",\"country\": \"Australia\"}]}";

            // For now, the loaded artist data is stored in a seperate list, in the future the lists may be combined to reflect one "true" list of known artists
            loadedArtistData = ReadFromString(s);
            Console.WriteLine(loadedArtistData);
        }

        // Converts the given string to an ArtistContainer object
        public ArtistContainer ReadFromString(string s)
        {
            try
            {
                return JsonSerializer.Deserialize<ArtistContainer>(s);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
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
