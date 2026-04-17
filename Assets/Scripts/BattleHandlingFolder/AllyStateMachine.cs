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
    public bool attackMenu = false;

    // FIXED: Added missing reference check
    /*
    void Start()
    {
        if (ally == null)
        {
            Debug.LogError("AllyStateMachine: No ally setup assigned!");
            enabled = false;
        }
    }*/

    public AllyStateMachine(GlobalBattleHandler instance, BaseAllySetup ally)
    {
        globalBattleHandler = instance;
        this.ally = ally;
        // FIXED: Don't add here - GlobalBattleHandler does it
        currentState = State.ADDTOLIST; // Start by adding to list
        currentTimeout = actionTimeout;
    }

    public override void localInit(GlobalBattleHandler instance) // BaseAllySetup allyType
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
                    // Tick all active buffs at the exact start of new turn
                    if (ally != null)
                    {
                        ally.TickBuffs();
                    }

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
                    if (!attackMenu)
                    {
                        TakeAction();
                    }
                    else
                    {
                        TakeAction2();
                    }
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

        globalBattleHandler.RequeueAndSort(this);

        currentState = State.WAITING;
        /*// FIXED: Only add if not already in queue
        if (!globalBattleHandler.battleQueue.Contains(this))
        {
            globalBattleHandler.battleQueue.Enqueue(this);
        }

        currentState = State.WAITING; // Wait for turn
        globalBattleHandler.currentUnit = null;*/
    }

    public override void TakeAction()
    {
        if (globalBattleHandler == null || ally == null) return;
        // Clogs the logs
        //Debug.Log("Ally TakeAction() called. Waiting for input (1=Attack, 2=Block, 3=Item, 4=Run)...");

        // If item menu is open and 5 is pressed, Close item menu and open up main actions menu.
        if (globalBattleHandler.itemMenuPanel != null && globalBattleHandler.itemMenuPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                globalBattleHandler.toggleMenus(false, false);
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Ally chose to ATTACK!");

            attackMenu = true;
            if (globalBattleHandler != null)
            {
                // Attack Menu text updates based on character.
                globalBattleHandler.updateAttackMenuText(ally);
                globalBattleHandler.toggleMenus(true, false);
            }
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
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Ally chose to RUN!");
            SceneManager.LoadScene("Scene 1");
        }
    }

    public void TakeAction2()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            string attackName = ally.attackNames[0];
            if (globalBattleHandler != null)
            {
                globalBattleHandler.ShowBattleLog($"{unitName} used {attackName}!");
            }
            Debug.Log("Ally chose to ATTACK 1!");
            float attackVal = ally.attack1();
            if (attackVal > 0)
            {
                globalBattleHandler.damageEnemy(0, attackVal);
            }
            currentState = State.ADDTOLIST;
            attackMenu = false;
            if (globalBattleHandler != null)
            {
                globalBattleHandler.toggleMenus(false, false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            string attackName = ally.attackNames[1];
            if (globalBattleHandler != null)
            {
                globalBattleHandler.ShowBattleLog($"{unitName} used {attackName}!");
            }
            Debug.Log("Ally chose to ATTACK 2!");
            float attackVal = ally.attack2();
            if (attackVal > 0)
            {
                globalBattleHandler.damageEnemy(0, attackVal);
            }
            currentState = State.ADDTOLIST;
            attackMenu = false;
            if (globalBattleHandler != null)
            {
                globalBattleHandler.toggleMenus(false, false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            string attackName = ally.attackNames[2];
            if (globalBattleHandler != null)
            {
                globalBattleHandler.ShowBattleLog($"{unitName} used {attackName}!");
            }
            Debug.Log("Ally chose to ATTACK 3!");
            float attackVal = ally.attack3();
            if (attackVal > 0)
            {
                globalBattleHandler.damageEnemy(0, attackVal);
            }
            currentState = State.ADDTOLIST;
            attackMenu = false;
            if (globalBattleHandler != null)
            {
                globalBattleHandler.toggleMenus(false, false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            string attackName = ally.attackNames[3];
            if (globalBattleHandler != null)
            {
                globalBattleHandler.ShowBattleLog($"{unitName} used {attackName}!");
            }
            Debug.Log("Ally chose to ATTACK 4!");
            float attackVal = ally.attack4();
            if (attackVal > 0)
            {
                globalBattleHandler.damageEnemy(0, attackVal);
            }
            currentState = State.ADDTOLIST;
            attackMenu = false;
            if (globalBattleHandler != null)
            {
                globalBattleHandler.toggleMenus(false, false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("Ally chose to return to the start menu!");
            attackMenu = false;
            if (globalBattleHandler != null)
            {
                globalBattleHandler.toggleMenus(false, false);
            }
        }
    }

    public override void basicAttack()
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
        globalBattleHandler.toggleMenus(false, true);
    }

    public override void Die()
    {
        Debug.Log("Ally has died.");

        // Just remove the ally; no reason to do this anymore...
        //gameObject.SetActive(false);

        // Let GlobalBattleHandler handle scene transition
        if (globalBattleHandler != null)
        {
            globalBattleHandler.RemoveDeadUnit(this);
        }

        // DO NOT load scene here - GlobalBattleHandler handles it
    }
}