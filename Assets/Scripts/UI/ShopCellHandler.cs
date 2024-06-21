using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCellHandler : MonoBehaviour
{
    public const string COIN_SPRITE = "<sprite=44>";
    private int _index;

    [Header("Components")]
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _count;
    [SerializeField] private TextMeshProUGUI _price;
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

    public virtual void DrawCellInformation(GoodsModel goods, ShopController controller)
    {
        _image.sprite = goods.Goods.Icon;
        _count.text = goods.Count.ToString();
        _price.text = $"{COIN_SPRITE}{goods.Price}";
        _controller = controller;
    }
}
