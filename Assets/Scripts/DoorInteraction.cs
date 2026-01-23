using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DoorInteraction : MonoBehaviour
{
    public string spawnPointID; // Unique ID for this door's destination in the next scene

    public float interactDistance = 3f;                // how close the player needs to be
    public Material defaultMaterial;
    public Material highlightMaterial;
    public Camera playerCamera;                        // assign your Main Camera
    public GameObject promptCanvas;                    // assign your "Press E" Canvas
    public string sceneName;

    private Transform player;
    private Renderer doorRenderer;
    private bool isHighlighted = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        doorRenderer = GetComponent<Renderer>();
        promptCanvas.SetActive(false);
    }

    void Update()
    {
        // Add distance check first (cheap)
        if (player == null) return;

        float sqrDistance = (player.position - transform.position).sqrMagnitude;
        float sqrInteractDistance = interactDistance * interactDistance;

        if (sqrDistance <= sqrInteractDistance)
        {
            // Only do expensive calculations when close enough
            Vector3 directionToDoor = (transform.position - playerCamera.transform.position).normalized;
            float dot = Vector3.Dot(playerCamera.transform.forward, directionToDoor);

            if (dot > 0.7f) // roughly means camera is looking toward the door
            {
                HighlightDoor(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Only allow interaction when highlighted
                    if (isHighlighted)
                    {
                        SpawnPointManager.targetSpawnPoint = spawnPointID;
                        SceneManager.LoadScene(sceneName);
                    }
                }
            }
            else
            {
                HighlightDoor(false);
            }
        }
        else
        {
            HighlightDoor(false);
        }
    }

    void HighlightDoor(bool state)
    {
        if (state && !isHighlighted)
        {
            doorRenderer.material = highlightMaterial;
            promptCanvas.SetActive(true);
            isHighlighted = true;
        }
        else if (!state && isHighlighted)
        {
            doorRenderer.material = defaultMaterial;
            promptCanvas.SetActive(false);
            isHighlighted = false;
        }
    }
}
