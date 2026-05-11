using Zenject;
using GoldenSpin.ZoneModule;
using GoldenSpin.RewardModule;

namespace GoldenSpin.InstallerModule
{
    public class AppInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ZoneController>().AsSingle();
            Container.BindInterfacesTo<RewardController>().AsSingle();
        }
    }
}
