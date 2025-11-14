using Zenject;

namespace Smartphone
{
    public class SmartphoneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Battery>().AsSingle();
            
            Container.Bind<ILevelCalculation>().To<Battery>().FromResolve();
            Container.Bind<IReadBatteryLevel>().To<Battery>().FromResolve();
        }
    }
}