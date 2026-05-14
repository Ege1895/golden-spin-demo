using UnityEngine;
using Zenject;
using GoldenSpin.AnalyticsModule;
using GoldenSpin.AudioModule;
using GoldenSpin.ConfigurationModule;
using GoldenSpin.CoreModule;
using GoldenSpin.CurrencyModule;
using GoldenSpin.GameModule;
using GoldenSpin.HapticModule;
using GoldenSpin.RewardModule;
using GoldenSpin.SaveModule;
using GoldenSpin.UIModule;
using GoldenSpin.WheelModule;
using GoldenSpin.ZoneModule;

namespace GoldenSpin.InstallerModule
{
    public class AppInstaller : MonoInstaller
    {
        [SerializeField]
        private ZoneConfigSO zoneConfigSO;

        [SerializeField]
        private WheelUIPanel wheelPanel;
        
        [SerializeField]
        private ZoneBarUIPanel zoneBarPanel;
        
        [SerializeField]
        private RewardListUIPanel rewardListPanel;
        
        [SerializeField]
        private BombUIPanel bombPanel;
        
        [SerializeField]
        private CollectUIPanel collectPanel;
        
        [SerializeField]
        private AwardReceivedUIPanel awardReceivedPanel;

        public override void InstallBindings()
        {
            BindServices();
            BindControllers();
            BindFactories();
            BindPanels();
            BindConfigs();
        }

        private void BindServices()
        {
            Container.Bind<ISaveService>().To<PlayerPrefsSaveService>().AsSingle();
            Container.Bind<IAnalyticsService>().To<DebugAnalyticsService>().AsSingle();
            Container.BindInterfacesTo<AudioService>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName(AudioConfig.AUDIO_SERVICE_GO_NAME)
                .AsSingle()
                .NonLazy();
            BindHapticService();
        }

        private void BindHapticService()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Container.BindInterfacesTo<AndroidHapticService>().AsSingle();
#else
            Container.Bind<IHapticService>().To<NullHapticService>().AsSingle();
#endif
        }

        private void BindControllers()
        {
            Container.BindInterfacesTo<CurrencyController>().AsSingle();
            Container.BindInterfacesTo<ZoneController>().AsSingle();
            Container.BindInterfacesTo<RewardController>().AsSingle();
            Container.BindInterfacesTo<WheelController>().AsSingle();
            Container.BindInterfacesTo<GameController>().AsSingle();
            Container.BindInterfacesTo<AudioFeedbackController>().AsSingle();
            Container.BindInterfacesTo<HapticFeedbackController>().AsSingle();
        }

        private void BindFactories()
        {
            Container.Bind<WheelSliceItemFactory>().AsSingle();
            Container.Bind<RewardListItemFactory>().AsSingle();
            Container.Bind<ZoneBarItemFactory>().AsSingle();
        }

        private void BindPanels()
        {
            Container.BindInstance(wheelPanel).AsSingle();
            Container.BindInstance(zoneBarPanel).AsSingle();
            Container.BindInstance(rewardListPanel).AsSingle();
            Container.BindInstance(bombPanel).AsSingle();
            Container.BindInstance(collectPanel).AsSingle();
            Container.BindInstance(awardReceivedPanel).AsSingle();
        }

        private void BindConfigs()
        {
            Container.BindInstance(zoneConfigSO).AsSingle();
        }
    }
}
