using System;

namespace SteamBadgeUnlocker.Picker
{
    internal static class InvariantShorthand
    {
        public static string _(FormattableString formattable)
        {
            return FormattableString.Invariant(formattable);
        }
    }
}
