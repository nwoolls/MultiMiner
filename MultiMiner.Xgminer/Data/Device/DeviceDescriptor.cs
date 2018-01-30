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
            Driver = String.Empty;
            Path = String.Empty;
            RelativeIndex = -1;
            Serial = String.Empty;
            Kind = DeviceKind.None;
        }

        private const string DualMinerDriver = "dualminer";
        private const string GridSeedDriver = "gridseed";
        private const string ZeusMinerDriver = "zeusminer";
        private const string FutureBitDriver = "futurebit";

        private bool IsScryptAsic()
        {
            return Driver.Equals(DualMinerDriver, StringComparison.OrdinalIgnoreCase) ||
                Driver.Equals(GridSeedDriver, StringComparison.OrdinalIgnoreCase) ||
                Driver.Equals(ZeusMinerDriver, StringComparison.OrdinalIgnoreCase) ||
                Driver.Equals(FutureBitDriver, StringComparison.OrdinalIgnoreCase);
        }

        public bool SupportsAlgorithm(string algorithm)
        {
            bool result;

            if (algorithm.Equals(AlgorithmNames.Scrypt, StringComparison.OrdinalIgnoreCase))
            {
                result = (Kind == DeviceKind.GPU) || (Kind == DeviceKind.CPU) || (Kind == DeviceKind.PXY) || IsScryptAsic();
            }
            else if (algorithm.Equals(AlgorithmNames.SHA256, StringComparison.OrdinalIgnoreCase))
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
            if (Kind == DeviceKind.PXY)
                return "proxy";
            if (Kind == DeviceKind.GPU)
                return "opencl:" + RelativeIndex;
            return String.Format("{0}:{1}", Driver, Path);
        }

        public void Assign(DeviceDescriptor source)
        {
            Kind = source.Kind;
            RelativeIndex = source.RelativeIndex;
            Driver = source.Driver;
            Path = source.Path;
            Serial = source.Serial;
        }

        public new bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            DeviceDescriptor target = (DeviceDescriptor)obj;

            if (Kind == DeviceKind.PXY)
                return target.Kind == DeviceKind.PXY && target.RelativeIndex == RelativeIndex;
            if (Kind == DeviceKind.GPU)
                return target.Kind == DeviceKind.GPU && target.RelativeIndex == RelativeIndex;
            if (Kind == DeviceKind.CPU)
                return target.Kind == DeviceKind.CPU && target.RelativeIndex == RelativeIndex;
            if (String.IsNullOrEmpty(target.Serial) || String.IsNullOrEmpty(this.Serial))
                //only match on Path if there is no Serial
                //check for Serial on both sides as the Equals() needs to be bi-directional
                return target.Driver.Equals(Driver, StringComparison.OrdinalIgnoreCase) && target.Path.Equals(Path, StringComparison.OrdinalIgnoreCase);
            return target.Driver.Equals(Driver, StringComparison.OrdinalIgnoreCase) && target.Serial.Equals(Serial, StringComparison.OrdinalIgnoreCase)
                   && target.Path.Equals(Path, StringComparison.OrdinalIgnoreCase);
        }
    }
}
