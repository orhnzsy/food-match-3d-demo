namespace FoodMatch.Game.Player
{
    public class PlayerDataManager
    {
        private PlayerData _playerData;

        public PlayerData PlayerData
        {
            get
            {
                //making sure it is never null
                if (_playerData == null)
                {
                    _playerData = new PlayerData();
                }

                return _playerData;
            }
            set
            {
                //making sure after getting it from save data it is not null
                if (value == null)
                {
                    _playerData = new PlayerData();
                    return;
                }

                _playerData = value;
            }
        }

        public void IncreaseLevelNumber()
        {
            PlayerData.CurrentLevel++;
        }

        public int GetCurrentLevel()
        {
            return PlayerData.CurrentLevel;
        }
    }
}