using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FoodMatch.Game.Level
{
    [Serializable]
    public class LevelData
    {
        [JsonProperty("items")]
        public List<string> Items { get; set; } //All item types in the game, it will be matched with the item data

        [JsonProperty("orders")]
        public List<OrderData> Orders { get; set; } //Order data for the level (aka win condition)

        [JsonProperty("time_limit")]
        public float TimeLimit { get; set; } //Time limit for the level
    }

    [Serializable]
    public class OrderData
    {
        [JsonProperty("item_type")]
        public string ItemType { get; set; } //Item type for the order it will be matched with the item data

        [JsonProperty("target_amount")]
        public int TargetAmount { get; set; } //Target amount of the item type for the order
    }
}