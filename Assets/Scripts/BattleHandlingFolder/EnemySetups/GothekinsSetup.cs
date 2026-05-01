using UnityEngine;

public class GothekinsSetup : BaseEnemySetup
{
    public GothekinsSetup()
    {
        this.enemyName = "Gothekins";

        this.baseHP = 120f;
        this.currHP = 120f;

        this.baseDamage = 6f;
        this.currDamage = 6f;

        this.baseDefense = 3f;
        this.currDefense = 3f;

        this.baseSpeed = 6f;
        this.currSpeed = 6f;

        this.canSpecial = false;
        this.characterPrefab = Resources.Load<GameObject>("Prefabs/Gothekins");

        this.attackNames = new string[] { "Merc", "Misery" };
    }

    // --- THE BRAIN ---
    public override ActionPayload ChooseAIAction()
    {
        // 40% chance to inflict Hemorrhage, 60% chance for a basic attack
        int roll = Random.Range(1, 101);
        if (roll <= 40) return GetAction(1); // Misery
        return GetAction(0);                 // Merc
    }

    // --- THE MOVES ---
    public override ActionPayload GetAction(int actionIndex)
    {
        if (actionIndex == 1) // Misery (Single Target + Hemorrhage)
        {
            return new ActionPayload
            {
                type = ActionType.Attack,
                actionName = attackNames[1],
                value = basicAttack() * 0.8f, // Maybe does slightly less upfront damage, but causes bleeding!
                isAOE = false,
                effect = EffectType.Hemorrhage,
                effectDuration = 3
            };
        }

        // Default: Merc (Basic Attack)
        return new ActionPayload
        {
            type = ActionType.Attack,
            actionName = attackNames[0],
            value = basicAttack(),
            isAOE = false,
            effect = EffectType.None
        };
    }
}
