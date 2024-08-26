using System;
using Enums;
using NaughtyAttributes;
using UnityEngine;

namespace NestableDataStores.ExampleUsage_ {
public class MyDataLook : DataNest {
    public enum MyColliderType {
        Polygon,
        Circle,
        Box
    }
    
    [SerializeField] private Vector2 defaultSize = Vector2.one;
    [SerializeField] private MyColliderType colliderType = MyColliderType.Polygon;
    [SerializeField] [ShowAssetPreview] private Sprite defaultSprite;
    [SerializeField] [ShowAssetPreview] private Sprite firingSprite;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color firingColor = Color.white;
    
    public Vector2 DefaultSize => defaultSize;
    public MyColliderType ColliderType => colliderType;
    public Sprite DefaultSprite => defaultSprite;
    public Sprite FiringSprite => firingSprite;
    public Color DefaultColor => defaultColor;
    public Color FiringColor => firingColor;
    
    
    # if UNITY_EDITOR
    public override string NestName => "Look";
    
    protected override void UpdateAssets() {
        // On changing default sprite and color
        var renderer = Prefab.GetOrCreateComponent<SpriteRenderer>();
        renderer.sprite = DefaultSprite;
        renderer.color = DefaultColor;
        renderer.drawMode = SpriteDrawMode.Sliced;
        
        // On changing default size
        Prefab.GetOrCreateComponent<Transform>().localScale = defaultSize;
        
        // On changing default color
        Prefab.GetOrCreateComponent<SpriteRenderer>().color = defaultColor;
        
        // On changing collider type
        Prefab.DeleteComponent<Collider2D>();
        switch (ColliderType) {
            case MyColliderType.Polygon:
                Prefab.GetOrCreateComponent<PolygonCollider2D>();
                break;
            case MyColliderType.Circle:
                Prefab.GetOrCreateComponent<CircleCollider2D>();
                break;
            case MyColliderType.Box:
                Prefab.GetOrCreateComponent<BoxCollider2D>();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    } 
    
    # endif
    
}
}
