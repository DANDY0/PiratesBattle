using Factories;
using States.Core;
using Utils;
using Utils.Extensions;
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
            BindFactories();
        }

        private void BindServices()
        {
            Container.BindInterfacesTo<MenuEntryPoint>().AsSingle();
        }

        private void BindHandlers()
        {
            Container.BindInterfacesAndSelfTo<MenuProfileHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<SelectedCharacterHandler>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<CharactersListHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterPageHandler>().AsSingle();
        }

        private void BindFactories()
        {
            Container.BindFactory<IFactory<CharacterElementView>, MenuCharactersFactory>();
        }

        private void ConfigureGameStateMachine()
        {
            var gameStateMachine = ProjectContext.Instance.Container.Resolve<IGameStateMachine>();
            gameStateMachine.SetSceneContainer(Container);
        }

    }
}