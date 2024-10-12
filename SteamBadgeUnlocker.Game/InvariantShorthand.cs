using System;

namespace SteamBadgeUnlocker.Game
{
    public static class InvariantShorthand
    {
        public static string _(FormattableString formattable)
        {
            return FormattableString.Invariant(formattable);
        }
    }
}
