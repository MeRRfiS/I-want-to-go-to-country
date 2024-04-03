using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Instrument Object", menuName = "Inventory System/Items/Hoe")]
public class Hoe : Instrument
{
    [field: SerializeField] public int TimeWork { get; private set; }

    private const string OPACITY = "_OPACITY";
    private const string BASE_COLOR = "_BASE_COLOR";

    private GameObject _seedbedObj;
    private SeedbedChecking _seedbedCheck;
    private Renderer _seedbedRend;

    private bool IsPatchObjNull() => _seedbedObj == null;

    public override void Init()
    {
        _durability = MaxDurability;
        Amount = 1;
    }

    private void MakePath()
    {
        _durability--;
        _seedbedObj.layer = LayerMask.NameToLayer(LayerConstants.DEFAULT);
        foreach (Transform seedbedLOD in _seedbedObj.transform)
        {
            seedbedLOD.gameObject.layer = LayerMask.NameToLayer(LayerConstants.DEFAULT);
        }
        _seedbedRend.material.SetFloat(OPACITY, 1f);
        _seedbedRend.material.SetColor(BASE_COLOR, new Color(1, 1, 1));
        _seedbedObj.GetComponent<Seedbed>().DestroyChecker();
        MonoBehaviour.Instantiate(_seedbedObj);
        _seedbedCheck = null;
        _seedbedObj = null;
    }

    public override void UseItem()
    {
        if (IsPatchObjNull()) return;
        if (_seedbedCheck.IsOnObject) return;

        UIController.GetInstance().ProgressBar(TimeWork, MakePath);
    }

    public override GameObject Updating(GameObject obj, GameObject prefab)
    {
        _seedbedObj = obj;
        if (!IsPatchObjNull())
        {
            _seedbedCheck = obj.GetComponent<Seedbed>().Checker;
        }

        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position,
                            startPoint.forward,
                            out hit,
                            ItemConstants.MAX_DISTANCE_TO_EARTH) && hit.collider.CompareTag(TagConstants.EARTH))
        {
            if (IsPatchObjNull())
            {
                _seedbedObj = MonoBehaviour.Instantiate(prefab);
                _seedbedCheck = _seedbedObj.GetComponent<Seedbed>().Checker;
            }

            Vector3 point = Vector3.zero;
            point = new Vector3(_seedbedCheck.IsVerFasten ? _seedbedCheck.FastenPos.x : hit.point.x,
                                1,
                                _seedbedCheck.IsHorFasten ? _seedbedCheck.FastenPos.z : hit.point.z);
            if (Vector3.Distance(point, hit.point) >= MechConstants.MAX_DISTANCE_FOR_FASTEN_PATCH)
            {
                _seedbedCheck.ResetFastenChecking();
                point = hit.point;
            }
            _seedbedObj.transform.position = new Vector3(point.x,
                                                      1,
                                                      point.z);

            _seedbedRend = GetActiveLOD();
            _seedbedRend.material.SetFloat(OPACITY, 0.8f);
            if (_seedbedCheck && _seedbedCheck.IsOnObject)
            {
                _seedbedRend.material.SetColor(BASE_COLOR, new Color(1, 0, 0));
            }
            else if (_seedbedCheck)
            {
                _seedbedRend.material.SetColor(BASE_COLOR, new Color(1, 1, 1));
            }

            if (_seedbedCheck.IsOnObject)
            {
                UIController.GetInstance().StopProgressBar();
            }
        }
        else
        {
            _seedbedObj = this.StopUpdating();
            UIController.GetInstance().StopProgressBar();
        }

        return _seedbedObj;
    }

    private Renderer GetActiveLOD()
    {
        Renderer result = null;

        LODGroup lodGroup = _seedbedObj.GetComponent<LODGroup>();
        Transform lodTransform = lodGroup.transform;

        foreach (Transform child in lodTransform)
        {
            result = child.GetComponent<Renderer>();
            if (result != null && result.isVisible) break;
        }

        return result;
    }

    public override GameObject StopUpdating()
    {
        if (!IsPatchObjNull()) MonoBehaviour.Destroy(_seedbedObj);
        _seedbedObj = null;

        return _seedbedObj;
    }

    public override void Destruct()
    {
        _durability = 0;
    }
}
