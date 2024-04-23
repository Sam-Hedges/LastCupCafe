using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    public GameObject order1, order2, order3;

    int id = 1;

    public List<GameObject> slotsStart = new List<GameObject>();

    public static List<GameObject> slots = new List<GameObject>();

    public static int[] customerID;

    public static int[] drinkID;

    private void Start()
    {
        slots = new List<GameObject>(slotsStart);
        customerID = new int[5];
        drinkID = new int[5];
    }


    //generates an order by rolling a random number
    public void TakeOrder()
    {
        for (int i = 0; i < customerID.Length; i++)
        {
            if (customerID[i] != 0) return;
            int slot = i;

            int order = Random.Range(0, 90);
            if (order <= 30)
            {
                Instantiate(order1, slots[slot].transform, worldPositionStays: false);
                drinkID[slot] = 1;
            }
            else if (order <= 60)
            {
                Instantiate(order2, slots[slot].transform, worldPositionStays: false);
                drinkID[slot] = 2;
            }
            else
            {
                Instantiate(order3, slots[slot].transform, worldPositionStays: false);
                drinkID[slot] = 3;
            }

            customerID[slot] = id;
            id += 1;

            return;
        }
    }
}
