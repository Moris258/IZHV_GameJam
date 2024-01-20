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
    private Rigidbody2D RB;
    public Rigidbody2D RigidBody{get {return RB;}}
    public const int playerCellCount = 5;
    public CellDefinition[,] PlayerCells;
    public List<CellDefinition> AvailableCellTypes = new List<CellDefinition>();

    void Awake(){
        PlayerCells = new CellDefinition[playerCellCount, playerCellCount];
    }

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        PlayerCells[2,2] = AvailableCellTypes[(int)CellType.cell];
        PlayerCells[1,2] = AvailableCellTypes[(int)CellType.spike];
        PlayerCells[3,2] = AvailableCellTypes[(int)CellType.booster];

        upgradePoints = StartingPoints;
        ResetPlayer();
        BuildPlayer(false);
        UpgradePointsChanged();
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
                bool valid = false;

                if(row != 0)
                    if(PlayerCells[row - 1, col] != null)
                        valid = true;

                if(row != 4)
                    if(PlayerCells[row + 1, col] != null)
                        valid = true;

                if(col != 0)
                    if(PlayerCells[row, col - 1] != null)
                        valid = true;

                if(col != 4)
                    if(PlayerCells[row, col + 1] != null)
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

        Vector3 originalPos = transform.position;
        Quaternion originalRot = transform.rotation;
        transform.position = new Vector2(0f, 0f);
        transform.rotation = new Quaternion();
        
        for(int row = 0; row < playerCellCount; row++){
            for(int col = 0; col < playerCellCount; col++){
                if(PlayerCells[row, col] == null) continue;
                CellDefinition cell = PlayerCells[row, col];

                GameObject newObject = Instantiate(cell.Prefab, transform);

                newObject.transform.position = new Vector3(blockOffset * (col - playerCellCount / 2), blockOffset * (playerCellCount / 2 - row), 0f);
                newObject.layer = gameObject.layer;
            }
        }
        transform.position = originalPos;
        transform.rotation = originalRot;
    }

    public void ResetPlayer(){
        for(int i = 0; i < gameObject.transform.childCount; i++){
            GameObject o = gameObject.transform.GetChild(i).gameObject;
            IncreaseUpgradePoints(o.GetComponent<CellController>().cost);
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