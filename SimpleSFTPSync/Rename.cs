using System;
using System.Globalization;

namespace SimpleSFTPSync
{
    static class Rename
    {
        /// <summary>
        /// Rename TV shows
        /// </summary>
        /// <param name="filename">Full file path</param>
        /// <returns>Renamed Version</returns>
        public static string TV(string filename)
        {
            filename = filename.Clean();

            // Usually 'Show Name s##e##' followed by garbage
            filename = filename.Replace("HDTV", string.Empty).Replace("Webrip", string.Empty);
            var found = false;
            for (var season = 1; season < 36; season++)
            {
                for (var episode = 1; episode < 36; episode++)
                {
                    var episodeNumber = "S" + (season < 10 ? "0" + season : season.ToString(CultureInfo.InvariantCulture)) + "E" + (episode < 10 ? "0" + episode : episode.ToString(CultureInfo.InvariantCulture));
                    var idx = filename.ToUpper().IndexOf(episodeNumber, StringComparison.Ordinal);
                    if (idx > 0)
                    {
                        filename = filename.Substring(0, idx) + episodeNumber + ".mkv";
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    break;
                }
            }
            return filename;
        }

        /// <summary>
        /// Rename Movies
        /// </summary>
        /// <param name="filename">Full file path</param>
        /// <returns>Renamed Version</returns>
        public static string Movie(string filename)
        {
            filename = filename.Clean();

            // Usually 'Movie Name yyyy' followed by garbage
            for (var year = 1960; year < 2030; year++)
            {
                var idx = filename.IndexOf(year.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
                if (idx <= 0)
                {
                    continue;
                }
                filename = filename.Substring(0, idx) + "(" + year + ").mkv";
                break;
            }
            return filename;
        }

        /// <summary>
        /// Shared cleaning code
        /// </summary>
        /// <param name="filename">Full file path</param>
        /// <returns>Cleaned Version</returns>
        private static string Clean(this string filename)
        {
            // Determine if we want to try to operate on the filename itself or the parent folder
            var chunks = filename.Split('\\');
            if (chunks[chunks.Length - 2].Contains("720p") || chunks[chunks.Length - 2].Contains("1080p"))
            {
                // Use parent
                filename = chunks[chunks.Length - 2] + ".mkv";
            }
            else
            {
                // Use filename
                filename = chunks[chunks.Length - 1].ToLowerInvariant();
            }

            // Strip things we know we don't want
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            filename = textInfo.ToTitleCase(filename.Replace("1080p", string.Empty).Replace("720p", string.Empty).Replace("x264", string.Empty).Replace("h264", string.Empty).Replace("ac3", string.Empty).Replace("dts", string.Empty)
                .Replace("blurayrip", string.Empty).Replace("bluray", string.Empty).Replace("dvdrip", string.Empty).Replace(".", " ").Replace("  ", " ").Replace("  ", " "));
            return filename;
        }
    }
}
