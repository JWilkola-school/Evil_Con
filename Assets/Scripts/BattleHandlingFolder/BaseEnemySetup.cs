using UnityEngine;
using System.Collections;

[System.Serializable]

//Class that is used to set up each enemy for battle
public class BaseEnemySetup : BaseUnitSetup
{
    // Add any additional fields here!
    public string enemyName;
    public string[] attackNames = new string[4];

    // universal way for State Machine to ask for specific attack payload
    public virtual ActionPayload GetAction(int actionIndex)
    {
        return new ActionPayload
        {
            type = ActionType.Attack,
            actionName = "Basic Attack",
            value = basicAttack(),
            isAOE = false,
            effect = EffectType.None
        };
    }

    public virtual ActionPayload ChooseAIAction()
    {
        int randomSlot = Random.Range(0, attackNames.Length);
        return GetAction(randomSlot);
    }
}
