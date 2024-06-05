using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class NPCAnimationManager : MonoBehaviour
{
    private const string HAPPY = "_IsHappy";
    private const string ANGRY = "_IsAngry";

    [SerializeField] private Animator _npc;

    public event Action OnStopHappy;

    public void IsAngry(bool status) => _npc.SetBool(ANGRY, status);
    public void IsHappy(bool status) => _npc.SetBool(HAPPY, status);

    public void StopBeHappy()
    {
        IsHappy(false);
        OnStopHappy?.Invoke();
    }
}
