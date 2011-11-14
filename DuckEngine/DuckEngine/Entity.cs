namespace DuckEngine
{
    public class Entity
    {
        public readonly Engine owner;
        public Engine Owner
        {
            get { return owner; }
        }

        public Entity(Engine _owner)
        {
            owner = _owner;
        }
    }
}
