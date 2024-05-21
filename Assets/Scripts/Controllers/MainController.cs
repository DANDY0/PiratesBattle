using Core.Abstracts;
using Signals;
using UniRx;
using Views;
using Zenject;

namespace Controllers
{
    public class MainController : Controller<MainView>, IInitializable
    {
        private readonly SignalBus _signalBus;

        public MainController
        (
            SignalBus signalBus
        )
        {
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.GetStream<SignalMainSetup>().Subscribe(_ => Setup()).AddTo(View);
        }


        private void Setup()
        {
        }
    }
}