using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using TMPro;

public class GlobalBattleHandler : MonoBehaviour
{
    public static GlobalBattleHandler instance;

    public UIHandler UI_Handler;

    public List<GenBattleObjects> battleList = new List<GenBattleObjects>();
    public List<EnemyStateMachine> livingEnemies = new List<EnemyStateMachine>(); // New list for targeting enemies for ally statemachine
    public List<AllyStateMachine> livingAllies = new List<AllyStateMachine>(); // list for targeting allies for enemy statemachine
    public Queue<GenBattleObjects> battleQueue = new Queue<GenBattleObjects>();
    public Dictionary<GenBattleObjects, int> turnCounts = new Dictionary<GenBattleObjects, int>();

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
    public GameObject itemMenuPanel;
    public TextMeshProUGUI[] attackButtonTexts;

    // Battle Log UI
    public TextMeshProUGUI battleLogText;
    public CanvasGroup battleLogCanvasGroup;
    public RectTransform battleLogPanelTransform;
    public float fadeInDuration = 0.2f;
    public float displayDuration = 1.0f;
    public float fadeOutDuration = 0.5f;
    public float floatDistance = 30f; 

    private Vector3 originalLogPosition;
    private Coroutine activeTextCoroutine;

    [SerializeField] private OverworldBattleHandler overworldBattleHandler;
    private Transform[] enemyMarkers;
    private Transform[] allyMarkers;
    private Dictionary<BaseUnitSetup, GameObject> gameObjectRefs;
    void Awake()
    {
        // SINGLETON PATTERN: Prevent duplicates
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        enemyMarkers = new Transform[3];
        allyMarkers = new Transform[3];
        gameObjectRefs = new Dictionary<BaseUnitSetup, GameObject>();
    }

    void Start()
    {
        Debug.Log("=== BATTLE STARTING ===");
        //enemyMarkers[0] = GameObject.Find("EnemyMarkers");//.transform;
        ///Debug.Log($"Ally HP: {allyHandler?.ally?.currHP}");
        //Debug.Log($"Enemy HP: {enemyHandler?.enemy?.currHP}");
        //Debug.Log($"Enemy Name: {enemyHandler?.enemy?.enemyName}");
        overworldBattleHandler = FindFirstObjectByType<OverworldBattleHandler>();
        InitializeBattle();
        if (battleLogText != null)
        {
            // Remember exactly where we placed it in the Editor
            originalLogPosition = battleLogPanelTransform.anchoredPosition;
            // Hide it immediately
            battleLogCanvasGroup.alpha = 0f;
        }
    }

