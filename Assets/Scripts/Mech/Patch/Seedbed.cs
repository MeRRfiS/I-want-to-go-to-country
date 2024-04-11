using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seedbed : MonoBehaviour
{
    [SerializeField] private List<Renderer> _seedbadLods;

    public void ChangeMaterial(Material material)
    {
        if (material == null) return;

        foreach(var lod in _seedbadLods)
        {
            lod.material = material;
        }
    }
}
