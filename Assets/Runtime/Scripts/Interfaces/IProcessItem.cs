using UnityEngine;

public interface IProcessItem {
    bool CanProcessItem(GameObject item);
    void ProcessItem(GameObject item);
}