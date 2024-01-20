using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerConstructManager : MonoBehaviour
{

    private static PlayerConstructManager instance;
    public static PlayerConstructManager Instance {get {return instance;}}
    private CellDefinition selectedItem;
    public List<CellDefinition> AvailableCellTypes = new List<CellDefinition>();
    public GameObject player;
    private VisualElement root;
    private Label costValue;
    private int totalCost = 0;

    void Awake(){
        if(instance != null && instance != this){
            Destroy(this);
        }
        else{
            instance = this;
            Setup();
        }
    }

    private void Setup(){
        root = GetComponentInChildren<UIDocument>().rootVisualElement;
        root.visible = false;       
        costValue = root.Q<Label>("CostValue"); 

        for(int i = 0; i < AvailableCellTypes.Count; i++){
            CellDefinition cell = AvailableCellTypes[i];
            root.Q<Button>(cell.Name).clicked += () => {selectedItem = cell;};
        }

        root.Q<Button>("Remove").clicked += () => {selectedItem = null;};

        for(int row = 0; row < PlayerController.playerCellCount; row++){
            VisualElement el = root.Q<VisualElement>("Row" + (row + 1));
            int Row = row;
            for(int col = 0; col < PlayerController.playerCellCount; col++){
                Button button = el.Q<Button>("Col" + (col + 1));
                int Col = col;
                button.clicked += () => {
                    if(Row == PlayerController.playerCellCount / 2 && Col == PlayerController.playerCellCount / 2) return;
                    PlayerController playerCon = player.GetComponent<PlayerController>();

                    playerCon.PlayerCells[Row, Col] = selectedItem; 
                    totalCost = playerCon.GetPlayerCost();
                    UpdateSingleCell(Row, Col, playerCon);
                    UpdateCostDisplay();
                };
            }
        }

    }

    public void ToggleInventory(){
        //if(!inventoryReady) return false;

        /*
        if(root.style.visibility == Visibility.Visible)
            root.style.visibility = Visibility.Hidden;
        else
            root.style.visibility = Visibility.Visible;*/
        root.visible = !root.visible;
        if(root.visible){
            UpdatePlayerCells();
            UpdateCostDisplay();
        }
    }

    void UpdateCostDisplay(){
        PlayerController playerCon = player.GetComponent<PlayerController>();
        costValue.text = $"{(int)totalCost}/{(int)playerCon.UpgradePoints}";
        if(totalCost > playerCon.UpgradePoints)
            costValue.style.color = new StyleColor(Color.red);
        else
            costValue.style.color = new StyleColor(Color.green);

    }

    void UpdateSingleCell(int row, int col, PlayerController playerCon){
        VisualElement el = root.Q<VisualElement>("Row" + (row + 1));
        Button button = el.Q<Button>("Col" + (col + 1));
        if(playerCon.PlayerCells[row, col] != null)
            button.style.backgroundImage = new StyleBackground(playerCon.PlayerCells[row, col].sprite);
        else
            button.style.backgroundImage = null;
    }

    void UpdatePlayerCells(){
        if(player == null) return;
        PlayerController playerCon = player.GetComponent<PlayerController>();
        totalCost = playerCon.GetPlayerCost();
        if(playerCon.PlayerCells == null) return;

        for(int row = 0; row < PlayerController.playerCellCount; row++){
            VisualElement el = root.Q<VisualElement>("Row" + (row + 1));
            for(int col = 0; col < PlayerController.playerCellCount; col++){
                Button button = el.Q<Button>("Col" + (col + 1));
                if(playerCon.PlayerCells[row, col] != null)
                    button.style.backgroundImage = new StyleBackground(playerCon.PlayerCells[row, col].sprite);
                else
                    button.style.backgroundImage = null;
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
}
