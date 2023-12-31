using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyAttack : MonoBehaviour
{
    [HideInInspector] public GameObject target;
    [HideInInspector] public Animator animator;

    public float enemyDamage = 10f;
    public float attackRange = 5f;

    private int attackingHash;

    void Awake()
    {
        attackingHash = Animator.StringToHash("Attack");
    }

    public void Update()
    {
        PerformAttack();
    }

    public void PerformAttack()
    {
        if (target && Vector3.Distance(transform.position, target.transform.position) < attackRange)
        {
            //Start weapon animation, sword/shooting/etc
            animator.SetTrigger(attackingHash);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        string tag = collider.tag;
        switch (tag)
        {
            case "Player":
                DamagePlayer(collider, tag);
                break;
        }
    }

    void DamagePlayer(Collider collider, string tag)
    {
        EntityHealth targetHealth = collider.gameObject.GetComponent<EntityHealth>();
        if (targetHealth != null) //If it hits a box collider from another part of the target
        {
            targetHealth.TakeDamage(enemyDamage);
        }
    }
}
