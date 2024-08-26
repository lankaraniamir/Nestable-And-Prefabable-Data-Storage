using Enums;
using NaughtyAttributes;
using Sticker;
using Sticker.D;
using UnityEngine;

namespace NestableDataStores.ExampleUsage_ {
public class MyDataAttack : DataNest {
    // Serialized fields
    [SerializeField] private StickerAttackType attackType;
    [SerializeField] [ShowIf(nameof(ShowProjectile))] private float shotDelay = .25f;
    [SerializeField] [Expandable] [ShowIf(nameof(ShowProjectile))] private StickerDataAttackProjectile projectile;
    
    // Public properties
    public StickerAttackType AttackType => attackType;
    public float ShotDelay => shotDelay;
    public StickerDataAttackProjectile Projectile => projectile;

    # if UNITY_EDITOR
    public override string NestName => "Attack";
    
    protected override void UpdateAssets() {
        // On attack type changed
        switch (attackType) {
            case StickerAttackType.Projectile:
                Prefab.GetOrCreateComponent<StickerAttack>();
                projectile ??= CreateNestedData<StickerDataAttackProjectile>();
                break;
            case StickerAttackType.None:
                Prefab.DeleteComponent<StickerAttack>();
                projectile = DeleteNestedData(projectile);
                break;
            default:
                Prefab.GetOrCreateComponent<StickerAttack>();
                projectile = DeleteNestedData(projectile);
                break;
        }
    } 

    bool ShowProjectile() { return attackType == StickerAttackType.Projectile; }
    
    # endif
}
}


