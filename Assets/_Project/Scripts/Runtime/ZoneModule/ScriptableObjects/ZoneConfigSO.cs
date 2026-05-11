using UnityEngine;

namespace GoldenSpin.ZoneModule
{
    [CreateAssetMenu(fileName = "ZoneConfigSO", menuName = "GoldenSpin/ZoneConfig")]
    public class ZoneConfigSO : ScriptableObject
    {
        public WheelModule.WheelConfigSO NormalWheelConfig;
        public WheelModule.WheelConfigSO SilverWheelConfig;
        public WheelModule.WheelConfigSO GoldenWheelConfig;
    }
}
