using System;
using UnityEngine;
using VContainer;

namespace VContainerTemplate.Infrastructure.Saving
{
    
    public class DataSaveableTemplate : IDataSaveable<DataSaveableTemplate.Save>
    {
        private ISaveService _saveService;

        [Serializable]
        public class Save
        {
            public bool SomeField = false;
        }

        public SaveKey SaveKey => SaveKey.PrivacyPolicyState;
        public Save SaveData { get; private set; }

        //use of save service from [Inject] is prohibited!
        [Inject]
        private void Inject(ISaveService saveService)
        {
            _saveService = saveService;
        }
        
        //if class exists in RootLifetimeScope this method must be called in InitializeSaveServiceState.cs:InitializeServices()
        public void LoadSave()
        {
            SaveData = _saveService.Load(this) ?? new Save();
            _saveService.AddToSaveables(this);
        }
    }
}