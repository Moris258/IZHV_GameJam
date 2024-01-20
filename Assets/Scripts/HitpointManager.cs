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
    private bool isInvicible = false;
    public GameObject HPText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.GamePaused) return;
        if(isInvicible){
            invincibilityTimer += Time.deltaTime;
            if(invincibilityTimer > InvincibilityPeriod){
                isInvicible = false;
                invincibilityTimer = 0f;
            }
        }

        if(RegeneratesHP){
            if(HPRegenTimer > HPRegenFrequency){
                IncreaseHP(HPRegen);
                HPRegenTimer = 0;
            }

            HPRegenTimer += Time.deltaTime;
        }
    }

    public void IncreaseHP(float amount){
        HitPoints += HPRegen;
        if(HitPoints > MaxHitPoints)
            HitPoints = MaxHitPoints;
        HitPointsChanged();
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
        if(isInvicible) return;
        
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
