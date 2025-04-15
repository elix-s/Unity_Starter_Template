using Cysharp.Threading.Tasks;
using System; 
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    private readonly Logger _logger;

    public SceneLoader(Logger logger)
    {
        _logger = logger;
    }
    
    public async UniTask<bool> LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool useAsync = true)
    {
        _logger.Log($"Attempting to load scene '{sceneName}' with mode '{mode}'. Async: {useAsync}");

        try
        {
            if (useAsync)
            {
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);

                if (asyncOperation == null)
                {
                    _logger.LogError($"Failed to start loading scene '{sceneName}'. LoadSceneAsync returned null.");
                    return false;
                }
                
                await asyncOperation.ToUniTask(Progress.Create<float>(p =>
                {
                    float displayProgress = Mathf.Clamp01(p / 0.9f);
                    _logger.Log($"Scene loading '{sceneName}': {(displayProgress * 100f):F1}%");
                }));
            }
            else
            {
                SceneManager.LoadScene(sceneName, mode);
            }

            _logger.Log($"Scene '{sceneName}' loaded successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to load scene '{sceneName}'. Error: {ex.Message}\n{ex.StackTrace}");
            return false;
        }
    }
    
    public async UniTask<bool> UnloadSceneAsync(string sceneName)
    {
        _logger.Log($"Attempting to unload scene '{sceneName}'.");
        
        try
        {
            Scene sceneToUnload = SceneManager.GetSceneByName(sceneName);
            
            if (!sceneToUnload.IsValid() || !sceneToUnload.isLoaded)
            {
                _logger.LogWarning($"Scene '{sceneName}' not found or not loaded. Cannot unload.");
                return false; 
            }

            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
            
            if (asyncOperation == null)
            {
                _logger.LogError($"Failed to start unloading scene '{sceneName}'. UnloadSceneAsync returned null.");
                return false;
            }

            await asyncOperation.ToUniTask(); 

            _logger.Log($"Scene '{sceneName}' unloaded successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to unload scene '{sceneName}'. Error: {ex.Message}\n{ex.StackTrace}");
            return false;
        }
    }
}
