using UnityEngine;

namespace NestableDataStores.ExampleUsage_ {

// [CreateAssetMenu(fileName = "Temporary", menuName = "HealthDataTemp")]
public class MyDataHealth : DataNest {
    [SerializeField] private float maxHealth = 1f;
    public float MaxHealth => maxHealth;
    
    # if UNITY_EDITOR
    public override string NestName => "Health";

# endif
}
}
