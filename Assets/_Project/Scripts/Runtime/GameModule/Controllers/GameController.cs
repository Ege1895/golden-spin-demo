using System;
using System.Collections.Generic;
using Zenject;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.GameModule
{
    public class GameController : IGameController, IInitializable
    {
        [Inject]
        private readonly IZoneController zoneController;
        
        [Inject]
        private readonly IWheelController wheelController;
        
        [Inject]
        private readonly IRewardController rewardController;
        
        [Inject]
        private readonly ICurrencyController currencyController;
        
        [Inject]
        private readonly IAnalyticsService analyticsService;

        public GameState CurrentState { get; private set; }

        public event Action<GameState> OnStateChanged;

        public void Initialize()
        {
            StartGame();
        }

        public void StartGame()
        {
            zoneController.ResetZone();
            rewardController.ClearRewards();
            PrepareCurrentWheel();
            EnterIdle();

            analyticsService.Track(AnalyticsConfig.EVENT_GAME_STARTED);
        }

        public void RequestSpin()
        {
            if (CurrentState != GameState.Idle)
            {
                return;
            }

            SetState(GameState.Spinning);
            TrackSpinRequested();
            wheelController.DetermineResultSliceIndex();
        }

        public void OnSpinAnimationCompleted(WheelSliceDTO resultSlice)
        {
            if (CurrentState != GameState.Spinning)
            {
                return;
            }

            wheelController.ProcessSpinResult(resultSlice);

            if (resultSlice.ContentType == SliceContentType.Bomb)
            {
                SetState(GameState.GameOver);
                TrackBomb();
                return;
            }

            SetState(GameState.SpinResult);
            TrackReward(resultSlice.Reward);
        }

        public void OnSpinResultClosed()
        {
            zoneController.AdvanceZone();
            TrackZoneAdvanced();

            if (zoneController.CurrentZone > ZoneConfig.MAX_ZONE_NUMBER)
            {
                CollectAndExit();
                return;
            }

            PrepareCurrentWheel();
            EnterIdle();
        }

        public void RequestCollect()
        {
            if (CurrentState != GameState.Idle)
            {
                return;
            }

            if (!zoneController.IsCurrentZoneSafeToLeave())
            {
                return;
            }

            CollectAndExit();
        }

        public void RestartGame()
        {
            SetState(GameState.Restarting);
            analyticsService.Track(AnalyticsConfig.EVENT_GAME_RESTARTED);
            StartGame();
        }

        public void ReviveAfterBomb()
        {
            analyticsService.Track(AnalyticsConfig.EVENT_BOMB_REVIVE_AD);
            PrepareCurrentWheel();
            EnterIdle();
        }

        public void ReviveWithCurrency()
        {
            if (CurrentState != GameState.GameOver)
            {
                return;
            }

            if (!currencyController.TrySpend(CurrencyConfig.REVIVE_COST))
            {
                return;
            }

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { AnalyticsConfig.PARAM_REVIVE_COST, CurrencyConfig.REVIVE_COST }
            };
            analyticsService.Track(AnalyticsConfig.EVENT_BOMB_REVIVE_PAID, parameters);

            rewardController.RestoreLastSnapshot();
            PrepareCurrentWheel();
            EnterIdle();
        }

        private void CollectAndExit()
        {
            SetState(GameState.Collecting);

            GrantGoldRewards();

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { AnalyticsConfig.PARAM_TOTAL_REWARDS, rewardController.CollectedRewards.Count }
            };
            analyticsService.Track(AnalyticsConfig.EVENT_REWARDS_COLLECTED, parameters);

            rewardController.CollectRewards();
        }

        private void GrantGoldRewards()
        {
            foreach (RewardDTO reward in rewardController.CollectedRewards)
            {
                if (reward.Type == RewardType.Gold)
                {
                    currencyController.Add(reward.Amount);
                }
            }
        }

        private void PrepareCurrentWheel()
        {
            ZoneDTO zone = zoneController.CurrentZoneDTO;
            wheelController.PrepareWheel(zone.SpinType, zoneController.GetCurrentZoneMultiplier());
        }

        private void EnterIdle()
        {
            SetState(GameState.Idle);
        }

        private void SetState(GameState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }

        private void TrackSpinRequested()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { AnalyticsConfig.PARAM_ZONE_NUMBER, zoneController.CurrentZone },
                { AnalyticsConfig.PARAM_SPIN_TYPE, zoneController.CurrentZoneDTO.SpinType.ToString() }
            };
            analyticsService.Track(AnalyticsConfig.EVENT_SPIN_REQUESTED, parameters);
        }

        private void TrackReward(RewardDTO reward)
        {
            if (reward == null)
            {
                return;
            }

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { AnalyticsConfig.PARAM_ZONE_NUMBER, zoneController.CurrentZone },
                { AnalyticsConfig.PARAM_REWARD_TYPE, reward.Type.ToString() },
                { AnalyticsConfig.PARAM_REWARD_AMOUNT, reward.Amount }
            };
            analyticsService.Track(AnalyticsConfig.EVENT_SPIN_COMPLETED_REWARD, parameters);
        }

        private void TrackBomb()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { AnalyticsConfig.PARAM_ZONE_NUMBER, zoneController.CurrentZone }
            };
            analyticsService.Track(AnalyticsConfig.EVENT_SPIN_COMPLETED_BOMB, parameters);
        }

        private void TrackZoneAdvanced()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { AnalyticsConfig.PARAM_ZONE_NUMBER, zoneController.CurrentZone }
            };
            analyticsService.Track(AnalyticsConfig.EVENT_ZONE_ADVANCED, parameters);
        }
    }
}
