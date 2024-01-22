using UnityEngine.Pool;
using UnityEngine;
using UnityEngine.Serialization;

public class GameObjectPoolManager : MonoBehaviour
{ 
    [SerializeField] private GameObject prefab;
    [SerializeField] private int defaultCapacity = 100;
    [SerializeField] private int maxCapacity = 1000;
    
    private ObjectPool<GameObject> pool;
    
    private void Start()
    {
        pool = new ObjectPool<GameObject>(() => { return Instantiate(prefab); }, 
            obj => { obj.SetActive(true); }, 
            obj => { obj.SetActive(false); },
            Destroy, false,
            defaultCapacity, maxCapacity);
    }
    
    public GameObject GetGameObject()
    {
        return pool.Get();
    }

    public void DisableGameObject(GameObject obj)
    {
        pool.Release(obj);
    }
}