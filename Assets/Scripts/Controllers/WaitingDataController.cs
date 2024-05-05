using System;
using Core.Abstracts;
using DG.Tweening;
using Services.Data;
using Services.SceneLoading;
using Utils;
using Views;
using Zenject;

namespace Controllers
{
    public class WaitingDataController : Controller<WaitingDataView>, IInitializable, IDisposable
    {
        private readonly ISceneLoadingService _sceneLoadingService;
        private readonly IDataService _dataService;
        private Tween _loadingTween;

        public WaitingDataController
        (
            ISceneLoadingService sceneLoadingService,
            IDataService dataService
        )
        {
            _sceneLoadingService = sceneLoadingService;
            _dataService = dataService;
        }

        public void Initialize()
        {
            View.Show();
            _loadingTween = View.StartLoadingAnimation();
            if (_dataService.DataIsLoaded)
                OnDataLoaded();
            else
                _dataService.DataLoadedEvent += OnDataLoaded;
        }
        
        public void Dispose()
        {
            _loadingTween.Kill();
            _loadingTween = null;
            _dataService.DataLoadedEvent -= OnDataLoaded;
        }

        private void OnDataLoaded()
        {
            _sceneLoadingService.LoadScene(SceneNames.Menu);
        }
    }
}