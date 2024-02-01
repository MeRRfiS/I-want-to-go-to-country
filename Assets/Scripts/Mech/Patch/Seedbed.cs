using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seedbed : MonoBehaviour
{
    [SerializeField] private SeedbedChecking checker;

    public SeedbedChecking Checker
    {
        get => checker;
    }

    public void DestroyChecker()
    {
        Destroy(checker);
        checker = null;
    }
}
