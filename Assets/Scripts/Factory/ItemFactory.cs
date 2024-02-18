using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ItemFactory : IItemFactory
{
    private readonly DiContainer _diContainer;

    public ItemFactory(DiContainer container)
    {
        _diContainer = container;
    }

    public GameObject Create(GameObject newItem)
    {
        GameObject result = null;
        result = _diContainer.InstantiatePrefab(newItem);

        return result;
    }
}
