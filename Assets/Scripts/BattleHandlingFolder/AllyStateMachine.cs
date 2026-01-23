using UnityEngine;
using UnityEngine.SceneManagement;

public class AllyStateMachine : GenBattleObjects
{
    public BaseAllySetup ally;

    public override float unitSpeed { get { return ally != null ? ally.currSpeed : 0; } }
    public override string unitName { get { return ally != null ? ally.allyName : "Unknown"; } }

    public State currentState;
    public GlobalBattleHandler globalBattleHandler;
    private bool printOnce = true;
    private float actionTimeout = 10f; // Time to wait for player input
    private float currentTimeout;

    // FIXED: Added missing reference check
    void Start()
    {
        if (ally == null)
        {
            Debug.LogError("AllyStateMachine: No ally setup assigned!");
            enabled = false;
        }
    }

    public override void localInit(GlobalBattleHandler instance)
    {
        globalBattleHandler = instance;
        // FIXED: Don't add here - GlobalBattleHandler does it
        currentState = State.ADDTOLIST; // Start by adding to list
        currentTimeout = actionTimeout;
    }

    public override void localUpdate()
    {
        if (globalBattleHandler == null || ally == null) return;

        switch (currentState)
        {
            case State.ADDTOLIST:
                addToList();
                printOnce = true;
                currentTimeout = actionTimeout; // Reset timeout
                break;

            case State.WAITING:
                // Wait for turn
                break;

            case State.ACTION:
                if (printOnce)
                {
                    Debug.Log(unitName + ": Your turn! Choose an action!");
                    printOnce = false;
                }

                // FIXED: Add timeout so enemy can take turn if player doesn't act
                currentTimeout -= Time.deltaTime;
                if (currentTimeout <= 0)
                {
                    Debug.Log(unitName + ": Timeout! Skipping turn.");
                    allyBlock(); // Default to block on timeout
                    currentState = State.ADDTOLIST;
                }
                else
                {
                    TakeAction();
                }
                break;

            case State.DEAD:
                Die();
                break;
        }
    }

    public override void addToList()
    {
        if (globalBattleHandler == null || ally == null) return;

        // FIXED: Only add if not already in queue
        if (!globalBattleHandler.battleQueue.Contains(this))
        {
            globalBattleHandler.battleQueue.Enqueue(this);
        }

        currentState = State.WAITING; // Wait for turn
        globalBattleHandler.currentUnit = null;
    }

    public override void TakeAction()
    {
        if (globalBattleHandler == null || ally == null) return;

        Debug.Log("Ally TakeAction() called. Waiting for input (1=Attack, 2=Block, 3=Item, 4=Run)...");

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Ally chose to ATTACK!");
            allyAttack();
            currentState = State.ADDTOLIST;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!ally.isBlocking)
            {
                Debug.Log("Ally chose to BLOCK!");
                allyBlock();
                currentState = State.ADDTOLIST;
            }
            else
            {
                Debug.Log("You're already blocking!");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Ally chose to use ITEM!");
            allyItem();
            currentState = State.ADDTOLIST;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Ally chose to RUN!");
            SceneManager.LoadScene("Scene 1");
        }
    }

    public void allyAttack()
    {
        Debug.Log("Ally attacking enemy!");

        if (globalBattleHandler != null)
        {
            globalBattleHandler.damageEnemy(0, ally.currDamage);
        }
    }

    public void allyBlock()
    {
        Debug.Log(unitName + ": Blocking!");
        ally.isBlocking = true;
    }

    public void allyItem()
    {
        Debug.Log(unitName + ": Using item!");
        // Add item logic here
    }

    public override void Die()
    {
        Debug.Log("Ally has died.");

        // Just deactivate the ally
        gameObject.SetActive(false);

        // Let GlobalBattleHandler handle scene transition
        if (globalBattleHandler != null)
        {
            globalBattleHandler.RemoveDeadUnit(this);
        }

        // DO NOT load scene here - GlobalBattleHandler handles it
    }
}