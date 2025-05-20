using System;
using Base.Utils;
using FoodMatch.Game.Level;
using FoodMatch.Game.Player;
using FoodMatch.Game.SaveLoad;
using FoodMatch.Level.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FoodMatch.Game
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private LevelDataManager LevelDataManager { get; set; }
        private PlayerDataManager PlayerDataManager { get; set; }
        private SaveLoadManager SaveLoadManager { get; set; }


        public LevelData CurrentLevelData { get; private set; }

        protected override void OnAwake()
        {
            //Let's make it smooth
            Application.targetFrameRate = 60;

            //Loading levels
            LevelDataManager = new LevelDataManager();
            LevelDataManager.Initialize();

            //Persistent level number
            PlayerDataManager = new PlayerDataManager();
            SaveLoadManager = new SaveLoadManager();

            //Loading player data
            PlayerDataManager.PlayerData = SaveLoadManager.Load<PlayerData>(PlayerData.FileName);

            LoadHome();
        }


        public void LoadHome()
        {
            //Add configuration if needed so we can have some animations and stuff when we return back
            //for now it is just a scene change
            SceneManager.LoadScene("Main");
        }

        public void LoadNextLevel()
        {
            //Assign level data so the level manager can use it and change the scene
            //After this point the flow will be on the level manager
            CurrentLevelData = LevelDataManager.GetLevelData(PlayerDataManager.GetCurrentLevel());
            SceneManager.LoadScene("Level");
        }

        public void IncreaseLevel()
        {
            //Increasing and saving level number to next level
            PlayerDataManager.IncreaseLevelNumber();
            SaveLoadManager.Save(PlayerData.FileName, PlayerDataManager.PlayerData);
        }

        //Being used for global scope of the game
        public int GetCurrentLevel()
        {
            return PlayerDataManager.GetCurrentLevel();
        }
    }
}