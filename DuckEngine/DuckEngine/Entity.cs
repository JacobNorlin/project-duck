namespace DuckEngine
{
    /// <summary>
    /// A basic entity from which all entities inherit.
    /// </summary>
    public class Entity
    {
        public readonly Engine owner;
        public Engine Owner
        {
            get { return owner; }
        }

        /// <summary>
        /// The base constructor for all entites.
        /// </summary>
        /// <param name="_owner">The Engine which owns the current entity.</param>
        public Entity(Engine _owner)
        {
            owner = _owner;
        }
    }
}
