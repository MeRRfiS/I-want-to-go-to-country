public class GoodsModel
{
    public Item Goods { get; set; }
    public int Count { get; set; }
    public int Price { get; set; }

    public GoodsModel Clone()
    {
        GoodsModel clone = (GoodsModel)this.MemberwiseClone();
        clone.Count = this.Count;
        clone.Price = this.Price;
        clone.Goods = this.Goods;

        return clone;
    }
}
