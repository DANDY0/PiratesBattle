using System;
using System.Collections.Generic;
using System.IO;
using Databases;
using Databases.Impls;
using Enums;
using Models;
using Models.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Services.PhotonNetwork;
using Services.Spreadsheets;
using UnityEngine;
using Utils;
using Zenject;

namespace Services.Data
{
    public class DataService : IInitializable, IDisposable, IDataService
    {
        private readonly ISpreadsheetsService _spreadsheetsService;
        private readonly ISpreadsheetsSettingsDatabase _spreadsheetsSettingsDatabase;
        private readonly IDataManagementDatabase _dataManagementDatabase;
        private readonly IPunNetworkService _punNetworkService;

        private const string StaticDataPrivateKey = "FPXrKfU2RxYddZpQdJb2VjDW2DwAzReCtedVhvfUQvcALCJf44apEg2BfadsE7RyffBwnGSE";
        
        public event Action DataLoadedEvent;

        public CachedUserData CachedUserLocalData { get; private set; }


        private readonly JsonSerializerSettings _jsonSerializerSettings = new()
        {
            Converters =
            {
                new StringEnumConverter()
            }
        };

        private Dictionary<EGameDataType, string> _gameDataPathes;

        public bool DataIsLoaded { get; private set; }


        public DataService
        (
            ISpreadsheetsService spreadsheetsService,
            ISpreadsheetsSettingsDatabase spreadsheetsSettingsDatabase,
            IDataManagementDatabase dataManagementDatabase,
            IPunNetworkService punNetworkService
        )
        {
            _spreadsheetsService = spreadsheetsService;
            _spreadsheetsSettingsDatabase = spreadsheetsSettingsDatabase;
            _dataManagementDatabase = dataManagementDatabase;
            _punNetworkService = punNetworkService;
        }

        public void Initialize()
        {
            _gameDataPathes = new Dictionary<EGameDataType, string>
            {
                { EGameDataType.UserData, $"{Application.persistentDataPath}/{EGameDataType.UserData}.dat" }
            };
            LoadConfigurationData();
        }

        public void Dispose()
        {
            if (_dataManagementDatabase.Settings.IsDataSave)
                SaveAllData();
        }

        private void LoadConfigurationData()
        {
            _spreadsheetsService.FillSpreadsheetsInfo();

            LoadAllData();
        }

        private void SaveAllData()
        {
            foreach (var key in _gameDataPathes.Keys)
                SaveData(key);
        }

        private async void LoadAllData()
        {
            foreach (var key in _gameDataPathes.Keys)
                LoadData(key);

            if (_spreadsheetsSettingsDatabase.Settings.RefreshSpreadsheets)
                await _spreadsheetsService.StartLoadSpreadsheetsData();

            DataIsLoaded = true;
            if (_spreadsheetsSettingsDatabase.Settings.RefreshSpreadsheets)
                _spreadsheetsService.SetupDatabases();

            DataLoadedEvent?.Invoke();
        }

        private void SaveData(EGameDataType gameDataType)
        {
            var data = string.Empty;
            var dataPath = _gameDataPathes[gameDataType];

            data = gameDataType switch
            {
                EGameDataType.UserData => Serialize(CachedUserLocalData),
                _ => data
            };

            if (data.Length <= 0) return;
#if UNITY_WEBGL && !UNITY_EDITOR
                PlayerPrefs.SetString(gameDataType.ToString(), data);
                PlayerPrefs.Save();
#else
            if (!File.Exists(dataPath)) File.Create(dataPath).Close();

            File.WriteAllText(dataPath, data);
#endif
        }

        private void LoadData(EGameDataType gameDataType)
        {
            var dataPath = _gameDataPathes[gameDataType];

            switch (gameDataType)
            {
                case EGameDataType.UserData:
                    CachedUserLocalData = DeserializeFromPath<CachedUserData>(dataPath, gameDataType);
                    if (CachedUserLocalData == null)
                    {
                        CachedUserLocalData = new CachedUserData
                        {
                        };

                        SaveData(EGameDataType.UserData);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameDataType), gameDataType, null);
            }
        }

        private T DeserializeFromPath<T>(string path, EGameDataType type) where T : class
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (!PlayerPrefs.HasKey(type.ToString()))
                return null;

            return JsonConvert.DeserializeObject<T>(Decrypt(PlayerPrefs.GetString(type.ToString())), JsonSerializerSettings);
#else
            return !File.Exists(path)
                ? null
                : JsonConvert.DeserializeObject<T>(Decrypt(File.ReadAllText(path)), _jsonSerializerSettings);

#endif
        }

        private string Serialize(object @object, Formatting formatting = Formatting.Indented) =>
            Encrypt(JsonConvert.SerializeObject(@object, formatting));


        private string Decrypt(string data) =>
            _dataManagementDatabase.Settings.IsDataEncrypted ? CryptographyUtils.Decrypt(data, StaticDataPrivateKey) : data;

        private string Encrypt(string data) =>
            _dataManagementDatabase.Settings.IsDataEncrypted ? CryptographyUtils.Encrypt(data, StaticDataPrivateKey) : data;
    }
}