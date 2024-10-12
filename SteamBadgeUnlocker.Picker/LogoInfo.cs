using System.Drawing;

namespace SteamBadgeUnlocker.Picker
{
    internal class LogoInfo
    {
        public readonly uint Id;
        public readonly Bitmap Bitmap;

        public LogoInfo(uint id, Bitmap bitmap)
        {
            this.Id = id;
            this.Bitmap = bitmap;
        }
    }
}
