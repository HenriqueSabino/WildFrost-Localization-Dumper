using Deadpan.Enums.Engine.Components.Modding;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine.Localization.Settings;

namespace LocalizationDumper
{
    public class LocalizationDumper(string modDirectory) : WildfrostMod(modDirectory)
    {
        public override string GUID => "herebin.wildfrost.localization_dumper";

        public override string[] Depends => [];

        public override string Title => "Localization Dumper";

        public override string Description => "Dumps the current game localization strings into various file in the folder \"LocalizationExport\" in the game folder and disables itself (refresh the mod page is needed).";

        protected override async void Load()
        {
            base.Load();

            var allTables = await LocalizationSettings.StringDatabase.GetAllTables().Task;

            Directory.CreateDirectory("./LocalizationExport/");
            foreach (var table in allTables)
            {
                var file = File.OpenWrite($"./LocalizationExport/{table.name}.csv");

                using var fileWriter = new StreamWriter(file);

                fileWriter.WriteLine("Key,Value");

                foreach (var entry in table.Select(x => x.Value))
                {
                    fileWriter.Write($"{this.StringToCSVCell(entry.Key ?? string.Empty)},");
                    fileWriter.WriteLine(this.StringToCSVCell(entry.LocalizedValue ?? string.Empty));
                }
            }

            base.Unload();
        }

        /// <summary>
        /// Turn a string into a CSV cell output
        /// </summary>
        /// <param name="str">String to output</param>
        /// <returns>The CSV cell formatted string</returns>
        private string StringToCSVCell(string str)
        {
            bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));
            if (mustQuote)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");
                foreach (char nextChar in str)
                {
                    sb.Append(nextChar);
                    if (nextChar == '"')
                        sb.Append("\"");
                }
                sb.Append("\"");
                return sb.ToString();
            }

            return str;
        }
    }
}
