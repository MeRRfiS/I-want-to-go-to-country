using UnityEngine;

[CreateAssetMenu(fileName = "New Seed Object", menuName = "Inventory System/Items/Seed")]
public class Seed : Plant
{
    [field: SerializeField] public GameObject Plant { get; private set; }

    public override void UseItem()
    {
        Transform startPoint = Camera.main.transform;

        if (Physics.Raycast(startPoint.position, startPoint.forward, out var hit, MechConstants.MAX_DISTANCE_FOR_USING_ITEM))
        {
            Transform seedbedTransform = hit.transform;
            if(seedbedTransform.TryGetComponent<Seedbed>(out var seedbed))
            {
                if (seedbed.TryPlant(this))
                {
                    Amount--;
                }
            }
        }
    }
}
