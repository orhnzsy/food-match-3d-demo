using System;
using Newtonsoft.Json;

namespace FoodMatch.Game.Player
{
    [Serializable]
    public class PlayerData
    {
        public static string FileName = "PlayerData";

        [JsonProperty("current_level")]
        public int CurrentLevel { get; set; } //Current level of the player, it will be used to load the level data
    }
}