using System;

namespace MultiMiner.Xgminer
{
    [Flags]
    public enum DeviceKind
    {
        None = 0,
        GPU = 1,
        USB = 2,
        PXY = 4, //proxy
        CPU = 8
    }
}
