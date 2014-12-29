using System.Windows.Forms;

namespace MultiMiner.Utility.Forms
{
    public partial class ListViewEx : NoFlickerListView
    {
        public bool CheckFromDoubleClick { get; set; }

        private bool checkWasFromDoubleClick;

        protected ListViewEx()
        {
            InitializeComponent();
        }

        protected override void OnItemCheck(ItemCheckEventArgs ice)
        {
            if (!CheckFromDoubleClick && checkWasFromDoubleClick)
            {
                ice.NewValue = ice.CurrentValue;
                checkWasFromDoubleClick = false;
            }
            else
                base.OnItemCheck(ice);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (e.Clicks > 1))
                checkWasFromDoubleClick = true;

            base.OnMouseDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            checkWasFromDoubleClick = false;

            base.OnKeyDown(e);
        }
    }
}
