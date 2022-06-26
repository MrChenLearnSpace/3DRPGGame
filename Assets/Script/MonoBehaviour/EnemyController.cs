using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD,PATROL,CHASE,DEAD}
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStates))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    public EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    protected CharacterStates enemyStats;
    [Header("====基础设置=====")]
    public float sightRadius;
    protected GameObject attackTarget;
    public bool isGuard;
    private float speed;
    public float LookAtTime;
    float remainLookAtTime;
    float lastAttackTime;
    
    [Header("=====巡逻状态======")]
    public float patrolRange;
    Vector3 wayPoint;
    Vector3 guardPoint;
    Quaternion guardRotation;

    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDeath;
    bool playerDeath;

    Collider coll;
    // Start is called before the first frame update
    //private void OnEnable() {
    //    GameManager.Instance.AddObserver(this);
    //}
    private void OnDisable() {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemyStats = GetComponent<CharacterStates>();
        coll = GetComponent<Collider>();
        speed = agent.speed;
        guardPoint = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = LookAtTime;
    }
    private void Start() {
        if (isGuard) {
            enemyStates = EnemyStates.GUARD;

        }else {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
        GameManager.Instance.AddObserver(this);
    }
    // Update is called once per frame
    void Update()
    {
        if (enemyStats.currentHealth == 0)
            isDeath = true;
        if (!playerDeath) {
            SwitchState();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
            
        }
        
    }
    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", enemyStats.isCritical);
        anim.SetBool("Death",isDeath);
    }
    void SwitchState()
    {
        if(isDeath) {
            enemyStates = EnemyStates.DEAD;

        }
        else if(FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;
                if(transform.position!= guardPoint) {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPoint;
                    if(Vector3.SqrMagnitude(guardPoint-transform.position)<=agent.stoppingDistance) {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                    }
                }
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;

                if(Vector3.Distance(wayPoint,transform.position)<=agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    else
                        GetNewWayPoint();
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyStates.CHASE:
                isWalk = false;
                isChase = true;
                agent.speed = speed;
                if(!FoundPlayer())
                {
                    isFollow = false;
                    if(remainLookAtTime>0) {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if(isGuard) {
                        enemyStates = EnemyStates.GUARD;
                    }
                    else {
                        enemyStates = EnemyStates.PATROL;
                    }
                }
                else
                {
                   
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                  
                }
                if(TargetInRange(true)||TargetInRange(false)) {
                    isFollow = false;
                    agent.isStopped = true;
                    if(lastAttackTime<0) {
                        lastAttackTime = enemyStats.attackData.coolDown;
                        enemyStats.isCritical = UnityEngine.Random.value < enemyStats.attackData.criticalChance;
                        Attack();
                    }
                }
                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                //agent.enabled = false;//待修改
                agent.radius = 0;
                Destroy(gameObject, 2.0f);
                break;
        }
    }
    void Attack() {
        transform.LookAt(attackTarget.transform);
        if(TargetInRange(true)) {
            anim.SetTrigger("Attack");
        }
        if(TargetInRange(false)) {
            anim.SetTrigger("Skill");
        }
    }
    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach(var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                 return true;
            }
        }
        attackTarget = null;
        return false;
    }
    bool TargetInRange(bool isAttack) {
        if (attackTarget) {
            float range = isAttack ? enemyStats.attackData.attackRange : enemyStats.attackData.skillRange;
            return Vector3.Distance(transform.position, attackTarget.transform.position) <= range;
        }
        else
            return false;
    }
    void GetNewWayPoint()
    {
        remainLookAtTime = LookAtTime;
        float randomX = UnityEngine.Random.Range(-patrolRange, patrolRange);
        float randomZ = UnityEngine.Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPoint.x+randomX,transform.position.y,guardPoint.z+randomZ);
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;

        //wayPoint = randomPoint;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRange);
    }
    void Hit() {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform)) {
            CharacterStates targetState = attackTarget.GetComponent<CharacterStates>();
            targetState.TakeDamage(enemyStats, targetState);
        }
    }

    public void EndNotify() {
        anim.SetBool("Win", true);
        playerDeath = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }
}
