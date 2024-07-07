using UnityEngine;

namespace PunNetwork.Views.Player
{
    public class PlayerEffects
    {
        private PlayerView _playerView;
        private ParticleSystem _destruction;
        private Collider _collider;

        public PlayerEffects(PlayerView playerView, ParticleSystem destruction, Collider collider)
        {
            _playerView = playerView;
            _destruction = destruction;
            _collider = collider;
        }

        public void Initialize()
        {
            // Any initialization logic if needed
        }
    
    }
}