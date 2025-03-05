using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    private readonly Logger _logger;

    public SceneLoader(Logger logger)
    {
        _logger = logger;
    }
    
    /// <param name="sceneName">Scene name</param>
    /// <param name="mode">Loading Mode (Single или Additive)</param>
    /// <param name="useAsync">Optional parameter: true - asynchronous loading, false - synchronous</param>
    public async UniTask LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool useAsync = true)
    {
        _logger.Log($"Start loading scene '{sceneName}' with mode '{mode}'. Asynchronous loading: {{useAsync}}");

        if (useAsync)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
            asyncOperation.allowSceneActivation = true;
            
            while (!asyncOperation.isDone)
            {
                float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
                _logger.Log($"Scene loading '{sceneName}': {(progress * 100f):F1}%");
                await UniTask.Yield(); 
            }
        }
        else
        {
            SceneManager.LoadScene(sceneName, mode);
        }

        _logger.Log($"Scene '{sceneName}' successfully uploaded.");
    }
}