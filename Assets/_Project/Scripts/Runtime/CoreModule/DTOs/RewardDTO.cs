using UnityEngine;

namespace GoldenSpin.CoreModule
{
    [System.Serializable]
    public class RewardDTO
    {
        public RewardType Type;
        
        public Sprite Icon;
        
        public string Label;
        
        public int Amount;
    }
}
