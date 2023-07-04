using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    private bool _isAnimStop = false;
    private PlantTypeEnum _type;

    public PlantTypeEnum PlantType
    {
        set => _type = value;
    }

    public void Harvesting()
    {
        if (!_isAnimStop) return;

        Destroy(gameObject);
    }

    public void StopAnimation()
    {
        _isAnimStop = true;
    }
}
