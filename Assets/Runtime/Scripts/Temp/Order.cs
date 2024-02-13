using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

public class Order : MonoBehaviour
{
    public GameObject order1, order2;

    bool active1, active2, active3, active4, active5 = false;

    int id = 1;

    int[] orderList;

    public List<GameObject> slots = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        orderList = new int[5];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            Debug.Log("Part 1");
            CheckAvailableSlots();
        }
    }

    void CheckAvailableSlots()
    {
        Debug.Log("Part 2");
        for (int i = 0; i < orderList.Length; i++)
        {
            int check = orderList[i];
            Debug.Log(check);
            if (check == 0)
            {
                TakeOrder(i);
                Debug.Log("Part 3");
                break;
            }
        }
    }

    void TakeOrder(int slot)
    {
        Debug.Log("Part 4");
        int order = Random.Range(0, 50);
        if (order <= 25)
        {
            Instantiate(order1, slots[slot].transform, worldPositionStays: false);
        }
        else
        {
            Instantiate(order2, slots[slot].transform, worldPositionStays: false);
        }
        orderList[slot] = id;
        id += 1;
    }
}
