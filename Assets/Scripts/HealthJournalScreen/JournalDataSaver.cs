using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace HealthJournalScreen
{
    public static class JournalDataSaver
    {
        private const string SaveFileName = "journal_data.json";
        private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        [Serializable]
        private class JournalDataWrapper
        {
            [JsonProperty("version")] public int Version { get; set; } = 1;

            [JsonProperty("lastUpdated")] public DateTime LastUpdated { get; set; }

            [JsonProperty("entries")] public List<JournalData> Entries { get; set; }

            public JournalDataWrapper(List<JournalData> entries)
            {
                Entries = entries;
                LastUpdated = DateTime.Now;
            }
        }

        public static bool SaveJournalData(List<JournalData> journalData)
        {
            try
            {
                JournalDataWrapper wrapper = new JournalDataWrapper(journalData);
                string jsonData = JsonConvert.SerializeObject(wrapper, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        DateTimeZoneHandling = DateTimeZoneHandling.Local
                    });

                File.WriteAllText(SavePath, jsonData);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }

        public static List<JournalData> LoadJournalData()
        {
            try
            {
                if (!File.Exists(SavePath))
                {
                    return new List<JournalData>();
                }

                string jsonData = File.ReadAllText(SavePath);
                JournalDataWrapper wrapper = JsonConvert.DeserializeObject<JournalDataWrapper>(jsonData,
                    new JsonSerializerSettings
                    {
                        DateTimeZoneHandling = DateTimeZoneHandling.Local
                    });

                return wrapper.Entries;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return new List<JournalData>();
            }
        }

        public static bool DeleteJournalData()
        {
            try
            {
                if (File.Exists(SavePath))
                {
                    File.Delete(SavePath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }
    }
}