using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStates : MonoBehaviour
{
    public CharacterData_OS templateData;
    public CharacterData_OS characterData;
    public AttackData_OS attackData;
    [HideInInspector]
    public bool isCritical;


    #region Read from Data_So
    public int MaxHealth {
        get => characterData != null ? characterData.maxHealth : 0;
        set => characterData.maxHealth=value;
    }    
    public int currentHealth {
        get => characterData != null ? characterData.currentHealth : 0;
        set => characterData.currentHealth = value;
    }   
    public int baseDef {
        get => characterData != null ? characterData.baseDef : 0;
        set => characterData.baseDef = value;
    }   
    public int currentDef {
        get => characterData != null ? characterData.currentDef : 0;
        set => characterData.currentDef = value;
    }
    #endregion

    private void Awake() {
        if(templateData!=null) {
            characterData = Instantiate(templateData);
        }
    }

    #region     Character Combat
    public void TakeDamage(CharacterStates attack,CharacterStates defener) {
        int damage = Mathf.Max(0, attack.CurrentDamage() - defener.currentDef);
        currentHealth = Mathf.Max(0, currentHealth - damage);
       //更新UI面板
       //经验更新
       if(isCritical) {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }
    }
    public int CurrentDamage() {
        float coredamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if(isCritical) {
            coredamage *= attackData.criticalMultiplier;
        }
        return (int)coredamage;
    }
    #endregion
}
