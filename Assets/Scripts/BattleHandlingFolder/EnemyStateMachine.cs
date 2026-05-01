using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyStateMachine : GenBattleObjects
{
    public BaseEnemySetup enemy;

    public override float unitSpeed { get { return enemy != null ? enemy.currSpeed : 0; } }
    public override string unitName { get { return enemy != null ? enemy.enemyName : "Unknown"; } }

    public State currentState;
    public GlobalBattleHandler globalBattleHandler;
    public bool printOnce = true;
    /*
    void Start()
    {
        if (enemy == null)
        {
            Debug.LogError("EnemyStateMachine: No enemy setup assigned!");
            enabled = false;
        }
    }*/

    public EnemyStateMachine(GlobalBattleHandler instance, BaseEnemySetup enemy)
    {
        globalBattleHandler = instance;
        this.enemy = enemy;
        currentState = State.ADDTOLIST; // Start by adding to list
    }

    public override void localInit(GlobalBattleHandler instance)
    {
        globalBattleHandler = instance;
        currentState = State.ADDTOLIST; // Start by adding to list
    }

    public override void localUpdate()
    {
        if (globalBattleHandler == null || enemy == null) return;

        switch (currentState)
        {
            case State.ADDTOLIST:
                addToList();
                printOnce = true;
                break;

            case State.WAITING:
                // Wait for turn
                break;

            case State.ACTION:
                if (printOnce)
                {
                    Debug.Log(unitName + ": Taking action!");
                    if (enemy != null)
                    {
                        enemy.TickEffects();
                    }
                    printOnce = false;
                }
                TakeAction();
                break;

            case State.DEAD:
                Die();
                break;
        }
    }

    public override void addToList()
    {
        if (globalBattleHandler == null || enemy == null) return;

        globalBattleHandler.RequeueAndSort(this);

        currentState = State.WAITING;

        /*// Only add if not already in queue
        if (!globalBattleHandler.battleQueue.Contains(this))
        {
            globalBattleHandler.battleQueue.Enqueue(this);
        }

        currentState = State.WAITING; // Wait for turn
        globalBattleHandler.currentUnit = null;*/
    }

    public override void TakeAction()
    {
        if (globalBattleHandler == null || enemy == null) return;

        int actionRoll = Random.Range(1, 101);

        if (actionRoll <= 20) // 20% chance to block
        {
            if (!enemy.isBlocking)
            {
                enemyBlock();
                return;
            }
        }

        ActionPayload chosenAction = enemy.ChooseAIAction();

        // Pick random living ally to attack
        if (globalBattleHandler.livingAllies.Count > 0)
        {
            // Collect all allies that aren't dead
            System.Collections.Generic.List<AllyStateMachine> validTargets = new System.Collections.Generic.List<AllyStateMachine>();
            for (int i = 0; i < globalBattleHandler.livingAllies.Count; i++)
            {
                if (globalBattleHandler.livingAllies[i] != null && globalBattleHandler.livingAllies[i].ally.currHP > 0)
                {
                    validTargets.Add(globalBattleHandler.livingAllies[i]);
                }
            }

            if (validTargets.Count > 0)
            {
                // Pick a random ally from the valid targets
                int randIndex = Random.Range(0, validTargets.Count);
                AllyStateMachine targetAlly = validTargets[randIndex];

                // Get their real position in the UI list so health bars update correctly
                int realTargetIndex = globalBattleHandler.livingAllies.IndexOf(targetAlly);

                // Execute the Payload!
                globalBattleHandler.ShowBattleLog($"{unitName} used {chosenAction.actionName}!");
                globalBattleHandler.ExecuteEnemyAction(chosenAction, targetAlly, realTargetIndex, this);
            }
        }

        currentState = State.ADDTOLIST;
    }

    public override void basicAttack()
    {
        Debug.Log(unitName + ": Attacking ally!");

        if (globalBattleHandler != null)
        {
            AllyStateMachine targetAlly = null;
            int targetIndex = 0;

            for (int i = 0; i < globalBattleHandler.livingAllies.Count; i++)
            {
                if (globalBattleHandler.livingAllies[i] != null)
                {
                    targetAlly = globalBattleHandler.livingAllies[i];
                    targetIndex = i;
                    break;
                }
            }

            if (targetAlly != null)
            {
                globalBattleHandler.damageAlly(targetAlly, enemy.currDamage, targetIndex);
            }
        }

        /*if (globalBattleHandler != null && globalBattleHandler.livingAllies.Count > 0)
        {
            AllyStateMachine targetAlly = globalBattleHandler.livingAllies[0];
            int targetIndex = 0;
            globalBattleHandler.damageAlly(targetAlly, enemy.currDamage, targetIndex);
        }*/

        currentState = State.ADDTOLIST; // Return to queue after action
    }

    public void enemyBlock()
    {
        Debug.Log(unitName + ": Blocking!");
        enemy.isBlocking = true;
        currentState = State.ADDTOLIST; // Return to queue after action
    }

    public override void Die()
    {
        Debug.Log(unitName + " has been defeated!");

        // CRITICAL FIX: Make sure this gets called
        if (globalBattleHandler != null)
        {
            // Remove from battle system
            globalBattleHandler.RemoveDeadUnit(this);

            // Important: Set state to DEAD so Update knows
            currentState = State.DEAD;

            // Just remove the enemy; no reason to do this anymore...
            // gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("globalBattleHandler is null in Enemy Die()!");
        }
    }
}