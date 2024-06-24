using Photon.PhotonUnityNetworking.Code.Common;
using Photon.PhotonUnityNetworking.Code.Common.Factory;
using PunNetwork.Services.ObjectsInRoom;
using PunNetwork.Services.SpawnPoints;
using Services.GamePools;
using Services.Input;
using Services.Pool;
using Signals;
using States.Core;
using UnityEngine;
using Zenject;

namespace Installers.Game
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private SpawnPointsService _spawnPointsService;
        
        public override void InstallBindings()
        {
            Di.Container = Container;
            ConfigureGameStateMachine();
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
            Container.BindInterfacesTo<ObjectsInRoomService>().AsSingle();
            Container.BindInterfacesTo<GameFactory>().AsSingle();
            Container.BindInterfacesTo<PoolService>().AsSingle();
            Container.BindInterfacesTo<PhotonPoolService>().AsSingle();
            Container.BindInterfacesTo<InputService>().AsSingle();
            Container.BindInterfacesTo<GameEntryPoint>().AsSingle();
        }
        
        private void ConfigureGameStateMachine()
        {
            var gameStateMachine = ProjectContext.Instance.Container.Resolve<IGameStateMachine>();
            gameStateMachine.SetSceneContainer(Container);
        }
    }
}