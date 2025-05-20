using FoodMatch.Game;
using FoodMatch.Game.Level;
using FoodMatch.Level.Data;
using UnityEngine;

namespace FoodMatch.Level.Controller
{
    public abstract class InLevelManager : MonoBehaviour
    {
        protected LevelData LevelData { get; set; }
        protected ItemDataRepo ItemDataRepo { get; set; }

        public virtual void PrepareLevel(LevelData levelData, ItemDataRepo itemDataRepo)
        {
            LevelData = levelData;
            ItemDataRepo = itemDataRepo;
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void Cleanup()
        {
        }
    }
}