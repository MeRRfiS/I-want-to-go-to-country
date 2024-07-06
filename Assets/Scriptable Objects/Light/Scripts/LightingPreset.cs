using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Lighting Preset", menuName = "Light System/Lighting Preset")]
public class LightingPreset : ScriptableObject
{
    public Gradient _ambientColor;
    public Gradient _directionalColor;
    public Gradient _forColor;
    public Gradient _waterColor;
}
