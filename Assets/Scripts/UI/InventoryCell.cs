using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class InventoryCell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int _index;
    [SerializeField] private CellTypeEnum _typeCell;


    public void OnPointerClick(PointerEventData eventData)
    {
        UIController.GetInstance().PinUpItemToMouse(_index, _typeCell);
        InventoryController.GetInstance().SelectItem(_index, _typeCell);
    }
}
