using System;
using UnityEngine;

namespace IndieDevTools.Exploders
{
    /// <summary>
    /// Sprite exploder settings that globally override component properties.
    /// Useful for optimizing CPU expendenture on various devices/ machines.
    /// 
    /// Note, that getter and setters for the serialized fields use a runtime copy
    /// of the data. This prevents the scriptable object values from changing at runtime.
    /// 
    /// Also, useful if you want to expose the settings to the player. In that case,
    /// you would want to save the settings data and then later load the data and use the
    /// property setters. All without effecting scriptable object in the editor.
    /// </summary>
    public class SpriteExploderSettings : ScriptableObject
    {
        /// <summary>
        /// The minimum particle size. The larger the particle, the more performant.
        /// So, higher values are useful on slower CPU machines. 
        /// </summary>
        public int MinimumParticlePixelSize
        {
            get => RuntimeData.MinimumParticlePixelSize;
            set => RuntimeData.MinimumParticlePixelSize = value;
        }
        [SerializeField, Tooltip("The minimum particle pixel size a Sprite Exploder can use")]
        int minimumParticlePixelSize = 8;

        /// <summary>
        /// Whether or not the particles use collision physics. On slower CPU machines,
        /// disable for better performance.
        /// </summary>
        public bool IsCollidable
        {
            get => RuntimeData.IsCollidable;
            set => RuntimeData.IsCollidable = value;
        }
        [SerializeField, Tooltip("Allows Sprite Exploder particles to use collision physics")]
        bool isCollidable = true;

        /// <summary>
        /// Runtime copy of the properties
        /// </summary>
        SpriteExploderSettingsData RuntimeData
        {
            get
            {
                if (runtimeData == null)
                {
                    runtimeData = SpriteExploderSettingsData.Create(minimumParticlePixelSize, isCollidable);
                }
                return runtimeData;
            }
        }
        [NonSerialized]
        SpriteExploderSettingsData runtimeData = null;

        const string settingsResourcePath = "SpriteExploderSettings";
        /// <summary>
        /// Gets the loaded sprite exploder settings component resource.
        /// </summary>
        public static SpriteExploderSettings GetResource()
        {
            CreateAsset();
            SpriteExploderSettings resource = Resources.Load<SpriteExploderSettings>(settingsResourcePath);
            return resource;
        }

        const string settingsAssetPath = "Assets/SpriteExploder/Resources/SpriteExploderSettings.asset";
        /// <summary>
        /// Gets the setting asset if it exists or creates a new one.
        /// </summary>
        static void CreateAsset()
        {
#if UNITY_EDITOR        
            SpriteExploderSettings resource = UnityEditor.AssetDatabase.LoadAssetAtPath<SpriteExploderSettings>(settingsAssetPath);
            bool isNotExistingResource = resource == null;

            if (isNotExistingResource)
            {
                SpriteExploderSettings asset = CreateInstance<SpriteExploderSettings>();

                UnityEditor.AssetDatabase.CreateAsset(asset, settingsAssetPath);
                UnityEditor.AssetDatabase.SaveAssets();
            }
#endif
        }
    }
}