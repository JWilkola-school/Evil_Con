using UnityEngine;
using UnityEngine.SceneManagement;

public class AllyStateMachine : GenBattleObjects
{
    public BaseAllySetup ally;

    public override float unitSpeed { get { return ally != null ? ally.currSpeed : 0; } }
    public override string unitName { get { return ally != null ? ally.allyName : "Unknown"; } }

    public State currentState;
    public GlobalBattleHandler globalBattleHandler;

    // TEMP: Add HeroUI Canvas as objects to hide
    public GameObject actionButtons;
    public GameObject itemMenuPanel;

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
        // Clogs the logs
        //Debug.Log("Ally TakeAction() called. Waiting for input (1=Attack, 2=Block, 3=Item, 4=Run)...");

        // If item menu is open and L is pressed, Close item menu and open up main actions menu.
        if (itemMenuPanel != null && itemMenuPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                itemMenuPanel.SetActive(false);
                actionButtons.SetActive(true);
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Ally chose to ATTACK!");

            attackMenu = true;
            if (globalBattleHandler != null)
            {
                globalBattleHandler.toggleMenus();
            }

            currentState = State.ADDTOLIST;
            /* if (!ally.allyName.Equals("DudeBro ManStrong"))
            {
                // Still do the base attack for an ally
                basicAttack();
                currentState = State.ADDTOLIST;
            }
            else
            {
                attackMenu = true;
                if (globalBattleHandler != null)
                {
                    globalBattleHandler.toggleMenus();
                }
            } */

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

    public void TakeAction2()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Ally chose to ATTACK 1!");
            basicAttack();
            currentState = State.ADDTOLIST;
            attackMenu = false;
            if (globalBattleHandler != null)
            {
                globalBattleHandler.toggleMenus();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Ally chose to ATTACK 2!");
            basicAttack();
            currentState = State.ADDTOLIST;
            attackMenu = false;
            if (globalBattleHandler != null)
            {
                globalBattleHandler.toggleMenus();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Ally chose to ATTACK 3!");
            basicAttack();
            currentState = State.ADDTOLIST;
            attackMenu = false;
            if (globalBattleHandler != null)
            {
                globalBattleHandler.toggleMenus();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Ally chose to ATTACK 4!");
            basicAttack();
            currentState = State.ADDTOLIST;
            attackMenu = false;
            if (globalBattleHandler != null)
            {
                globalBattleHandler.toggleMenus();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("Ally chose to return to the start menu!");
            attackMenu = false;
            if (globalBattleHandler != null)
            {
                globalBattleHandler.toggleMenus();
            }
        }
    }

    /* public void allyAttack()
    {
        Debug.Log("Ally attacking enemy!");

        if (globalBattleHandler != null)
        {
            globalBattleHandler.damageEnemy(0, ally.currDamage);
        }
    } */

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
        // Hide the main actions
        if (actionButtons != null) actionButtons.SetActive(false);
        if (itemMenuPanel != null) itemMenuPanel.SetActive(true);
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