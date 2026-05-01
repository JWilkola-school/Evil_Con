using NUnit.Framework.Internal.Commands;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class AllyStateMachine : GenBattleObjects
{
    public BaseAllySetup ally;

    // Targeting System (Select which enemy to attack) Variables to hold attacks in place, when selecting target to attack. 
    public bool targetMenu = false;
    private float pendingAttackValue = 0f;
    private string pendingAttackName = "";

    public override float unitSpeed { get { return ally != null ? ally.currSpeed : 0; } }
    public override string unitName { get { return ally != null ? ally.allyName : "Unknown"; } }

    public State currentState;
    public GlobalBattleHandler globalBattleHandler;
    public ActionPayload pendingAction;
    private ActionPayload storedChargePayload;

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
                // charge turn = skip turn
                if (ally != null && ally.chargeTimeLeft > 0)
                {
                    if (printOnce)
                    {
                        ally.chargeTimeLeft--;
                        if (ally.chargeTimeLeft <= 0) // unleash charge attack
                        {
                            if (globalBattleHandler != null)
                            {
                                globalBattleHandler.ShowBattleLog($"{unitName} unleashed {storedChargePayload.actionName}!");
                                int savedTargetIndex = ally.pendingChargeTarget;
                                if (savedTargetIndex < globalBattleHandler.livingEnemies.Count && globalBattleHandler.livingEnemies[savedTargetIndex] != null)
                                {
                                    EnemyStateMachine targetEnemy = globalBattleHandler.livingEnemies[savedTargetIndex];
                                    globalBattleHandler.ExecuteTargetedAction(storedChargePayload, targetEnemy, savedTargetIndex);
                                }
                                else
                                {
                                    Debug.Log("Charge target enemy is dead! Attack is wasted.");
                                    globalBattleHandler.ShowBattleLog("But the target was already gone...");
                                }
                                
                            }
                        }
                        currentState = State.ADDTOLIST;
                        printOnce = false;
                    }
                    break;
                }

                // Regular turn
                if (printOnce)
                {
                    Debug.Log(unitName + ": Your turn! Choose an action!");
                    // Tick all active buffs at the exact start of new turn
                    if (ally != null)
                    {
                        ally.TickEffects();
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
                    if (!attackMenu && !targetMenu)
                    {
                        TakeAction();
                    }
                    else if (attackMenu && !targetMenu)
                    {
                        TakeAction2();
                    }
                    else if (targetMenu)
                    {
                        TakeActionTarget();
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
                globalBattleHandler.ShowBattleLog($"{unitName} is blocking!");
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
            pendingAction = ally.attack1();

            if (pendingAction.isAOE)
            {
                attackMenu = false;
                globalBattleHandler.ShowBattleLog($"{unitName} used {pendingAction.actionName}");

                globalBattleHandler.ExecuteAOEAction(pendingAction);
                FinishAction();
            }
            else
            {
                attackMenu = false;
                targetMenu = true;
                PromptTargetSelection();
            }
            /*Debug.Log("Ally chose to ATTACK 1!");

            pendingAttackValue = ally.attack1();
            pendingAttackName = ally.attackNames[0];

            attackMenu = false;
            targetMenu = true;

            Debug.Log("Select a target! (Press 1 or 2)");
            PromptTargetSelection();*/
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            pendingAction = ally.attack2();

            if (pendingAction.isAOE)
            {
                attackMenu = false;
                globalBattleHandler.ShowBattleLog($"{unitName} used {pendingAction.actionName}");

                globalBattleHandler.ExecuteAOEAction(pendingAction);
                FinishAction();
            }
            else
            {
                attackMenu = false;
                targetMenu = true;
                PromptTargetSelection();
            }
            /*Debug.Log("Ally chose to ATTACK 2!");

            pendingAttackValue = ally.attack2();
            pendingAttackName = ally.attackNames[1];

            attackMenu = false;
            targetMenu = true;

            Debug.Log("Select a target! (Press 1 or 2)");
            PromptTargetSelection();*/
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            pendingAction = ally.attack3();

            if (pendingAction.isAOE)
            {
                attackMenu = false;
                globalBattleHandler.ShowBattleLog($"{unitName} used {pendingAction.actionName}");

                globalBattleHandler.ExecuteAOEAction(pendingAction);
                FinishAction();
            }
            else
            {
                attackMenu = false;
                targetMenu = true;
                PromptTargetSelection();
            }
            /*Debug.Log("Ally chose to ATTACK 3!");

            pendingAttackValue = ally.attack3();
            pendingAttackName = ally.attackNames[2];

            attackMenu = false;
            targetMenu = true;

            Debug.Log("Select a target! (Press 1 or 2)");
            PromptTargetSelection();*/
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            pendingAction = ally.attack4();

            if (pendingAction.isAOE)
            {
                attackMenu = false;
                globalBattleHandler.ShowBattleLog($"{unitName} used {pendingAction.actionName}");

                globalBattleHandler.ExecuteAOEAction(pendingAction);
                FinishAction();
            }
            else
            {
                attackMenu = false;
                targetMenu = true;
                PromptTargetSelection();
            }

            /*Debug.Log("Ally chose to ATTACK 4!");

            pendingAttackValue = ally.attack4();
            pendingAttackName = ally.attackNames[3];

            if (pendingAttackName == "SMASH!!!") // Dudebro's SMASH!!!
            {
                attackMenu = false;
                globalBattleHandler.ShowBattleLog($"{unitName} used {pendingAttackName}!");

                for (int i = 0; i < globalBattleHandler.livingEnemies.Count; i++)
                {
                    EnemyStateMachine e = globalBattleHandler.livingEnemies[i];
                    if (e != null)
                    {
                        globalBattleHandler.damageEnemy(e, pendingAttackValue, i);
                        e.enemy.ApplyEffect(EffectType.Crush, 3); // Apply Crush to everyone
                    }
                }
                FinishAction();
            }
            else // Security Guard Badge attack here.
            {
                attackMenu = false;
                targetMenu = true;
                PromptTargetSelection();
            }

            attackMenu = false;
            targetMenu = true;

            Debug.Log("Select a target! (Press 1 or 2)");
            PromptTargetSelection();*/
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

    // Method to execute attack on targeted enemy
    public void TakeActionTarget()
    {
        int targetIndex = -1;

        // Map keyboard input to enemy list index
        if (Input.GetKeyDown(KeyCode.Alpha1)) targetIndex = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) targetIndex = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) targetIndex = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) targetIndex = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            targetMenu = false;
            attackMenu = true;
            Debug.Log("Cancelled targeting. Choose an attack!");
            return;
        }

        // Valid target selected, execute attack
        if (targetIndex != -1)
        {
            // Check to make sure enemy exists in living enemies list
            if (targetIndex < globalBattleHandler.livingEnemies.Count && globalBattleHandler.livingEnemies[targetIndex] != null)
            {
                EnemyStateMachine selectedEnemy = globalBattleHandler.livingEnemies[targetIndex];
                if (globalBattleHandler != null)
                {
                    if (pendingAction.type == ActionType.Charge)
                    {
                        globalBattleHandler.ShowBattleLog($"{unitName} began charging {pendingAction.actionName}!");
                        ally.chargeTimeLeft = 1;
                        ally.pendingChargeTarget = targetIndex;
                        storedChargePayload = pendingAction; // Save the payload for the unleash turn!
                    }
                    else
                    {
                        globalBattleHandler.ShowBattleLog($"{unitName} used {pendingAction.actionName}!");
                        globalBattleHandler.ExecuteTargetedAction(pendingAction, selectedEnemy, targetIndex);
                    }
                }
                targetMenu = false;
                FinishAction();
            }
            else
            {
                Debug.Log("No enemy in that slot! Pick a different target.");
                if (globalBattleHandler != null)
                {
                    globalBattleHandler.ShowBattleLog("No target here! Select a different target. (Press 1 or 2)");
                }
            }
        }
    }

    public override void basicAttack()
    {
        Debug.Log("Ally attacking enemy!");

        if (globalBattleHandler != null && globalBattleHandler.livingEnemies.Count > 0)
        {
            EnemyStateMachine targetEnemy = globalBattleHandler.livingEnemies[0];
            int targetIndex = 0;

            // Pass the new required parameters: Who to hit, the damage, and the UI index
            globalBattleHandler.damageEnemy(targetEnemy, ally.currDamage, targetIndex);
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

        // Let GlobalBattleHandler handle scene transition
        if (globalBattleHandler != null)
        {
            globalBattleHandler.RemoveDeadUnit(this);
        }
    }

    private void PromptTargetSelection()
    {
        string targetText = "Targets: ";
        for (int i = 0; i < globalBattleHandler.livingEnemies.Count; i++)
        {
            if (globalBattleHandler.livingEnemies[i] != null)
            {
                targetText += $"[{i + 1}] {globalBattleHandler.livingEnemies[i].unitName}   ";
            }
        }

        if (globalBattleHandler != null)
        {
            globalBattleHandler.ShowBattleLog(targetText);
        }
    }

    private bool hasTakenSecondAction = false;
    private void FinishAction()
    {
        // Does the character have Adrenaline, and is this their first action of the turn?
        if (ally.HasEffect(EffectType.Adrenaline) && !hasTakenSecondAction)
        {
            hasTakenSecondAction = true;

            if (globalBattleHandler != null)
            {
                globalBattleHandler.ShowBattleLog($"{unitName} is surging with Adrenaline! Extra turn!");

                // Force the main action menu (Attack, Block, Item) back open!
                globalBattleHandler.toggleMenus(false, false);
            }

            // Reset the targeting sub-menus just in case
            targetMenu = false;
            attackMenu = false;

            // BONUS FIX: Reset their timeout clock! Otherwise, if they took 9 seconds 
            // to do their first attack, they would instantly timeout on their extra turn!
            currentTimeout = actionTimeout;

            // THE FIX: Do NOT change the state to WAITING! Keep them in ACTION!
            currentState = State.ACTION;
        }
        else
        {
            hasTakenSecondAction = false;

            // THIS is what officially ends the turn and tells the Stage Director to move on!
            currentState = State.ADDTOLIST;
        }
    }
}