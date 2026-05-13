using UnityEngine;
using GoldenSpin.CoreModule;

namespace GoldenSpin.UIModule
{
    public static class TierColors
    {
        public static readonly Color Bronze = HexToColor("#CD7F32");
        public static readonly Color Silver = HexToColor("#C0C0C0");
        public static readonly Color Gold   = HexToColor("#FFC72C");

        public static Color For(SpinType spinType)
        {
            switch (spinType)
            {
                case SpinType.Golden: return Gold;
                case SpinType.Silver: return Silver;
                default:              return Bronze;
            }
        }

        public static Color For(ZoneType zoneType)
        {
            switch (zoneType)
            {
                case ZoneType.Super: return Gold;
                case ZoneType.Safe:  return Silver;
                default:             return Bronze;
            }
        }

        private static Color HexToColor(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out Color c);
            return c;
        }
    }
}
