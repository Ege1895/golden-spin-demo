using System.Collections.Generic;
using UnityEngine;
using GoldenSpin.CoreModule;

namespace GoldenSpin.WheelModule
{
    [CreateAssetMenu(fileName = "WheelConfigSO", menuName = "GoldenSpin/WheelConfig")]
    public class WheelConfigSO : ScriptableObject
    {
        public SpinType SpinType;
        [Tooltip("Editor'dan slice içeriklerini buradan değiştir")]
        public List<WheelSliceSO> Slices;
    }
}
