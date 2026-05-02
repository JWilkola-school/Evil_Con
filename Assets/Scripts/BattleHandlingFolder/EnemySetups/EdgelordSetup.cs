using UnityEngine;

public class EdgelordSetup : BaseEnemySetup
{
    public EdgelordSetup()
    {
        this.enemyName = "Edgelord";

        this.baseHP = 650f;
        this.currHP = 650f;

        this.baseDamage = 10f;
        this.currDamage = 10f;

        this.baseDefense = 4f;
        this.currDefense = 4f;

        this.baseSpeed = 8f;
        this.currSpeed = 8f;

        this.canSpecial = false;
        this.characterPrefab = Resources.Load<GameObject>("Prefabs/Edge Lord");

        this.attackNames = new string[] { "Omae wa mou...shinderu", "Judgement of the Wandering Samurai", "Alone Amidst the Clouds I Solemnly Wander" };
    }
    
    // --- THE BOSS BRAIN ---
    public override ActionPayload ChooseAIAction()
    {
        int roll = Random.Range(1, 101);

        // 20% chance: High Damage AOE Ultimate
        if (roll <= 20) return GetAction(2);

        // 40% chance: Hemorrhage Attack
        if (roll <= 60) return GetAction(1);

        // 40% chance: Basic Attack
        return GetAction(0);
    }

    // --- THE MOVES ---
    public override ActionPayload GetAction(int actionIndex)
    {
        if (actionIndex == 2) // Alone Amidst the Clouds... (High Damage AOE)
        {
            return new ActionPayload
            {
                type = ActionType.Attack,
                actionName = attackNames[2],
                value = basicAttack() * 1.5f, // Hits everyone extremely hard!
                isAOE = true,
                effect = EffectType.None,
                baseCooldown = 10
            };
        }

        if (actionIndex == 1) // Judgement of the Wandering Samurai (Hemorrhage)
        {
            return new ActionPayload
            {
                type = ActionType.Attack,
                actionName = attackNames[1],
                value = basicAttack() * 1.2f,
                isAOE = false,
                effect = EffectType.Hemorrhage,
                effectDuration = 3,
                effectValue = 15f,
                baseCooldown = 5
            };
        }

        // Default: Omae wa mou... shindeiru (Basic Attack)
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
