using System;
using UnityEngine;

namespace IndieDevTools.Exploders
{
    /// <summary>
    /// Serialized settings data used by the SpriteExploderSettings
    /// scriptable object at runtime. Optionally, this class can be used
    /// to save parameters exposed to the player.
    /// </summary>
    [Serializable]
    public class SpriteExploderSettingsData
    {
        /// <summary>
        /// The minimum particle size. The larger the particle, the more performant.
        /// So, higher values are useful on slower CPU machines. 
        /// </summary>
        public int MinimumParticlePixelSize
        {
            get
            {
                return Mathf.Max(1, minimumParticlePixelSize);
            }
            set
            {
                minimumParticlePixelSize = value;
            }
        }
        [SerializeField]
        int minimumParticlePixelSize = 8;

        /// <summary>
        /// Whether or not the particles use collision physics. On slower CPU machines,
        /// disable for better performance.
        /// </summary>
        public bool IsCollidable
        {
            get
            {
                return isCollidable;
            }
            set
            {
                isCollidable = value;
            }
        }
        [Tooltip("Allows Sprite Exploder particles to use collision physics")]
        [SerializeField]
        bool isCollidable = true;

        /// <summary>
        /// Helper method used to create settings data.
        /// </summary>
        /// <param name="minimumParticlePixelSize">The minimum particle size in pixel units.</param>
        /// <param name="isCollidable">Whether or not particles will be collidable.</param>
        /// <returns></returns>
        public static SpriteExploderSettingsData Create(int minimumParticlePixelSize, bool isCollidable)
        {
            return new SpriteExploderSettingsData
            {
                MinimumParticlePixelSize = minimumParticlePixelSize,
                IsCollidable = isCollidable
            };
        }
    }
}
