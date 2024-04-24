using System;
using System.Collections;
using UnityEngine;

public class CashRegister : Workstation, IMinigame
{
    [SerializeField] private VoidEventChannelSO registerOrderChannel;
    [SerializeField] private GameObjectEventChannelSO queryFufillmentOrderChannel;

    public override void MinigameButton()
    {
        registerOrderChannel.RaiseEvent();
        Debug.Log("Order Taken");
    }

    public override bool OnPlaceItem(GameObject newItem)
    {
        if (newItem.GetComponent<Mug>())
        {
            queryFufillmentOrderChannel.RaiseEvent(newItem);
            Destroy(newItem);
            return true;
        }

        return false;
    }
}