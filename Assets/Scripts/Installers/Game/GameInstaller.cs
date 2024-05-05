using Services;
using Signals;
using Zenject;

namespace Installers.Game
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindSignals();
            BindServices();
        }

        private void BindSignals()
        {
            Container.DeclareSignal<SignalMainSetup>();
        }
        
        private void BindServices()
        {
            Container.BindInterfacesTo<GameStarter>().AsSingle();
        }
    }
}