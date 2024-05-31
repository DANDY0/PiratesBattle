using Zenject;

namespace Utils
{
    public static class DependencyInjector
    {
        public static DiContainer Container;

        public static void InjectDependencies(object obj) => Container?.Inject(obj);
    }
}