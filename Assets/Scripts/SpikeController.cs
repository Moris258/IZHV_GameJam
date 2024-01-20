using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType{
    blunt,
    piercing,
    normal,
}

public class SpikeController : MonoBehaviour
{
    private Collider2D coll;
    public float Damage = 20.0f;
    public float KnockbackForce = 1000.0f;

    public DamageType damageType = DamageType.normal;
    public LayerMask enemyMask;
    public LayerMask playerMask;
    private LayerMask activeMask;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider2D>();
        if(transform.parent.tag == "Player")
            activeMask = enemyMask;
        else
            activeMask = playerMask;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other){
        bool hitObject = coll.IsTouchingLayers(activeMask);
        if(!hitObject) return;

        GameObject gameObject = other.gameObject;
        
        //Find which collider was actually hit
        for(int i = 0; i < gameObject.transform.childCount; i++){
            GameObject o = gameObject.transform.GetChild(i).gameObject;
            if(o.GetComponent<Collider2D>().IsTouching(coll)){
                gameObject = o;
                break;
            }
        }

        HitpointManager hitpointManager = gameObject.GetComponent<HitpointManager>();
        if(hitpointManager == null) return;

        hitpointManager.TakeDamage(Damage, damageType);
        //Move hit object
        Vector3 moveDir = other.gameObject.transform.position - transform.parent.position;

        other.gameObject.GetComponent<Rigidbody2D>().AddForce(moveDir.normalized * KnockbackForce);
    }
}
