using System;
using Core.Abstracts;
using Services.Data;
using Signals;
using UniRx;
using Views;
using Zenject;

namespace Controllers
{
    public class MainController : Controller<MainView>, IInitializable
    {
        private readonly IDataService _dataService;
        private readonly SignalBus _signalBus;

        public MainController
        (
            IDataService dataService,
            SignalBus signalBus
        )
        {
            _dataService = dataService;
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