using System;
using System.Linq;
using ThunderKit.Core.Data;
using ThunderKit.uTinyRipper;
using UnityEditor;
using uTinyRipper;
using ThunderKit.Core.UIElements;
#if UNITY_2019_1_OR_NEWER
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#else
using UnityEditor.Experimental.UIElements;
using UnityEngine.Experimental.UIElements;
#endif

namespace PassivePicasso.GameImporter
{
    public class GameImportUtility : ThunderKitSetting
    {
        private ClassIDType[] AllClassIDTypes = GetAllClassIDTypes();

        private static ClassIDType[] GetAllClassIDTypes()
        {
            return (Enum.GetValues(typeof(ClassIDType)) as ClassIDType[]).OrderBy(c => $"{c}").ToArray();
        }

        public ClassIDType[] ClassIDTypes = (Enum.GetValues(typeof(ClassIDType)) as ClassIDType[]).OrderBy(c => $"{c}").ToArray();

        private ListView typeList;
        private ListView addTypeList;
        private Button addAllTypes, removeAllTypes;
        private string searchValue;
        public override void CreateSettingsUI(VisualElement rootElement)
        {
            var importUtilitySo = new SerializedObject(GetOrCreateSettings<GameImportUtility>());

            var script = MonoScript.FromScriptableObject(this);
            var scriptPath = AssetDatabase.GetAssetPath(script);
            var scriptDir = System.IO.Path.GetDirectoryName(scriptPath);
            var templatesDir = System.IO.Path.Combine(scriptDir, "UIToolkit");
            var importerSettingsTemplate = System.IO.Path.Combine(templatesDir, "UnityGameImporter.uxml").Replace("\\", "/");
            var importerStyle = System.IO.Path.Combine(templatesDir, "UnityGameImporter.uss");
            var template = TemplateHelpers.LoadTemplateInstance(importerSettingsTemplate);
            TemplateHelpers.AddSheet(template, importerStyle);

            addAllTypes = template.Q<Button>("add-all-types");
            addAllTypes.clickable.clicked += OnAddAllClicked;

            removeAllTypes = template.Q<Button>("remove-all-types");
            removeAllTypes.clickable.clicked += OnRemoveAllClicked;

            typeList = template.Q<ListView>("type-list");
#if UNITY_2020_1_OR_NEWER
            typeList.onItemsChosen += OnRemoveItem;
#elif UNITY_2018_1_OR_NEWER
            typeList.onItemChosen += OnRemoveItem;
#endif
            typeList.bindItem = BindTypesItem;
            typeList.makeItem = MakeTypesItem;
            typeList.itemsSource = ClassIDTypes;

            addTypeList = template.Q<ListView>("add-type-list");
#if UNITY_2020_1_OR_NEWER
            typeList.onItemsChosen += OnAddItem;
#elif UNITY_2018_1_OR_NEWER
            typeList.onItemChosen += OnAddItem;
#endif
            addTypeList.bindItem = BindAllTypesItem;
            addTypeList.makeItem = MakeTypesItem;
            addTypeList.selectionType = SelectionType.Multiple;

            UpdateAllClassIDTypes();

            rootElement.Add(template);
            rootElement.Bind(importUtilitySo);
        }

        private void OnRemoveAllClicked()
        {
            ClassIDTypes = Array.Empty<ClassIDType>();
            AllClassIDTypes = GetAllClassIDTypes();
            typeList.itemsSource = ClassIDTypes;
            addTypeList.itemsSource = AllClassIDTypes;
        }

        private void OnAddAllClicked()
        {
            ClassIDTypes = GetAllClassIDTypes();
            typeList.itemsSource = ClassIDTypes;
            UpdateAllClassIDTypes();
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
            var acidt = Enum.GetValues(typeof(ClassIDType))
                                      .OfType<ClassIDType>()
                                      .Where(cid => !ClassIDTypes.Contains(cid));

            if (!string.IsNullOrWhiteSpace(searchValue))
                acidt = acidt.Where(c => $"{c}".IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) > -1);

            AllClassIDTypes = acidt.OrderBy(c => $"{c}").ToArray();

            addTypeList.itemsSource = AllClassIDTypes;
        }

#if UNITY_2020_1_OR_NEWER
        private void OnRemoveItem(IEnumerable<object> objs) => UpdateClassIDTypes(objs.First(), true);
#else
        private void OnRemoveItem(object obj) => UpdateClassIDTypes(obj, true);
#endif

#if UNITY_2020_1_OR_NEWER
        private void OnAddItem(IEnumerable<object> objs) => UpdateClassIDTypes(objs.First(), false);
#else
        private void OnAddItem(object obj) => UpdateClassIDTypes(obj, false);
#endif

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

        [MenuItem("Tools/Game Asset Importer")]
        static void Import()
        {
            var ripper = CreateInstance<SimpleRipperInterface>();
            var tkSettings = GetOrCreateSettings<ThunderKitSettings>();
            var importUtility = GetOrCreateSettings<GameImportUtility>();
            using (var progressBarLogger = new ProgressBarLogger())
                ripper.Load(System.IO.Path.Combine(tkSettings.GamePath, tkSettings.GameExecutable), importUtility.ClassIDTypes, Platform.StandaloneWin64Player, TransferInstructionFlags.AllowTextSerialization, progressBarLogger);

            AssetDatabase.Refresh();
        }
    }
}