using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Library;
using Viperinius.Plugin.SpotifyImport.Matchers;
using Viperinius.Plugin.SpotifyImport.Tests.TestHelpers;
using Xunit;

namespace Viperinius.Plugin.SpotifyImport.Tests.Matchers
{
    public class TrackComparisonTests : IDisposable
    {
        public void Dispose()
        {
            TrackHelper.ClearAlbums();
        }

        [Fact]
        public void Track_Matches_Default()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", null, null, null),
                TrackHelper.CreateJfItem("Track", null, null, "Just Artist"),
                TrackHelper.CreateJfItem("Track", null, "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", null, "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", null),
                TrackHelper.CreateJfItem("Track", "album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist on Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", "Just artist"),
            };

            foreach (var jf in items)
            {
                Assert.True(TrackComparison.TrackNameEqual(jf, prov, ItemMatchLevel.Default), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void Track_NoMatches_Default()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem(null, "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, null, null, null),
                TrackHelper.CreateJfItem("track", "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(".Track", null, null, null),
                TrackHelper.CreateJfItem("Tr'ack", null, null, null),
                TrackHelper.CreateJfItem("Tr####ack", null, null, null),
            };

            foreach (var jf in items)
            {
                Assert.False(TrackComparison.TrackNameEqual(jf, prov, ItemMatchLevel.Default), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void Track_Matches_CaseInsensitive()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("track", "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("track", null, null, null),
                TrackHelper.CreateJfItem("track", null, null, "Just Artist"),
                TrackHelper.CreateJfItem("track", null, "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("track", "Album", null, "Just Artist"),
                TrackHelper.CreateJfItem("track", "Album", "Artist On Album", null),
                TrackHelper.CreateJfItem("track", "album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("track", "Album", "Artist on Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", "Just artist"),
            };

            foreach (var jf in items)
            {
                Assert.True(TrackComparison.TrackNameEqual(jf, prov, ItemMatchLevel.IgnoreCase), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void Track_NoMatches_CaseInsensitive()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem(null, "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, null, null, null),
                TrackHelper.CreateJfItem("track.", "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(".Track", null, null, null),
                TrackHelper.CreateJfItem("Tr'ack", null, null, null),
                TrackHelper.CreateJfItem("Tr####ack", null, null, null),
            };

            foreach (var jf in items)
            {
                Assert.False(TrackComparison.TrackNameEqual(jf, prov, ItemMatchLevel.IgnoreCase), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void Track_Matches_NoPunctuation()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("track", "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(".track", null, null, null),
                TrackHelper.CreateJfItem("tr,ack", null, null, "Just Artist"),
                TrackHelper.CreateJfItem("tr###ack", null, "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("track", "Album", null, "Just Artist"),
                TrackHelper.CreateJfItem("trac!k", "Album", "Artist On Album", null),
                TrackHelper.CreateJfItem("Track", "album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("tr-ack", "Album", "Artist on Album", "Just Artist"),
                TrackHelper.CreateJfItem("tr&ack", "Album", "Artist On Album", "Just artist"),
            };

            foreach (var jf in items)
            {
                Assert.True(TrackComparison.TrackNameEqual(jf, prov, ItemMatchLevel.IgnorePunctuationAndCase), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void Track_NoMatches_NoPunctuation()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem(null, "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, null, null, null),
                TrackHelper.CreateJfItem("trac.", "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(".$Track", null, null, null),
                TrackHelper.CreateJfItem("Tr`ack", null, null, null),
                TrackHelper.CreateJfItem("Tr0ack", null, null, null),
            };

            foreach (var jf in items)
            {
                Assert.False(TrackComparison.TrackNameEqual(jf, prov, ItemMatchLevel.IgnorePunctuationAndCase), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void Album_Matches_Default()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, "Album", null, null),
                TrackHelper.CreateJfItem(null, "Album", null, "Just Artist"),
                TrackHelper.CreateJfItem(null, "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", null, "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", null),
                TrackHelper.CreateJfItem("track", "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist on Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", "Just artist"),
            };

            foreach (var jf in items)
            {
                Assert.True(TrackComparison.AlbumNameEqual(jf, prov, ItemMatchLevel.Default), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void Album_NoMatches_Default()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem(null, "album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, null, null, null),
                TrackHelper.CreateJfItem("Track", ".Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Al'bum", null, null),
                TrackHelper.CreateJfItem("Track", "albu", null, null),
            };

            foreach (var jf in items)
            {
                Assert.False(TrackComparison.AlbumNameEqual(jf, prov, ItemMatchLevel.Default), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void Album_Matches_CaseInsensitive()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("Track", "album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, "album", null, null),
                TrackHelper.CreateJfItem(null, "album", null, "Just Artist"),
                TrackHelper.CreateJfItem(null, "album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "album", null, "Just Artist"),
                TrackHelper.CreateJfItem("Track", "album", "Artist On Album", null),
                TrackHelper.CreateJfItem("track", "album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "album", "Artist on Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", "Just artist"),
            };

            foreach (var jf in items)
            {
                Assert.True(TrackComparison.AlbumNameEqual(jf, prov, ItemMatchLevel.IgnoreCase), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void Album_NoMatches_CaseInsensitive()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("Track", null, "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, null, null, null),
                TrackHelper.CreateJfItem("track.", "album.", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, ".Album", null, null),
                TrackHelper.CreateJfItem(null, "Al'bum", null, null),
                TrackHelper.CreateJfItem(null, "Al####bum", null, null),
            };

            foreach (var jf in items)
            {
                Assert.False(TrackComparison.AlbumNameEqual(jf, prov, ItemMatchLevel.IgnoreCase), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void Album_Matches_NoPunctuation()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("Track", "album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, ".album", null, null),
                TrackHelper.CreateJfItem(null, "al,bum", null, "Just Artist"),
                TrackHelper.CreateJfItem(null, "al###bum", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "album", null, "Just Artist"),
                TrackHelper.CreateJfItem("Track", "alb!um", "Artist On Album", null),
                TrackHelper.CreateJfItem("track", "a-lbum", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "albu&m", "Artist on Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", "Just artist"),
            };

            foreach (var jf in items)
            {
                Assert.True(TrackComparison.AlbumNameEqual(jf, prov, ItemMatchLevel.IgnorePunctuationAndCase), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void Album_NoMatches_NoPunctuation()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("Track", null, "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, null, null, null),
                TrackHelper.CreateJfItem("Track", "albu.", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, ".$Album", null, null),
                TrackHelper.CreateJfItem(null, "Al`bum", null, null),
                TrackHelper.CreateJfItem(null, "Al0bum", null, null),
            };

            foreach (var jf in items)
            {
                Assert.False(TrackComparison.AlbumNameEqual(jf, prov, ItemMatchLevel.IgnorePunctuationAndCase), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void AlbumArtist_Matches_Default()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, null, "Artist On Album", null),
                TrackHelper.CreateJfItem(null, null, "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", null, "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", null),
                TrackHelper.CreateJfItem("track", "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", "Just artist"),
            };

            foreach (var jf in items)
            {
                Assert.True(TrackComparison.AlbumArtistContained(jf, prov, ItemMatchLevel.Default), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void AlbumArtist_NoMatches_Default()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem(null, "Album", "Artist on Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, null, null, null),
                TrackHelper.CreateJfItem("Track", "Album", ".Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist On' Album", null),
                TrackHelper.CreateJfItem("Track", "Album", "artist Album", null),
            };

            foreach (var jf in items)
            {
                Assert.False(TrackComparison.AlbumArtistContained(jf, prov, ItemMatchLevel.Default), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void AlbumArtist_Matches_CaseInsensitive()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("Track", "Album", "Artist on Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, null, "Artist on Album", null),
                TrackHelper.CreateJfItem(null, null, "Artist on Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, "Album", "Artist on Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist on Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist on Album", null),
                TrackHelper.CreateJfItem("track", "Album", "Artist on Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "album", "Artist on Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", "Just artist"),
            };

            foreach (var jf in items)
            {
                Assert.True(TrackComparison.AlbumArtistContained(jf, prov, ItemMatchLevel.IgnoreCase), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void AlbumArtist_NoMatches_CaseInsensitive()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("Track", "Album", null, "Just Artist"),
                TrackHelper.CreateJfItem(null, null, null, null),
                TrackHelper.CreateJfItem("track.", "album.", "Artist on. Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, null, ".Artist On Album", null),
                TrackHelper.CreateJfItem(null, null, "Artist On' Album", null),
                TrackHelper.CreateJfItem(null, null, "Artist O####n Album", null),
            };

            foreach (var jf in items)
            {
                Assert.False(TrackComparison.AlbumArtistContained(jf, prov, ItemMatchLevel.IgnoreCase), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void AlbumArtist_Matches_NoPunctuation()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("Track", "album", "Artist on Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, null, ".Artist on Album", null),
                TrackHelper.CreateJfItem(null, null, "Artist on, Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, null, "Artist o###n Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", null, "Artist on Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist O!n Album", null),
                TrackHelper.CreateJfItem("track", "Album", "Artist O-n Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist on Al&bum", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", "Just artist"),
            };

            foreach (var jf in items)
            {
                Assert.True(TrackComparison.AlbumArtistContained(jf, prov, ItemMatchLevel.IgnorePunctuationAndCase), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void AlbumArtist_NoMatches_NoPunctuation()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("Track", "Album", null, "Just Artist"),
                TrackHelper.CreateJfItem(null, null, null, null),
                TrackHelper.CreateJfItem("Track", "Album", "Artist .lbum", "Just Artist"),
                TrackHelper.CreateJfItem(null, null, ".$Artist On Album", null),
                TrackHelper.CreateJfItem(null, null, "Artist`On Album", null),
                TrackHelper.CreateJfItem(null, null, "Artist0On Album", null),
            };

            foreach (var jf in items)
            {
                Assert.False(TrackComparison.AlbumArtistContained(jf, prov, ItemMatchLevel.IgnorePunctuationAndCase), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void Artist_Matches_Default()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, null, null, "Just Artist"),
                TrackHelper.CreateJfItem(null, null, "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem(null, "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", null, "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", null, "Just Artist"),
                TrackHelper.CreateJfItem("track", "Album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "album", "Artist On Album", "Just Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist on Album", "Just Artist"),
            };

            foreach (var jf in items)
            {
                Assert.True(TrackComparison.ArtistContained(jf, prov, ItemMatchLevel.Default), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void Artist_NoMatches_Default()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem(null, "Album", "Artist On Album", "Just artist"),
                TrackHelper.CreateJfItem(null, null, null, null),
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", ".Just Artist"),
                TrackHelper.CreateJfItem("Track", null, null, "Just 'Artist"),
                TrackHelper.CreateJfItem("Track", null, null, "Jus artist"),
            };

            foreach (var jf in items)
            {
                Assert.False(TrackComparison.ArtistContained(jf, prov, ItemMatchLevel.Default), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void Artist_Matches_CaseInsensitive()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", "Just artist"),
                TrackHelper.CreateJfItem(null, null, null, "Just artist"),
                TrackHelper.CreateJfItem(null, "Album", null, "Just artist"),
                TrackHelper.CreateJfItem(null, "Album", "Artist On Album", "Just artist"),
                TrackHelper.CreateJfItem("Track", "Album", null, "Just artist"),
                TrackHelper.CreateJfItem("Track", null, "Artist On Album", "Just artist"),
                TrackHelper.CreateJfItem("track", "Album", "Artist On Album", "Just artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist on Album", "Just artist"),
                TrackHelper.CreateJfItem("Track", "album", "Artist On Album", "Just Artist"),
            };

            foreach (var jf in items)
            {
                Assert.True(TrackComparison.ArtistContained(jf, prov, ItemMatchLevel.IgnoreCase), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void Artist_NoMatches_CaseInsensitive()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", null),
                TrackHelper.CreateJfItem(null, null, null, null),
                TrackHelper.CreateJfItem("track.", "album.", "Artist On Album", "Just artist."),
                TrackHelper.CreateJfItem(null, null, null, ".Just Artist"),
                TrackHelper.CreateJfItem(null, null, null, "Just 'Artist"),
                TrackHelper.CreateJfItem(null, null, null, "Just ###Artist"),
            };

            foreach (var jf in items)
            {
                Assert.False(TrackComparison.ArtistContained(jf, prov, ItemMatchLevel.IgnoreCase), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void Artist_Matches_NoPunctuation()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", "Just artist"),
                TrackHelper.CreateJfItem(null, null, null, ".Just artist"),
                TrackHelper.CreateJfItem(null, null, null, "Just ,artist"),
                TrackHelper.CreateJfItem(null, null, "Artist On Album", "Just### artist"),
                TrackHelper.CreateJfItem("Track", "Album", null, "Just artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", "Just !Artist"),
                TrackHelper.CreateJfItem("track", "Album", "Artist On Album", "Just- Artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist on Album", "Just& artist"),
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", "Just Artist"),
            };

            foreach (var jf in items)
            {
                Assert.True(TrackComparison.ArtistContained(jf, prov, ItemMatchLevel.IgnorePunctuationAndCase), TrackHelper.GetErrorString(jf));
            }
        }

        [Fact]
        public void Artist_NoMatches_NoPunctuation()
        {
            var prov = TrackHelper.CreateProviderItem("Track", "Album", "Artist On Album", "Just Artist");
            var items = new List<Audio>
            {
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", null),
                TrackHelper.CreateJfItem(null, null, null, null),
                TrackHelper.CreateJfItem("Track", "Album", "Artist On Album", "Just .rtist"),
                TrackHelper.CreateJfItem(null, null, null, ".$Just Artist"),
                TrackHelper.CreateJfItem(null, null, null, "Just `Artist"),
                TrackHelper.CreateJfItem(null, null, null, "Just0 Artist"),
            };

            foreach (var jf in items)
            {
                Assert.False(TrackComparison.ArtistContained(jf, prov, ItemMatchLevel.IgnorePunctuationAndCase), TrackHelper.GetErrorString(jf));
            }
        }
    }
}
