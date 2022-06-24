using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="New Data",menuName ="Character Stats/Data")]

public class CharacterData_OS : ScriptableObject
{
    // Start is called before the first frame update
    [Header("==== State Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDef ;
    public int currentDef;
}
