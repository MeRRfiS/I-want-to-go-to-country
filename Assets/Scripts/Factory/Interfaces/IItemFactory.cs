using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemFactory
{
    public GameObject Create(GameObject newItem);
}
