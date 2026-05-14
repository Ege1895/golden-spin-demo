using System.Collections.Generic;
using UnityEngine;
using GoldenSpin.CoreModule;

namespace GoldenSpin.WheelModule
{
    [CreateAssetMenu(fileName = "WheelConfigSO", menuName = "GoldenSpin/WheelConfig")]
    public class WheelConfigSO : ScriptableObject
    {
        public SpinType SpinType;
        
        [Tooltip("Change slice contents from the editor")]
        public List<WheelSliceSO> Slices;
    }
}
