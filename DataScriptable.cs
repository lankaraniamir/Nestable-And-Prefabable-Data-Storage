using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static NestableDataStores.DataFilePaths;

namespace NestableDataStores {
public abstract class DataScriptable : ScriptableObject, IDataAsset {
    
    #if UNITY_EDITOR

    // IDataAsset interface
    public Object Reference => this;
    
    public abstract string Name { get; set; }
    public abstract int? Index { get; set; }
    public string IDString => Index != null ? $"{Name}_{Index}" : Name;
    
    public abstract string GroupName { get; }
    public string Ext => DATA_EXT;
    public abstract string Dir { get;  }
    public string DesiredAssetPath => $"{Dir}{IDString}{Ext}";
    
    public string AssetPath => AssetDatabase.GetAssetPath(this);

    // Prefab management
    [field: SerializeReference] [field: HideInInspector] public DataPrefabEditor Prefab { get; protected set; }

    // Nested data management
    protected T CreateNestedData<T>(int? idx = null) where T : DataNest {
        T newInstance = CreateInstance<T>();
        newInstance.Initialize(this, idx);
        return newInstance;
    }
    protected static T DeleteNestedData<T>(T asset) where T : DataNest { // TODO: Remove and only allow direct delete
        asset?.Delete();
        return null;
    }

    protected FieldInfo[] ChildrenDataNestFields => 
        GetType()
        .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
        .Where(field => field.FieldType.IsSubclassOf(typeof(DataNest)))
        .ToArray();
    protected DataNest[] ChildrenDataNests => 
        GetType()
        .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
        .Where(field => field.FieldType.IsSubclassOf(typeof(DataNest)))
        .Select(field => field.GetValue(this) as DataNest)
        .ToArray();
    public DataNest[] ChildrenDataNestAssets => 
        GetType()
        .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
        .Where(field => field.FieldType.IsSubclassOf(typeof(DataNest)))
        .Select(field => field.GetValue(this) as DataNest)
        .Where(child => child != null)
        .ToArray();
    
    // Update handler
    protected virtual double UpdateInterval => .1;
    private bool _isDirty;
    private double _nextUpdateTime;

    public void MoveAssetToDefaultLocation() {
        if (AssetPath == DesiredAssetPath) return;
        
        if (!string.IsNullOrEmpty(AssetDatabase.ValidateMoveAsset(AssetPath, DesiredAssetPath))) 
            IDataAsset.CreateAssetFolder(DesiredAssetPath);
            
        AssetDatabase.MoveAsset(AssetPath, DesiredAssetPath);
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(this);
    }
    
    void OnEnable() {
        _nextUpdateTime = EditorApplication.timeSinceStartup + UpdateInterval;
        EditorApplication.update += OnEditorUpdate;
    }
    void OnDestroy() {
        EditorApplication.update -= OnEditorUpdate;
    }
    void OnEditorUpdate() {
        if (!_isDirty || EditorApplication.timeSinceStartup < _nextUpdateTime) return;
        _isDirty = false;
        OnEditTimerElapsed();
    }
    void OnValidate() {
        _nextUpdateTime = EditorApplication.timeSinceStartup + UpdateInterval;
        _isDirty = true;
    }
    protected abstract void OnEditTimerElapsed();
    
    public new void SetDirty() {
        EditorUtility.SetDirty(this);
    }
    
    #endif
    
}
}
