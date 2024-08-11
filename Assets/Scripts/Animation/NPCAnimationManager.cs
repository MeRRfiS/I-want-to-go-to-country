using System;
using UnityEngine;

public sealed class NPCAnimationManager : MonoBehaviour
{
    private const string HAPPY = "_IsHappy";
    private const string ANGRY = "_IsAngry";
    private const string DEAD = "_IsDead";

    [SerializeField] private Animator _npc;

    public event Action OnStopHappy;
    public event Action OnDead;

    public void IsAngry(bool status) => _npc.SetBool(ANGRY, status);
    public void IsHappy(bool status) => _npc.SetBool(HAPPY, status);
    public void IsDead(bool status) => _npc.SetBool(DEAD, status);

    public void StopBeHappy()
    {
        IsHappy(false);
        OnStopHappy?.Invoke();
    }

    public void NPCIsDead()
    {
        IsDead(false);
        OnDead?.Invoke();
    }
}
