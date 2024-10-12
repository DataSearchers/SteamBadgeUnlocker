using System.Globalization;
using System.Windows.Forms;

namespace SteamBadgeUnlocker.Picker
{
    internal class GameInfo
    {
        private string _Name;

        public uint Id;
        public string Type;
        public int ImageIndex;

        public string Name
        {
            get => this._Name;
            set => this._Name = value ?? "App " + this.Id.ToString(CultureInfo.InvariantCulture);
        }

        public string ImageUrl;

        public ListViewItem Item;

        public GameInfo(uint id, string type)
        {
            this.Id = id;
            this.Type = type;
            this.Name = null;
            this.ImageIndex = 0;
            this.ImageUrl = null;
        }
    }
}
