using Microsoft.Xna.Framework.Graphics;

namespace DuckEngine.Interfaces
{
    /// <summary>
    /// An interface for drawing 2D to the screen
    /// </summary>
    public interface IDraw2D
    {
        /// <summary>
        /// A function responsible for drawing 2D to the screen.
        /// </summary>
        /// <param name="spriteBatch">Exposes varioud draw methods.</param>
        void Draw2D(SpriteBatch spriteBatch);
    }
}
