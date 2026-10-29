using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;
using VContainerTemplate.Infrastructure.Logger;

namespace VContainerTemplate.Infrastructure.Saving
{
    public abstract class BaseSaveService : ISaveService, IInitializable, IDisposable
    {
        protected Dictionary<SaveKey, object> _readyToSaveDictionary = new();

        protected string _cachedSaveFileName;
        protected bool _hasLoaded;

        private bool _isSaving;
        private bool _saveRequestedWhileSaving;

        public abstract TSave Load<TSave>(IDataSaveable<TSave> dataSaveable) where TSave : class;

        public void AddToSaveables<TSave>(IDataSaveable<TSave> dataSaveable) where TSave : class
        {
            if (_readyToSaveDictionary.ContainsKey(dataSaveable.SaveKey))
            {
                _readyToSaveDictionary[dataSaveable.SaveKey] = dataSaveable.SaveData;
                return;
            }

            _readyToSaveDictionary.Add(dataSaveable.SaveKey, dataSaveable.SaveData);
        }

        public abstract void LoadSaveFile(bool useDefaultFileName = true, string fileName = null);

        public abstract UniTask StoreSaveFile(bool useDefaultFileName = true, string fileName = null);

        public void Initialize()
        {
            Application.focusChanged += OnApplicationFocus;
        }

        public void Dispose()
        {
            Application.focusChanged -= OnApplicationFocus;
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus) return;
            if (!Application.isPlaying) return;
            if (_readyToSaveDictionary.Count == 0) return;
            if (!_hasLoaded) return;

            SaveOnFocusLost().Forget();
        }

        private async UniTaskVoid SaveOnFocusLost()
        {
            if (_isSaving)
            {
                _saveRequestedWhileSaving = true;
                return;
            }

            _isSaving = true;
            try
            {
                do
                {
                    _saveRequestedWhileSaving = false;
                    await StoreSaveFile(_cachedSaveFileName != null, _cachedSaveFileName);
                } while (_saveRequestedWhileSaving);
            }
            catch (Exception e)
            {
                Log.Error($"Exception caught when saving game data on focus lost!\n{e}", LogTag.SaveService);
            }
            finally
            {
                _isSaving = false;
            }
        }
    }
}