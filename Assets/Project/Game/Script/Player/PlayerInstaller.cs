using Player;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IPlayerInput>().To<InputManager>().FromComponentInHierarchy().AsCached();
        Container.Bind<IReadCurrentNoise>().To<PlayerNoiseHandler>().FromComponentInHierarchy().AsCached();
        Container.Bind<IWriteCurrentNoise>().To<PlayerNoiseHandler>().FromComponentInHierarchy().AsCached();
        
    }
}