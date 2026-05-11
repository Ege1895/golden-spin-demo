namespace GoldenSpin.CoreModule
{
    [System.Serializable]
    public class WheelSliceDTO
    {
        public SliceContentType ContentType;
        public RewardDTO Reward;   // ContentType == Reward ise dolu, Bomb ise null
    }
}
