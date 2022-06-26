using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Golem : EnemyController
{
    // Start is called before the first frame update
    [Header("   Skill   ")]
    public float kickForce = 10;

    public GameObject RockPerfab;
    public Transform handPos;
    // Start is called before the first frame update
    public void KickOff() {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform)) {
            CharacterStates targetStates = attackTarget.GetComponent<CharacterStates>();

            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;

            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
            targetStates.TakeDamage(enemyStats, targetStates); 
           
        }
    }
    public void ThrowRock() {
        if(attackTarget!=null) {
            GameObject rock = Instantiate(RockPerfab, handPos.position, Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;
            rock.GetComponent<Rock>().FlyToTarget();
        }
    }
}
