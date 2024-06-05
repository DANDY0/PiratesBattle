using PunNetwork;
using PunNetwork.Services;
using PunNetwork.Services.Impls;
using Services;
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
            DependencyInjector.Container = Container;
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
            Container.BindInterfacesTo<PlayersInRoomService>().AsSingle();
            


            Container.BindInterfacesTo<BulletsPool>().AsSingle();
        }
        
        private void ConfigureGameStateMachine()
        {
            var gameStateMachine = ProjectContext.Instance.Container.Resolve<IGameStateMachine>();
            gameStateMachine.SetSceneContainer(Container);
        }
    }
}