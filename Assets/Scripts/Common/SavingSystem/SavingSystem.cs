using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Common.SavingSystem
{
    public class SavingSystem
    {
        public async UniTask<T> LoadDataAsync<T>(DataMigrator<T> migrator = null)
            where T : IVersionedData, new()
        {
            string fileName = typeof(T).Name + ".json";
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            if (!File.Exists(filePath))
            {
                Debug.Log($"File {filePath} not found. Creating new data instance.");
                return new T();
            }

            try
            {
                string encoded = await UniTask.RunOnThreadPool(() => File.ReadAllText(filePath));
                byte[] encryptedBytes = Convert.FromBase64String(encoded);

                const byte encryptionKey = 0xAF;
                byte[] jsonBytes = Xor(encryptedBytes, encryptionKey);

                string json = System.Text.Encoding.UTF8.GetString(jsonBytes);

                var jo = JObject.Parse(json);
                int fileVersion = jo["Version"]?.Value<int>() ?? 0;

                T currentInstance = new T();
                int currentVersion = currentInstance.Version;

                if (fileVersion != currentVersion)
                {
                    Debug.Log($"Data version mismatch: saved={fileVersion}, current={currentVersion}. Migration required.");

                    if (migrator == null)
                    {
                        Debug.LogWarning($"No migrator provided for type {typeof(T).Name}. Returning default.");
                        return new T();
                    }
                    else
                    {
                        return migrator.Migrate(json, fileVersion, currentVersion);
                    }
                }

                T data = JsonConvert.DeserializeObject<T>(json);
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading data from {filePath}: {ex.Message}");
                return new T();
            }
        }
        
        public async UniTask SaveDataAsync<T>(T data) where T : IVersionedData
        {
            string fileName = typeof(T).Name + ".json";
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

                const byte encryptionKey = 0xAF;
                byte[] encryptedBytes = Xor(jsonBytes, encryptionKey);

                string encoded = Convert.ToBase64String(encryptedBytes);
                await UniTask.RunOnThreadPool(() => File.WriteAllText(filePath, encoded));
                Debug.Log($"The data has been successfully saved to {filePath}.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error saving data to {filePath}: {ex.Message}");
            }
        }
        
        public void ClearData<T>()
        {
            string fileName = typeof(T).Name + ".json";
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    Debug.Log($"Saved file {filePath} deleted.");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error deleting file {filePath}: {ex.Message}");
                }
            }
            else
            {
                Debug.Log($"File {filePath} not found.");
            }
        }
        
        private static byte[] Xor(byte[] data, byte key)
        {
            byte[] result = new byte[data.Length];
            
            for (int i = 0; i < data.Length; i++)
                result[i] = (byte)(data[i] ^ key);
            
            return result;
        }
    }
}
