using System;
using System.Collections.Generic;
using GoldenSpin.CoreModule;

namespace GoldenSpin.Tests.TestDoubles
{
    public class WheelControllerTestDouble : IWheelController
    {
        private readonly List<WheelSliceDTO> slices = new List<WheelSliceDTO>();

        public IReadOnlyList<WheelSliceDTO> CurrentSlices => slices;
        public bool PrepareWheelCalled { get; private set; }
        public SpinType LastPreparedSpinType { get; private set; }
        public int DetermineResultCallCount { get; private set; }
        public int ProcessSpinResultCallCount { get; private set; }

        public event Action<SpinType> OnWheelPrepared;
        public event Action<int> OnSpinStarted;
        public event Action<WheelSliceDTO> OnSpinCompleted;

        public void PrepareWheel(SpinType spinType, int zoneMultiplier)
        {
            PrepareWheelCalled = true;
            LastPreparedSpinType = spinType;
            OnWheelPrepared?.Invoke(spinType);
        }

        public int DetermineResultSliceIndex()
        {
            DetermineResultCallCount++;
            OnSpinStarted?.Invoke(0);
            return 0;
        }

        public void ProcessSpinResult(WheelSliceDTO resultSlice)
        {
            ProcessSpinResultCallCount++;
            OnSpinCompleted?.Invoke(resultSlice);
        }

        public float GetTargetAngleForSlice(int sliceIndex) => 0f;
    }
}
