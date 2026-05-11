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
        [SerializeField] private ZoneConfigSO    zoneConfigSO;
        [SerializeField] private WheelPanel      wheelPanel;
        [SerializeField] private ZoneBarPanel    zoneBarPanel;
        [SerializeField] private RewardListPanel rewardListPanel;
        [SerializeField] private BombPanel       bombPanel;
        [SerializeField] private CollectPanel    collectPanel;
        [SerializeField] private WheelSliceItem  sliceItemPrefab;
        [SerializeField] private ZoneBarItem     zoneBarItemPrefab;
        [SerializeField] private RewardListItem  rewardListItemPrefab;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ZoneController>().AsSingle();
            Container.BindInterfacesTo<RewardController>().AsSingle();
            Container.BindInterfacesTo<WheelController>().AsSingle();
            Container.BindInterfacesTo<GameController>().AsSingle();
            Container.BindInstance(zoneConfigSO).AsSingle();

            Container.Bind<PanelManager>().AsSingle();

            Container.BindFactory<WheelSliceDTO, WheelSliceItem, WheelSliceItem.Factory>()
                .FromComponentInNewPrefab(sliceItemPrefab);
            Container.Bind<WheelSliceItemFactory>().AsSingle();
            Container.BindInstance(wheelPanel).AsSingle();

            Container.BindFactory<ZoneDTO, ZoneBarItem, ZoneBarItem.Factory>()
                .FromComponentInNewPrefab(zoneBarItemPrefab);
            Container.Bind<ZoneBarItemFactory>().AsSingle();
            Container.BindInstance(zoneBarPanel).AsSingle();

            Container.BindFactory<RewardDTO, RewardListItem, RewardListItem.Factory>()
                .FromComponentInNewPrefab(rewardListItemPrefab);
            Container.Bind<RewardListItemFactory>().AsSingle();
            Container.BindInstance(rewardListPanel).AsSingle();

            Container.BindInstance(bombPanel).AsSingle();
            Container.BindInstance(collectPanel).AsSingle();
        }
    }
}
