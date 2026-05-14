using UnityEngine;
using Zenject;
using GoldenSpin.AnalyticsModule;
using GoldenSpin.CoreModule;
using GoldenSpin.CurrencyModule;
using GoldenSpin.GameModule;
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
        [SerializeField]
        private TopBarUIPanel topBarPanel;

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
        }

        private void BindControllers()
        {
            Container.BindInterfacesTo<CurrencyController>().AsSingle();
            Container.BindInterfacesTo<ZoneController>().AsSingle();
            Container.BindInterfacesTo<RewardController>().AsSingle();
            Container.BindInterfacesTo<WheelController>().AsSingle();
            Container.BindInterfacesTo<GameController>().AsSingle();
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

            if (topBarPanel != null)
            {
                Container.BindInstance(topBarPanel).AsSingle();
            }
        }

        private void BindConfigs()
        {
            Container.BindInstance(zoneConfigSO).AsSingle();
        }
    }
}
