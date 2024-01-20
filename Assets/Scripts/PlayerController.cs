using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float baseRotationSpeed = 100f;
    public float rotationThreshold = 0.5f;
    public int HPPerCell = 20;
    public GameObject UpgradePointsText;
    private const float blockOffset = 1.28f;
    private int upgradePoints = 10;
    public int UpgradePoints{get {return upgradePoints;}}
    private Rigidbody2D RB;
    public Rigidbody2D RigidBody{get {return RB;}}
    private int playerCellCount = 5;
    public CellDefinition[,] PlayerCells;
    public List<CellDefinition> AvailableCellTypes = new List<CellDefinition>();

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        PlayerCells = new CellDefinition[playerCellCount, playerCellCount];
        PlayerCells[2,2] = AvailableCellTypes[0];
        PlayerCells[2,3] = AvailableCellTypes[0];
        PlayerCells[2,1] = AvailableCellTypes[0];
        PlayerCells[1,3] = AvailableCellTypes[2];
        PlayerCells[1,1] = AvailableCellTypes[2];
        PlayerCells[1,2] = AvailableCellTypes[2];
        PlayerCells[3,2] = AvailableCellTypes[1];
        PlayerCells[3,1] = AvailableCellTypes[1];
        PlayerCells[3,3] = AvailableCellTypes[1];

        BuildPlayer();
        UpgradePointsChanged();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.GamePaused) return;
        
        float rotationSpeed = baseRotationSpeed * (5 / RB.mass);

        Vector2 localTarget = transform.InverseTransformPoint(CalculateLookTarget());
        if (localTarget.x < -rotationThreshold) {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        } else if (localTarget.x > rotationThreshold) {
            transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
        }
    }
    
    void FixedUpdate(){
    }

    void BuildPlayer(){
        ResetPlayer();
        
        for(int row = 0; row < playerCellCount; row++){
            for(int col = 0; col < playerCellCount; col++){
                if(PlayerCells[row, col] == null) continue;
                CellDefinition cell = PlayerCells[row, col];

                GameObject newObject = Instantiate(cell.Prefab, transform);

                newObject.transform.position = new Vector3(blockOffset * (col - playerCellCount / 2), blockOffset * (playerCellCount / 2 - row), 0f);
                newObject.layer = gameObject.layer;

                if(cell.Type == CellType.cell)
                    IncreaseMaxHitPoints(HPPerCell);
            }
        }
    }

    void ResetPlayer(){
        for(int i = 0; i < gameObject.transform.childCount; i++)
            Destroy(gameObject.transform.GetChild(i).gameObject);
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