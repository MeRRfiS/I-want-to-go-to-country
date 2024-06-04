using TMPro;
using UnityEngine;

public class SellShopCellHandler : ShopCellHandler
{
    [SerializeField] private TMP_Text _sellOneText;
    [SerializeField] private TMP_Text _sellTenText;
    [SerializeField] private TMP_Text _sellAllText;

    public override void DrawCellInformation(GoodsModel goods, ShopController controller)
    {
        base.DrawCellInformation(goods, controller);
        _sellOneText.text = $"1 = {COIN_SPRITE}{goods.Goods.Price}";
        _sellTenText.text = $"10 = {COIN_SPRITE}{goods.Goods.Price * 10}";
        _sellAllText.text = $"All = {COIN_SPRITE}{goods.Goods.Price * controller.GetItemCount(goods)}";
    }
}