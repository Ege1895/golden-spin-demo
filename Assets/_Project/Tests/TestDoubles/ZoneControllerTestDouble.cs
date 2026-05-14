using System;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.Tests.TestDoubles
{
    public class ZoneControllerTestDouble : IZoneController
    {
        public ZoneDTO CurrentZoneDTO { get; set; }
        public int CurrentZone { get; set; } = 1;
        public bool SafeToLeave { get; set; } = false;
        public int ZoneMultiplier { get; set; } = 1;
        public int AdvanceZoneCallCount { get; private set; }
        public int ResetZoneCallCount { get; private set; }

        public event Action<ZoneDTO> OnZoneChanged;

        public ZoneControllerTestDouble()
        {
            CurrentZoneDTO = new ZoneDTO { ZoneNumber = 1, Type = ZoneType.Normal, SpinType = SpinType.Normal };
        }

        public void AdvanceZone()
        {
            AdvanceZoneCallCount++;
            CurrentZone++;
            CurrentZoneDTO = new ZoneDTO { ZoneNumber = CurrentZone };
            OnZoneChanged?.Invoke(CurrentZoneDTO);
        }

        public void ResetZone()
        {
            ResetZoneCallCount++;
            CurrentZone = ZoneConfig.STARTING_ZONE_NUMBER;
            OnZoneChanged?.Invoke(CurrentZoneDTO);
        }

        public ZoneDTO GetZoneDTO(int zoneNumber) => new ZoneDTO { ZoneNumber = zoneNumber };
        public bool IsCurrentZoneSafeToLeave() => SafeToLeave;
        public int GetCurrentZoneMultiplier() => ZoneMultiplier;
    }
}
