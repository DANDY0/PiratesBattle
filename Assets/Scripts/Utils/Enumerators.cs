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
            PlayerNumber,
            IsSpawned,
            PlayerLives,
            PlayerSpawnedData,
            IsPoolsPrepared,
            IsPoolsPreparedAndSynced
        }

        public enum GameObjectEntryKey
        {
            Bullet = 0,
            Player = 1
        }
    }
}