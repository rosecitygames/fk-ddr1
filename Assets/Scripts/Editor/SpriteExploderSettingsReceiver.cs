using UnityEditor.Presets;

namespace RCG.SpriteExploder.Editor
{
    public class SpriteExploderSettingsReceiver : PresetSelectorReceiver
    {
        Preset initialValues;
        SpriteEploderWindowSettings currentSettings;
        SpriteExploderEditorWindow currentWindow;

        public void Init(SpriteEploderWindowSettings settings, SpriteExploderEditorWindow window)
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

            currentSettings.ApplySettings(currentWindow);
        }

        public override void OnSelectionClosed(Preset selection)
        {
            OnSelectionChanged(selection);
            DestroyImmediate(this);
        }
    }
}