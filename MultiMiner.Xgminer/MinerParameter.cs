namespace MultiMiner.Xgminer
{
    public static class MinerParameter
    {
        public const string ApiListen = "--api-listen";
        public const string EnumerateDevices = "--ndevs";
        public const string DeviceList = "--device ?";
        public const string ScanSerialAll = "--scan all";
        public const string ScanSerialErupterAll = "--scan erupter:all";
        public const string ScanSerialAntminerAll = "--scan antminer:all";
        public const string ScanSerialErupterNoAuto = "--scan erupter:noauto";
        public const string ScanSerialOpenCL = "--scan opencl:auto";
        public const string ScanSerialOpenCLNoAuto = "--scan opencl:noauto";
        public const string ScanSerialBigPicNoAuto = "--scan bigpic:noauto";
        public const string ScanSerialBflNoAuto = "--scan bfl:noauto";
        public const string ScanSerialCpu = "--scan cpu:auto";
        public const string ScanSerialNoAuto = "--scan noauto";
    }
}
