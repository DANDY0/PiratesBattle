using PunNetwork.Services.ObjectsInRoom;
using PunNetwork.Services.SpawnPoints;
using Services.GamePools;
using Services.Input;
using Signals;
using UnityEngine;
using Utils.Extensions;
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
            Container.InjectSceneContainer();
        }

        private void BindSignals()
        {
            Container.DeclareSignal<SignalMainSetup>();
        }
        
        private void BindServices()
        {
            Container.Bind<ISpawnPointsService>().FromInstance(_spawnPointsService).AsSingle();
            Container.BindInterfacesTo<PhotonPoolService>().AsSingle();
            Container.BindInterfacesTo<InputService>().AsSingle();
            Container.BindInterfacesTo<ObjectsInRoomService>().AsSingle();
            Container.BindInterfacesTo<GameEntryPoint>().AsSingle();
        }
    }
}