using Enums;
using NaughtyAttributes;
using UnityEngine;

namespace NestableDataStores.ExampleUsage_ {
public class MyDataMeta : DataNest {
    [SerializeField] private RarityType rarity;
    [SerializeField] private int price;
    [SerializeField] private string displayName;
    [SerializeField] [ResizableTextArea] private string description;

    public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? displayName : Name;
    public string Description => description;
    public RarityType Rarity => rarity;
    public int Price => price;
    
    # if UNITY_EDITOR
    public override string NestName => "Meta";
    # endif
}
}