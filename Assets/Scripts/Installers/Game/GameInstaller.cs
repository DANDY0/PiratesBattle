using PunNetwork.Services.MatchInfo;
using PunNetwork.Services.PlayersStats;
using PunNetwork.Services.SpawnPlayer;
using PunNetwork.Services.SpawnPoints;
using Services.GameplayService;
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
        [SerializeField] private SpawnPointsHandler _spawnPointsHandler;
        [SerializeField] private CameraSwitcher _cameraSwitcher;
        
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
            Container.Bind<ISpawnPointsHandler>().FromInstance(_spawnPointsHandler).AsSingle();
            Container.Bind<ICameraSwitcher>().FromInstance(_cameraSwitcher).AsSingle();
            Container.BindInterfacesTo<PhotonPoolService>().AsSingle();
            Container.BindInterfacesTo<InputService>().AsSingle();
            Container.BindInterfacesTo<SpawnPlayerService>().AsSingle();
            Container.BindInterfacesTo<MatchInfoService>().AsSingle();
            Container.BindInterfacesTo<PlayersStatsService>().AsSingle();
            Container.BindInterfacesTo<GameplayService>().AsSingle();

            Container.BindInterfacesTo<GameEntryPoint>().AsSingle();
        }
    }
}