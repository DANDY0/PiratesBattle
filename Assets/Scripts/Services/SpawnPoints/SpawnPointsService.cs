using Photon.Pun;
using UnityEngine;

namespace Services.SpawnPoints
{
    public class SpawnPointsService : MonoBehaviour, ISpawnPointsService
    {
        [SerializeField] private Transform _singlePoint;
        [SerializeField] private Transform _duelLeftPoint;
        [SerializeField] private Transform _duelRightPoint;
        [SerializeField] private Transform _teamLeftNearPoint;
        [SerializeField] private Transform _teamLeftFarPoint;
        [SerializeField] private Transform _teamRightNearPoint;
        [SerializeField] private Transform _teamRightFarPoint;

        public Vector3 GetSpawnPoint(int totalPlayers, int playerIndex)
        {
            return totalPlayers switch
            {
                1 => _singlePoint.position,
                2 => playerIndex == 0 ? _duelLeftPoint.position : _duelRightPoint.position,
                4 => playerIndex switch
                {
                    0 => _teamLeftNearPoint.position,
                    1 => _teamLeftFarPoint.position,
                    2 => _teamRightNearPoint.position,
                    3 => _teamRightFarPoint.position,
                    _ => throw new System.ArgumentOutOfRangeException("Invalid player index for four players.")
                },
                _ => GetDynamicSpawnPoint(totalPlayers, playerIndex)
            };
        }

        private Vector3 GetDynamicSpawnPoint(int totalPlayers, int playerIndex)
        {
            var leftMost = _duelLeftPoint.position.x;
            var rightMost = _duelRightPoint.position.x;
            var increment = (rightMost - leftMost) / (totalPlayers - 1);

            return new Vector3(leftMost + increment * playerIndex, 0, 0);
        }
    }
}