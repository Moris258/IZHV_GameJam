using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public CellType type;
    public int cost;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCellDestroy(PlayerController playerController){
        playerController.IncreaseUpgradePoints(cost);
        switch(type){
            case CellType.cell:
                playerController.IncreaseMaxHitPoints(-CellDefinition.CellHPIncrease);
            break;
        }
    }
}
