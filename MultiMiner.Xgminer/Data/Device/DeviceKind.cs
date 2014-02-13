using System;

namespace MultiMiner.Xgminer.Data
{
    [Flags]
    public enum DeviceKind
    {
        None = 0,
        GPU = 1 << 0,
        USB = 1 << 1,
        PXY = 1 << 2, //proxy
        CPU = 1 << 3,
        NET = 1 << 4  //network device
    }
}
