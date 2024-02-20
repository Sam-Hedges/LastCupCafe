using UnityEngine;

public interface ICombineItem {
    bool CanCombineItem(GameObject item);
    void CombineItem(GameObject item);
}