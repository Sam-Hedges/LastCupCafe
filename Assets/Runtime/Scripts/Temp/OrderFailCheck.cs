using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderFailCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //for (int i = 0; i < Order.customerID.Length; i++)
        //{
        //    Debug.Log(i);
        //    if (Order.customerID[i] != 0)
        //    {
        //        Debug.Log("Check 2");
        //        if (Order.slots[i].GetComponentInChildren<Timer>().fail == true)
        //        {
        //            Debug.Log("Order failed");
        //            Order.drinkID[i] = 0;
        //            Order.customerID[i] = 0;
        //            Destroy(Order.slots[i].transform.GetChild(0).gameObject);
        //            //Serve.serveID = 0;
        //        }
        //    }
        //}

        for (int i = 0; i < Order.customerID.Length; i++)
        {
            if (Order.drinkID[i] == Serve.serveID)
            {
                Debug.Log("Customer served");
                Order.drinkID[i] = 0;
                Order.customerID[i] = 0;
                Destroy(Order.slots[i].transform.GetChild(0).gameObject);
                Serve.serveID = 0;
                break;
            }
        }
    }
}
