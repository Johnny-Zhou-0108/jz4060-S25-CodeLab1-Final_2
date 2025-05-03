using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterStatHealthModifierSO : CharacterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        Debug.Log($"Affecting character's health by {val}");
        Health health = GameObject.Find("Player").GetComponent<Health>();
        if (health != null)
        {
            health.AddHealth(val); // Pass val directly as a float
        }
    }
}
