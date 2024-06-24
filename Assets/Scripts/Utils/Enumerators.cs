namespace Utils
{
    public class Enumerators
    {
        public enum TeamRole
        {
            MyPlayer,
            AllyPlayer,
            EnemyPlayer,
        }

        public enum PlayerProperty
        {
            IsPlayersSpawned,
            PlayerHP,
            PlayerSpawnedData,
            IsPoolsPrepared
        }

        public enum GameObjectEntryKey
        {
            Bullet = 0,
            Player = 1
        }

        public enum GameResult
        {
            Win = 0,
            Lose = 1,
            Draw = 2
        }
    }
}