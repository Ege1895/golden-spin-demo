using System;
using System.Collections.Generic;

namespace GoldenSpin.CoreModule
{
    public interface IWheelController
    {
        IReadOnlyList<WheelSliceDTO> CurrentSlices { get; }

        event Action<int>           OnSpinStarted;
        event Action<WheelSliceDTO> OnSpinCompleted;

        void  PrepareWheel(SpinType spinType, int zoneMultiplier);
        int   DetermineResultSliceIndex();
        void  ProcessSpinResult(WheelSliceDTO resultSlice);
        float GetTargetAngleForSlice(int sliceIndex);
    }
}
