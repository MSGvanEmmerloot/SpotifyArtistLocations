using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyArtistLocations.Data.SpotifyJSON
{
    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
    }

    #region Compact JSON classes
    public class SpotifyPlaylistCompact
    {
        public PagingCompact tracks { get; set; }
    }

    public class PagingCompact
    {
        public List<PlaylistTrackCompact> items { get; set; }
        public string next { get; set; }
    }

    public class AlbumPagingCompact
    {
        public List<AlbumSimplifiedCompact> items { get; set; }
    }

    public class PlaylistTrackCompact
    {
        public TrackCompact track { get; set; }
    }

    public class TrackCompact
    {
        public AlbumSimplifiedCompact album { get; set; }
        public List<ArtistSimplified> artists { get; set; }
        public Dictionary<string, string> external_ids { get; set; }
        public string id { get; set; }
        public string name { get; set; }
    }

    public class AlbumSimplifiedCompact
    {
        public string id { get; set; }
        public string name { get; set; }
        public string release_date { get; set; }
    }

    // This class is not simplified due to problems with limiting the artist array fields
    //public class ArtistSimplified
    //{
    //    public Dictionary<string, string> external_urls { get; set; }
    //    public string href { get; set; }
    //    public string id { get; set; }
    //    public string name { get; set; }
    //    public string type { get; set; }
    //    public string uri { get; set; }
    //}
    #endregion Compact JSON classes

    public class AudioFeatures
    {
        // Danceability describes how suitable a track is for dancing based on a combination of musical elements including tempo, rhythm stability, beat strength, and overall regularity. A value of 0.0 is least danceable and 1.0 is most danceable.
        public float dancability { get; set; }

        // Energy is a measure from 0.0 to 1.0 and represents a perceptual measure of intensity and activity.Typically, energetic tracks feel fast, loud, and noisy.For example, death metal has high energy, while a Bach prelude scores low on the scale.Perceptual features contributing to this attribute include dynamic range, perceived loudness, timbre, onset rate, and general entropy.
        public float energy { get; set; }

        // The key the track is in. Integers map to pitches using standard Pitch Class notation. E.g. 0 = C, 1 = C♯/D♭, 2 = D, and so on.
        public int key { get; set; }

        // The overall loudness of a track in decibels (dB). Loudness values are averaged across the entire track and are useful for comparing relative loudness of tracks. Loudness is the quality of a sound that is the   primary psychological correlate of physical strength (amplitude). Values typical range between -60 and 0 db.
        public float loudness { get; set; }

        // Mode indicates the modality (major or minor) of a track, the type of scale from which its melodic content is derived. Major is represented by 1 and minor is 0.
        public int mode { get; set; }

        // Speechiness detects the presence of spoken words in a track. The more exclusively speech-like the recording (e.g. talk show, audio book, poetry), the closer to 1.0 the attribute value. Values above 0.66 describe tracks that are probably made entirely of spoken words. Values between 0.33 and 0.66 describe tracks that may contain both music and speech, either in sections or layered, including such cases as rap music.  Values below 0.33 most likely represent music and other non-speech-like tracks.
        public float speechiness { get; set; }

        // A confidence measure from 0.0 to 1.0 of whether the track is acoustic. 1.0 represents high confidence the track is acoustic.
        public float acousticness { get; set; }

        // Predicts whether a track contains no vocals. “Ooh” and “aah” sounds are treated as instrumental in this context. Rap or spoken word tracks are clearly “vocal”. The closer the instrumentalness value is to 1.0,   the greater likelihood the track contains no vocal content. Values above 0.5 are intended to represent instrumental tracks, but confidence is higher as the value approaches 1.0.
        public float instrumentalness { get; set; }

        // Detects the presence of an audience in the recording. Higher liveness values represent an increased probability that the track was performed live. A value above 0.8 provides strong likelihood that the track is live.
        public float liveness { get; set; }

        // A measure from 0.0 to 1.0 describing the musical positiveness conveyed by a track. Tracks with high valence sound more positive (e.g. happy, cheerful, euphoric), while tracks with low valence sound more negative (e.g. sad, depressed, angry).
        public float valence { get; set; }

        // The overall estimated tempo of a track in beats per minute (BPM). In musical terminology, tempo is the speed or pace of a given piece and derives directly from the average beat duration.
        public float tempo { get; set; }

        // The object type: “audio_features”
        public string type { get; set; }

        // The Spotify ID for the track.
        public string id { get; set; }

        // The Spotify URI for the track.
        public string uri { get; set; }

        // A link to the Web API endpoint providing full details of the track.
        public string track_href { get; set; }

        // An HTTP URL to access the full audio analysis of this track. An access token is required to access this data.
        public string analysis_url { get; set; }

        // The duration of the track in milliseconds.
        public int duration_ms { get; set; }

        // An estimated overall time signature of a track. The time signature (meter) is a notational convention to specify how many beats are in each bar (or measure).
        public int time_signature { get; set; }
    }

    #region Full JSON classes
    public class SpotifyPlaylist
    {
        public bool collaborative { get; set; }
        public string description { get; set; }
        public Dictionary<string, string> external_urls { get; set; }
        public Followers followers { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<Image> images { get; set; }
        public string name { get; set; }
        public User owner { get; set; }
        public string primary_color { get; set; }
        public bool ispublic { get; set; } //public
        public string snapshot_id { get; set; }
        public Paging tracks { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Followers
    {
        public string href { get; set; }
        public int total { get; set; }
    }

    public class Image
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class User
    {
        public string display_name { get; set; }
        public Dictionary<string, string> external_urls { get; set; }
        public Followers followers { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<Image> images { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Paging
    {
        public string href { get; set; }
        public List<PlaylistTrack> items { get; set; }
        public int limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
        public string previous { get; set; }
        public int total { get; set; }
    }

    public class PlaylistTrack
    {
        public string added_at { get; set; }
        public User added_by { get; set; }
        public bool is_local { get; set; }
        public string primaryColor { get; set; }
        public Track track { get; set; }
        public VideoThumbnail video_thumbnail { get; set; }
    }    

    public class VideoThumbnail
    {
        public string url { get; set; }
    }

    public class Track
    {
        public AlbumSimplified album { get; set; }
        public List<ArtistSimplified> artists { get; set; }
        public List<string> available_markets { get; set; }
        public int disc_number { get; set; }
        public int duration_ms { get; set; }
        public bool episode { get; set; }
        public bool isExplicit { get; set; } //explicit
        public Dictionary<string, string> external_ids { get; set; }
        public Dictionary<string, string> external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public bool is_playable { get; set; }
        public TrackLink linked_from { get; set; }
        public Restrictions restrictions { get; set; }
        public string name { get; set; }
        public int popularity { get; set; }
        public string preview_url { get; set; }
        public bool track { get; set; }
        public int track_number { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
        public bool is_local { get; set; }
    }

    public class TrackLink
    {
        public Dictionary<string, string> external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Restrictions
    {
        public Dictionary<string, string> restriction { get; set; }
    }

    public class Album
    {
        public string album_type { get; set; }
        public List<ArtistSimplified> artists { get; set; }
        public List<string> available_markets { get; set; }
        //public List<string> external_urls { get; set; }
        public Dictionary<string, string> external_ids { get; set; }
        public Dictionary<string, string> external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<Image> images { get; set; }
        public string name { get; set; }
        public string release_date { get; set; }
        public string release_date_precision { get; set; }
        public int total_tracks { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class AlbumSimplified
    {
        public string album_group { get; set; }
        public string album_type { get; set; }
        public List<ArtistSimplified> artists { get; set; }
        public List<string> available_markets { get; set; }
        public Dictionary<string, string> external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<Image> images { get; set; }
        public string name { get; set; }
        public string release_date { get; set; }
        public string release_date_precision { get; set; }
        public int total_tracks { get; set; }
        public Restrictions restrictions { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Artist
    {
        public Dictionary<string, string> external_urls { get; set; }
        public Followers followers { get; set; }
        public List<string> genres { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<Image> images { get; set; }
        public string name { get; set; }
        public int popularity { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class ArtistSimplified
    {
        public Dictionary<string, string> external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }
#endregion

    public class SpotifyDummyClass
    {
        public SpotifyPlaylist dummyPlaylist { get; private set; }

        public SpotifyDummyClass()
        {
            dummyPlaylist = CreateDummySpotifyPlaylist();
        }

        private SpotifyPlaylist CreateDummySpotifyPlaylist()
        {
            return new SpotifyPlaylist
            {
                collaborative = false,
                description = "",
                external_urls = new Dictionary<string, string> { { "spotify", "blabla" } },
                followers = new Followers { href = null, total = 3 },
                images = new List<Image> { new Image { height = 640, url = "blabla", width = 640 } },
                name = "Favo per band",
                owner = new User
                {
                    display_name = "Mika",
                    external_urls = new Dictionary<string, string> { { "spotify", "blabla" } },
                    href = "blabla",
                    id = "123456",
                    type = "user",
                    uri = "spotifyURI"
                },
                primary_color = null,
                ispublic = true,
                snapshot_id = "blabla",
                tracks = new Paging
                {
                    href = "blabla",
                    items = new List<PlaylistTrack>{new PlaylistTrack{
                            added_at = "2014-10-27",
                            added_by = new User {
                                display_name = "Mika",
                                external_urls = new Dictionary<string, string> { { "spotify", "blabla" } },
                                href = "blabla",
                                id = "123456",
                                type = "user",
                                uri = "spotifyURI"
                            },
                            is_local = false,
                            primaryColor = null,
                            track = new Track
                            {
                                album = new AlbumSimplified{
                                    album_type="album",
                                    artists = new List<ArtistSimplified>{ new ArtistSimplified{
                                    external_urls = new Dictionary<string, string> { { "spotify", "blabla" } },
                                    href="blabla",
                                    id="blablaID",
                                    name="Children of Bodom",
                                    type="artist",
                                    uri="blablaURI"
                                    } },
                                    available_markets = new List<string>{"AB", "CD"},
                                    external_urls = new Dictionary<string, string> { { "spotify", "blabla" } },
                                    href="blabla",
                                    id="blablaID",
                                    images = new List<Image> { new Image { height = 640, url = "blabla", width = 640 } },
                                    name="Follow The Reaper",
                                    release_date="2008-01-01",
                                    release_date_precision="day",
                                    total_tracks=11,
                                    type="album",
                                    uri="blablaAlbum"
                                },
                                artists = new List<ArtistSimplified>{ new ArtistSimplified{
                                    external_urls = new Dictionary<string, string> { { "spotify", "blabla" } },
                                    href="blabla",
                                    id="blablaID",
                                    name="Children of Bodom",
                                    type="artist",
                                    uri="blablaURI"
                                } },
                                available_markets = new List<string>{"AB", "CD"},
                                disc_number=1,
                                duration_ms=33186,
                                episode=false,
                                isExplicit=false,
                                external_ids = new Dictionary<string, string> { { "isrc", "FISFS0000016" } },
                                external_urls = new Dictionary<string, string> { { "spotify", "blabla" } },
                                href="blabla",
                                id="id12346",
                                is_local=false,
                                name="Children of Bodom",
                                popularity=0,
                                preview_url=null,
                                track=true,
                                track_number=3,
                                type="track",
                                uri="blablaURI"
                            },
                            video_thumbnail = new VideoThumbnail{url=null}
                        } },
                    limit = 100,
                    next = "nextSong",
                    offset = 0,
                    previous = null,
                    total = 301
                },
                type = "playlist",
                uri = "link"
            };
        }
    }
}
