using Views;
using Zenject;

namespace Factories
{
    public class MenuCharactersFactory: IFactory<CharacterElementView>
    {
        private readonly DiContainer _di;

        public MenuCharactersFactory
        (
            DiContainer di
        )
        {
            _di = di;
        }
        
        public CharacterElementView Create()
        {
            var view = _di.Resolve<CharacterElementView>();
            var go = _di.InstantiatePrefabForComponent<CharacterElementView>(view);
            return go;
        }
    }
    
}