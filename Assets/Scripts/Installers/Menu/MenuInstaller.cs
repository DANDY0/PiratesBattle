using States.Core;
using Utils;
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
        }

        private void BindServices()
        {
            Container.BindInterfacesTo<MenuEntryPoint>().AsSingle();
        }
        
        private void ConfigureGameStateMachine()
        {
            var gameStateMachine = ProjectContext.Instance.Container.Resolve<IGameStateMachine>();
            gameStateMachine.SetSceneContainer(Container);
        }

    }
}