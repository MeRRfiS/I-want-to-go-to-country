using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int Id { get; set; }
    public int Price { get; set; }
    public virtual void Use() { }

    public virtual GameObject Visualization(GameObject obj, GameObject prefab) => null;
    public virtual GameObject StopVisualization() => null;
}
