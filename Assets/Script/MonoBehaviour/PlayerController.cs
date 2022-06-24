using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStates))]
public class PlayerController : MonoBehaviour
{
    Animator anim;
    NavMeshAgent agent;
    GameObject attackTarget;
    float lastAttackTime;
    CharacterStates characterStates;
    bool isDeath;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStates = GetComponent<CharacterStates>();
    }
    // Start is called before the first frame update
    void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
        GameManager.Instance.RigisterPlayer(characterStates);
    }

   

    // Update is called once per frame
    void Update()
    {
        isDeath = characterStates.currentHealth == 0;
        if (isDeath)
            GameManager.Instance.NotifyObserver();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }
    void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDeath);
    }
    void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        if (isDeath) return;

        agent.isStopped = false;
        agent.destination = target;
    }
    void EventAttack(GameObject target)
    {
        if (isDeath) return;

        if (target != null) { 
            attackTarget = target;
            characterStates.isCritical = UnityEngine.Random.value < characterStates.attackData.criticalChance; 
            StartCoroutine(MoveToAttackTarget());
        }

    }

    IEnumerator  MoveToAttackTarget()
    {

        transform.LookAt(attackTarget.transform);

       
        while(Vector3.Distance(attackTarget.transform.position,transform.position)>characterStates.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        agent.isStopped = true;
        if (lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterStates.isCritical);
            anim.SetTrigger("Attack");
            lastAttackTime = characterStates.attackData.coolDown;


        }
    }
   
    void Hit() {
        CharacterStates targetState = attackTarget.GetComponent<CharacterStates>();
        targetState.TakeDamage(characterStates, targetState);
    }
}
