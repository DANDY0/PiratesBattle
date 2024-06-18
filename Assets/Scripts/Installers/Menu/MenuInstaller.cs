using ScriptsPhotonCommon;
using ScriptsPhotonCommon.Factory;
using ScriptsPhotonCommon.Pool;
using States.Core;
using Utils;
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
            Container.BindInterfacesAndSelfTo<ChooseCharacterHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<MenuProfileHandler>().AsSingle();
        }

        private void ConfigureGameStateMachine()
        {
            var gameStateMachine = ProjectContext.Instance.Container.Resolve<IGameStateMachine>();
            gameStateMachine.SetSceneContainer(Container);
        }

    }
}