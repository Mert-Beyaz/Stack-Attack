using UnityEngine;

[CreateAssetMenu(fileName = "PoolItem", menuName = "Pooling/Pool Item")]
public class PoolItemSO : ScriptableObject
{
    public PoolType PoolType;
    public GameObject Prefab;
    public int InitialSize = 10;
}