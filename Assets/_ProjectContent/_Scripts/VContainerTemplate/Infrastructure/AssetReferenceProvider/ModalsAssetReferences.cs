using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainerTemplate.Infrastructure.Logger;
using VContainerTemplate.UI.ModalPopups;

namespace VContainerTemplate.Infrastructure.AssetReferenceProvider
{
    public class ModalsAssetReferences : ScriptableObject
    {
        [SerializeField] private List<ModalReferenceEntry> modalReferences = new List<ModalReferenceEntry>();

        public AssetReferenceGameObject TypeToReference<T>() where T : ModalPopup
        {
            var type = typeof(T);
            foreach (var entry in modalReferences)
            {
                if (entry.Type == type)
                    return entry.AssetReference;
            }

            Log.Error($"In AssetReferenceProvider.ModalsAssetReferences not found reference to modal for type {type}", LogTag.AssetReferenceProvider);
            return null;
        }

        public void AddOrUpdate(Type type, AssetReferenceGameObject reference)
        {
            var entry = modalReferences.Find(e => e.Type == type);
            if (entry != null)
            {
                entry.SetAssetReference(reference);
            }
            else
            {
                var newEntry = new ModalReferenceEntry();
                newEntry.SetType(type);
                newEntry.SetAssetReference(reference);
                modalReferences.Add(newEntry);
            }
        }
    }
}