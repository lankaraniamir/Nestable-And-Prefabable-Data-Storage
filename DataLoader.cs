using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NestableDataStores {
public static class DataLoader {
    
    private static readonly Dictionary<Type, Dictionary<string, DataHolder>> _dataMap = new Dictionary<Type, Dictionary<string, DataHolder>>();
    private static readonly Dictionary<Type, Dictionary<string, Dictionary<string, GameObject>>> _prefabMap = new Dictionary<Type, Dictionary<string, Dictionary<string, GameObject>>>();
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize() {
        LoadData();
    }
    
    private static void LoadData() {
        IEnumerable<Type> holders = Assembly
            .GetAssembly(typeof(DataHolder))
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(DataHolder)));
        
        foreach (Type holder in holders) {
            _dataMap[holder] = new Dictionary<string, DataHolder>();
            _prefabMap[holder]["main"] = new Dictionary<string, GameObject>();
            Object[] dataArray = Resources.FindObjectsOfTypeAll(holder);
            foreach (Object data in dataArray) {
                DataHolder dataHolder = data as DataHolder;
                if (dataHolder == null) continue;
                _dataMap[holder][data.name] = dataHolder;
                foreach (KeyValuePair<string, GameObject> prefab in dataHolder.NestedPrefabMap) {
                    _prefabMap[holder][data.name][prefab.Key] = prefab.Value;
                }
            }
        }
    }
    
    public static T GetData<T>(string name) where T : DataHolder {
        return _dataMap[typeof(T)][name] as T;
    }
    public static GameObject GetMainPrefabData<T>(string name) where T : DataHolder {
        return _prefabMap[typeof(T)][name]["main"];
    }
    public static GameObject GetPrefabData<T>(string name, string nestName) where T : DataHolder {
        return _prefabMap[typeof(T)][name][nestName];
    }
}
}
