using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class SavingSystem
{
    /// <summary>
    /// Loads data from a JSON file located at Application.persistentDataPath.
    /// If the file does not exist, returns a new instance of type T.
    /// </summary>
    /// <typeparam name="T">The type of data to load (eg AppData)</typeparam>
    /// <returns>An instance of data type T, filled from a file or a new instance</returns>
    public T LoadData<T>()
    {
        string fileName = typeof(T).Name + ".json";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        
        if (!File.Exists(filePath))
        {
            Debug.Log($"File {filePath} not found. Creating new data instance.");
            return Activator.CreateInstance<T>();
        }

        try
        {
            string json = File.ReadAllText(filePath);
            T data = JsonConvert.DeserializeObject<T>(json);
            return data;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading data from {filePath}: {ex.Message}");
            return Activator.CreateInstance<T>();
        }
    }
    
    public async UniTask<T> LoadDataAsync<T>()
    {
        string fileName = typeof(T).Name + ".json";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        if (!File.Exists(filePath))
        {
            Debug.Log($"File {filePath} not found. Creating new data instance.");
            return Activator.CreateInstance<T>();
        }
        
        try
        {
            string json = await UniTask.Run(() => File.ReadAllText(filePath));
            T data = JsonConvert.DeserializeObject<T>(json);
            return data;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading data from {filePath}: {ex.Message}");
            return Activator.CreateInstance<T>();
        }
    }

    /// <summary>
    /// Serializes data to JSON and saves it to a file in Application.persistentDataPath.
    /// </summary>
    /// <typeparam name="T">The type of data being transferred (e.g. AppData)</typeparam>
    /// <param name="data">The data instance to be saved</param>
    public void SaveData<T>(T data)
    {
        string fileName = typeof(T).Name + ".json";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, json);
            Debug.Log($"The data has been successfully saved to {filePath}.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving data to {filePath}: {ex.Message}");
        }
    }
    
    public async UniTask SaveDataAsync<T>(T data)
    {
        string fileName = typeof(T).Name + ".json";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            await UniTask.Run(() => File.WriteAllText(filePath, json));
            Debug.Log($"The data has been successfully saved to {filePath}.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving data to {filePath}: {ex.Message}");
        }
    }
    
    public T ClearData<T>()
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
       
        return Activator.CreateInstance<T>();
    }
}
