using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Serve : MonoBehaviour
{
    public static int serveID = 0; //refer to this when serving customer in other script

    //public Timer timer;

    void Start()
    {
        //CheckTimer();
    }

    void Update()
    {
        if (serveID == 1)
        {
            for (int i = 0; i < Order.customerID.Length; i++)
            {
                if (Order.drinkID[i] == 1)
                {
                    Debug.Log("Customer served");
                    Order.drinkID[i] = 0;
                    Order.customerID[i] = 0;
                    Destroy(Order.slots[i].transform.GetChild(0).gameObject);
                    serveID = 0;
                    break;
                }
            }
        }
        else if (serveID == 2)
        {
            for (int i = 0; i < Order.customerID.Length; i++)
            {
                if (Order.drinkID[i] == 2)
                {
                    Debug.Log("Customer served");
                    Order.drinkID[i] = 0;
                    Order.customerID[i] = 0;
                    Destroy(Order.slots[i].transform.GetChild(0).gameObject);
                    serveID = 0;
                    break;
                }
            }
        }
        //CheckTimer();
    }

    //void CheckTimer() //DO NOT USE
    //{
    //    for (int i = 0; i < Order.customerID.Length; i++)
    //    {
    //        if (Order.slots[i].transform.GetChild(0).gameObject != null)
    //        {
    //            if (Order.slots[i].GetComponentInChildren<Timer>().fail == true)
    //            {
    //                Debug.Log("Order Failed");
    //                Order.drinkID[i] = 0;
    //                Order.customerID[i] = 0;
    //                Destroy(Order.slots[i].transform.GetChild(0).gameObject);
    //                serveID = 0;
    //                break;
    //            }
    //        }
    //    }
    //}
}
