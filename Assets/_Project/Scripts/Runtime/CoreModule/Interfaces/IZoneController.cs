using System;

namespace GoldenSpin.CoreModule
{
    public interface IZoneController
    {
        ZoneDTO CurrentZoneDTO { get; }
        
        ZoneDTO GetZoneDTO(int zoneNumber);
        
        event Action<ZoneDTO> OnZoneChanged;

        void AdvanceZone();
        
        void ResetZone();
        
        bool IsCurrentZoneSafeToLeave();
        
        int GetCurrentZoneMultiplier();
        
        int CurrentZone { get; }
    }
}
