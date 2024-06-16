using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace PunNetwork.Services.Impls
{
    public class SpawnPointsService : MonoBehaviour, ISpawnPointsService
    {
        [SerializeField] private Transform _leftSpawnArea;
        [SerializeField] private Transform _rightSpawnArea;

        private readonly float _intervalZ = 2;
        
        public Vector3 GetPlayerPosition(int index, PhotonTeam team)
        {
            var playerArea = team.Name == "Blue" ? _leftSpawnArea : _rightSpawnArea;
            
            float middlePoint = (PhotonNetwork.PlayerList.Length - 1) * _intervalZ / 2;

            var valueZ = index * _intervalZ - middlePoint;
            return new Vector3(playerArea.position.x,playerArea.position.y,valueZ);
        }
        
    }
}