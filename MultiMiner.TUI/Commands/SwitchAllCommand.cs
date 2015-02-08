using MultiMiner.UX.ViewModels;
using System.Linq;

namespace MultiMiner.TUI.Commands
{
    class SwitchAllCommand
    {
        private readonly ApplicationViewModel app;

        public SwitchAllCommand(ApplicationViewModel app)
        {
            this.app = app;
        }

        public bool HandleCommand(string[] input)
        {
            if (input.Count() == 2)
            {
                app.SetAllDevicesToCoin(input[1], true);
                return true;
            }

            return false;
        }
    }
}
