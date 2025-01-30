using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    private readonly Logger _logger;

    public SceneLoader(Logger logger)
    {
        _logger = logger;
    }
    
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        _logger.Log("Loading scene " + sceneName);
    }
}