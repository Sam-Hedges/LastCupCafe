using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;

public class RiggingHandler : MonoBehaviour
{
    [Header("Arm Rigging")]
    [SerializeField] private Rig playerArmsRig;
    [SerializeField] private float pickupAnimDuration = 0.25f;

    private void Awake() {
        playerArmsRig = GetComponentInChildren<Rig>();
        playerArmsRig.weight = 0;
    }

    public void PickupItem() {
        DOTween.To(() => playerArmsRig.weight, x => playerArmsRig.weight = x, 1f, pickupAnimDuration);
    }
    public void DropItem() {
        DOTween.To(() => playerArmsRig.weight, x => playerArmsRig.weight = x, 0, pickupAnimDuration);
    }
}
