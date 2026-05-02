using UnityEngine;

public class FurKingSetup : BaseEnemySetup
{
    public FurKingSetup()
    {
        this.enemyName = "Fur King";

        this.baseHP = 800f;
        this.currHP = 800f;

        this.baseDamage = 8f;
        this.currDamage = 8f;

        this.baseDefense = 5f;
        this.currDefense = 5f;

        this.baseSpeed = 7f;
        this.currSpeed = 7f;

        this.canSpecial = false;
        this.characterPrefab = Resources.Load<GameObject>("Prefabs/Fur King");

        this.attackNames = new string[] { "Gouge", "Wild Swipe", "Blood Hunt", "Forsaken Bloodlust" };
    }

    public override ActionPayload ChooseAIAction()
    {
        int roll = Random.Range(1, 101);

        if (roll <= 10) return GetAction(3);      // 10% chance: Forsaken Bloodlust!
        if (roll <= 30) return GetAction(2);      // 20% chance: Blood Hunt (Cripple)
        if (roll <= 60) return GetAction(1);      // 30% chance: Wild Swipe (AOE)
        return GetAction(0);                      // 40% chance: Gouge (Basic)
    }

    // --- THE MOVES ---
    public override ActionPayload GetAction(int actionIndex)
    {
        if (actionIndex == 1) // Wild Swipe (AOE)
            return new ActionPayload { 
                type = ActionType.Attack, 
                actionName = attackNames[1], 
                value = basicAttack() * 0.7f, 
                isAOE = true, 
                effect = EffectType.None
            };

        if (actionIndex == 2) // Blood Hunt (Cripple)
            return new ActionPayload { type = ActionType.Attack, 
                actionName = attackNames[2], 
                value = basicAttack() * 1.2f, 
                isAOE = false, 
                effect = EffectType.Cripple, 
                effectDuration = 3 
            };

        if (actionIndex == 3) // Forsaken Bloodlust (Heavy + Adrenaline Extra Turn!)
            return new ActionPayload { type = ActionType.Attack, 
                actionName = attackNames[3], 
                value = basicAttack() * 1.5f, 
                isAOE = false, 
                effect = EffectType.None, 
                selfEffect = EffectType.Adrenaline, 
                selfEffectDuration = 3 
            };

        // Default: Gouge
        return new ActionPayload { type = ActionType.Attack, 
            actionName = attackNames[0], 
            value = basicAttack(), 
            isAOE = false, 
            effect = EffectType.None 
        };
    }
}
