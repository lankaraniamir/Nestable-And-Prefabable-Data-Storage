using UnityEditor;
using UnityEngine;
using static NestableDataStores.DataFilePaths;

namespace NestableDataStores {
public abstract class DataNest : DataScriptable {
    
    # if UNITY_EDITOR
    
    // IDataAsset interface
    [SerializeReference] [field: HideInInspector] private string _groupName;
    sealed public override string GroupName => _groupName;
    sealed public override string Name { get; set; }
    sealed public override int? Index { get; set; }
    public virtual string NestName => GetType().Name;
    sealed public override string Dir => $"{BASE_DIR}{GroupName}/{SO_DIR_NAME}/{NestName}/";

    // If true, this data nest will create data associated with a different prefab than the parent
    protected virtual bool UniquePrefab => false;

    public void Initialize(DataScriptable data, int? idx = null) {
        _groupName = data.GroupName;
        Name = data.Name;
        Index = idx;
        Prefab = UniquePrefab ? new DataPrefabEditor(this) : data.Prefab;
        
        IDataAsset.CreateAssetFolder(DesiredAssetPath);
        EditorApplication.delayCall += () => {
            AssetDatabase.CreateAsset(this, DesiredAssetPath);
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(this);
            
            OnFinishedInitializing();
            OnEditTimerElapsed();
        };
    }
    
    public void Delete() {
        OnBeginDeletion();
        if (UniquePrefab) Prefab.Delete();
        EditorApplication.delayCall += () => {
            AssetDatabase.DeleteAsset(AssetPath);
            AssetDatabase.SaveAssets();
        };
    }

    protected override void OnEditTimerElapsed() {
        UpdateAssets();
        Prefab?.SaveChanges();
    }

    protected virtual void UpdateAssets() { }

    // If desire customization of initialization and deletion logic
    protected virtual void OnFinishedInitializing() { } 
    protected virtual void OnBeginDeletion() {}

    # endif
}
}
