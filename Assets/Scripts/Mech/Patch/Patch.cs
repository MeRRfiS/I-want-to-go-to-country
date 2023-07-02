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

    public void DestroyChecker()
    {
        Destroy(checker);
        checker = null;
    }
}
