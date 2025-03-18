using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace PractisesScreen
{
    public class PracticeDataSaver
    {
        private const string SaveFolder = "PracticeData";
        private const string SaveFile = "practices.json";
        private string SavePath => Path.Combine(Application.persistentDataPath, SaveFolder, SaveFile);

        public void SavePracticeData(List<PracticeData> practiceDataList)
        {
            string directoryPath = Path.Combine(Application.persistentDataPath, SaveFolder);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string json = JsonConvert.SerializeObject(practiceDataList, Formatting.Indented);

            File.WriteAllText(SavePath, json);
        }

        public List<PracticeData> LoadPracticeData()
        {
            string directoryPath = Path.Combine(Application.persistentDataPath, SaveFolder);

            if (!Directory.Exists(directoryPath) || !File.Exists(SavePath))
            {
                return new List<PracticeData>();
            }

            try
            {
                string json = File.ReadAllText(SavePath);

                List<PracticeData> practiceDataList = JsonConvert.DeserializeObject<List<PracticeData>>(json);

                foreach (var practice in practiceDataList)
                {
                    practice.TimeDisplay = $"{practice.Minutes:D2}:{practice.Seconds:D2}";
                }

                return practiceDataList;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return new List<PracticeData>();
            }
        }
    }
}