using System.Windows.Forms;

namespace SteamBadgeUnlocker.Game
{
	internal class DoubleBufferedListView : ListView
	{
		public DoubleBufferedListView()
		{
			base.DoubleBuffered = true;
		}
	}
}
