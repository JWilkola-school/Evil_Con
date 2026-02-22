using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GlobalBattleHandler : MonoBehaviour
{
    public static GlobalBattleHandler instance;

    public UIHandler UI_Handler;

    public List<GenBattleObjects> battleList = new List<GenBattleObjects>();
    public Queue<GenBattleObjects> battleQueue = new Queue<GenBattleObjects>();

    public GenBattleObjects currentUnit = null;
    public AllyStateMachine currAlly = null;
    public EnemyStateMachine currEnemy = null;

    // Track battle state to prevent multiple updates
    private bool isBattleActive = true;

    // Reference to track for cleanup
    private List<GenBattleObjects> activeUnits = new List<GenBattleObjects>();

    // Track battle outcome
    private bool enemyDefeated = false;
    private bool playerDefeated = false;

    // NEW: Death scene name
    public string deathSceneName = "DeathScene";
    public string victorySceneName = "Scene 1"; // Return to overworld on victory

    // NEW: Menu References
    public GameObject startMenu;
    public GameObject attackMenu;

    [SerializeField] private OverworldBattleHandler overworldBattleHandler;

    void Awake()
    {
        // SINGLETON PATTERN: Prevent duplicates
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        Debug.Log("=== BATTLE STARTING ===");
        ///Debug.Log($"Ally HP: {allyHandler?.ally?.currHP}");
        //Debug.Log($"Enemy HP: {enemyHandler?.enemy?.currHP}");
        //Debug.Log($"Enemy Name: {enemyHandler?.enemy?.enemyName}");
        overworldBattleHandler = FindFirstObjectByType<OverworldBattleHandler>();
        InitializeBattle();
    }

    void InitializeBattle()
    {
        //if (allyHandler == null || enemyHandler == null || UI_Handler == null)
        if (UI_Handler == null)
        {
            Debug.LogError("GlobalBattleHandler: Missing required references!");
            enabled = false;
            return;
        }

        // New: Poor programming practice to assume these have been initialized.
        // Initialize handlers
        //allyHandler.localInit(this);
        //enemyHandler.localInit(this);



        // Clear lists before populating
        battleList.Clear();
        battleQueue.Clear();
        activeUnits.Clear();

        // Reset outcome flags
        enemyDefeated = false;
        playerDefeated = false;

        float alliesHP = 0.0f;
        float enemiesHP = 0.0f;

        // Only add active units
        //if (allyHandler != null && allyHandler.gameObject.activeInHierarchy)
        {
            // overworldBattleHandler.getAllies
            BaseAllySetup[] alliesArray = overworldBattleHandler.getAllies();
            foreach (BaseAllySetup ally in alliesArray)
            {
                AllyStateMachine curr = new AllyStateMachine(this, ally);
                battleList.Add(curr);
                activeUnits.Add(curr);
                alliesHP += ally.currHP;
                if (currAlly == null)
                {
                    currAlly = curr;
                }

            }


            //battleList.Add(allyHandler);
            //activeUnits.Add(allyHandler);
        }

        //if (enemyHandler != null && enemyHandler.gameObject.activeInHierarchy)
        {
            BaseEnemySetup[] enemiesArray = overworldBattleHandler.getEnemies();
            foreach(BaseEnemySetup enemy in  enemiesArray)
            {
                EnemyStateMachine curr = new EnemyStateMachine(this, enemy);
                battleList.Add(curr);
                activeUnits.Add(curr);
                enemiesHP += enemy.currHP;
                if (currEnemy == null)
                {
                    currEnemy = curr;
                }

            }
            //battleList.Add(enemyHandler);
            //activeUnits.Add(enemyHandler);
        }

        // Validate list before sorting
        if (battleList.Count > 0)
        {
            battleList = battleList.OrderByDescending(obj => obj != null ? obj.unitSpeed : 0).ToList();
        }

        // Enqueue all units in speed order
        foreach (GenBattleObjects obj in battleList)
        {
            if (obj != null)
            {
                battleQueue.Enqueue(obj);
                Debug.Log("Added to queue: " + obj.unitName + " (Speed: " + obj.unitSpeed + ")");
            }
        }

        // Initialize UI with current HP values (just the sum of all)
        UI_Handler.uiInit(alliesHP, enemiesHP);

        Debug.Log("Battle initialized with " + battleQueue.Count + " units");
    }

    void Update()
    {
        if (!isBattleActive) return;

        // Check if battle should end immediately
        CheckBattleEndImmediate();

        // If battle ended, stop processing
        if (!isBattleActive) return;

        // If no current unit, get next from queue
        if (currentUnit == null && battleQueue.Count > 0)
        {
            currentUnit = battleQueue.Dequeue();
            


            // Safety check
            //if (currentUnit == null || !currentUnit.gameObject.activeInHierarchy)
            if (currentUnit == null)
            {
                currentUnit = null;
                return;
            }

            // Set unit to ACTION state
            if (currentUnit is AllyStateMachine ally)
            {
                ally.currentState = State.ACTION;
                Debug.Log("Ally's turn: " + ally.unitName);
            }
            else if (currentUnit is EnemyStateMachine enemy)
            {
                enemy.currentState = State.ACTION;
                Debug.Log("Enemy's turn: " + enemy.unitName);
            }
        }

        // Update current unit
        // Redundant check
        //if (currentUnit != null && currentUnit.gameObject.activeInHierarchy)
        
        //{
            currentUnit.localUpdate();
        //}
    }

    // Check for immediate battle end

    void CheckBattleEndImmediate()
    {
        // This method is redundant because a check is done when someone dies.
        /*
        bool allyAlive = allyHandler != null && allyHandler.ally.currHP > 0;
        bool enemyAlive = enemyHandler != null && enemyHandler.enemy.currHP > 0;

        if (!allyAlive || !enemyAlive)
        {
            Debug.Log($"Battle should end! Ally alive: {allyAlive}, Enemy alive: {enemyAlive}");
            StartCoroutine(EndBattleCoroutine(!allyAlive));
        }*/
    }

    public void damageEnemy(float weaponDamage, float allyDamage)
    {
        if (!isBattleActive || currEnemy == null || currEnemy.enemy == null)
            return;

        Debug.Log($"damageEnemy called with weaponDamage: {weaponDamage}, allyDamage: {allyDamage}");

        float damage = Mathf.Max(0, ((1.2f * weaponDamage) + (1.5f * allyDamage)) * 5f);
        damage -= Mathf.Max(0, (1.5f * currEnemy.enemy.currDefense) * 0.3f);

        if (currEnemy.enemy.isBlocking)
        {
            damage *= 0.5f;
            currEnemy.enemy.isBlocking = false; // Block consumed
        }

        /* Math handling*/
        currEnemy.enemy.currHP -= damage;
        currEnemy.enemy.currHP = Mathf.Max(0, currEnemy.enemy.currHP);

        Debug.Log($"Damage calculation: Base={(1.2f * weaponDamage + 1.5f * allyDamage) * 5f}, " +
                  $"Defense={(1.5f * currEnemy.enemy.currDefense) * 0.3f}, " +
                  $"Final={damage}, Enemy HP now={currEnemy.enemy.currHP}");

        // Update UI immediately
        UI_Handler.updateHealthEnemy(currEnemy.enemy.currHP);

        Debug.Log("Enemy took " + damage + " damage. HP: " + currEnemy.enemy.currHP);

        /* Death Checking*/

        // Check for death IMMEDIATELY after damage
        if (currEnemy.enemy.currHP <= 0)
        {
            Debug.Log("ENEMY HEALTH REACHED 0! Triggering death...");
            currEnemy.currentState = State.DEAD;
            enemyDefeated = true; // Track enemy defeat

            // Immediately trigger Die() method
            if (currEnemy != null)
            {
                currEnemy.Die();
            }

            // Also remove from system
            RemoveDeadUnit(currEnemy);

            // Check if battle should end NOW
            CheckBattleEndImmediate();
        }
    }

    public void damageAlly(float enemyDamage)
    {
        if (!isBattleActive || currAlly == null || currAlly.ally == null)
        if (!isBattleActive)
            return;

        Debug.Log($"damageAlly called with enemyDamage: {enemyDamage}");

        float damage = Mathf.Max(0, (1.5f * enemyDamage) * 5f);
        damage -= Mathf.Max(0, (1.5f * currAlly.ally.currDefense) * 0.3f);

        if (currAlly.ally.isBlocking)
        {
            damage *= 0.5f;
            currAlly.ally.isBlocking = false; // Block consumed
        }

        currAlly.ally.currHP -= damage;
        currAlly.ally.currHP = Mathf.Max(0, currAlly.ally.currHP);

        Debug.Log($"Ally damage: Base={(1.5f * enemyDamage) * 5f}, " +
                  $"Defense={(1.5f * currAlly.ally.currDefense) * 0.3f}, " +
                  $"Final={damage}, Ally HP now={currAlly.ally.currHP}");

        // Update UI immediately
        UI_Handler.updateHealthAlly(currAlly.ally.currHP);

        Debug.Log("Ally took " + damage + " damage. HP: " + currAlly.ally.currHP);

        if (currAlly.ally.currHP <= 0)
        {
            Debug.Log("ALLY HEALTH REACHED 0! Triggering death...");
            currAlly.currentState = State.DEAD;
            playerDefeated = true; // Track player defeat

            // Immediately trigger Die() method
            if (currAlly != null)
            {
                currAlly.Die();
            }

            //RemoveDeadUnit(allyHandler);

            // Check if battle should end NOW
            CheckBattleEndImmediate();
        }
    }

    // Updated RemoveDeadUnit method
    public void RemoveDeadUnit(GenBattleObjects deadUnit)
    {
        if (deadUnit == null) return;

        Debug.Log($"RemoveDeadUnit called for: {deadUnit.unitName}");

        // Remove from active units
        if (activeUnits.Contains(deadUnit))
        {
            activeUnits.Remove(deadUnit);
        }

        // Remove from battle list
        if (battleList.Contains(deadUnit))
        {
            battleList.Remove(deadUnit);
        }

        // Remove from queue if present
        var tempQueue = new Queue<GenBattleObjects>();
        while (battleQueue.Count > 0)
        {
            var unit = battleQueue.Dequeue();
            if (unit != deadUnit)
            {
                tempQueue.Enqueue(unit);
            }
        }
        battleQueue = tempQueue;

        // If it's the current unit, clear it
        if (currentUnit == deadUnit)
        {
            currentUnit = null;
        }

        int state = -1;
        if (deadUnit is AllyStateMachine) state = 0;
        if (deadUnit is EnemyStateMachine) state = 2;

        // Check if any unit is still alive
        bool anyAllyAlive = false;
        bool anyEnemyAlive = false;

        foreach (var unit in activeUnits)
        {
            if (unit is AllyStateMachine)
            {
                anyAllyAlive = true;
                if (state == 0) {
                    currAlly = (AllyStateMachine)unit;
                    state = 1;
                }
            }
            else if (unit is EnemyStateMachine)
            {
                anyEnemyAlive = true;
                if (state == 2)
                {
                    currEnemy = (EnemyStateMachine)unit;
                    state = 3;
                }
            }
        }

        Debug.Log($"After removal - Ally alive: {anyAllyAlive}, Enemy alive: {anyEnemyAlive}");

        // End battle if only one side remains
        if (!anyAllyAlive || !anyEnemyAlive)
        {
            Debug.Log($"RemoveDeadUnit triggered battle end. Ally: {anyAllyAlive}, Enemy: {anyEnemyAlive}");
            StartCoroutine(EndBattleCoroutine(!anyAllyAlive));
        }
    }

    // UPDATED EndBattleCoroutine with death scene
    IEnumerator EndBattleCoroutine(bool playerLost)
    {
        if (!isBattleActive)
        {
            Debug.Log("Battle already ending, skipping...");
            yield break; // Already ending
        }

        Debug.Log("Starting EndBattleCoroutine...");
        isBattleActive = false;
        overworldBattleHandler.clear();

        string result = playerLost ? "DEFEAT" : "VICTORY";
        Debug.Log("Battle Over: " + result);

        // Track who was defeated
        if (!playerLost)
        {
            enemyDefeated = true;
            Debug.Log($"Enemy {BattleTransitioner.EncounteredEnemyName} was defeated!");
        }
        else
        {
            playerDefeated = true;
            Debug.Log("Player was defeated!");
        }

        // Wait 2 seconds before loading scene
        Debug.Log("Waiting 2 seconds before loading scene...");
        yield return new WaitForSeconds(2f);

        // NEW: Load different scenes based on outcome
        if (playerLost)
        {
            // Player died - load Death Scene
            Debug.Log($"Loading Death Scene: {deathSceneName}");

            // Clear battle data
            BattleTransitioner.ClearBattleData();

            // Reset flags
            enemyDefeated = false;
            playerDefeated = false;

            SceneManager.LoadScene(deathSceneName);
        }
        else
        {
            // Player won - remove enemy and return to overworld
            if (enemyDefeated && !string.IsNullOrEmpty(BattleTransitioner.EnemySceneID))
            {
                Debug.Log($"Removing enemy with ID: {BattleTransitioner.EnemySceneID}");

                // Use the EnemyManager to permanently remove the enemy
                if (EnemyManager.Instance != null)
                {
                    EnemyManager.Instance.RemoveDefeatedEnemy(BattleTransitioner.EnemySceneID);
                }
            }

            // Clear battle data
            BattleTransitioner.ClearBattleData();

            // Reset flags
            enemyDefeated = false;
            playerDefeated = false;

            // Load Victory Scene (Scene 1)
            Debug.Log($"Loading Victory Scene: {victorySceneName}");
            // Old code to return to overworld
            //SceneManager.LoadScene(victorySceneName);
            // New code to return to overworld
            SceneTransitioner.Instance.StartTransition(victorySceneName);
        }
    }

    // Cleanup
    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
    public void toggleMenus()
    {
        if (currAlly.attackMenu)
        {
            attackMenu.SetActive(true);
            startMenu.SetActive(false);
        }
        else
        {
            attackMenu.SetActive(false);
            startMenu.SetActive(true);
        }
    }

    // DEBUG: Add GUI buttons for testing
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 250));

        if (GUILayout.Button("TEST: Deal 999 Damage to Enemy", GUILayout.Height(30)))
        {
            if (currEnemy != null && currEnemy.enemy != null)
            {
                Debug.Log("TEST: Forcing enemy death (VICTORY)");
                currEnemy.enemy.currHP = 0;
                currEnemy.currentState = State.DEAD;
                currEnemy.Die();
                CheckBattleEndImmediate();
            }
        }

        if (GUILayout.Button("TEST: Deal 999 Damage to Ally", GUILayout.Height(30)))
        {
            if (currAlly != null && currAlly.ally != null)
            {
                Debug.Log("TEST: Forcing ally death (DEFEAT -> Death Scene)");
                currAlly.ally.currHP = 0;
                currAlly.currentState = State.DEAD;
                currAlly.Die();
                CheckBattleEndImmediate();
            }
        }

        if (GUILayout.Button("Debug Battle Status", GUILayout.Height(30)))
        {
            Debug.Log("=== DEBUG BATTLE STATUS ===");
            Debug.Log($"Battle Active: {isBattleActive}");
            Debug.Log($"Current Unit: {(currentUnit != null ? currentUnit.unitName : "None")}");
            Debug.Log($"Queue Count: {battleQueue.Count}");
            Debug.Log($"Active Units: {activeUnits.Count}");
            Debug.Log($"Ally HP: {currAlly?.ally?.currHP}");
            Debug.Log($"Enemy HP: {currEnemy?.enemy?.currHP}");
            Debug.Log($"Death Scene: {deathSceneName}");
            Debug.Log($"Victory Scene: {victorySceneName}");
        }

        GUILayout.EndArea();
    }
}