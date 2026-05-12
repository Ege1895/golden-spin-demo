using System;
using Zenject;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.ZoneModule
{
    public class ZoneController : IZoneController, IInitializable
    {
        public ZoneDTO CurrentZoneDTO { get; private set; }
        
        public int CurrentZone { get; private set; }

        public event Action<ZoneDTO> OnZoneChanged;

        public void Initialize()
        {
            ResetZone();
        }

        public void AdvanceZone()
        {
            CurrentZone++;
            CurrentZoneDTO = GetZoneDTO(CurrentZone);
            OnZoneChanged?.Invoke(CurrentZoneDTO);
        }

        public void ResetZone()
        {
            CurrentZone = ZoneConfig.STARTING_ZONE_NUMBER;
            CurrentZoneDTO = GetZoneDTO(CurrentZone);
            OnZoneChanged?.Invoke(CurrentZoneDTO);
        }

        public ZoneDTO GetZoneDTO(int zoneNumber)
        {
            ZoneType zoneType = DetermineZoneType(zoneNumber);

            return new ZoneDTO
            {
                ZoneNumber = zoneNumber,
                Type = zoneType,
                SpinType = DetermineSpinType(zoneType)
            };
        }

        public bool IsCurrentZoneSafeToLeave()
        {
            return CurrentZoneDTO.Type == ZoneType.Safe
                || CurrentZoneDTO.Type == ZoneType.Super;
        }

        public int GetCurrentZoneMultiplier()
        {
            if (CurrentZoneDTO.Type == ZoneType.Super)
            {
                return RewardConfig.GOLDEN_ZONE_MULTIPLIER;
            }

            return CurrentZone * RewardConfig.ZONE_MULTIPLIER_INCREMENT;
        }

        private ZoneType DetermineZoneType(int zoneNumber)
        {
            if (zoneNumber % ZoneConfig.SUPER_ZONE_INTERVAL == 0)
            {
                return ZoneType.Super;
            }

            if (zoneNumber % ZoneConfig.SAFE_ZONE_INTERVAL == 0)
            {
                return ZoneType.Safe;
            }

            return ZoneType.Normal;
        }

        private SpinType DetermineSpinType(ZoneType zoneType)
        {
            switch (zoneType)
            {
                case ZoneType.Super:
                    return SpinType.Golden;
                case ZoneType.Safe:
                    return SpinType.Silver;
                default:
                    return SpinType.Normal;
            }
        }
    }
}
