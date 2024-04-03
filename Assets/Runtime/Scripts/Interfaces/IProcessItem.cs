using UnityEngine;

public interface IProcessItem {
    bool CanProcessItem(Item item);
    void ProcessItem(Item item);
}