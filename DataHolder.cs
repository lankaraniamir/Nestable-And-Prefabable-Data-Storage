using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static NestableDataStores.DataFilePaths;

namespace NestableDataStores {

// Note: Holder only holds references to assets that are not optional
// For optional assets, have the holder reference a data nest that creates and removes another nest as needed
public abstract class DataHolder : DataScriptable {

    [SerializeField] private string shortName;

    # if UNITY_EDITOR
    
    // IDataAsset interface
    sealed public override string Name { get => shortName; set => shortName = value; }
    sealed public override int? Index { get => null; set { Debug.Log("Error: Cannot reindex holder");} }
    sealed public override string Dir => $"{BASE_DIR}{GroupName}/{HOLDER_DIR_NAME}/";

    private void Reset() {
        // Wait for asset to be named before triggering other actions
        string assetName = Path.GetFileNameWithoutExtension(AssetPath);
        if (string.IsNullOrEmpty(assetName)) return;
        
        name = assetName;
        Name = assetName;
        MoveAssetToDefaultLocation();
        
        // Basically an on init
        if (Prefab == null) {
            CreatePrefab();
            CreateAllDataNests();
            OnEditTimerElapsed();
        }
    }

    private void CreatePrefab() {
        Prefab = new DataPrefabEditor(this);
        InitializePrefabComponents();
    }

    protected virtual void InitializePrefabComponents() { }

    protected void CreateAllDataNests() {
        foreach (FieldInfo field in ChildrenDataNestFields) {
            Type type = field.FieldType;

            // Make a version of the generic nested data creation method using the specific type and call it
            object[] parameters = { Missing.Value };
            MethodInfo methodInfo = typeof(DataNest).GetMethod(nameof(CreateNestedData), BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo genericMethod = methodInfo?.MakeGenericMethod(type);
            object dataNest = genericMethod?.Invoke(this, parameters);
            field.SetValue(this, dataNest);
        }
    }

    private void RenameAssets() {
        if (string.IsNullOrWhiteSpace(Name)) {
            Debug.Log("Name cannot be empty");
            Name = name;
            return;
        }

        if (AllNestedAssets.Any(asset => !string.IsNullOrEmpty(asset.RenameAsset(Name)))) {
            Debug.LogWarning("Data objects with this name already exists");
            Name = name;
            return;
        }
        AssetDatabase.SaveAssets();
        SetAllDirty();
    }
    
    [ContextMenu("Delete Data")]
    private void DeleteAll() {
        EditorApplication.delayCall += () => {
            List<string> failedPaths = new List<string>();
            AssetDatabase.DeleteAssets(AllNestedAssetPaths, failedPaths);
            AssetDatabase.SaveAssets();
            foreach (string path in failedPaths) {
                Debug.LogWarning($"Failed to delete asset at {path}");
            }
        };
    }
    
    // All update logic should be done in data nests and holder should only update name changes
    protected override void OnEditTimerElapsed() {
        if (Name != name) RenameAssets();
    }
    
    private IDataAsset[] AllNestedAssets {
        get {
            Queue<DataScriptable> dataQueue = new Queue<DataScriptable>(new DataScriptable[] {this});
            HashSet<IDataAsset> assets = new HashSet<IDataAsset>(dataQueue);
            while (dataQueue.Count > 0) {
                DataScriptable current = dataQueue.Dequeue();
                DataPrefabEditor prefab = current.Prefab;
                if (prefab != null) assets.Add(prefab);
                current.ChildrenDataNestAssets.ToList().ForEach(child => {
                    dataQueue.Enqueue(child);
                    assets.Add(child);
                });
            }
            return assets.ToArray();
        }
    }
    private string[] AllNestedAssetPaths {
        get {
            IDataAsset[] assets = AllNestedAssets;
            HashSet<string> paths = new HashSet<string>();
            foreach (IDataAsset asset in assets) paths.Add(asset.AssetPath);
            return paths.ToArray();
        }
    }
    public Dictionary<string, GameObject> NestedPrefabMap => AllNestedAssets
                .OfType<DataPrefabEditor>()
                .ToDictionary(prefab => prefab.NestName, prefab => prefab.Asset);
    
    private void SetAllDirty() { foreach (IDataAsset asset in AllNestedAssets) { asset.SetDirty(); } } 
    
    protected override double UpdateInterval => .3;
    #endif
}
}