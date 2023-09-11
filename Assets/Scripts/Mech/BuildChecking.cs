using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildChecking : MonoBehaviour
{
    private bool isInObject = false; 
    private bool isNearObject = false;

    public bool IsOnObject
    {
        get => isInObject;
    }

    public bool IsNearObject
    {
        get => isNearObject;
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
}
