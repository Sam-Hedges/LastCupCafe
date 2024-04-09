using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serve : MonoBehaviour
{
    public static int serveID = -1; //refer to this when serving customer in other script

    public GameObject MainTimer;

    public float newTime;

    //public Timer timer;

    void Start()
    {
        newTime = MainTimer.GetComponent<Timer>().timer;
    }

    void Update()
    {
        newTime = MainTimer.GetComponent<Timer>().timer;

        for (int i = 0; i < Order.customerID.Length; i++)
        {
            if (Order.drinkID[i] == serveID)
            {
                Debug.Log("Customer served");
                Order.drinkID[i] = 0;
                Order.customerID[i] = 0;
                Destroy(Order.slots[i].transform.GetChild(0).gameObject);
                serveID = -1;
                break;
            }
        }

        for (int i = 0; i < Order.customerID.Length; i++)
        {
            if (Order.customerID[i] != 0)
            {
                if (Order.slots[i].GetComponentInChildren<Timer>().fail == true)
                {
                    Debug.Log("Order failed");
                    Order.drinkID[i] = 0;
                    Order.customerID[i] = 0;
                    Destroy(Order.slots[i].transform.GetChild(0).gameObject);
                    serveID = -1;
                    newTime = MainTimer.GetComponent<Timer>().timer - 20f;
                }
            }
        }

    }
}
