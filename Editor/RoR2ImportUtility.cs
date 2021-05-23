using UnityEditor;
using PassivePicasso;
using ThunderKit.uTinyRipper;
using ThunderKit.Core.Data;
using uTinyRipper;
using UnityEngine.Experimental.UIElements;
using UnityEngine;
using UnityEditor.Experimental.UIElements;

namespace PassivePicasso.RoR2Importer
{
    public class RoR2ImportUtility : ThunderKitSetting
    {

        public ClassIDType[] Types = {
            ClassIDType.Prefab,
            ClassIDType.PrefabInstance,
            ClassIDType.TextAsset,
            ClassIDType.TextMesh,
            ClassIDType.Texture,
            ClassIDType.Texture2D,
            ClassIDType.Material,
            ClassIDType.ProceduralMaterial,
            ClassIDType.Scene,
            ClassIDType.SceneAsset,
            ClassIDType.SceneSettings,
            ClassIDType.Animation,
            ClassIDType.AnimationClip,
            ClassIDType.AnimationManager,
            ClassIDType.Animator,
            ClassIDType.AnimatorController,
            ClassIDType.Avatar,
            ClassIDType.ComputeShader,
            ClassIDType.Shader,
            ClassIDType.ShaderVariantCollection,
            ClassIDType.Preset,
            ClassIDType.PresetManager,
            ClassIDType.Terrain,
            ClassIDType.TerrainData,
            ClassIDType.TerrainLayer,
            ClassIDType.Tree,
            ClassIDType.TagManager,
            ClassIDType.PhysicsManager,
            ClassIDType.GraphicsSettings,
            ClassIDType.NavMeshSettings,
            ClassIDType.PlayerSettings,
            ClassIDType.QualitySettings,
            ClassIDType.RenderSettings,
            ClassIDType.SceneSettings,
            ClassIDType.EditorSettings,
            ClassIDType.EditorBuildSettings,
            ClassIDType.EditorUserBuildSettings,
            ClassIDType.LightmapSettings,
            ClassIDType.Physics2DSettings,
        };

        public override void CreateSettingsUI(VisualElement rootElement)
        {
            var importUtilitySo = new SerializedObject(GetOrCreateSettings<RoR2ImportUtility>());

            VisualElement typesField = new VisualElement();
            string label = ObjectNames.NicifyVariableName(nameof(Types));
            PropertyField propertyField = new PropertyField
            {
                bindingPath = nameof(Types),
                label = label
            };
            typesField.Add(propertyField);

            rootElement.Add(typesField);

            rootElement.Bind(importUtilitySo);
        }
        [InitializeOnLoadMethod]
        static void InitializeImporter()
        {
            GetOrCreateSettings<RoR2ImportUtility>();
        }

        [MenuItem("Tools/RoR2 Asset Importer")]
        static void Import()
        {
            var ripper = ScriptableObject.CreateInstance<SimpleRipperInterface>();
            var tkSettings = GetOrCreateSettings<ThunderKitSettings>();
            var importUtility = GetOrCreateSettings<RoR2ImportUtility>();
            using (var progressBarLogger = new ProgressBarLogger())
                ripper.Load(tkSettings.GamePath, importUtility.Types, Platform.StandaloneWin64Player, TransferInstructionFlags.AllowTextSerialization, progressBarLogger);
        }
    }
}