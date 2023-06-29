using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patch : MonoBehaviour
{
    private bool isInObject = false;

    public bool IsOnObject
    {
        get => isInObject;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == TagConstants.EARTH) return;

        isInObject = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isInObject = false;
    }

    //private void OnCollisionStay(Collision collision)
    //{
    //    Debug.Log("Can't place");
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    Debug.Log("Can place");
    //}
}
