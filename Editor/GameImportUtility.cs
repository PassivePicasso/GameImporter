using System;
using System.Linq;
using ThunderKit.Core.Data;
using ThunderKit.uTinyRipper;
using UnityEditor;
using uTinyRipper;
using ThunderKit.Core.UIElements;
using System.Collections.Generic;
#if UNITY_2019_1_OR_NEWER
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

        private static ClassIDType[] GetAllClassIDTypes()
        {
            return (Enum.GetValues(typeof(ClassIDType)) as ClassIDType[]).OrderBy(c => $"{c}").ToArray();
        }

        public List<ClassIDType> UnassignedClassIdTypes = new List<ClassIDType>(GetAllClassIDTypes());
        public List<ClassIDType> AssignedClassIdTypes = new List<ClassIDType>(GetAllClassIDTypes());

        private ListView assignedTypes;
        private ListView unassignedTypes;
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

            assignedTypes = template.Q<ListView>("type-list");
#if UNITY_2022_1_OR_NEWER
            assignedTypes.onItemsChosen += OnRemoveItem;
#else
            assignedTypes.onItemChosen += OnRemoveItem;
#endif
            assignedTypes.bindItem = BindTypesItem;
            assignedTypes.makeItem = MakeTypesItem;
            assignedTypes.itemsSource = AssignedClassIdTypes;

            unassignedTypes = template.Q<ListView>("add-type-list");
#if UNITY_2022_1_OR_NEWER
            assignedTypes.onItemsChosen += OnAddItem;
#else
            assignedTypes.onItemChosen += OnAddItem;
#endif
            unassignedTypes.bindItem = BindAllTypesItem;
            unassignedTypes.makeItem = MakeTypesItem;
            unassignedTypes.selectionType = SelectionType.Multiple;

            UnassignedClassIdTypes.Clear();
            UnassignedClassIdTypes.AddRange(GetAllClassIDTypes().Where(cid => !AssignedClassIdTypes.Contains(cid)));
            unassignedTypes.itemsSource = UnassignedClassIdTypes;

            rootElement.Add(template);
            rootElement.Bind(importUtilitySo);
        }

        private void OnRemoveAllClicked()
        {
            AssignedClassIdTypes.Clear();
            UnassignedClassIdTypes.Clear();
            UnassignedClassIdTypes.AddRange(GetAllClassIDTypes());
            RefreshItems();
        }
        void RefreshItems()
        {

#if UNITY_2022_1_OR_NEWER
            assignedTypes.RefreshItems();
            unassignedTypes.RefreshItems();
#else
            assignedTypes.Refresh();
            unassignedTypes.Refresh();
#endif
        }
        private void OnAddAllClicked()
        {
            UnassignedClassIdTypes.Clear();
            AssignedClassIdTypes.Clear();
            AssignedClassIdTypes.AddRange(GetAllClassIDTypes());

            RefreshItems();
        }

        private void UpdateClassIDTypes(object obj, bool remove)
        {
            var cid = (ClassIDType)obj;
            if (remove)
            {
                AssignedClassIdTypes.Remove(cid);
                UnassignedClassIdTypes.Add(cid);
            }
            else
            {
                AssignedClassIdTypes.Add(cid);
                UnassignedClassIdTypes.Remove(cid);
            }
            RefreshItems();
        }


#if UNITY_2022_1_OR_NEWER
        private void OnRemoveItem(IEnumerable<object> classObjects)
#else
        private void OnRemoveItem(object idType)
#endif
        {
#if UNITY_2022_1_OR_NEWER
            foreach (var idType in classObjects.OfType<ClassIDType>().ToArray())
#endif
                UpdateClassIDTypes(idType, true);
        }

#if UNITY_2022_1_OR_NEWER
        private void OnAddItem(IEnumerable<object> classObjects)
#else
        private void OnAddItem(object idType)
#endif
        {
#if UNITY_2022_1_OR_NEWER
            foreach (var idType in classObjects.OfType<ClassIDType>().ToArray())
#endif
                UpdateClassIDTypes(idType, false);
        }

        private VisualElement MakeTypesItem() => new Label();
        private void BindTypesItem(VisualElement element, int index)
        {
            if (!(element is Label label)) return;

            label.text = $"{AssignedClassIdTypes[index]}";
        }

        private void BindAllTypesItem(VisualElement element, int index)
        {
            if (!(element is Label label)) return;

            label.text = $"{UnassignedClassIdTypes[index]}";
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
                ripper.Load(System.IO.Path.Combine(tkSettings.GamePath, tkSettings.GameExecutable), importUtility.AssignedClassIdTypes, Platform.StandaloneWin64Player, TransferInstructionFlags.AllowTextSerialization, progressBarLogger);

            AssetDatabase.Refresh();
        }
    }
}