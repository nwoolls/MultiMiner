using MultiMiner.UX.Data.Configuration;

namespace MultiMiner.UX.ViewModels
{
    public class ApplicationViewModel
    {
        #region Private fields
        //configuration
        public readonly Engine.Data.Configuration.Engine EngineConfiguration = new Engine.Data.Configuration.Engine();
        public readonly Application ApplicationConfiguration = new Application();
        public readonly Paths PathConfiguration = new Paths();
        public readonly Perks PerksConfiguration = new Perks();
        public readonly NetworkDevices NetworkDevicesConfiguration = new NetworkDevices();
        public readonly Metadata MetadataConfiguration = new Metadata();
        #endregion
    }
}
