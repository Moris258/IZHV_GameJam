using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private VisualElement cursor;
    private Label costValue;
    private Label descrLabel;
    private Label HPLabel;
    private Label costLabel;

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

    void Update(){
        
        if(Input.GetKeyDown("r") && selectedItem != null && selectedItem.Rotatable){
            selectedItem.rotation += 90;
            if(selectedItem.rotation >= 360)
                selectedItem.rotation = 0;

            cursor.style.rotate = new StyleRotate(new Rotate(selectedItem.rotation));
        }
    }

    void MouseMoved(MouseMoveEvent e){
        cursor.style.left = e.mousePosition.x - 32;
        cursor.style.top = e.mousePosition.y - 32;
    }

    private void Setup(){
        root = GetComponentInChildren<UIDocument>().rootVisualElement;
        root.visible = false;       


        cursor = root.Q<VisualElement>("Cursor");
        root.RegisterCallback<MouseMoveEvent>(MouseMoved);
        
        costValue = root.Q<Label>("CostValue"); 
        descrLabel = root.Q<Label>("ItemDescriptionLabel"); 
        HPLabel = root.Q<Label>("HPLabel"); 
        costLabel = root.Q<Label>("CellCostLabel"); 

        for(int i = 0; i < AvailableCellTypes.Count; i++){
            CellDefinition cell = AvailableCellTypes[i];
            cell.rotation = 0;
            root.Q<Button>(cell.Name).clicked += () => {
                selectedItem = cell;
                if(selectedItem != null){
                    descrLabel.text = "Descr: " + selectedItem.Description;
                    HPLabel.text = "HP: " + ((int)selectedItem.Prefab.GetComponent<HitpointManager>().MaxHitPoints).ToString();
                    costLabel.text = "Cost: " + selectedItem.Prefab.GetComponent<CellController>().cost.ToString();
                    cursor.style.rotate = new StyleRotate(new Rotate(selectedItem.rotation));
                }
                cursor.style.backgroundImage = new StyleBackground(selectedItem.sprite);
            };
        }

        root.Q<Button>("Remove").clicked += () => {
            selectedItem = null;
            
            descrLabel.text = "Descr: No item selected";
            HPLabel.text = "HP: 0";
            costLabel.text = "Cost: 0";
            cursor.style.backgroundImage = null;
        };

        for(int row = 0; row < PlayerController.playerCellCount; row++){
            VisualElement el = root.Q<VisualElement>("Row" + (row + 1));
            int Row = row;
            for(int col = 0; col < PlayerController.playerCellCount; col++){
                Button button = el.Q<Button>("Col" + (col + 1));
                int Col = col;
                button.clicked += () => {
                    if(Row == PlayerController.playerCellCount / 2 && Col == PlayerController.playerCellCount / 2) return;
                    PlayerController playerCon = player.GetComponent<PlayerController>();

                    if(selectedItem != null)
                        playerCon.PlayerCells[Row, Col] = selectedItem.CloneCellDefinition();
                    else
                        playerCon.PlayerCells[Row, Col] = null;
                    totalCost = playerCon.GetPlayerCost();
                    UpdateSingleCell(Row, Col, playerCon);
                    UpdateCostDisplay();
                };
            }
        }

    }

    public void ToggleInventory(){
        if(root == null) return;

        root.visible = !root.visible;
        if(root.visible){
            UpdatePlayerCells();
            UpdateCostDisplay();
            selectedItem = null;
            if(cursor != null)
                cursor.style.backgroundImage = null;
        }
    }

    void UpdateCostDisplay(){
        PlayerController playerCon = player.GetComponent<PlayerController>();
        costValue.text = $"{totalCost}/{playerCon.UpgradePoints}";
        if(totalCost > playerCon.UpgradePoints)
            costValue.style.color = new StyleColor(Color.red);
        else
            costValue.style.color = new StyleColor(Color.green);

    }

    void UpdateSingleCell(int row, int col, PlayerController playerCon){
        VisualElement el = root.Q<VisualElement>("Row" + (row + 1));
        Button button = el.Q<Button>("Col" + (col + 1));
        if(playerCon.PlayerCells[row, col] != null){
            button.style.backgroundImage = new StyleBackground(playerCon.PlayerCells[row, col].sprite);
            button.style.rotate = new StyleRotate(new Rotate(playerCon.PlayerCells[row, col].rotation));
        }
        else
            button.style.backgroundImage = null;
    }

    void UpdatePlayerCells(){
        if(player == null) return;
        PlayerController playerCon = player.GetComponent<PlayerController>();
        totalCost = playerCon.GetPlayerCost();
        if(playerCon.PlayerCells == null) return;

        for(int row = 0; row < PlayerController.playerCellCount; row++){
            for(int col = 0; col < PlayerController.playerCellCount; col++){
                UpdateSingleCell(row, col, playerCon);
            }
        }
    }
}
