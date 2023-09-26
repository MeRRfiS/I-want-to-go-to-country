using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade Collection", menuName = "Craft System/Upgrade")]
public class UpgradesCollection : ScriptableObject
{
    public List<UpgradeModel> _upgrades;
}

[System.Serializable]
public class UpgradeModel: InformationModel
{
    public int _price;
    public bool _active;
}