using ScriptsPhotonCommon;
using States.Core;
using Utils;
using Zenject;

namespace Installers.Initial
{
    public class InitialInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Di.Container = Container;
            ConfigureGameStateMachine();
            BindServices();
        }

        private void BindServices()
        {
            Container.BindInterfacesTo<InitialEntryPoint>().AsSingle();
        }

        private void ConfigureGameStateMachine()
        {
            var gameStateMachine = ProjectContext.Instance.Container.Resolve<IGameStateMachine>();
            gameStateMachine.SetSceneContainer(Container);
        }
    }
}