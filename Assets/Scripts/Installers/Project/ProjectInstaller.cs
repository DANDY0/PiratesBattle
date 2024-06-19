using Photon.PhotonUnityNetworking.Code.Common;
using Photon.Realtime;
using Services.Data;
using Services.Input;
using Services.Localization;
using Services.Network;
using Services.SceneLoading;
using Services.Sound.Impls;
using Services.Spreadsheets;
using Services.Window;
using States.Core;
using Utils;
using Zenject;

namespace Installers.Project
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Di.Container = Container;
            SignalBusInstaller.Install(Container);
            BindServices();
        }

        private void BindServices()
        {
            var loadBalancingClient = new LoadBalancingClient();
            Container.Bind<ILoadBalancingClient>().FromInstance(loadBalancingClient).AsSingle();
            Container.BindInterfacesTo<NetworkService>().AsSingle();
            Container.BindInterfacesTo<DataService>().AsSingle();
            Container.BindInterfacesTo<LocalizationService>().AsSingle();
            Container.BindInterfacesTo<SpreadsheetsService>().AsSingle();
            Container.BindInterfacesTo<WindowService>().AsSingle();
            Container.BindInterfacesTo<SceneLoadingService>().AsSingle();
			Container.BindInterfacesTo<GameStateMachine>().AsSingle();

            Container.BindInterfacesTo<AudioSourcePool>().AsSingle();
            Container.BindInterfacesTo<SoundService>().AsSingle();
        }
    }
}