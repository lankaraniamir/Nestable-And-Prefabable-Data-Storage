using System;
using UnityEditor;
using UnityEngine;
using static NestableDataStores.DataFilePaths;
using Object = UnityEngine.Object;

namespace NestableDataStores {

// sealed public class DataPrefabEditor : UnityEngine.Object, IDataAsset {
[Serializable]
sealed public class DataPrefabEditor : IDataAsset {

    #if UNITY_EDITOR
    
    // IDataAsset interface
    [field: SerializeReference, HideInInspector]
    public GameObject Asset { get; private set; }
    public Object Reference => Asset;
    
    [field: SerializeReference, HideInInspector]
    private DataScriptable Data { get; set; }

    public string Name { get; set; }
    public int? Index { get; set; }
    public string GroupName { get; }
    public string NestName { get; }
    
    public string Ext => PREFAB_EXT;
    public string Dir => $"{BASE_DIR}{GroupName}/{PREFAB_DIR_NAME}/{NestName}/";
    public string IDString => Index != null ? $"{Name}_{Index}" : Name;
    public string DesiredAssetPath => $"{Dir}{IDString}{Ext}";
    public string AssetPath => AssetDatabase.GetAssetPath(Asset);
    public DataPrefabEditor(DataScriptable data) {
        Data = data;
        Name = data.Name;
        Index = data.Index;
        GroupName = data.GroupName;
        if (data is DataNest nest) NestName = nest.NestName;
        else NestName = "Main";
        // CreatePrefab(); -- using lazy loading to avoid creation when not necessary
    }
    

    private void CreatePrefab() {
        IDataAsset.CreateAssetFolder(DesiredAssetPath);
        GameObject prefab = EditorUtility.CreateGameObjectWithHideFlags(IDString, HideFlags.HideInHierarchy);
        Asset = PrefabUtility.SaveAsPrefabAsset(prefab, DesiredAssetPath);
        Object.DestroyImmediate(prefab);
        EditorUtility.SetDirty(Asset);
    }

    public void Delete() { // TODO: Return prefab editor so it can be set to null
        EditorApplication.delayCall += () => {
            AssetDatabase.DeleteAsset(AssetPath);
            AssetDatabase.SaveAssets();
        };
    }
    
    public T GetOrCreateComponent<T>() where T : Component {
        // Lazy load so that no prefab made when not needed
        if (Asset == null) CreatePrefab();
        
        if (!Asset.TryGetComponent(out T newComponent)) {
            newComponent = Asset.AddComponent<T>();
            if (newComponent is IDataHandler handler) handler.SetData(Data);
        }
        return newComponent;
    }
    
    public T DeleteComponent<T>() where T : Component {
        T component = Asset.GetComponent<T>();
        if (component != null) Object.DestroyImmediate(component, true);
        return null;
    }
    
    public void SaveChanges() {
        PrefabUtility.SavePrefabAsset(Asset);
        EditorUtility.SetDirty(Asset);
        EditorUtility.SetDirty(Data);
    }
    
    public void SetDirty() {
        EditorUtility.SetDirty(Asset);
    }
    
    #endif
}
}
