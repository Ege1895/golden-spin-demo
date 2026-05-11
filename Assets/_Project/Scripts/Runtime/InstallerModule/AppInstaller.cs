using Zenject;
using GoldenSpin.ZoneModule;

namespace GoldenSpin.InstallerModule
{
    public class AppInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ZoneController>().AsSingle();
        }
    }
}
