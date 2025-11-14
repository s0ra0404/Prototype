using Zenject;

namespace AI
{
    public class AIInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ISearchService>().To<SearchService>().AsSingle();
        }
    }
}

