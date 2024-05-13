using UnityEngine;

namespace Services.SpawnPoints
{
    public interface ISpawnPointsService
    {
        Vector3 GetSpawnPoint(int totalPlayers, int playerIndex);
    }}