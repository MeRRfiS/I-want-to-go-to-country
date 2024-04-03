using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance;

    public static FMODEvents GetInstance() => instance;

    [field: Header("Walk SFX")]
    [field: SerializeField] public EventReference WalkOnGrass { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
