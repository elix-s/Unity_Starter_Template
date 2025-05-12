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
        //do not store the encryption key in plaintext in the real application
        private static readonly byte[] _encryptionKey = { 0xAF, 0x12, 0x34, 0xCD, 0xEF, 0x56, 0x78, 0x90 };
        
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
                
                byte[] jsonBytes = XorWithKey(encryptedBytes, _encryptionKey);
                string json = System.Text.Encoding.UTF8.GetString(jsonBytes);
                
                int fileVersion; 

                try 
                {
                    var jo = JObject.Parse(json);
                    
                    if (jo.TryGetValue("Version", StringComparison.OrdinalIgnoreCase, out JToken versionToken) && versionToken.Type == JTokenType.Integer)
                    {
                        fileVersion = versionToken.Value<int>();
                    }
                    else
                    {
                        Debug.LogError($"Invalid or missing 'Version' field in file {filePath}. Assuming data is corrupted or incompatible.");
                        return new T();
                    }
                }
                catch (JsonReaderException jsonEx) 
                {
                    Debug.LogError($"Error parsing JSON from file {filePath}: {jsonEx.Message}. File might be corrupted.");
                    return new T(); 
                }
                catch (Exception ex) 
                {
                    Debug.LogError($"Error processing version from {filePath}: {ex.Message}");
                    return new T();
                }

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
                    
                    return migrator.Migrate(json, fileVersion, currentVersion);
                }
                
                try
                {
                    T data = JsonConvert.DeserializeObject<T>(json);
                    return data;
                }
                catch(Exception ex)
                {
                    Debug.LogError($"Error deserializing data object from {filePath}: {ex.Message}");
                    return new T(); 
                }
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
                
                byte[] encryptedBytes = XorWithKey(jsonBytes, _encryptionKey);

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
        
        private static byte[] XorWithKey(byte[] data, byte[] key)
        {
            byte[] result = new byte[data.Length];
            
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)(data[i] ^ key[i % key.Length]); 
            }
            
            return result;
        }
    }
}
