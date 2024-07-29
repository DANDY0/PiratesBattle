using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace PunNetwork.Services.SpawnPoints
{
    public interface ISpawnPointsHandler
    {
        // Vector3 GetSpawnPoint(int totalPlayers, int playerIndex);
        public Vector3 GetPlayerPosition(int index, PhotonTeam team);

    }}