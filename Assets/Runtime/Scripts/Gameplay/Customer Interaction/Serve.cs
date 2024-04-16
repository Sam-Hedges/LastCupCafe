using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Needs some type of priority system so that orders with the lowest time are completed first

public class Serve : Workstation
{
    int serveID = -1;

    public GameObject MainTimer;

    public float newTime;

    void Start()
    {
        newTime = MainTimer.GetComponent<Timer>().timer;
    }

    void Update()
    {
        newTime = MainTimer.GetComponent<Timer>().timer;

        if(currentlyStoredItem.TryGetComponent(out Mug mug))       //if currently place item is a mug
        {                                                          //
            serveID = currentlyStoredItem.GetComponent<Mug>().id;  //gets its id to compare to orders
        }

        for (int i = 0; i < Order.customerID.Length; i++) //checks for any orders matching the drinks id and removes the first one in the list
        {
            if (Order.drinkID[i] == serveID)
            {
                //Debug.Log("Customer served");
                Order.drinkID[i] = 0;
                Order.customerID[i] = 0;
                Destroy(Order.slots[i].transform.GetChild(0).gameObject);
                Destroy(currentlyStoredItem.gameObject);
                currentlyStoredItem = null;
                serveID = -1;
                break;
            }
        }

        for (int i = 0; i < Order.customerID.Length; i++) //Removes time from main timer when an order runs out of time
        {
            if (Order.customerID[i] != 0)
            {
                if (Order.slots[i].GetComponentInChildren<Timer>().fail == true)
                {
                    //Debug.Log("Order failed");
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
