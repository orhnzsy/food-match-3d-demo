using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace FoodMatch.Game.SaveLoad
{
    public class SaveLoadManager
    {
        private static readonly string SaveFolder = Path.Combine(Application.persistentDataPath, "Saves");

        public T Load<T>(string fileName)
        {
            try
            {
                string path = Path.Combine(SaveFolder, fileName + ".json");

                if (!File.Exists(path))
                {
                    Debug.Log($"Save file {fileName}.json doesn't exist");
                    return default;
                }

                string json = File.ReadAllText(path);

                T data = JsonConvert.DeserializeObject<T>(json);

                Debug.Log($"Data loaded from {path}");
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data: {e.Message}");
                return default;
            }
        }

        public void Save<T>(string fileName, T data)
        {
            try
            {
                if (!Directory.Exists(SaveFolder))
                {
                    Directory.CreateDirectory(SaveFolder);
                    Debug.Log($"Created save directory: {SaveFolder}");
                }

                string path = Path.Combine(SaveFolder, fileName + ".json");
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save data: {e.Message}");
            }
        }
    }
}