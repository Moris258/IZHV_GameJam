using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class HitpointManager : MonoBehaviour
{
    public float HitPoints = 20.0f;
    public float MaxHitPoints = 20.0f;
    public float HPRegen = 0.0f;
    public float HPRegenFrequency = 5f;
    private float HPRegenTimer = 0f;
    public bool RegeneratesHP = false;
    public float InvincibilityPeriod = 1.0f;
    private float invincibilityTimer = 0.0f;
    private bool isInvincible = false;
    public GameObject HPText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.GamePaused) return;
        if(isInvincible){
            invincibilityTimer += Time.deltaTime;
            if(invincibilityTimer > InvincibilityPeriod){
                isInvincible = false;
                invincibilityTimer -= InvincibilityPeriod;
            }
        }

        if(RegeneratesHP){
            if(HPRegenTimer > HPRegenFrequency){
                IncreaseHP(HPRegen);
                HPRegenTimer = 0;
            }

            HPRegenTimer += Time.deltaTime;
        }
        if(tag == "Player" && Input.GetKeyDown("h") && HitPoints != MaxHitPoints){
            float amountHealed = MaxHitPoints - HitPoints;
            PlayerController playerController = transform.GetComponent<PlayerController>();
            if(amountHealed < playerController.UpgradePoints)
            {
                IncreaseHP(amountHealed);
                playerController.IncreaseUpgradePoints((int)-amountHealed);
            }
            else{
                IncreaseHP(playerController.UpgradePoints);
                playerController.IncreaseUpgradePoints(-playerController.UpgradePoints);
            }
        }
    }

    public void IncreaseHP(float amount){
        HitPoints += amount;
        if(HitPoints > MaxHitPoints)
            HitPoints = MaxHitPoints;
        if(HitPoints < 0)
            HitPoints = 0;
        HitPointsChanged();
    }

    public void IncreaseMaxHitpoints(int amount){
        MaxHitPoints += amount;

        if(HitPoints > MaxHitPoints)
            HitPoints = MaxHitPoints;
        if(HitPoints < 0)
            HitPoints = 0;

        HitPointsChanged();
    }

    private void DisplayHP(Text text){
        text.text = $"{(int)HitPoints}/{(int)MaxHitPoints}";
    }

    private Color GetCellColor(){
        return new Color(1 - HitPoints / MaxHitPoints, HitPoints / MaxHitPoints, 0f);
    }

    private void HitPointsChanged(){
        if(HPText != null) DisplayHP(HPText.transform.Find("Value").GetComponent<Text>());

        switch(tag){
            case "Player":
            case "Enemy":
            for(int i = 0; i < transform.childCount; i++){
                GameObject o = transform.GetChild(i).gameObject;
                if(o.tag == "Cell"){
                    o.transform.GetChild(0).GetComponent<SpriteRenderer>().color = GetCellColor();
                }
            }
            break;
            default:
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = GetCellColor();
            break;
        }
    }

    public void TakeDamage(float amount, DamageType type){
        //TODO: use damage type for something
        if(isInvincible) return;
        
        if(tag != "Cell"){
            HitPoints -= amount;
            isInvincible = true;
    
            HitPointsChanged();

            if(HitPoints <= 0f){
                switch (tag)
                {
                    case "Player":
                    GameManager.Instance.EndGame();
                    break;
                    case "Enemy":
                    GameManager.Instance.EnemyDied(transform.GetComponent<EnemyController>());
                    break;
                    default:
                    break;
                }

                Destroy(gameObject);
            }
        }
        else{
            transform.parent.GetComponent<HitpointManager>().TakeDamage(amount, type);
        }
    }
}
