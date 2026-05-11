using System.Collections.Generic;

namespace GoldenSpin.CoreModule
{
    [System.Serializable]
    public class CollectedRewardDTO
    {
        public List<RewardDTO> Rewards;

        public CollectedRewardDTO()
        {
            Rewards = new List<RewardDTO>();
        }
    }
}
