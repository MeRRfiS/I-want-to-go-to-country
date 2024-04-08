using UnityEngine;

public class Plant: Item
{
    [field: SerializeField] public PlantTypeEnum PlantRare { get; private set; }
}
