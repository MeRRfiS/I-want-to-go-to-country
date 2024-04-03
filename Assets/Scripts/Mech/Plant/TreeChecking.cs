using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeChecking : MonoBehaviour
{
    private bool isInObject = false;
    private bool isNearTree = false;

    public LayerMask layerMask;

    public bool IsOnObject
    {
        get => isInObject;
    }

    public bool IsNearTree
    {
        get => isNearTree;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == TagConstants.EARTH) return;
        if (other.tag == TagConstants.PLAYER) return;
        if (other.tag == TagConstants.ITEM) return;

        Debug.Log(other.gameObject);

        isInObject = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isInObject = false;
    }

    private void Update()
    {
        CheckingTrees();
    }

    private void OnDrawGizmosSelected()
    {
        ApplyDebugSphere();
    }

    private void ApplyDebugSphere()
    {
        float distance = MechConstants.MAX_DISTANCE_FOR_USING_ITEM;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, distance);
    }

    private void CheckingTrees()
    {
        RaycastHit[] hits;
        hits = Physics.SphereCastAll(transform.position,
                                     MechConstants.MAX_DISTANCE_FOR_USING_ITEM,
                                     transform.position,
                                     0, 
                                     layerMask);

        if (hits.Length > 0)
        {
            isNearTree = true;
        }
        else
        {
            isNearTree = false;
        }
    }
}
