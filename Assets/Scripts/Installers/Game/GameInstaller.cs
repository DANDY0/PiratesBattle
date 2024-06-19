using Photon.PhotonUnityNetworking.Code.Common;
using Photon.PhotonUnityNetworking.Code.Common.Factory;
using Photon.PhotonUnityNetworking.Code.Common.PhotonFactory;
using Photon.PhotonUnityNetworking.Code.Common.Pool;
using PunNetwork;
using PunNetwork.Services;
using PunNetwork.Services.ObjectsInRoom;
using PunNetwork.Services.SpawnPoints;
using Services;
using Services.Data;
using Services.GamePools;
using Services.Input;
using Signals;
using States.Core;
using UnityEngine;
using Utils;
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
            Container.BindInterfacesTo<GameEntryPoint>().AsSingle();
            Container.Bind<ISpawnPointsService>().FromInstance(_spawnPointsService).AsSingle();
            Container.BindInterfacesTo<ObjectsInRoomService>().AsSingle();
            Container.BindInterfacesTo<GameFactory>().AsSingle();
            Container.BindInterfacesTo<PoolService>().AsSingle();
            Container.BindInterfacesTo<PhotonFactory>().AsSingle();
            Container.BindInterfacesTo<GamePoolsService>().AsSingle();
            Container.BindInterfacesTo<InputService>().AsSingle();
        }
        
        private void ConfigureGameStateMachine()
        {
            var gameStateMachine = ProjectContext.Instance.Container.Resolve<IGameStateMachine>();
            gameStateMachine.SetSceneContainer(Container);
        }
    }
}