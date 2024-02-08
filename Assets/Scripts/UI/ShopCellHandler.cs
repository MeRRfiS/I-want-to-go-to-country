using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCellHandler : MonoBehaviour
{
    private int _index;

    [Header("Components")]
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _count;
    [SerializeField] private TextMeshProUGUI _price;
    [SerializeField] private Button _button;
    private ShopController _controller;

    public int Index
    {
        get => _index; 
        set => _index = value;
    }

    public void BuyItem()
    {
        _controller.BuyItemFromShop(_index);
    }

    public void SellItem(int count)
    {
        _controller.SellItemToShop(_index, count);
    }

    public void DrawCellInformation(GoodsModel goods, ShopController controller)
    {
        _image.sprite = goods.Goods.Icon;
        _count.text = goods.Count.ToString();
        _price.text = goods.Price.ToString();
        _controller = controller;
    }
}
