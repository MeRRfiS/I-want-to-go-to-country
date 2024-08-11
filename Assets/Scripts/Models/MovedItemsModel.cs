public sealed class MovedItemsModel
{
    public int FirstIndex { get; set; }
    public int SecondIndex { get; set; }
    public CellTypeEnum FirstCellTypeEnum { get; set; }
    public CellTypeEnum SecondCellTypeEnum { get; set; }
    public Inventory FromInventory { get; set; }
    public Inventory ToInventory { get; set; }
}
