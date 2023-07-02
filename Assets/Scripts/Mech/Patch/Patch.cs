using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patch : MonoBehaviour
{
    [SerializeField] private PatchChecking checker;

    public PatchChecking Checker
    {
        get => checker;
    }

    private void Start()
    {
        //gameObject.layer = LayerMask.NameToLayer(LayerConstants.IGNORE_REYCAST);
    }

    public void DestroyChecker()
    {
        Destroy(checker);
        checker = null;
    }
}
