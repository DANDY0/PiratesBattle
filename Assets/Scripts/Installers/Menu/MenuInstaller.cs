using Controllers.MainMenu;
using States.Core;
using Utils.Extensions;
using Zenject;

namespace Installers.Menu
{
    public class MenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindServices();
            BindHandlers();
            Container.InjectSceneContainer();
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
    }
}