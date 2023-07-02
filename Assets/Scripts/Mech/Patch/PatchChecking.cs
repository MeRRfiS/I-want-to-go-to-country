using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Patch))]
public class PatchChecking : MonoBehaviour
{
    private bool isInObject = false;
    //Checking horizontal and vertical symmetrical;
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

    public Vector3 FastenPos
    {
        get => new Vector3(fastenXPos, 0, fastenZPos);
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

    private void Update()
    {
        ApplyDebugRays();
        isHorFasten = CheckingHorizontal();
        isVerFasten = CheckingVertical();
    }

    private bool CheckingHorizontal()
    {
        if (isVerFasten)
        {
            if (hitHor.collider == hitVer.collider) return false;
        }

        RaycastHit hitLeft;
        RaycastHit hitRight;
        hitHor = new RaycastHit();
        if (Physics.Raycast(transform.position, transform.right, out hitRight, MechConstants.DISTANCE_TO_PATCH))
        {
            if (hitRight.collider.CompareTag(TagConstants.PATCH))
            {
                hitHor = hitRight;
            }
        }
        else if(Physics.Raycast(transform.position, -transform.right, out hitLeft, MechConstants.DISTANCE_TO_PATCH))
        {
            if (hitLeft.collider.CompareTag(TagConstants.PATCH))
            {
                hitHor = hitLeft;
            }
        }

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

    private bool CheckingVertical()
    {
        if (isHorFasten)
        {
            if (hitHor.collider == hitVer.collider) return false;
        }

        RaycastHit hitForward;
        RaycastHit hitDown;
        hitVer = new RaycastHit();
        if (Physics.Raycast(transform.position, transform.forward, out hitForward, MechConstants.DISTANCE_TO_PATCH))
        {
            if (hitForward.collider.CompareTag(TagConstants.PATCH))
            {
                hitVer = hitForward;
            }
        }
        else if (Physics.Raycast(transform.position, -transform.forward, out hitDown, MechConstants.DISTANCE_TO_PATCH))
        {
            if (hitDown.collider.CompareTag(TagConstants.PATCH))
            {
                hitVer = hitDown;
            }
        }

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

    private void ApplyDebugRays()
    {
        float distance = MechConstants.DISTANCE_TO_PATCH;
        Vector3 forward = transform.forward * distance;
        Vector3 back = -transform.forward * distance;
        Vector3 right = transform.right * distance;
        Vector3 left = -transform.right * distance;

        Debug.DrawRay(transform.position, forward, Color.green);
        Debug.DrawRay(transform.position, back, Color.green);
        Debug.DrawRay(transform.position, right, Color.green);
        Debug.DrawRay(transform.position, left, Color.green);
    }

    public void ResetFastenChecking()
    {
        isHorFasten = false;
        isVerFasten = false;
    }
}
