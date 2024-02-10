using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour, IInteractable
{
    public void OnInteract() {
        throw new System.NotImplementedException();
    }
}
