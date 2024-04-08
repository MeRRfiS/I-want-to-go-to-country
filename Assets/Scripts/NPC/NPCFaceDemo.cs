using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFaceDemo : MonoBehaviour
{
    public enum Face
    {
        Normal = 0,
        Angry = 1,
        Happy = 2
    }

    [SerializeField] private bool _isNormal;
    [SerializeField] private bool _isAngry;
    [SerializeField] private bool _isHappy;

    [Header("Sprites")]
    [SerializeField] private List<Texture> _faces;

    [Header("Components")]
    [SerializeField] private Renderer _renderer;

    private void Update()
    {
        if (_isNormal)
        {
            _renderer.materials[5].SetTexture("_BaseColorMap", _faces[(int)Face.Normal]);
        }
        else if(_isAngry)
        {
            _renderer.materials[5].SetTexture("_BaseColorMap", _faces[(int)Face.Angry]);
        }
        else if(_isHappy) 
        {
            _renderer.materials[5].SetTexture("_BaseColorMap", _faces[(int)Face.Happy]);
        }
    }
}
