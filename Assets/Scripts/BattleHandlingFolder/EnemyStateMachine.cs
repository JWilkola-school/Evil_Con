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

    void Start()
    {
        if (enemy == null)
        {
            Debug.LogError("EnemyStateMachine: No enemy setup assigned!");
            enabled = false;
        }
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

        // Only add if not already in queue
        if (!globalBattleHandler.battleQueue.Contains(this))
        {
            globalBattleHandler.battleQueue.Enqueue(this);
        }

        currentState = State.WAITING; // Wait for turn
        globalBattleHandler.currentUnit = null;
    }

    public override void TakeAction()
    {
        if (globalBattleHandler == null || enemy == null) return;

        int choice = Random.Range(1, 11);

        if (choice <= 6) // 60% chance to attack
        {
            enemyAttack();
        }
        else // 40% chance to block
        {
            if (!enemy.isBlocking)
            {
                enemyBlock();
            }
            else
            {
                // If already blocking, attack instead
                enemyAttack();
            }
        }
    }

    public void enemyAttack()
    {
        Debug.Log(unitName + ": Attacking ally!");

        if (globalBattleHandler != null)
        {
            globalBattleHandler.damageAlly(enemy.currDamage);
        }

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

            // Deactivate the GameObject
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("globalBattleHandler is null in Enemy Die()!");
        }
    }
}