using System;

namespace GoldenSpin.CoreModule
{
    public interface IZoneController
    {
        int     CurrentZone    { get; }
        ZoneDTO CurrentZoneDTO { get; }

        event Action<ZoneDTO> OnZoneChanged;

        void    AdvanceZone();
        void    ResetZone();
        ZoneDTO GetZoneDTO(int zoneNumber);
        bool    IsCurrentZoneSafeToLeave();
        int     GetCurrentZoneMultiplier();
    }
}
