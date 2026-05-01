using Unity.VisualScripting;
using UnityEngine;

public class FurlingSetup : BaseEnemySetup
{
    public FurlingSetup()
    {
        this.enemyName = "Furling";

        this.baseHP = 100f;
        this.currHP = 100f;

        this.baseDamage = 5f;
        this.currDamage = 5f;

        this.baseDefense = 2f;
        this.currDefense = 2f;

        this.baseSpeed = 5f;
        this.currSpeed = 5f;

        this.canSpecial = false;
        this.characterPrefab = Resources.Load<GameObject>("Prefabs/Furling");

        this.attackNames = new string[] { "Scratch", "Flirt" };
    }

    public override ActionPayload GetAction(int actionIndex)
    {
        if (actionIndex == 1) // 1 = Flirt
        {
            return new ActionPayload { 
                type = ActionType.Attack,
                actionName = attackNames[1],
                value = basicAttack() * 0.5f, // Flirt does half damage
                effectValue = 2f,             // Drops the ally's attack stat by 2!
                isAOE = false,
                effect = EffectType.Charmed,  // Applies the Charmed status!
                effectDuration = 2,
                baseCooldown = 3
            };
        }

        return new ActionPayload {
            type = ActionType.Attack,
            actionName = attackNames[0],
            value = basicAttack(),
            isAOE = false,
            effect = EffectType.None
        };
    }
}
