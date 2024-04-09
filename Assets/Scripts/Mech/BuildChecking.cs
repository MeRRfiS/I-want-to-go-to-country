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

    [SerializeField] private Transform _rayStart;

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

        hitHor = GetHit(_rayStart.right);
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
        if (Physics.Raycast(_rayStart.position, side, out hitSideTwo, MechConstants.DISTANCE_TO_BUILDING))
        {
            if (hitSideTwo.collider.CompareTag(TagConstants.BUILDING) ||
                hitSideTwo.collider.CompareTag(TagConstants.CHEST))
            {
                hit = hitSideTwo;
            }
        }

        if(hit.collider != null) return hit;

        if (Physics.Raycast(_rayStart.position, -side, out hitSideOne, MechConstants.DISTANCE_TO_BUILDING))
        {
            if (hitSideOne.collider.CompareTag(TagConstants.BUILDING) ||
                hitSideOne.collider.CompareTag(TagConstants.CHEST))
            {
                hit = hitSideOne;
            }
        }

        return hit;
    }

    private void Update()
    {
        ApplyDebugRays();
        isHorFasten = CheckingHorizontal();
        isVerFasten = CheckingVertical();
    }

    private bool CheckingVertical()
    {
        if (isHorFasten)
        {
            if (hitHor.collider == hitVer.collider) return false;
        }

        hitVer = GetHit(_rayStart.forward);
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
        float distance = MechConstants.DISTANCE_TO_BUILDING;
        Vector3 forward = _rayStart.forward * distance;
        Vector3 back = -_rayStart.forward * distance;
        Vector3 right = _rayStart.right * distance;
        Vector3 left = -_rayStart.right * distance;

        Debug.DrawRay(_rayStart.position, forward, Color.red);
        Debug.DrawRay(_rayStart.position, back, Color.red);
        Debug.DrawRay(_rayStart.position, right, Color.red);
        Debug.DrawRay(_rayStart.position, left, Color.red);
    }

    public void ResetFastenChecking()
    {
        isHorFasten = false;
        isVerFasten = false;
    }
}
