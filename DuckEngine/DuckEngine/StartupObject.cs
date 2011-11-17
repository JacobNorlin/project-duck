namespace DuckEngine
{
    public abstract class StartupObject
    {
        public abstract void Initialize(Engine Owner);
        public abstract void LoadContent(Engine Owner);
    }
}
