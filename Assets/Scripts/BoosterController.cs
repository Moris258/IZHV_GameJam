using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterController : MonoBehaviour
{
    private float cooldown;
    public float cooldownLength = 1.0f;
    public float speed = 1000.0f;

    // Start is called before the first frame update
    void Start()
    {
        cooldown = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.GamePaused) return;

        GameObject parent = transform.parent.gameObject;
        if(cooldown > 0.0)
            cooldown -= Time.deltaTime;


        if(parent.tag == "Player"){
            var verticalMovement = Input.GetAxisRaw("Vertical");
            if(verticalMovement > 0.0 && CanMove()){
                MoveObject(parent.GetComponent<Rigidbody2D>(), parent.transform.up);
            }
        }
        else{
            //TODO: enemy behaviour
            EnemyController enemyController = parent.GetComponent<EnemyController>();
            //if(enemyController == null) return;
            if(enemyController.ShouldMove && CanMove()){
                MoveObject(parent.GetComponent<Rigidbody2D>(), parent.transform.up);
            }
        }
    }

    void MoveObject(Rigidbody2D target, Vector3 direction){
        if(target == null || direction == null) return;
        
        target.AddForce(new Vector2(direction.x, direction.y) * speed * (5 / target.mass));

        cooldown = cooldownLength;
    }

    bool CanMove(){
        return cooldown <= 0.0f;
    }
}
