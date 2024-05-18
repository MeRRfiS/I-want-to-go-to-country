using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

[System.Serializable]
public class Blackboard
{
    [Header("Location Node")]
    public Transform Road;
    public NavMeshSurface Surface;
}
