namespace DuckEngine.Interfaces
{
    public interface ICollide
    {
        void Collide(Entity other);
        bool BroadPhaseFilter(Entity other);
    }
}
