using System;
using System.Collections.Generic;

namespace GoldenSpin.CoreModule
{
    public interface IWheelController
    {
        IReadOnlyList<WheelSliceDTO> CurrentSlices { get; }

        event Action<SpinType> OnWheelPrepared;

        event Action<int> OnSpinStarted;

        event Action<WheelSliceDTO> OnSpinCompleted;

        void PrepareWheel(SpinType spinType, int zoneMultiplier);
        
        void ProcessSpinResult(WheelSliceDTO resultSlice);
        
        float GetTargetAngleForSlice(int sliceIndex);
        
        int DetermineResultSliceIndex();
    }
}
