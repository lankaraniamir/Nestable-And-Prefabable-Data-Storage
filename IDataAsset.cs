using System.IO;
using UnityEditor;
using UnityEngine;

namespace NestableDataStores {
public interface IDataAsset {
    
    #if UNITY_EDITOR

    // enum DataType {
    //     Holder = 1,
    //     Prefab = 2,
    //     ScriptableObject = 3,
    // }
    
    // public UnityEngine.Object Reference { get; }
    // public string Ext { get; } // not really needed rn
    // public string DataTypeName { get; } // not really needed rn

    // ReSharper disable once ValueParameterNotUsed
    // public int? Index { get => null; set { } } 
    // public string IDString => Index != null ? Name + "_" + Index : Name;
    
    public Object Reference { get; } 
    public string Name { get; set; } 
    public int? Index { get; set; } 
    public string Ext { get; }
    public string Dir { get; }
    
    
    // public string DesiredAssetPath => Dir + IDString + Ext;
    public string IDString => Index != null ? $"{Name}_{Index}" : Name;
    public string DesiredAssetPath => $"{Dir}{IDString}{Ext}";
    public string AssetPath => AssetDatabase.GetAssetPath(Reference);
    
    
    public void SetDirty() => EditorUtility.SetDirty(Reference);
    public string RenameAsset(string sName) {
        Name = sName;
        return AssetDatabase.RenameAsset(AssetPath, IDString);
    }
    public string ReindexAsset(int? index = null) {
        Index = index;
        return AssetDatabase.RenameAsset(AssetPath, IDString);
    }
    public static bool SwapAssetIndexes<T>(T a, T b) where T : IDataAsset {
        int? idxA = a.Index;
        int? idxB = b.Index;
        if (idxA == null || idxB == null) return false;
        AssetDatabase.RenameAsset(b.AssetPath, b.IDString + "_temp");

        a.Index = idxB;
        b.Index = idxA;
        AssetDatabase.RenameAsset(a.AssetPath, a.IDString);
        AssetDatabase.RenameAsset(b.AssetPath, b.IDString);
        return false;
    }
    public void MoveAssetToDefaultLocation() {
        if (AssetPath == DesiredAssetPath) return;
        
        if (!string.IsNullOrEmpty(AssetDatabase.ValidateMoveAsset(AssetPath, DesiredAssetPath))) 
            CreateAssetFolder(DesiredAssetPath);
            
        EditorApplication.delayCall += () => {
            AssetDatabase.MoveAsset(AssetPath, DesiredAssetPath);
            AssetDatabase.SaveAssets();
            SetDirty();
        };
    }
    public static void CreateAssetFolder(string path) {
        string folder = Path.GetDirectoryName(path);
        if (!AssetDatabase.IsValidFolder(folder) && folder != null) {
            Directory.CreateDirectory(folder);
            AssetDatabase.Refresh();
        }
    }
    // CreateAsset
    // DeleteAsset



    #endif

}
}

// public string GroupName { get;  }
// public string NestName { get;  }
// public string Dir { get;  }
