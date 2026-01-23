using UnityEngine;

public class PlayerSpawnController : MonoBehaviour
{
    void Start()
    {
        if (string.IsNullOrEmpty(SpawnPointManager.targetSpawnPoint))
            return; // No specific spawn requested → use default

        // New recommended API
        SpawnPoint[] points = Object.FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);

        foreach (var point in points)
        {
            if (point.spawnPointID == SpawnPointManager.targetSpawnPoint)
            {
                transform.position = point.transform.position;
                transform.rotation = point.transform.rotation;
                return;
            }
        }
    }
}
