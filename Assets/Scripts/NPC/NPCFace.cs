using System.Collections.Generic;
using UnityEngine;

public sealed class NPCFace : MonoBehaviour
{
    public enum Face
    {
        Normal = 0,
        Angry = 1,
        Happy = 2
    }

    [Header("Sprites")]
    [SerializeField] private List<Texture> _faces;

    [Header("Components")]
    [SerializeField] private Renderer _renderer;

    private Face _face;

    public void NormalFace() => _face = Face.Normal;
    public void AngryFace() => _face = Face.Angry;
    public void HappyFace() => _face = Face.Happy;

    private void Update()
    {
        _renderer.materials[5].SetTexture("_MainTex", _faces[(int)_face]);
    }
}
