using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType{
    spike,
    cell,
    booster,
}

public enum CellOrientation{
    up = 0,
    right = 90,
    down = 180,
    left = 270,
    all = -1
}

[CreateAssetMenu(fileName="New Cell", menuName="Data/Cell")]
public class CellDefinition : ScriptableObject
{
    public string Name;
    public GameObject Prefab;
    public CellType Type;
    public int rotation;
    public Sprite sprite;
    public CellOrientation AttachmentPoint;
    public const int CellHPIncrease = 20;
    public const float CellRotationIncrease = 50;
    public string Description;
    public bool Rotatable;

    public CellDefinition CloneCellDefinition(){
        CellDefinition newCell = CreateInstance<CellDefinition>();
        newCell.sprite = sprite;
        newCell.name = name;
        newCell.rotation = rotation;
        newCell.Prefab = Prefab;
        newCell.Description = Description;
        newCell.Type = Type;
        newCell.AttachmentPoint = AttachmentPoint;
        newCell.Rotatable = Rotatable;

        return newCell;
    }

}
