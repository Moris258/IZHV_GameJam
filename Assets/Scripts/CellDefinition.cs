using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType{
    spike,
    cell,
    booster,
}

[CreateAssetMenu(fileName="New Cell", menuName="Data/Cell")]
public class CellDefinition : ScriptableObject
{
    public string Name;
    public GameObject Prefab;
    public CellType Type;
    public float rotation;
    public Sprite sprite;
    public const int CellHPIncrease = 20;

}
