using Services.Data;
using Services.Localization;
using Services.Network;
using Services.SceneLoading;
using Services.Sound.Impls;
using Services.Spreadsheets;
using Services.Window;
using Zenject;

namespace Installers.Project
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            BindServices();
        }

        private void BindServices()
        {
            Container.BindInterfacesTo<NetworkService>().AsSingle();
            Container.BindInterfacesTo<DataService>().AsSingle();
            Container.BindInterfacesTo<LocalizationService>().AsSingle();
            Container.BindInterfacesTo<SpreadsheetsService>().AsSingle();
            Container.BindInterfacesTo<WindowService>().AsSingle();
            Container.BindInterfacesTo<SceneLoadingService>().AsSingle();
            
            Container.BindInterfacesTo<AudioSourcePool>().AsSingle();
            Container.BindInterfacesTo<SoundService>().AsSingle();
        }
    }
}