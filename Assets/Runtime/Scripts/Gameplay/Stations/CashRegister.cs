using System;
using System.Collections;
using UnityEngine;

public class CashRegister : Workstation, IMinigame
{
    Order orderSystem;

    public void Awake()
    {
        orderSystem = GetComponent<Order>();
    }
    public override void MinigameButton()
        {
        orderSystem.CheckAvailableSlots();
        Debug.Log("Order Taken");
    }
}
