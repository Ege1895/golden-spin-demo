using UnityEngine;
using GoldenSpin.CoreModule;

namespace GoldenSpin.UIModule
{
    public static class TierColors
    {
        private const string HEX_BRONZE = "#CD7F32";
        private const string HEX_SILVER = "#C0C0C0";
        private const string HEX_GOLD = "#FFC72C";

        public static readonly Color Bronze = HexToColor(HEX_BRONZE);
        public static readonly Color Silver = HexToColor(HEX_SILVER);
        public static readonly Color Gold = HexToColor(HEX_GOLD);

        public static Color For(SpinType spinType)
        {
            switch (spinType)
            {
                case SpinType.Golden:
                    return Gold;
                case SpinType.Silver:
                    return Silver;
                default:
                    return Bronze;
            }
        }

        public static Color For(ZoneType zoneType)
        {
            switch (zoneType)
            {
                case ZoneType.Super:
                    return Gold;
                case ZoneType.Safe:
                    return Silver;
                default:
                    return Bronze;
            }
        }

        private static Color HexToColor(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out Color c);
            return c;
        }
    }
}