    void InitializeBattle()
    {
        enemyMarkers[0] = GameObject.Find("/EnemyMarkers/EnemyMarker1").transform;
        enemyMarkers[1] = GameObject.Find("/EnemyMarkers/EnemyMarker2").transform;
        enemyMarkers[2] = GameObject.Find("/EnemyMarkers/EnemyMarker3").transform;
        if (enemyMarkers[0] == null)
        {
            return;
        }

        allyMarkers[0] = GameObject.Find("/AllyMarkers/AllyMarker1").transform;
        allyMarkers[1] = GameObject.Find("/AllyMarkers/AllyMarker2").transform;
        allyMarkers[2] = GameObject.Find("/AllyMarkers/AllyMarker3").transform;
        if (allyMarkers[0] == null)
        {
            return;
        }


        //if (allyHandler == null || enemyHandler == null || UI_Handler == null)
        if (UI_Handler == null)
        {
            Debug.LogError("GlobalBattleHandler: Missing required references!");
            enabled = false;
            return;
        }

        // Clear lists before populating
        battleList.Clear();
        battleQueue.Clear();
        activeUnits.Clear();
        turnCounts.Clear();

        // Reset outcome flags
        enemyDefeated = false;
        playerDefeated = false;

        float alliesHP = 0.0f;
        float enemiesHP = 0.0f;

        // Only add active units
        //if (allyHandler != null && allyHandler.gameObject.activeInHierarchy)
        {
            int markerIndex = 0;
            BaseAllySetup[] alliesArray = overworldBattleHandler.getAllies();
            foreach (BaseAllySetup ally in alliesArray)
            {
                AllyStateMachine curr = new AllyStateMachine(this, ally);
                battleList.Add(curr);
                activeUnits.Add(curr);
                alliesHP += ally.currHP;
                // Instantiate the object in the battle scene and add it to a dictionary
                // Why? So we can disable the object later when it dies!
                GameObject currCharacterInstance = Instantiate(ally.characterPrefab, allyMarkers[markerIndex], false);

                currCharacterInstance.transform.localPosition = Vector3.zero;
                currCharacterInstance.transform.localRotation = Quaternion.identity;
                currCharacterInstance.transform.localScale = Vector3.one;

                gameObjectRefs.Add(ally, currCharacterInstance);
                markerIndex++;

                if (currAlly == null)
                {
                    currAlly = curr;
                }

                livingAllies.Add(curr);
            }


            //battleList.Add(allyHandler);
            //activeUnits.Add(allyHandler);
        }

        //if (enemyHandler != null && enemyHandler.gameObject.activeInHierarchy)
        {
            int markerIndex = 0;
            BaseEnemySetup[] enemiesArray = overworldBattleHandler.getEnemies();

            // A-B-C lettering to differentiate duplicate enemies for visual indication
            Dictionary<string, int> nameCounts = new Dictionary<string, int>(); // Count how many enemies exist
            foreach (BaseEnemySetup e in enemiesArray)
            {
                if (nameCounts.ContainsKey(e.enemyName)) nameCounts[e.enemyName]++;
                else nameCounts[e.enemyName] = 1;
            }
            // Letters available
            Dictionary<string, int> currentLetterIndexes = new Dictionary<string, int>();
            char[] letters = { 'A', 'B', 'C', 'D', 'E' };

            foreach(BaseEnemySetup enemy in enemiesArray)
            {
                // This part of the code adds a letter for a duplicate enemies
                string originalName = enemy.enemyName;
                if (nameCounts[originalName] > 1)
                {
                    if (!currentLetterIndexes.ContainsKey(originalName))
                        currentLetterIndexes[originalName] = 0;
                    int letterIndex = currentLetterIndexes[originalName];
                    if (letterIndex < letters.Length)
                        enemy.enemyName = originalName + " " + letters[letterIndex];
                    currentLetterIndexes[originalName]++;
                }
                
                EnemyStateMachine curr = new EnemyStateMachine(this, enemy);
                battleList.Add(curr);
                activeUnits.Add(curr);
                enemiesHP += enemy.currHP;
                // Instantiate the object in the battle scene and add it to a dictionary
                // Why? So we can disable the object later when it dies!
                GameObject currCharacterInstance = Instantiate(enemy.characterPrefab, enemyMarkers[markerIndex], false);

                currCharacterInstance.transform.localPosition = Vector3.zero;
                currCharacterInstance.transform.localRotation = Quaternion.identity;
                currCharacterInstance.transform.localScale = Vector3.one;

                gameObjectRefs.Add(enemy, currCharacterInstance);
                markerIndex++;
                if (currEnemy == null)
                {
                    currEnemy = curr;
                }
                livingEnemies.Add(curr); // adds current enemies to targeting list.

            }
            //battleList.Add(enemyHandler);
            //activeUnits.Add(enemyHandler);
        }

        // Validate list before sorting
        if (battleList.Count > 0)
        {
            battleList = battleList.OrderByDescending(obj => obj != null ? obj.unitSpeed : 0).ToList();
        }

        
        // Enqueue all units in speed order at base.
        foreach (GenBattleObjects obj in battleList)
        {
            if (obj != null)
            {
                battleQueue.Enqueue(obj);
                Debug.Log("Added to queue: " + obj.unitName + " (Speed: " + obj.unitSpeed + ")");
            }
        }

        // Initialize UI with current HP values (just the sum of all)
        UI_Handler.uiInit(overworldBattleHandler.getAllies(), overworldBattleHandler.getEnemies());

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

                // ensure internal state and UI resets to start menu
                ally.attackMenu = false;
                toggleMenus(false, false);
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

    public void damageEnemy(EnemyStateMachine targetEnemy, float attackVal, int targetIndex) // Weapon damage removed. Target enemy and attack value are the new parameters.
    {
        if (!isBattleActive || targetEnemy == null || targetEnemy.enemy == null)
            return;

        Debug.Log($"damageEnemy called with allyDamage: {attackVal}");

        float damage = Mathf.Max(0, ((1.5f * attackVal)) * 5f);
        damage -= Mathf.Max(0, (1.5f * targetEnemy.enemy.currDefense) * 0.3f);

        // Crush Status effect
        if (targetEnemy.enemy.HasEffect(EffectType.Crush))
        {
            float healAmount = Mathf.Round(damage * 0.3f);

            for (int i = 0; i < livingAllies.Count; i++)
            {
                if (livingAllies[i] != null && livingAllies[i].unitName == "DudeBro ManStrong")
                {
                    livingAllies[i].ally.currHP += healAmount;
                    livingAllies[i].ally.currHP = Mathf.Min(livingAllies[i].ally.baseHP, livingAllies[i].ally.currHP);

                    UI_Handler.updateHealthAlly(i, livingAllies[i].ally.currHP);
                    ShowBattleLog($"DudeBro absorbed {healAmount} HP!");
                    break;
                }
            }
        }

        if (targetEnemy.enemy.isBlocking)
        {
            damage *= 0.5f;
            targetEnemy.enemy.isBlocking = false; // Block consumed
        }

        /* Math handling*/
        targetEnemy.enemy.currHP -= damage;
        targetEnemy.enemy.currHP = Mathf.Max(0, targetEnemy.enemy.currHP);

        Debug.Log($"Damage calculation: Base={(1.5f * attackVal) * 5f}, " +
                  $"Defense={(1.5f * targetEnemy.enemy.currDefense) * 0.3f}, " +
                  $"Final={damage}, Enemy HP now={targetEnemy.enemy.currHP}");

        // Update UI immediately
        UI_Handler.updateHealthEnemy(targetIndex, targetEnemy.enemy.currHP);

        Debug.Log("Enemy took " + damage + " damage. HP: " + targetEnemy.enemy.currHP);

        /* Death Checking*/

        // Check for death IMMEDIATELY after damage
        if (targetEnemy.enemy.currHP <= 0)
        {
            Debug.Log("ENEMY HEALTH REACHED 0! Triggering death...");
            targetEnemy.currentState = State.DEAD;
            enemyDefeated = true; // Track enemy defeat

            // Immediately trigger Die() method
            //if (currEnemy != null)
            //{
            //    currEnemy.Die();
            //}
            GameObject dyingObject = gameObjectRefs[targetEnemy.enemy];
            gameObjectRefs.Remove(targetEnemy.enemy);
            Destroy(dyingObject);

            // Also remove from system
            RemoveDeadUnit(targetEnemy);

            // Check if battle should end NOW
            CheckBattleEndImmediate();
        }
    }

    public void damageAlly(AllyStateMachine targetAlly, float enemyDamage, int targetIndex)
    {
        if (!isBattleActive || targetAlly == null || targetAlly.ally == null)
            return;

        Debug.Log($"damageAlly called with enemyDamage: {enemyDamage}");

        float damage = Mathf.Max(0, (1.5f * enemyDamage) * 5f);
        damage -= Mathf.Max(0, (1.5f * targetAlly.ally.currDefense) * 0.3f);

        if (targetAlly.ally.isBlocking)
        {
            damage *= 0.5f;
            targetAlly.ally.isBlocking = false; // Block consumed
        }

        targetAlly.ally.currHP -= damage;
        targetAlly.ally.currHP = Mathf.Max(0, targetAlly.ally.currHP);

        Debug.Log($"Ally damage: Base={(1.5f * enemyDamage) * 5f}, " +
                  $"Defense={(1.5f * targetAlly.ally.currDefense) * 0.3f}, " +
                  $"Final={damage}, Ally HP now={targetAlly.ally.currHP}");

        // Update UI immediately
        UI_Handler.updateHealthAlly(targetIndex, targetAlly.ally.currHP);

        Debug.Log("Ally took " + damage + " damage. HP: " + targetAlly.ally.currHP);

        if (targetAlly.ally.currHP <= 0)
        {
            Debug.Log("ALLY HEALTH REACHED 0! Triggering death...");
            targetAlly.currentState = State.DEAD;
            playerDefeated = true; // Track player defeat

            // Immediately trigger Die() method

            // Thought: we wouldn't even be able to reach the conditional
            // if currAlly was null since it would cause currAlly.ally.currHP to fail!
            GameObject dyingObject = gameObjectRefs[targetAlly.ally];
            gameObjectRefs.Remove(targetAlly.ally);
            Destroy(dyingObject);


            //if (currAlly != null)
            //{
            //    currAlly.Die();
            //}

            //RemoveDeadUnit(targetAlly); may just straightup delete ally off the field. May not want that.

            // Check if battle should end NOW
            CheckBattleEndImmediate();
        }
    }

    // Updated RemoveDeadUnit method
    public void RemoveDeadUnit(GenBattleObjects deadUnit)
    {
        if (deadUnit == null) return;

        Debug.Log($"RemoveDeadUnit called for: {deadUnit.unitName}");

        // Remove from living enemies list
        if (deadUnit is EnemyStateMachine deadEnemy)
        {
            int index = livingEnemies.IndexOf(deadEnemy);
            if (index != -1) livingEnemies[index] = null;
        }

        if (deadUnit is AllyStateMachine deadAlly)
        {
            int index = livingAllies.IndexOf(deadAlly);
            if (index != -1) livingAllies[index] = null;
        }

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
    
    // togglable menus between start, attack, and item menus.
    public void toggleMenus(bool showAttackMenu, bool showItemMenu) // Now adds Item Menu toggling as well.
    {
        // Attack menu matches showAttackMenu bool
        attackMenu.SetActive(showAttackMenu);

        // Item menu matches showItemMenu bool
        itemMenuPanel.SetActive(showItemMenu);

        // Start menu only active if both others are turned off
        startMenu.SetActive(!showAttackMenu && !showItemMenu);
    }

    // updates attack menu text based on character
    public void updateAttackMenuText(BaseAllySetup activeAllyData)
    {
        if (activeAllyData == null || attackButtonTexts == null) return;

        for (int i = 0; i < attackButtonTexts.Length; i++)
        {
            if (i < activeAllyData.attackNames.Length)
            {
                attackButtonTexts[i].text = activeAllyData.attackNames[i];
            }
        }
    }

    // Method that requeues and sorts allies and enemies in the queue dynamically based on speed order.
    public void RequeueAndSort(GenBattleObjects finishedUnit)
    {
        // Add +1 to turn count of current finished unit
        if (!turnCounts.ContainsKey(finishedUnit))
        {
            turnCounts[finishedUnit] = 0;
        }
        turnCounts[finishedUnit]++;
        
        // temporary list to store all allies and enemies in
        List<GenBattleObjects> currentLine = battleQueue.ToList();

        // adds unit that just finished turn to the list.
        if (!currentLine.Contains(finishedUnit))
        {
            currentLine.Add(finishedUnit);
        }

        // Re-sort based on these parameters
        currentLine = currentLine
            .OrderBy(obj => turnCounts.ContainsKey(obj) ? turnCounts[obj] : 0) // Units with 0 turns go before units with 1 turn
            .ThenByDescending(obj => obj != null ? obj.unitSpeed : 0) // if both units have 0 turns, fastest goes first
            .ToList();

        // clear old queue, requeue with new sort order
        battleQueue.Clear();
        foreach (GenBattleObjects obj in currentLine)
        {
            battleQueue.Enqueue(obj);
        }

        currentUnit = null;

        Debug.Log($"Queue Sorted! Next up: {battleQueue.Peek().unitName}");
    }

    public void ShowBattleLog(string message)
    {
        if (battleLogText == null || battleLogCanvasGroup == null) return;

        // If a message is already playing, stop it so they don't overlap and glitch
        if (activeTextCoroutine != null)
        {
            StopCoroutine(activeTextCoroutine);
        }

        // Start the new fade animation
        activeTextCoroutine = StartCoroutine(AnimateBattleLog(message));
    }

    private IEnumerator AnimateBattleLog(string message)
    {
        // Setup: Apply the text, but keep it invisible
        battleLogText.text = message;
        battleLogCanvasGroup.alpha = 0f;

        // Calculate our start (above) and end (above) positions
        Vector3 highPosition = originalLogPosition + new Vector3(0, floatDistance, 0);

        // --- PHASE 1: FADE IN & MOVE DOWN ---
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / fadeInDuration;

            // Move from highPosition down to originalLogPosition
            battleLogPanelTransform.anchoredPosition = Vector3.Lerp(highPosition, originalLogPosition, percent);
            // Fade from 0 to 1
            battleLogCanvasGroup.alpha = Mathf.Lerp(0f, 1f, percent);

            yield return null;
        }

        // Lock it perfectly in place just in case
        battleLogPanelTransform.anchoredPosition = originalLogPosition;
        battleLogCanvasGroup.alpha = 1f;

        // --- PHASE 2: HANG TIME ---
        yield return new WaitForSeconds(displayDuration);

        // --- PHASE 3: FADE OUT & MOVE UP ---
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / fadeOutDuration;

            // Move from originalLogPosition back up to highPosition
            battleLogPanelTransform.anchoredPosition = Vector3.Lerp(originalLogPosition, highPosition, percent);
            // Fade from 1 down to 0
            battleLogCanvasGroup.alpha = Mathf.Lerp(1f, 0f, percent);

            yield return null;
        }

        // Make sure it is completely invisible at the end
        battleLogCanvasGroup.alpha = 0f;
    }

    public void ExecuteTargetedAction(ActionPayload payload, EnemyStateMachine targetEnemy, int targetIndex)
    {
        // 1. Deal Damage (If it's an Attack or a Charge)
        if (payload.type == ActionType.Attack || payload.type == ActionType.Charge)
        {
            damageEnemy(targetEnemy, payload.value, targetIndex);
        }

        // 2. Apply Debuffs (If the payload contains one)
        if (payload.effect != EffectType.None)
        {
            targetEnemy.enemy.ApplyEffect(payload.effect, payload.effectDuration, payload.effectValue);
            ShowBattleLog($"Target was inflicted with {payload.effect}!");
        }
    }

    public void ExecuteAOEAction(ActionPayload payload)
    {
        for (int i = 0; i < livingEnemies.Count; i++)
        {
            EnemyStateMachine e = livingEnemies[i];
            if (e != null)
            {
                damageEnemy(e, payload.value, i);
                if (payload.effect != EffectType.None)
                {
                    e.enemy.ApplyEffect(payload.effect, payload.effectDuration, payload.effectValue);
                }
            }
        }
        ShowBattleLog("The attack hit everyone!");
    }
}