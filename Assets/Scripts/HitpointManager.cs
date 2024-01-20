using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class HitpointManager : MonoBehaviour
{
    public float HitPoints = 20.0f;
    public float MaxHitPoints = 20.0f;
    public GameObject HPText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseMaxHitpoints(int amount){
        this.MaxHitPoints += amount;
        this.HitPoints += amount;
        HitPointsChanged();
    }

    private void DisplayHP(Text text){
        text.text = $"{(int)HitPoints}/{(int)MaxHitPoints}";
    }

    private void HitPointsChanged(){
        if(HPText == null) return;

        DisplayHP(HPText.transform.Find("Value").GetComponent<Text>());
    }

    public void TakeDamage(float amount, DamageType type){
        //TODO: use damage type for something
        
        if(tag != "Cell"){
            HitPoints -= amount;
    
            if(tag == "Player") HitPointsChanged();

            if(HitPoints <= 0f){
                switch (tag)
                {
                    case "Player":
                    GameManager.Instance.EndGame();
                    break;
                    case "Enemy":
                    GameManager.Instance.EnemyDied();
                    break;
                    default:
                    break;
                }

                Destroy(gameObject);
            }


            Debug.Log("Cell took damage. " + amount + tag);
        }
        else{
            transform.parent.GetComponent<HitpointManager>().TakeDamage(amount, type);
        }
    }
}
