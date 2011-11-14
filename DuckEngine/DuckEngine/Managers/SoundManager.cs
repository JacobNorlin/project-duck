using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckEngine.Sound
{
    /// <summary>
    /// A class which handles playback of sounds.
    /// </summary>
    public class SoundManager
    {
        /// <summary>
        /// Play a sound once
        /// </summary>
        /// <param name="soundName">The name of the sound to play.</param>
        void Play(string soundName)
        {

        }

        /// <summary>
        /// Return a Sound object to allow repeated and continous playback.
        /// </summary>
        /// <param name="soundName">The name of the sound</param>
        /// <returns>The Sound object.</returns>
        Sound GetSound(string soundName)
        {
            return default(Sound);
        }
    }
}
