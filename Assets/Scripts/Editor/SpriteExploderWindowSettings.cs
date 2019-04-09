using UnityEngine;

namespace RCG.SpriteExploder.Editor
{
    public class SpriteEploderWindowSettings : ScriptableObject
    {
        [SerializeField]
        int defaultPixelSize = 8;
        [SerializeField]
        bool isUsingCollision = true;

        public void Init(SpriteExploderEditorWindow window)
        {
            defaultPixelSize = window.DefaultPixelSize;
            isUsingCollision = window.IsUsingCollision;
        }

        public void ApplySettings(SpriteExploderEditorWindow window)
        {
            window.DefaultPixelSize = defaultPixelSize;
            window.IsUsingCollision = isUsingCollision;
            window.Repaint();
        }
    }
}