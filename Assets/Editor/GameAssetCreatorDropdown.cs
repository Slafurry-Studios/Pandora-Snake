using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Slafurry.Utils.Attributes;

namespace Slafurry.Editor.GameAssetCreator
{
    /// <summary>
    /// Searchable dropdown listing every ScriptableObject marked with
    /// [GameAssetCreator]. Category strings support "/" to build nested
    /// submenus - e.g. Category = "Audio/Music" produces:
    ///   Audio
    ///     Music
    ///       Music Library
    ///
    /// Ordering is fully explicit via the attribute's Order value,
    /// completely independent of Unity's native Create menu priority
    /// system - no bubbling to parent submenus like [CreateAssetMenu] has.
    /// Search is built into AdvancedDropdown automatically.
    /// </summary>
    public class GameAssetCreatorDropdown : AdvancedDropdown
    {
        private class Entry
        {
            public Type Type;
            public string Category;
            public string DisplayName;
            public int Order;
        }

        private class TypeDropdownItem : AdvancedDropdownItem
        {
            public readonly Type Type;
            public TypeDropdownItem(string displayName, Type type) : base(displayName) => Type = type;
        }

        private readonly List<Entry> _entries;

        public GameAssetCreatorDropdown(AdvancedDropdownState state) : base(state)
        {
            _entries = ScanEntries();
            minimumSize = new Vector2(280, 350);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Create Game Asset");
            var folderCache = new Dictionary<string, AdvancedDropdownItem> { [""] = root };

            // sort by category path first, then by Order within that same
            // category, so siblings render in exactly the order specified
            var sorted = _entries.OrderBy(e => e.Category).ThenBy(e => e.Order).ToList();

            foreach (var entry in sorted)
            {
                var parent = GetOrCreateFolder(root, folderCache, entry.Category);
                parent.AddChild(new TypeDropdownItem(entry.DisplayName, entry.Type));
            }

            return root;
        }

        private AdvancedDropdownItem GetOrCreateFolder(AdvancedDropdownItem root, Dictionary<string, AdvancedDropdownItem> cache, string categoryPath)
        {
            if (string.IsNullOrEmpty(categoryPath)) return root;
            if (cache.TryGetValue(categoryPath, out var existing)) return existing;

            var parts = categoryPath.Split('/');
            AdvancedDropdownItem current = root;
            string builtPath = "";

            foreach (var part in parts)
            {
                builtPath = string.IsNullOrEmpty(builtPath) ? part : $"{builtPath}/{part}";

                if (!cache.TryGetValue(builtPath, out var folder))
                {
                    folder = new AdvancedDropdownItem(part);
                    current.AddChild(folder);
                    cache[builtPath] = folder;
                }

                current = folder;
            }

            return current;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            if (item is TypeDropdownItem typeItem)
                CreateAsset(typeItem.Type, typeItem.name);
        }

        private static List<Entry> ScanEntries()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(SafeGetTypes)
                .Where(t => typeof(ScriptableObject).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => new { Type = t, Attr = t.GetCustomAttribute<GameAssetCreatorAttribute>() })
                .Where(x => x.Attr != null)
                .Select(x => new Entry
                {
                    Type = x.Type,
                    Category = x.Attr.Category,
                    DisplayName = x.Attr.DisplayName,
                    Order = x.Attr.Order
                })
                .ToList();
        }

        private static IEnumerable<Type> SafeGetTypes(Assembly asm)
        {
            try { return asm.GetTypes(); }
            catch { return Array.Empty<Type>(); }
        }

        private static void CreateAsset(Type type, string displayName)
        {
            string folder = GetSelectedFolderOrDefault();
            string fileName = $"New {displayName}.asset";
            string path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{fileName}");

            var instance = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(instance, path);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = instance;
        }

        private static string GetSelectedFolderOrDefault()
        {
            foreach (var obj in Selection.GetFiltered<UnityEngine.Object>(SelectionMode.Assets))
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (AssetDatabase.IsValidFolder(path)) return path;
                return Path.GetDirectoryName(path);
            }
            return "Assets";
        }
    }
}
