using System;
using System.Collections;
using UnityEngine;

public class CashRegister : Workstation, IMinigame
{
    private Order orderSystem;

    public void Awake()
    {
        orderSystem = GetComponent<Order>();
    }
    public override void MinigameButton()
    {
        orderSystem.TakeOrder();
        Debug.Log("Order Taken");
    }
}
