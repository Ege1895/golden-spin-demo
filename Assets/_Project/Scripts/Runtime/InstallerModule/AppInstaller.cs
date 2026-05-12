using UnityEngine;
using Zenject;
using GoldenSpin.CoreModule;
using GoldenSpin.ZoneModule;
using GoldenSpin.RewardModule;
using GoldenSpin.WheelModule;
using GoldenSpin.GameModule;
using GoldenSpin.UIModule;

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

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ZoneController>().AsSingle();
            Container.BindInterfacesTo<RewardController>().AsSingle();
            Container.BindInterfacesTo<WheelController>().AsSingle();
            Container.BindInterfacesTo<GameController>().AsSingle();
            Container.BindInstance(zoneConfigSO).AsSingle();

            Container.Bind<PanelManager>().AsSingle();

            Container.Bind<WheelSliceItemFactory>().AsSingle();
            Container.Bind<RewardListItemFactory>().AsSingle();
            Container.Bind<ZoneBarItemFactory>().AsSingle();

            Container.BindInstance(wheelPanel).AsSingle();
            Container.BindInstance(zoneBarPanel).AsSingle();
            Container.BindInstance(rewardListPanel).AsSingle();
            Container.BindInstance(bombPanel).AsSingle();
            Container.BindInstance(collectPanel).AsSingle();
        }
    }
}
