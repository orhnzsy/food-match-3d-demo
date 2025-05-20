using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace FoodMatch.Game.Level
{
    public class LevelDataManager
    {
        private readonly List<LevelData> Levels = new();

        public void Initialize()
        {
            var levelMetadataTextAsset = Resources.Load<TextAsset>("level_map");

            if (levelMetadataTextAsset == null)
            {
                return;
            }

            //getting metadata first
            var levelMetadata = JsonConvert.DeserializeObject<LevelMetadata>(levelMetadataTextAsset.text);

            foreach (var levelFilename in levelMetadata.LevelFiles)
            {
                //loading level files for deserialization
                var levelDataTextAsset = Resources.Load<TextAsset>($"Levels/{levelFilename}");
                if (levelDataTextAsset == null)
                {
                    Debug.LogWarning($"Skipping level file: {levelFilename} because it was not found.");
                    continue;
                }

                var levelData = JsonConvert.DeserializeObject<LevelData>(levelDataTextAsset.text);
                Levels.Add(levelData);
            }
        }

        public LevelData GetLevelData(int levelNumber)
        {
            //modulo operation to loop through levels
            var levelIndex = levelNumber % Levels.Count;
            return Levels[levelIndex];
        }
    }
}