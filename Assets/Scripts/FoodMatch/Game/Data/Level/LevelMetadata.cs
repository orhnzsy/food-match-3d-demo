using System.Collections.Generic;
using Newtonsoft.Json;

namespace FoodMatch.Game.Level
{
    [System.Serializable]
    public class LevelMetadata
    {
        [JsonProperty("level_files")]
        public List<string> LevelFiles { get; set; } //level file names under Resources/Levels
    }
}