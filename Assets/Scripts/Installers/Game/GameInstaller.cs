using Services;
using Services.PunNetwork;
using Services.SpawnPoints;
using Signals;
using UnityEngine;
using Zenject;

namespace Installers.Game
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private SpawnPointsService _spawnPointsService;
        
        public override void InstallBindings()
        {
            BindSignals();
            BindServices();
        }

        private void BindSignals()
        {
            Container.DeclareSignal<SignalMainSetup>();
        }
        
        private void BindServices()
        {
            Container.Bind<ISpawnPointsService>().FromInstance(_spawnPointsService).AsSingle();
            Container.BindInterfacesTo<GameStarter>().AsSingle();
            Container.BindInterfacesTo<PlayersInRoomService>().AsSingle();
            Container.BindInterfacesTo<PlayerNetworkService>().AsSingle();
        }
    }
}