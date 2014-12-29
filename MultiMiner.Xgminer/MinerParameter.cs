namespace MultiMiner.Xgminer
{
    public static class MinerParameter
    {
        public const string EnumerateDevices = "--ndevs";
        public const string DeviceList = "--device ?";
        public const string ScanSerialAll = "--scan all";
        public const string ScanSerialErupterAll = "--scan erupter:all";
        public const string ScanSerialNanofuryAll = "--scan nanofury:all";
        public const string ScanSerialOpenCL = "--scan opencl:auto";
        public const string ScanSerialOpenCLNoAuto = "--scan opencl:noauto";
        public const string ScanSerialCpu = "--scan cpu:auto";
        public const string ScanSerialNoAuto = "--scan noauto";
        public const string ScanSerialAuto = "--scan auto";
        public const string Scrypt = "--scrypt";
    }
}
