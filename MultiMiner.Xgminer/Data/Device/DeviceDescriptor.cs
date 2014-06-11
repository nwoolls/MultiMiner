using System;

namespace MultiMiner.Xgminer.Data
{
    [Serializable]
    public class DeviceDescriptor
    {
        public DeviceKind Kind { get; set; }
        public int RelativeIndex { get; set; }
        public string Driver { get; set; }
        public string Path { get; set; }
        public string Serial { get; set; }

        public DeviceDescriptor()
        {
            this.Driver = String.Empty;
            this.Path = String.Empty;
            this.RelativeIndex = -1;
            this.Serial = String.Empty;
            this.Kind = DeviceKind.None;
        }

        private const string DualMinerDriver = "dualminer";
        private const string GridSeedDriver = "gridseed";

        private bool IsScryptAsic()
        {
            return Driver.Equals(DualMinerDriver, StringComparison.OrdinalIgnoreCase) ||
                Driver.Equals(GridSeedDriver, StringComparison.OrdinalIgnoreCase);
        }

        public bool SupportsAlgorithm(CoinAlgorithm algorithm)
        {
            bool result = false;

            if (algorithm == CoinAlgorithm.Scrypt)
            {
                result = (Kind == DeviceKind.GPU) || (Kind == DeviceKind.CPU) || (Kind == DeviceKind.PXY) || IsScryptAsic();
            }
            else if (algorithm == CoinAlgorithm.SHA256)
            {
                result = !IsScryptAsic();
            }
            else
            {
                result = (Kind == DeviceKind.GPU);
            }

            return result;
        }

        public override string ToString()
        {
            if (this.Kind == DeviceKind.PXY)
                return "proxy";
            else if (this.Kind == DeviceKind.GPU)
                return "opencl:" + this.RelativeIndex;
            else
                return String.Format("{0}:{1}", this.Driver, this.Path);
        }

        public void Assign(DeviceDescriptor source)
        {
            this.Kind = source.Kind;
            this.RelativeIndex = source.RelativeIndex;
            this.Driver = source.Driver;
            this.Path = source.Path;
            this.Serial = source.Serial;
        }

        public new bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            DeviceDescriptor target = (DeviceDescriptor)obj;

            if (this.Kind == DeviceKind.PXY)
                return target.Kind == DeviceKind.PXY && target.RelativeIndex == this.RelativeIndex;
            else if (this.Kind == DeviceKind.GPU)
                return target.Kind == DeviceKind.GPU && target.RelativeIndex == this.RelativeIndex;
            else if (this.Kind == DeviceKind.CPU)
                return target.Kind == DeviceKind.CPU && target.RelativeIndex == this.RelativeIndex;
            else if (String.IsNullOrEmpty(target.Serial) || String.IsNullOrEmpty(this.Serial))
                //only match on Path if there is no Serial
                //check for Serial on both sides as the Equals() needs to be bi-directional
                return target.Driver.Equals(this.Driver, StringComparison.OrdinalIgnoreCase) && target.Path.Equals(this.Path, StringComparison.OrdinalIgnoreCase);
            else
                //match on Driver, Serial AND Path
                //cannot just do Serial because, while it should be unique, in practice it is not
                return target.Driver.Equals(this.Driver, StringComparison.OrdinalIgnoreCase) && target.Serial.Equals(this.Serial, StringComparison.OrdinalIgnoreCase)
                    && target.Path.Equals(this.Path, StringComparison.OrdinalIgnoreCase);
        }
    }
}
