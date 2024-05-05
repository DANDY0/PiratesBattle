using Signals;
using Zenject;

namespace Services
{
    public class GameStarter : IInitializable
    {
        private readonly SignalBus _signalBus;

        public GameStarter
        (
            SignalBus signalBus
        )
        {
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            SetupForContinuingGame();
        }

        private void SetupForContinuingGame()
        {
            _signalBus.Fire<SignalMainSetup>();
        }
    }
}