using UnityEngine;

[CreateAssetMenu(fileName = "New Instrument Object", menuName = "Inventory System/Items/Axe")]
public class Axe : Instrument 
{
    private TreesController _tree = null;

    [field: SerializeField] public int HitCount { get; private set; }
    [field: SerializeField] public int TimeChop { get; private set; }

    public override void Init()
    {
        if (_durability == 0)
        {
            _durability = MaxDurability;
        }
        Amount = 1;
    }

    private void ChopTree()
    {
        _tree.ChoppingTree(HitCount);
        _durability--;
    }

    public override void UseItem()
    {
        if (_tree == null) return;

        if (_tree.IsCanChoppingTree())
        {
            UIController.GetInstance().ProgressBar(TimeChop, ChopTree);
        }
    }

    public override GameObject Updating(GameObject obj, GameObject prefab)
    {
        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position,
                           startPoint.forward,
                           out hit,
                           MechConstants.MAX_DISTANCE_FOR_USING_ITEM))
        {
            Transform hitObj = hit.transform;

            if (hitObj.CompareTag(TagConstants.TREE))
            {
                _tree = hitObj.GetComponent<TreesController>();
            }
            else
            {
                _tree = null;
                UIController.GetInstance().StopProgressBar();
            }
        }
        else
        {
            _tree = null;
            UIController.GetInstance().StopProgressBar();
        }

        return base.Updating(obj, prefab);
    }

    public override void Destruct()
    {
        _durability = 0;
    }
}
