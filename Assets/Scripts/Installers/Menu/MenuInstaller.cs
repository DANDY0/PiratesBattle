using Controllers.MainMenu;
using Photon.PhotonUnityNetworking.Code.Common;
using Photon.PhotonUnityNetworking.Code.Common.Factory;
using Services.Pool;
using States.Core;
using Utils;
using Utils.Extensions;
using Views;
using Views.MainMenuView;
using Zenject;

namespace Installers.Menu
{
    public class MenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Di.Container = Container;
            ConfigureGameStateMachine();
            BindServices();
            BindHandlers();
        }

        private void BindServices()
        {
            Container.BindInterfacesTo<MenuEntryPoint>().AsSingle();
            Container.BindInterfacesTo<GameFactory>().AsSingle();
            Container.BindInterfacesTo<PoolService>().AsSingle();
        }

        private void BindHandlers()
        {
            Container.BindInterfacesAndSelfTo<MenuProfileHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<SelectedCharacterHandler>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<CharactersListHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterPageHandler>().AsSingle();
        }

        private void ConfigureGameStateMachine()
        {
            var gameStateMachine = ProjectContext.Instance.Container.Resolve<IGameStateMachine>();
            gameStateMachine.SetSceneContainer(Container);
        }

    }
}