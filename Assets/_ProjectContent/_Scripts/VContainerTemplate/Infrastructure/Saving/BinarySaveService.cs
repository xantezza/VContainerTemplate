using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using VContainerTemplate.Infrastructure.Logger;

namespace VContainerTemplate.Infrastructure.Saving
{
    // used in production
    public class BinarySaveService : BaseSaveService
    {
        private static readonly byte[] Key = Convert.FromBase64String("c29tZUtleTExMjM0NTY3OA==");

        private const int IvLength = 16;

        protected virtual string _defaultFileName => "binaryDefaultSave";

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
                var path = $"{Application.persistentDataPath}/{fileName}.dat";

                if (!File.Exists(path))
                {
                    Log.Trace("No game data to load", LogTag.SaveService);
                    _hasLoaded = true;
                    return;
                }

                _cachedSaveFileName = fileName;

                var content = JsonConvert.DeserializeObject<Dictionary<SaveKey, object>>(Decrypt(File.ReadAllBytes(path)));

                _readyToSaveDictionary = content ?? new Dictionary<SaveKey, object>();

                _hasLoaded = true;
                Log.Trace("Game data loaded!", LogTag.SaveService);
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
                var path = $"{Application.persistentDataPath}/{fileName}.dat";
                tempPath = $"{path}.tmp";

                var encrypted = Encrypt(JsonConvert.SerializeObject(_readyToSaveDictionary));

                await File.WriteAllBytesAsync(tempPath, encrypted);
                File.Copy(tempPath, path, true);

                Log.Trace($"Game data saved! At path: \n{path}", LogTag.SaveService);
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

        private static byte[] Encrypt(string plainText)
        {
            var payload = Encoding.UTF8.GetBytes(plainText);
            var iv = new byte[IvLength];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(iv);
            }

            using var aes = Aes.Create();

            using var memoryStream = new MemoryStream();
            memoryStream.Write(iv, 0, IvLength);

            using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(Key, iv), CryptoStreamMode.Write))
            {
                cryptoStream.Write(payload, 0, payload.Length);
                cryptoStream.FlushFinalBlock();
            }

            return memoryStream.ToArray();
        }

        private static string Decrypt(byte[] data)
        {
            if (data == null || data.Length <= IvLength)
                throw new InvalidDataException("Save file is empty or corrupted");

            var iv = new byte[IvLength];
            Buffer.BlockCopy(data, 0, iv, 0, IvLength);

            using var aes = Aes.Create();

            using var source = new MemoryStream(data, IvLength, data.Length - IvLength);
            using var cryptoStream = new CryptoStream(source, aes.CreateDecryptor(Key, iv), CryptoStreamMode.Read);
            using var reader = new StreamReader(cryptoStream, Encoding.UTF8);

            var content = reader.ReadToEnd();
            if (string.IsNullOrEmpty(content))
                throw new InvalidDataException("Save file is corrupted");

            return content;
        }
    }
}