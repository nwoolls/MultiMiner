using MultiMiner.UX.ViewModels;
using System.Collections;
using System.Windows.Forms;

namespace MultiMiner.Win.Controls
{
    public class ItemComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            ListViewItem item1 = x as ListViewItem;
            ListViewItem item2 = y as ListViewItem;

            DeviceViewModel d1 = (DeviceViewModel)item1.Tag;
            DeviceViewModel d2 = (DeviceViewModel)item2.Tag;

            return d1.CompareTo(d2);
        }
    }

    public partial class DeviceListView : ListViewEx
    {
        public DeviceListView()
        {
            ListViewItemSorter = new ItemComparer();
            Sorting = SortOrder.Ascending;
        }
    }
}
