using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;


public class LevelManager : MonoBehaviour
{
    // Prevents Multiple Instances of a LevelManager that could cause loading issues
    public static LevelManager Instance;
    public LevelManager() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }
    
    private List<AsyncOperation> SceneQueue;
    
    public void LoadScene(string sceneName) {

        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
    }
    
    // Loads a scene in and adds it to a list to be queued to load later
    // Useful for loading bars, etc
    public void LoadSceneQueue(string sceneName) {
        
        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;
        
        SceneQueue.Add(scene);
    }
    
    // Activates the next queued scene and removes it from the queue
    public void ActivateQueuedScene(string sceneName) {
        
        SceneQueue[0].allowSceneActivation = true;
        SceneQueue.RemoveAt(0);
    }
    
}