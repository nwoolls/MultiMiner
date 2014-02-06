using MultiMiner.Utility.Forms;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    public partial class DeviceListView : NoFlickerListView
    {
        private bool checkFromDoubleClick = false;

        public DeviceListView()
        {
            InitializeComponent();
        }

        protected override void OnItemCheck(ItemCheckEventArgs ice)
        {
            if (checkFromDoubleClick)
            {
                ice.NewValue = ice.CurrentValue;
                checkFromDoubleClick = false;
            }
            else
                base.OnItemCheck(ice);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (e.Clicks > 1))
                checkFromDoubleClick = true;

            base.OnMouseDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            checkFromDoubleClick = false;

            base.OnKeyDown(e);
        }
    }
}
