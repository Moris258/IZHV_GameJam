using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float baseRotationSpeed = 100f;
    public float rotationThreshold = 0.5f;
    public GameObject UpgradePointsText;
    private const float blockOffset = 1.28f;
    public int StartingPoints = 100;
    private int upgradePoints = 10;
    public int UpgradePoints{get {return upgradePoints;}}
    public int PassiveUpgradePointGain = 1;
    public float PassiveUpgradePointGainInterval = 5.0f;
    private float upgradePointGainTimer = 0.0f;
    private Rigidbody2D RB;
    public Rigidbody2D RigidBody{get {return RB;}}
    public const int playerCellCount = 5;
    public CellDefinition[,] PlayerCells;
    public List<CellDefinition> AvailableCellTypes = new List<CellDefinition>();
    private int CellsBeforeRebuild = 0;
    private float HPBeforeRebuild = 0f;

    void Awake(){
        PlayerCells = new CellDefinition[playerCellCount, playerCellCount];
    }

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        PlayerCells[2,2] = AvailableCellTypes[(int)CellType.cell].CloneCellDefinition();
        PlayerCells[1,2] = AvailableCellTypes[(int)CellType.spike].CloneCellDefinition();
        PlayerCells[3,2] = AvailableCellTypes[(int)CellType.booster].CloneCellDefinition();

        upgradePoints = StartingPoints;
        ResetPlayer();
        BuildPlayer(false);
        UpgradePointsChanged();
        GetComponent<HitpointManager>().IncreaseHP(20);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.GamePaused) {
            if(RB.simulated)
                RB.simulated = false;
            return;
        }
        
        if(!RB.simulated)
            RB.simulated = true;

        if(upgradePointGainTimer >= PassiveUpgradePointGainInterval){
            IncreaseUpgradePoints(PassiveUpgradePointGain);
            upgradePointGainTimer -= PassiveUpgradePointGainInterval;
        }
        upgradePointGainTimer += Time.deltaTime;
        
        float rotationSpeed = baseRotationSpeed * (5 / RB.mass);

        Vector2 localTarget = transform.InverseTransformPoint(CalculateLookTarget());
        if (localTarget.x < -rotationThreshold) {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        } else if (localTarget.x > rotationThreshold) {
            transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
        }
    }

    public bool CanBuildPlayer(){
        if(upgradePoints < GetPlayerCost()) return false;

        for(int row = 0; row < playerCellCount; row++){
            for(int col = 0; col < playerCellCount; col++){
                if(PlayerCells[row, col] == null) continue;
                CellDefinition cell = PlayerCells[row, col];

                bool valid = false;
                if(cell.AttachmentPoint == CellOrientation.all){
                    if(row != 0)
                        if(PlayerCells[row - 1, col] != null && PlayerCells[row - 1, col].Type == CellType.cell)
                            valid = true;
                    if(row != playerCellCount - 1)
                        if(PlayerCells[row + 1, col] != null && PlayerCells[row + 1, col].Type == CellType.cell)
                            valid = true;

                    if(col != 0)
                        if(PlayerCells[row, col - 1] != null && PlayerCells[row, col - 1].Type == CellType.cell)
                            valid = true;

                    if(col != playerCellCount - 1)
                        if(PlayerCells[row, col + 1] != null && PlayerCells[row, col + 1].Type == CellType.cell)
                            valid = true;
                }
                else{
                    int checkDir = cell.rotation + (int)cell.AttachmentPoint;
                    if(checkDir >= 360)
                        checkDir -= 360;
                
                    switch(checkDir){
                        case 0:
                        if(row != 0)
                            if(PlayerCells[row - 1, col] != null && PlayerCells[row - 1, col].Type == CellType.cell)
                                valid = true;
                        break;
                        case 90:
                        if(col != playerCellCount - 1)
                            if(PlayerCells[row, col + 1] != null && PlayerCells[row, col + 1].Type == CellType.cell)
                                valid = true;
                        break;
                        case 180:
                        if(row != playerCellCount - 1)
                            if(PlayerCells[row + 1, col] != null && PlayerCells[row + 1, col].Type == CellType.cell)
                                valid = true;
                        break;
                        case 270:
                        if(col != 0)
                            if(PlayerCells[row, col - 1] != null && PlayerCells[row, col - 1].Type == CellType.cell)
                                valid = true;
                        break;
                    }
                }

                if(row == playerCellCount / 2 && col == playerCellCount / 2)
                    valid = true;

                if(!valid)
                    return false;

            }
        }


        return true;
    }
    
    public void BuildPlayer(bool checkPoints){
        if(checkPoints){
            if(upgradePoints < GetPlayerCost()) return;
        }
        IncreaseUpgradePoints(-GetPlayerCost());

        int cells = 0;
        Vector3 originalPos = transform.position;
        Quaternion originalRot = transform.rotation;
        transform.position = new Vector2(0f, 0f);
        transform.rotation = new Quaternion();
        RB.mass = 0;
        
        for(int row = 0; row < playerCellCount; row++){
            for(int col = 0; col < playerCellCount; col++){
                if(PlayerCells[row, col] == null) continue;
                CellDefinition cell = PlayerCells[row, col];

                GameObject newObject = Instantiate(cell.Prefab, transform);

                int targetRotation = cell.rotation;
                
                if(targetRotation != 0 && targetRotation != 180)
                    targetRotation -= 180;

                newObject.transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);
                newObject.transform.position = new Vector3(blockOffset * (col - playerCellCount / 2), blockOffset * (playerCellCount / 2 - row), 0f);
                
                newObject.layer = gameObject.layer;
                RB.mass += cell.Weight;
                if(cell.Type == CellType.cell) cells++;
            }
        }
        transform.position = originalPos;
        transform.rotation = originalRot;
        GetComponent<HitpointManager>().IncreaseHP(CellDefinition.CellHPIncrease * (cells - CellsBeforeRebuild) + HPBeforeRebuild);
    }

    public void ResetPlayer(){
        CellsBeforeRebuild = 0;
        HPBeforeRebuild = GetComponent<HitpointManager>().HitPoints;
        for(int i = 0; i < gameObject.transform.childCount; i++){
            GameObject o = gameObject.transform.GetChild(i).gameObject;
            IncreaseUpgradePoints(o.GetComponent<CellController>().cost);
            if(o.tag == "Cell") CellsBeforeRebuild++;
            Destroy(o);
        }
    }

    public int GetPlayerCost(){
        int total = 0;
        for(int row = 0; row < playerCellCount; row++){
            for(int col = 0; col < playerCellCount; col++){
                if(PlayerCells[row, col] == null) continue;
                CellDefinition cell = PlayerCells[row, col];
                total += cell.Prefab.GetComponent<CellController>().cost;
            }
        }

        return total;
    }

    Vector2 CalculateLookTarget(){
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        return new Vector2(mousePosition.x, mousePosition.y);
    }

    public void IncreaseUpgradePoints(int value){
        this.upgradePoints += value;
        UpgradePointsChanged();
    }

    private void DisplayPoints(Text text){
        text.text = $"{upgradePoints}";
    }

    private void UpgradePointsChanged(){
        if(UpgradePointsText == null) return;

        DisplayPoints(UpgradePointsText.transform.Find("Value").GetComponent<Text>());
    }

    public void IncreaseMaxHitPoints(int value){
        gameObject.GetComponent<HitpointManager>().IncreaseMaxHitpoints(value);
    }

}