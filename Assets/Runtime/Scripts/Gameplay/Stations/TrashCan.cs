using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class TrashCan : Workstation
{
    private void Update()
    {
        if (currentlyStoredItem)
        {
            Destroy(currentlyStoredItem.gameObject);
            currentlyStoredItem = null;
        }
    }
}
