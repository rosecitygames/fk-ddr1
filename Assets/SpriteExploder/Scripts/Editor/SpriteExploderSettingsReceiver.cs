using UnityEditor.Presets;

namespace RCG.SpriteExploder.Editor
{
    public class SpriteExploderSettingsReceiver : PresetSelectorReceiver
    {
        Preset initialValues;
        SpriteExploderSettings currentSettings;
        SpriteExploderEditorWindow currentWindow;

        public void Init(SpriteExploderSettings settings, SpriteExploderEditorWindow window)
        {
            currentWindow = window;
            currentSettings = settings;
            initialValues = new Preset(currentSettings);
        }

        public override void OnSelectionChanged(Preset selection)
        {
            if (selection != null)
            {
                selection.ApplyTo(currentSettings);
            }
            else
            {
                initialValues.ApplyTo(currentSettings);
            }
        }

        public override void OnSelectionClosed(Preset selection)
        {
            OnSelectionChanged(selection);
            DestroyImmediate(this);
        }
    }
}