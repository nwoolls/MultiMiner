using System;
namespace MultiMiner.Xgminer
{
    public class DeviceDescriptor
    {
        public DeviceKind Kind { get; set; }
        public int RelativeIndex { get; set; }
        public string Driver { get; set; }
        public string Path { get; set; }

        public void Assign(DeviceDescriptor source)
        {
            this.Kind = source.Kind;
            this.RelativeIndex = source.RelativeIndex;
            this.Driver = source.Driver;
            this.Path = source.Path;
        }

        public bool Equals(DeviceDescriptor target)
        {
            if (this.Kind == DeviceKind.PXY)
                return target.Kind == DeviceKind.PXY;
            else if (this.Kind == DeviceKind.GPU)
                return target.Kind == DeviceKind.GPU && target.RelativeIndex == this.RelativeIndex;
            else
                return target.Driver.Equals(this.Driver, StringComparison.OrdinalIgnoreCase) && target.Path.Equals(this.Path, StringComparison.OrdinalIgnoreCase);
        }
    }
}
