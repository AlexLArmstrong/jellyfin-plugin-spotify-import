using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MediaBrowser.Model.IO;

namespace Viperinius.Plugin.SpotifyImport
{
    internal static class MissingTrackStore
    {
        public static string FilenameTemplate => "{%PLAYLIST%}_missing_{%TS%}.json";

        public static string DirName => "jfplugin_spotify_import";

        public static string? CurrentTmpDir { get; private set; }

        public static string GetFilePath(string playlistName)
        {
            CreateCurrentTmpDir();
            var trimmedPlaylistName = RemoveIllegalCharacters(playlistName);

            var dateFormat = "yyyy-MM-dd_HH-mm";
            var dateFormatSetting = Plugin.Instance?.Configuration.MissingTrackListsDateFormat ?? string.Empty;
            if (CheckDateFormatValid(dateFormatSetting))
            {
                dateFormat = dateFormatSetting;
            }

            var fullPath = Path.Combine(CurrentTmpDir!, FilenameTemplate.Replace("{%PLAYLIST%}", trimmedPlaylistName, StringComparison.InvariantCulture)
                                                                        .Replace("{%TS%}", DateTime.UtcNow.ToString(dateFormat, CultureInfo.InvariantCulture), StringComparison.InvariantCulture));
            return fullPath;
        }

        public static List<string> GetFileList()
        {
            CreateCurrentTmpDir();
            var rawFiles = Directory.GetFiles(CurrentTmpDir!);
            return rawFiles.OrderBy(f => f).ToList();
        }

        public static async Task WriteFile(string fileName, List<ProviderTrackInfo> tracks)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            using var writer = File.Create(fileName);
            await JsonSerializer.SerializeAsync(writer, tracks, options).ConfigureAwait(false);
        }

        private static void CreateCurrentTmpDir()
        {
            if (!string.IsNullOrWhiteSpace(CurrentTmpDir) && Directory.Exists(CurrentTmpDir))
            {
                return;
            }

            var tmpDir = Path.Combine(Path.GetTempPath(), DirName);
            Directory.CreateDirectory(tmpDir);
            CurrentTmpDir = tmpDir;
        }

        private static string RemoveIllegalCharacters(string raw)
        {
            var invalidChars = new string(Path.GetInvalidFileNameChars());
            var regex = new Regex($"[{Regex.Escape(invalidChars)}]");
            return regex.Replace(raw, string.Empty);
        }

        private static bool CheckDateFormatValid(string format)
        {
            try
            {
                var now = DateTime.ParseExact(
                    DateTime.UtcNow.ToString(format, CultureInfo.InvariantCulture),
                    format,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.NoCurrentDateDefault);
                return !now.Equals(default);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
