using States.Core;
using Utils;
using Views;
using Zenject;

namespace Installers.Menu
{
    public class MenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            DependencyInjector.Container = Container;
            ConfigureGameStateMachine();
            BindServices();
            BindHandlers();
        }

        private void BindServices()
        {
            Container.BindInterfacesTo<MenuEntryPoint>().AsSingle();
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