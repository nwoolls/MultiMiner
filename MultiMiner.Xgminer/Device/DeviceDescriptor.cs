using System;
namespace MultiMiner.Xgminer
{
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
        }

        public string Description()
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

        public bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            DeviceDescriptor target = (DeviceDescriptor)obj;

            if (this.Kind == DeviceKind.PXY)
                return target.Kind == DeviceKind.PXY;
            else if (this.Kind == DeviceKind.GPU)
                return target.Kind == DeviceKind.GPU && target.RelativeIndex == this.RelativeIndex;
            else if (String.IsNullOrEmpty(target.Serial))
                //only match on Path if there is no Serial
                return target.Driver.Equals(this.Driver, StringComparison.OrdinalIgnoreCase) && target.Path.Equals(this.Path, StringComparison.OrdinalIgnoreCase);
            else
                return target.Driver.Equals(this.Driver, StringComparison.OrdinalIgnoreCase) && target.Serial.Equals(this.Serial, StringComparison.OrdinalIgnoreCase);
        }
    }
}
