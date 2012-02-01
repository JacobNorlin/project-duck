namespace DuckEngine.Interfaces
{
    public interface ICollideEvent
    {
        void Collide(Entity other);
        bool BroadPhaseFilter(Entity other);
    }
}
