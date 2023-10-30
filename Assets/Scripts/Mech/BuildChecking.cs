using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildChecking : MonoBehaviour
{
    private bool isInObject = false; 
    private bool isNearObject = false;
    private bool isHorFasten = false;
    private bool isVerFasten = false;
    private float fastenXPos = 0.0f;
    private float fastenZPos = 0.0f;
    private RaycastHit hitHor;
    private RaycastHit hitVer;

    public bool IsOnObject
    {
        get => isInObject;
    }

    public bool IsHorFasten
    {
        get => isHorFasten;
    }

    public bool IsVerFasten
    {
        get => isVerFasten;
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

    private bool CheckingHorizontal()
    {
        if (isVerFasten)
        {
            if (hitHor.collider == hitVer.collider) return false;
        }

        hitHor = GetHit(transform.right);
        if (hitHor.collider != null)
        {
            fastenZPos = hitHor.collider.transform.position.z;
            return true;
        }
        else
        {
            return false;
        }
    }
    public Vector3 FastenPos
    {
        get => new Vector3(fastenXPos, 0, fastenZPos);
    }

    private RaycastHit GetHit(Vector3 side)
    {
        RaycastHit hitSideOne;
        RaycastHit hitSideTwo;
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, side, out hitSideTwo, MechConstants.DISTANCE_TO_BUILDING))
        {
            if (hitSideTwo.collider.CompareTag(TagConstants.BUILDING))
            {
                hit = hitSideTwo;
            }
        }
        else if (Physics.Raycast(transform.position, -side, out hitSideOne, MechConstants.DISTANCE_TO_BUILDING))
        {
            if (hitSideOne.collider.CompareTag(TagConstants.BUILDING))
            {
                hit = hitSideOne;
            }
        }

        return hit;
    }

    private void Update()
    {
        isHorFasten = CheckingHorizontal();
        isVerFasten = CheckingVertical();
    }

    private bool CheckingVertical()
    {
        if (isHorFasten)
        {
            if (hitHor.collider == hitVer.collider) return false;
        }

        hitVer = GetHit(transform.forward);
        if (hitVer.collider != null)
        {
            fastenXPos = hitVer.collider.transform.position.x;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ResetFastenChecking()
    {
        isHorFasten = false;
        isVerFasten = false;
    }
}
