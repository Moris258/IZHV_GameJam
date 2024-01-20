using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public CellType type;
    public int cost;

    void Awake(){
        if(transform.parent.tag == "Player"){
            PlayerController playerController = transform.parent.GetComponent<PlayerController>();
            switch(type){
                case CellType.cell:
                    playerController.IncreaseMaxHitPoints(CellDefinition.CellHPIncrease);
                break;

                case CellType.booster:
                    playerController.baseRotationSpeed += CellDefinition.CellRotationIncrease;
                break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy(){
        if(transform.parent.tag == "Player"){
            PlayerController playerController = transform.parent.GetComponent<PlayerController>();
            switch(type){
                case CellType.cell:
                    playerController.IncreaseMaxHitPoints(-CellDefinition.CellHPIncrease);
                break;

                case CellType.booster:
                    playerController.baseRotationSpeed -= CellDefinition.CellRotationIncrease;
                break;
            }
        }
    }
}
