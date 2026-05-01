using UnityEngine;

public class BattleEnvironmentSetup : MonoBehaviour
{
    [Header("Assign your Custom Skybox Materials Here!")]
    public Material floor1Skybox;
    public Material furrKingsDomainSkybox;
    public Material edgelordCorridorsSkybox;

    void Start()
    {
        // 1. Read the return ticket!
        string originScene = BattleTransitioner.overworldSceneName;

        // 2. Change the lighting environment based on the ticket
        if (originScene == "Floor 1")
        {
            RenderSettings.skybox = floor1Skybox;
        }
        else if (originScene == "Furr-King's Domain") // Type your EXACT scene name here!
        {
            RenderSettings.skybox = furrKingsDomainSkybox;
        }
        else if (originScene == "Edge Lord Domain")
        {
            RenderSettings.skybox = edgelordCorridorsSkybox;
        }
        else
        {
            Debug.LogWarning("No skybox assigned for scene: " + originScene);
        }

        // Optional: Force the lighting to update instantly
        DynamicGI.UpdateEnvironment();
    }
}
