using UnityEngine;

namespace RCG.SpriteExploder
{
    public class SpriteExploderSettings : ScriptableObject
    {
        [SerializeField]
        int particlePixelSize = 8;
        public int ParticlePixelSize
        {
            get
            {
                return particlePixelSize;
            }
        }

        [SerializeField]
        bool isUsingCollision = true;
        public bool IsUsingCollision
        {
            get
            {
                return isUsingCollision;
            }
        }
 
        const string settingsResourcePath = "SpriteExploderSettings";
        public static SpriteExploderSettings GetResource()
        {
            CreateAsset();
            SpriteExploderSettings resource = Resources.Load<SpriteExploderSettings>(settingsResourcePath);
            return resource;
        }

        const string settingsAssetPath = "Assets/Resources/SpriteExploderSettings.asset";
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