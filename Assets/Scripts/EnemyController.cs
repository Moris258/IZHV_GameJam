using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D RB;

    public float aggroRange = 20.0f;

    public int PointsReward = 10;

    public float rotationThreshold = 0.4f;

    public float baseRotationSpeed = 100f;

    public float rotationMoveThreshold = 10.0f;

    private bool shouldMove = false;

    public bool ShouldMove {get {return shouldMove;}}


    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
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

        GameObject player = GameManager.Instance.player;

        if(player != null && Vector2.Distance(transform.position, player.transform.position) < aggroRange){
            //Rotate towards player
            Vector2 localTarget = transform.InverseTransformPoint(player.transform.position);
            if (localTarget.x < -rotationThreshold) {
                transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            } else if (localTarget.x > rotationThreshold) {
                transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
            }

            //Move towards player
            if(Math.Abs(localTarget.x) < rotationMoveThreshold){
                shouldMove = true;
            }
            else{
                shouldMove = false;
            }
        }
        else{
            shouldMove = false;
        }
    }
}
