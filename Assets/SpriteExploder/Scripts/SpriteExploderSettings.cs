using UnityEngine;

namespace RCG.SpriteExploder
{
    public class SpriteExploderSettings : ScriptableObject
    {
        [Tooltip("The minimum particle pixel size a Sprite Exploder can use")]
        [SerializeField]
        int minimumParticlePixelSize = 8;
        public int MinimumParticlePixelSize
        {
            get
            {
                return Mathf.Max(1, minimumParticlePixelSize);
            }
        }

        [Tooltip("Allows Sprite Exploder particles to use collision physics")]
        [SerializeField]
        bool isCollidable = true;
        public bool IsCollidable
        {
            get
            {
                return isCollidable;
            }
        }
 
        const string settingsResourcePath = "SpriteExploderSettings";
        public static SpriteExploderSettings GetResource()
        {
            CreateAsset();
            SpriteExploderSettings resource = Resources.Load<SpriteExploderSettings>(settingsResourcePath);
            return resource;
        }

        const string settingsAssetPath = "Assets/SpriteExploder/Resources/SpriteExploderSettings.asset";
        public static void CreateAsset()
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