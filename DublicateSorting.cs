public Program()
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update100;
        Me.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
    }

    private int Ticks = 2;


    public void Main(string argument, UpdateType updateSource)
    {
        if (updateSource != UpdateType.Update100) return;
        var Cargos = new List<IMyCargoContainer>();
        GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(Cargos);
        string str = "Duplicate sorting started.\r\n";
        if (Ticks == 2)
        {
            int k = 0;
            foreach (IMyCargoContainer cargo in Cargos)
            {
                var mInv = new List<MyInventoryItem>();
                cargo.GetInventory().GetItems(mInv);
                for (var i = 0; i < mInv.Count; i++)
                {
                    for (var j = i+1; j < mInv.Count; j++)
                    {
                        if (mInv[i].Type.TypeId == mInv[j].Type.TypeId &&
                            mInv[i].Type.SubtypeId == mInv[j].Type.SubtypeId)
                        {
                            cargo.GetInventory().TransferItemTo(cargo.GetInventory(), j, i);
                            k++;
                        }
                    }
                }
            }

            str += k > 0 ? $"Sorting {k} elements" : "All containers are sorted.";
        }
        else
        {
            str += "All containers are sorted.";
        }
        str += $" {Ticks}";
        Ticks--;
        if (Ticks <= 0)
            Ticks = 2;
        
        Me.GetSurface(0).WriteText(str);
        Echo(str);
        
    }
