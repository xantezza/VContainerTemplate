using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using VContainerTemplate.Infrastructure.Logger;

namespace VContainerTemplate.Infrastructure.Saving
{
    // used in dev purposes
    public class JsonSaveService : BaseSaveService
    {
        protected virtual string _defaultFileName => "jsonDefaultSave";

        public override TSave Load<TSave>(IDataSaveable<TSave> dataSaveable)
        {
            if (_readyToSaveDictionary.TryGetValue(dataSaveable.SaveKey, out var value))
            {
                return JsonConvert.DeserializeObject<TSave>(JsonConvert.SerializeObject(value));
            }

            return null;
        }

        public override void LoadSaveFile(bool useDefaultFileName = true, string fileName = null)
        {
            if (useDefaultFileName) fileName = _defaultFileName;

            try
            {
                var path = $"{Application.persistentDataPath}/{fileName}.txt";

                if (!File.Exists(path))
                {
                    Log.Trace("No game data to load", LogTag.SaveService);
                    _hasLoaded = true;
                    return;
                }

                _cachedSaveFileName = fileName;

                var fileContent = File.ReadAllText(path);

                _readyToSaveDictionary = JsonConvert.DeserializeObject<Dictionary<SaveKey, object>>(fileContent) ?? new Dictionary<SaveKey, object>();

                _hasLoaded = true;
                Log.Trace($"Game data loaded! \n{fileContent}", LogTag.SaveService);
            }
            catch (Exception e)
            {
                Log.Error($"Exception caught when loading save!\n{e}", LogTag.SaveService);
                _hasLoaded = true;
            }
        }

        public override async UniTask StoreSaveFile(bool useDefaultFileName = true, string fileName = null)
        {
            if (useDefaultFileName || _cachedSaveFileName == null) fileName = _defaultFileName;
            else if (fileName == null && _cachedSaveFileName != null) fileName = _cachedSaveFileName;

            string tempPath = null;

            try
            {
                var path = $"{Application.persistentDataPath}/{fileName}.txt";
                tempPath = $"{path}.tmp";

                var serializedObject = JsonConvert.SerializeObject(_readyToSaveDictionary, Formatting.Indented);

                await File.WriteAllTextAsync(tempPath, serializedObject);
                File.Copy(tempPath, path, true);

                Log.Trace($"Game data saved! At path: \n{path} \nContent: \n{serializedObject}", LogTag.SaveService);
            }
            catch (Exception e)
            {
                Log.Error($"Exception caught when saving game data!\n{e}", LogTag.SaveService);
            }
            finally
            {
                if (File.Exists(tempPath)) File.Delete(tempPath);
            }
        }
    }
}