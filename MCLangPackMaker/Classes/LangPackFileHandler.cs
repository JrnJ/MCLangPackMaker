using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace MCLangPackMaker.Classes
{
    public enum MCVersion
    {
        V1x8x9 = 189,
        V1x19x2 = 1192
    }

    internal static class LangPackFileHandler
    {
        public static string Default1x8x9LangPackPath = @"C:\\Users\\jeroe\\source\\WPF\\MCLangPackMaker\\LangPacks\\Defaults\\en_US_1.8.9.lang";
        public static string Default1x19x2LangPackPath = @"C:\\Users\\jeroe\\source\\WPF\\MCLangPackMaker\\LangPacks\\Defaults\\en_us_1.19.2.json";

        public static string DefaultSavePath = @"C:\\Users\\jeroe\\source\\WPF\\MCLangPackMaker\\LangPacks\\";

        public static string ConversionsPath = @"C:\\Users\\jeroe\\source\\WPF\\MCLangPackMaker\\LangPacks\\Defaults\\conversions.txt";

        #region ForUser
        public static async Task<ObservableCollection<LangPackValue>?> GetLangPack(MCVersion mcVersion, string path)
        {
            switch (mcVersion)
            {
                case MCVersion.V1x8x9:
                    return await Get1x8x9LangPack(path);
                case MCVersion.V1x19x2:
                    return await Get1x19x2LangPack(path);
                default:
                    return default;
            }
        }

        public static async Task SaveLangPack(MCVersion fromMcVersion, MCVersion toMcVersion, ObservableCollection<LangPackValue> langPackValues)
        {
            // If the same, no conversion
            if (fromMcVersion == toMcVersion)
            {
                switch (toMcVersion)
                {
                    case MCVersion.V1x8x9:
                        await Save1x8x9LangPack(langPackValues, DefaultSavePath + "en_us_1.8.9.new.lang");
                        return;
                    case MCVersion.V1x19x2:
                        await Save1x19x2LangPack(langPackValues, DefaultSavePath + "en_us_1.19.2.new.json");
                        return;
                    default:
                        return;
                }
            }
            else
            { 
                // Get Default language pack for toMcVersion
                ObservableCollection<LangPackValue> defaultLangPack = await GetDefaultLangPack(toMcVersion);
                defaultLangPack ??= new();

                LangPackConversions langPackConversions = await GetConversionsFile();
                int index = 0;
                bool found = false;

                for (int i = 0; i < langPackConversions.Versions.Length; i++)
                {
                    if (langPackConversions.Versions[i] == toMcVersion)
                    {
                        index = i;
                        found = true;
                        break;
                    }
                }

                // Return if version doesnt exist
                if (!found)
                {
                    return;
                }

                // Loop over langPackValues
                for (int i = 0; i < langPackValues.Count; i++)
                {
                    // Check if changes are made
                    if (langPackValues[i].MadeChanges)
                    {
                        // Find it in the conversion file
                        for (int j = 0; j < langPackConversions.Conversions.Count; j++)
                        {
                            if (langPackValues[i].Key == langPackConversions.Conversions[j][index])
                            {
                                // Find key in defaultLangPack and change that value
                                for (int k = 0; k < defaultLangPack.Count; k++)
                                {
                                    if (defaultLangPack[k].Key == langPackValues[i].Key)
                                    {
                                        // Change value
                                        defaultLangPack[k] = langPackValues[i];
                                    }
                                }
                            }
                        }
                    }
                }

                switch (toMcVersion)
                {
                    case MCVersion.V1x19x2:
                        await Save1x19x2LangPack(defaultLangPack, DefaultSavePath + "en_us_1.19.2.new.json");
                        break;
                    case MCVersion.V1x8x9:
                        await Save1x8x9LangPack(defaultLangPack, DefaultSavePath + "en_US_1.8.9.new.lang");
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion ForUser

        #region GET
        private static async Task<ObservableCollection<LangPackValue>?> GetDefaultLangPack(MCVersion mcVersion)
        {
            switch (mcVersion)
            {
                case MCVersion.V1x19x2:
                    return await Get1x19x2LangPack(Default1x19x2LangPackPath);
                case MCVersion.V1x8x9:
                    return await Get1x8x9LangPack(Default1x8x9LangPackPath);
                default:
                    return default;
            }
        }

        private static async Task<LangPackConversions> GetConversionsFile()
        {
            string[] lines = await File.ReadAllLinesAsync(ConversionsPath);

            string[] stringVersions = lines[0].Split(',');

            MCVersion[] versions = new MCVersion[stringVersions.Length];
            for (int i = 0; i < stringVersions.Length; i++)
            {
                versions[i] = (MCVersion)int.Parse(stringVersions[i]);
            }

            List<string[]> conversions = new();

            for (int i = 1; i < lines.Length; i++)
            {
                // Skip if not the same as template length
                string[] values = lines[i].Split(',');
                if (values.Length == versions.Length)
                {
                    string[] value = 
                    {
                        values[0], values[1]
                    };

                    conversions.Add(value);
                }
            }

            return new LangPackConversions(versions, conversions);
        }

        private static async Task<ObservableCollection<LangPackValue>> Get1x8x9LangPack(string path)
        {
            string[] lines = await File.ReadAllLinesAsync(path);
            ObservableCollection<LangPackValue> values = new();

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > 0)
                {
                    string[] splitOn = lines[i].Split("=");
                    values.Add(new(splitOn[0], splitOn[1]));
                }
            }

            return values;
        }

        private static async Task<ObservableCollection<LangPackValue>> Get1x19x2LangPack(string path)
        {
            string[] lines = await File.ReadAllLinesAsync(path);
            ObservableCollection<LangPackValue> values = new();

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > 2)
                {
                    if (lines[i][2] == '"')
                    {
                        string[] splitOn = lines[i].Split("\": \"");
                        values.Add(new(splitOn[0][3..], splitOn[1][..^2]));
                    }
                }
            }

            return values;
        }
        #endregion GET

        #region SAVE
        private static async Task Save1x8x9LangPack(ObservableCollection<LangPackValue> values, string path)
        {
            string[] lines = new string[values.Count];

            for (int i = 0; i < values.Count; i++)
            {
                lines[i] = values[i].Key + "=" + values[i].Value;
            }

            await File.WriteAllLinesAsync(path, lines);
        }

        private static async Task Save1x19x2LangPack(ObservableCollection<LangPackValue> values, string path)
        {
            string[] lines = new string[values.Count + 2];
            lines[0] = "{";

            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].MadeChanges)
                {
                    
                }

                lines[i + 1] = '"' + values[i].Key + "\": \"" + values[i].Value + "\",";
            }

            lines[^2] = lines[^2].Remove(lines[^2].Length - 1);
            lines[^1] = "}";

            await File.WriteAllLinesAsync(path, lines);
        }
        #endregion SAVE
    }
}
