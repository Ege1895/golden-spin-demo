using UnityEngine;
using Zenject;
using GoldenSpin.ZoneModule;
using GoldenSpin.RewardModule;
using GoldenSpin.WheelModule;
using GoldenSpin.GameModule;
using GoldenSpin.UIModule;

namespace GoldenSpin.InstallerModule
{
    public class AppInstaller : MonoInstaller
    {
        [SerializeField] private ZoneConfigSO zoneConfigSO;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ZoneController>().AsSingle();
            Container.BindInterfacesTo<RewardController>().AsSingle();
            Container.BindInterfacesTo<WheelController>().AsSingle();
            Container.BindInterfacesTo<GameController>().AsSingle();
            Container.BindInstance(zoneConfigSO).AsSingle();

            Container.Bind<PanelManager>().AsSingle();
        }
    }
}
