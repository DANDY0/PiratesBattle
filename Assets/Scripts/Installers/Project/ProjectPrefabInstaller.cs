using Behaviours;
using Databases;
using Databases.Impls;
using Helpers;
using Photon.Pun;
using Services.PunNetwork;
using UnityEngine;
using Utils.Extensions;
using Zenject;

namespace Installers.Project
{
    [CreateAssetMenu(menuName = "Installers/ProjectPrefabInstaller", fileName = "ProjectPrefabInstaller")]
    public class ProjectPrefabInstaller : ScriptableObjectInstaller
    {
        [Header("Databases")] 
        [SerializeField] private LocalizationDatabase _localizationDatabase;
        [SerializeField] private SpreadsheetsSettingsDatabase _spreadsheetsSettingsDatabase;
        [SerializeField] private SoundsDatabase _soundsDatabase;
        [SerializeField] private SettingsDatabase _settingsDatabase;
        [SerializeField] private DataManagementDatabase _dataManagementDatabase;

        [Header("Common")] 
        [SerializeField] private PunNetworkService _punNetworkService;
        [SerializeField] private CoroutineRunner _coroutineRunner;
        [SerializeField] private AudioMixerProvider _audioMixerProvider;
        [SerializeField] private GameRestarter _gameRestarter;

        public override void InstallBindings()
        {
            BindDatabases();
            BindPrefabs();
        }

        private void BindPrefabs()  
        {
            Container.BindPrefab(_coroutineRunner);
            Container.BindPrefab(_audioMixerProvider);
            Container.BindPrefab(_gameRestarter);
        }

        private void BindDatabases()
        {
            Container.Bind<ILocalizationDatabase>().FromInstance(_localizationDatabase).AsSingle();
            
            Container.Bind<ISpreadsheetsSettingsDatabase>().FromInstance(_spreadsheetsSettingsDatabase).AsSingle();
            Container.Bind<ISoundsDatabase>().FromInstance(_soundsDatabase).AsSingle();
            Container.Bind<ISettingsDatabase>().FromInstance(_settingsDatabase).AsSingle();

            Container.Bind<IDataManagementDatabase>().FromInstance(_dataManagementDatabase).AsSingle();
        }
    }
}