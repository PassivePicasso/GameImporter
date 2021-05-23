using System;
using System.Linq;
using ThunderKit.Core.Data;
using ThunderKit.uTinyRipper;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEngine.Experimental.UIElements;
using uTinyRipper;

namespace PassivePicasso.GameImporter
{
    public class GameImportUtility : ThunderKitSetting
    {

        private ClassIDType[] AllClassIDTypes = (Enum.GetValues(typeof(ClassIDType)) as ClassIDType[]).OrderBy(c => $"{c}").ToArray();
        public ClassIDType[] ClassIDTypes = {
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

        private ListView typeList;
        private ListView addTypeList;
        private string searchValue;
        public override void CreateSettingsUI(VisualElement rootElement)
        {
            var importUtilitySo = new SerializedObject(GetOrCreateSettings<GameImportUtility>());

            var importerRoot = new VisualElement();
            importerRoot.AddStyleSheetPath("Packages/com.passivepicasso.unitygameimporter/Editor/UIToolkit/UnityGameImporter.uss");
            importerRoot.AddToClassList("importer-root");

            var typesField = new VisualElement();
            typeList = new ListView(ClassIDTypes, (int)EditorGUIUtility.singleLineHeight, MakeTypesItem, BindTypesItem);
            var typesLabel = new Label($"Exported {ObjectNames.NicifyVariableName(nameof(ClassIDTypes))}");

            typesField.Add(typesLabel);
            typesField.Add(typeList);
            typesField.AddToClassList("grow");
            typesField.AddToClassList("types-field");
            typeList.AddToClassList("types-field-list");
            typeList.AddToClassList("grow");
            typeList.onItemChosen += OnRemoveItem;

            var addTypesField = new VisualElement();
            addTypeList = new ListView(AllClassIDTypes, (int)EditorGUIUtility.singleLineHeight, MakeTypesItem, BindAllTypesItem);
            var addTypesLabel = new Label($"{ObjectNames.NicifyVariableName(nameof(AllClassIDTypes))}");
            var searchElement = new VisualElement();
            searchElement.AddToClassList("searchfield");
            var searchLabel = new Label("Search");
            var searchField = new TextField();
            searchField.AddToClassList("grow");
            searchField.OnValueChanged(OnSearchChanged);
            searchElement.Add(searchLabel);
            searchElement.Add(searchField);

            addTypesField.Add(addTypesLabel);
            addTypesField.Add(searchElement);
            addTypesField.Add(addTypeList);
            addTypesField.AddToClassList("grow");
            addTypesField.AddToClassList("alltypes-field");
            addTypeList.AddToClassList("alltypes-field-list");
            addTypeList.AddToClassList("grow");
            addTypeList.onItemChosen += OnAddItem;

            importerRoot.Add(typesField);
            importerRoot.Add(addTypesField);
            rootElement.Add(importerRoot);

            UpdateAllClassIDTypes();
            rootElement.Bind(importUtilitySo);
        }

        private void UpdateClassIDTypes(object obj, bool remove)
        {
            var enumer = remove ? ClassIDTypes.Where(cid => !cid.Equals((ClassIDType)obj)) : ClassIDTypes.Append((ClassIDType)obj);
            ClassIDTypes = enumer.OrderBy(cid => $"{cid}").ToArray();

            typeList.itemsSource = ClassIDTypes;
            UpdateAllClassIDTypes();
        }

        private void UpdateAllClassIDTypes()
        {
            AllClassIDTypes = Enum.GetValues(typeof(ClassIDType))
                                      .OfType<ClassIDType>()
                                      .Where(cid => !ClassIDTypes.Contains(cid))
                                      .Where(c => $"{c}".IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) > -1)
                                      .OrderBy(c => $"{c}")
                                      .ToArray();
            addTypeList.itemsSource = AllClassIDTypes;
        }

        private void OnSearchChanged(ChangeEvent<string> evt)
        {
            searchValue = evt.newValue;
            UpdateAllClassIDTypes();
        }

        private void OnRemoveItem(object obj) => UpdateClassIDTypes(obj, true);
        private void OnAddItem(object obj) => UpdateClassIDTypes(obj, false);

        private VisualElement MakeTypesItem() => new Label();
        private void BindTypesItem(VisualElement element, int index)
        {
            if (!(element is Label label)) return;

            label.text = $"{ClassIDTypes[index]}";
        }

        private void BindAllTypesItem(VisualElement element, int index)
        {
            if (!(element is Label label)) return;

            label.text = $"{AllClassIDTypes[index]}";
        }

        [InitializeOnLoadMethod]
        static void InitializeImporter()
        {
            GetOrCreateSettings<GameImportUtility>();
        }

        [MenuItem("Tools/RoR2 Asset Importer")]
        static void Import()
        {
            var ripper = CreateInstance<SimpleRipperInterface>();
            var tkSettings = GetOrCreateSettings<ThunderKitSettings>();
            var importUtility = GetOrCreateSettings<GameImportUtility>();
            using (var progressBarLogger = new ProgressBarLogger())
                ripper.Load(tkSettings.GamePath, importUtility.ClassIDTypes, Platform.StandaloneWin64Player, TransferInstructionFlags.AllowTextSerialization, progressBarLogger);

            AssetDatabase.Refresh();
        }
    }
}