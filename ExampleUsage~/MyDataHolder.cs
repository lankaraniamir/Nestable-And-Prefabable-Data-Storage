using NaughtyAttributes;
using Sticker;
using Sticker.D;
using UnityEngine;

namespace NestableDataStores.ExampleUsage_ {

[CreateAssetMenu(fileName = "New", menuName = "MyData")]
sealed public class MyDataHolder : DataHolder {
    
    [Header("Metadata")]

    [SerializeField] [Expandable] private MyDataAttack attack;
    [SerializeField] [Expandable] private MyDataLook look;
    [SerializeField] [Expandable] private MyDataHealth health;
    [SerializeField] [Expandable] private MyDataMeta meta;
    

    public MyDataAttack Attack => attack;
    public MyDataHealth Health => health;
    public MyDataLook Look => look;
    public MyDataMeta Meta => meta;

    public const string GROUP_NAME = "My";
    
    # if UNITY_EDITOR
    public override string GroupName => GROUP_NAME;
    
    #endif

}
}