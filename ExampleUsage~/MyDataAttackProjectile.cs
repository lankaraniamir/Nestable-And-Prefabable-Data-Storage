using UnityEngine;

namespace NestableDataStores.ExampleUsage_ {

[System.Serializable]
public class MyDataAttackProjectile : DataNest {
    [SerializeField] private Vector2 size = Vector2.one;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Color color;
    [SerializeField] private float homingType;

    [SerializeField] private float speed = 20f;
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private float range = 30;
    [SerializeField] private float damage = 1;

    public Vector2 Size => size;
    public Sprite Sprite => sprite;
    public Color? DefaultColor => color != Color.clear ? color : null;
    public float HomingType => homingType;
    
    public float Speed => speed;
    public float Duration => duration;
    public float Range => range;
    public float Damage => damage;
    
    
    # if UNITY_EDITOR
    protected override bool UniquePrefab => true;
    public override string NestName => "Projectile";
    
    protected override void UpdateAssets() {
        // On changing default size
        Prefab.GetOrCreateComponent<Transform>().localScale = size;
        
        // On changing default sprite
        Prefab.GetOrCreateComponent<SpriteRenderer>().sprite = sprite;
        
        // On changing default color
        Prefab.GetOrCreateComponent<SpriteRenderer>().color = color;
    } 
    
    # endif
}
}
