using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MovedItemsModel
{
    public int FirstIndex { get; set; }
    public int SecondIndex { get; set; }
    public CellTypeEnum FirstCellTypeEnum { get; set; }
    public CellTypeEnum SecondCellTypeEnum { get; set; }
    public Inventory FromInventory { get; set; }
    public Inventory ToInventory { get; set; }
}
