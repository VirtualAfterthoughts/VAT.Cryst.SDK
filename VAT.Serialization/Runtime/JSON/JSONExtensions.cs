using System.IO;

using Newtonsoft.Json.Linq;

namespace VAT.Serialization.JSON {
    public static partial class JSONExtensions {
        /// <summary>
        /// Writes the desired json to a file at the specified path.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="path"></param>
        public static void WriteToFile(this JObject json, string path) {
            File.WriteAllText(path, json.ToString());
        }

        /// <summary>
        /// Reads json from the file at the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static JObject ReadFromFile(this string path) {
            return JObject.Parse(File.ReadAllText(path));
        }
    }
}
