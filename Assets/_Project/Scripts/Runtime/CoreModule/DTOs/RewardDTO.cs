namespace GoldenSpin.CoreModule
{
    [System.Serializable]
    public class RewardDTO
    {
        public RewardType Type;
        public string Label;
        public int Amount;
        public string SpriteKey;   // Addressable key
    }
}
