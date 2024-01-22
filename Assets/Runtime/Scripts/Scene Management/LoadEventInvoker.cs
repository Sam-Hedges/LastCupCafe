using UnityEngine;

public class LoadEventInvoker : MonoBehaviour
{
    public LoadEventChannelSO loadEventChannel;

    public void InvokeLoadEvent(GameSceneSO sceneToLoad)
    {
        if (loadEventChannel != null)
        {
            loadEventChannel.RaiseEvent(sceneToLoad);
        }
        else
        {
            Debug.LogError("LoadEventChannelSO is not assigned!");
        }
    }
}