using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Serialization;
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

    public void DestroyChecker()
    {
        Destroy(this);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == TagConstants.EARTH) return;
        if (other.tag == TagConstants.PLAYER) return;

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
        float distance = MechConstants.DISTANCE_TO_TREE;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, distance);
    }

    private void CheckingTrees()
    {
        RaycastHit[] hits;
        hits = Physics.SphereCastAll(transform.position,
                                     MechConstants.DISTANCE_TO_TREE,
                                     transform.position,
                                     0, 
                                     layerMask);

        if (hits.Length > 0)
        {
            Debug.Log("I found tree!");
            isNearTree = true;
        }
        else
        {
            isNearTree = false;
        }
        //RaycastHit hitVer = GetHit(transform.forward);
        //RaycastHit hitHor = GetHit(transform.right);
        //if (hitVer.collider != null || hitHor.collider != null)
        //{
        //    isNearTree = true;
        //}
        //else
        //{
        //    isNearTree = false;
        //}
    }
}
