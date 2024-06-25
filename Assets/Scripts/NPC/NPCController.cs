using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    [field: Header("Components")]
    [field: SerializeField] public NPCFace Face { get; private set; }
    [field: SerializeField] public GameObject NpcObject { get; private set; }
    [field: SerializeField] public NPCAnimationManager Animation { get; private set; }
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public ShopController Shop { get; private set; }

    [field: Header("Settings")]
    [field: SerializeField] public Transform HomePosition { get; private set; }
    [field: SerializeField] public Transform ShopPosition { get; private set; }
    [field: SerializeField] public float RotationInShop { get; private set; }

    public bool IsHold { get; set; }

    public void DropFromHand()
    {
        IsHold = false;
        HandsAnimationManager.GetInstance().IsHoldNPC(false);
    }
}
