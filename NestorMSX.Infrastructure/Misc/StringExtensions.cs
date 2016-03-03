using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Konamiman.NestorMSX.Misc
{
    public static class StringExtensions
    {
        /// <summary>
        /// Default base path for the AsAbsolutePath method.
        /// </summary>
        public static string DefaultBasePath { get; set; } = "$MyDocuments$/NestorMSX";

        /// <summary>
        /// Returns the full path of a file, relative to the application directory.
        /// </summary>
        /// <param name="path">File name</param>
        /// <returns>File path, relative to the application directory 
        /// (or unmodified if it was already an absolute path)</returns>
        public static string AsApplicationFilePath(this string path)
        {
            return path.AsAbsolutePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
        }

        /// <summary>
        /// Returns the full path of a file, relative to the specified base path
        /// and with all the appearances of $SpecialFolder$ (as defined in <b>Environment.SpecialFolder</b>) 
        /// properly substituted.
        /// </summary>
        /// <param name="path">File name</param>
        /// <param name="basePath">Base path to compose the full path.
        /// If null, <see cref="DefaultBasePath"/> is used.</param>
        /// <returns>File path, relative to the base directory 
        /// (or unmodified if it was already an absolute path) and will all the appearances of
        /// $SpecialFolder$ properly substituted.</returns>
        public static string AsAbsolutePath(this string path, string basePath = null)
        {
            if(basePath == null)
                basePath = DefaultBasePath.AsAbsolutePath("");

            path = path.Replace(
                "$NestorMSX$",
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                StringComparison.InvariantCultureIgnoreCase);

            var specialFolderNames = Enum.GetNames(typeof(Environment.SpecialFolder));
            foreach(var name in specialFolderNames) {
                var namePlaceholder = "$" + name + "$";
                if(path.Contains(namePlaceholder)) {
                    var enumValue = (Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), name);
                    path = path.Replace(namePlaceholder, Environment.GetFolderPath(enumValue));
                }
            }

            if(Path.IsPathRooted(path))
                return path;

            path = Path.Combine(basePath, path);

            return path;
        }

        /// <summary>
        /// Performs a string replacement with a given string comparison type.
        /// </summary>
        /// <param name="str">The source string</param>
        /// <param name="oldValue">The substring to replace</param>
        /// <param name="newValue">The replacement value</param>
        /// <param name="comparison">The type of comparison to use</param>
        /// <returns>The original string with the replacement performed</returns>
        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            if(oldValue == null) {
                throw new ArgumentNullException(nameof(oldValue));
            }
            if(oldValue == "") {
                throw new ArgumentException("String cannot be of zero length.", nameof(oldValue));
            }

            var sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while(index != -1) {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }

        public static string FormatWith(this string str, params object[] parameters)
        {
            return string.Format(str, parameters);
        }

        public static string[] WithEmptiesAsNulls(this string[] array)
        {
            return array.Select(s => string.IsNullOrWhiteSpace(s) ? null : s).ToArray();
        }

        public static string[] WithMinimumSizeOf(this string[] array, int minimumSize)
        {
            if(array.Length >= minimumSize)
                return array.ToArray();

            var extraItemsCount = minimumSize - array.Length;
            var extraItems = Enumerable.Repeat<string>(null, extraItemsCount);
            return array.Concat(extraItems).ToArray();
        }
    }
}
