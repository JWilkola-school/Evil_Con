using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private bool tutorialRan = false;
    // Has the dialogue started?
    private bool isRunning = false;
    
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private Image backgroundImageRef;
    [SerializeField] private Image characterOneImageRef;
    [SerializeField] private Image characterTwoImageRef;
    [SerializeField] private Image textboxRef;
    [SerializeField] private TextMeshProUGUI CharacterTMP;
    // The max scale that one of the character image's dimensions
    // can have on the canvas
    [SerializeField] private float canvasMaxScale = 350f;

    // Disable player input while dialogue occurs
    [SerializeField] private BasicPlayerController playerCont;
    [SerializeField] private ThirdPersonCamera cam;

    // Set to public if testing
    private Dialogue currDialogue;
    private int index;

    // BIG HELP:
    // All Backgrounds to be used by this script should be of the same resolution
    // Place in Resources Folder
    // TODO: Decide on background resolution to properly scale the background!

   

    void Start()
    {
        index = -1;
    }

    Color rgbHexToColor(int rgbHex)
    {
        int red = (rgbHex & 0xFF0000) >> 16;
        int green = (rgbHex & 0xFF00) >> 8;
        int blue = (rgbHex & 0xFF);
        float redF = red / 255.0f;
        float greenF = green / 255.0f;
        float blueF = blue / 255.0f;
        return new Color(redF, greenF, blueF);
    }

    void dialogueProgress()
    {
        CharacterTMP.text = currDialogue.getDialogueTexts()[index];
        int currSpeaker = currDialogue.getDialogueOwners()[index];
        Color textboxColor;
        if (currSpeaker == 1)
        {
            textboxColor = rgbHexToColor(currDialogue.getDialogueColorOne());
        }
        else
        {
            textboxColor = rgbHexToColor(currDialogue.getDialogueColorTwo());
        }

        textboxRef.color = textboxColor;
    }

    void run()
    {
        /* Image Setup */
        dialogueCanvas.SetActive(true);
        // Background images should already be at the desired scale
        backgroundImageRef.sprite = currDialogue.getBackgroundImage();
        backgroundImageRef.SetNativeSize();

        Sprite charOneSprite = currDialogue.getCharacterOneImage();
        // Get the native aspect ratio
        float charOneHeight = charOneSprite.rect.height;
        float charOneWidth = charOneSprite.rect.width;

        // We want to scale it such that the greater dimension is 350
        float newMultiplier = canvasMaxScale / charOneHeight;
        if ((canvasMaxScale / charOneWidth) < newMultiplier)
        {
            newMultiplier = canvasMaxScale / charOneWidth;
        }
        characterOneImageRef.sprite = charOneSprite;
        characterOneImageRef.rectTransform.sizeDelta = new Vector2(charOneWidth * newMultiplier, charOneHeight * newMultiplier);


        Sprite charTwoSprite = currDialogue.getCharacterTwoImage();
        float charTwoHeight = charTwoSprite.rect.height;
        float charTwoWidth = charTwoSprite.rect.width;

        newMultiplier = canvasMaxScale / charTwoHeight;
        if ((canvasMaxScale / charTwoWidth) < newMultiplier)
        {
            newMultiplier = canvasMaxScale / charTwoWidth;
        }

        characterTwoImageRef.sprite = charTwoSprite;
        characterTwoImageRef.rectTransform.sizeDelta = new Vector2(charTwoWidth * newMultiplier, charTwoHeight * newMultiplier);

        /* */
        index = 0;
        dialogueProgress();
        isRunning = true;
        playerCont.enabled = false;
        cam.enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (!tutorialRan)
        {
            /*
            var testBG = Resources.Load<Sprite>("Fur-Kingdom-Map");
            var testChar1 = Resources.Load<Sprite>("Crowd");
            var testChar2 = Resources.Load<Sprite>("Tree");
            int[] dialogueOwners = { 1, 1, 2, 1, 2 };
            int characterOneDialogueColor = 0xAD8570;
            int characterTwoDialogueColor = 0xFF6060;
            string[] dialogueText = { 
                "hi", "you smell", "WHAT", "you heard me", "you'll pay!" 
            };
            */
            
            var testBG = Resources.Load<Sprite>("Fur-Kingdom-Map");
            var testChar1 = Resources.Load<Sprite>("Crowd");
            var testChar2 = Resources.Load<Sprite>("Tree");
            int[] dialogueOwners = { 1, 1, 1, 1, 1 };
            int characterOneDialogueColor = 0xFF9650;
            int characterTwoDialogueColor = 0;
            string[] dialogueText = {
                "Welcome to the Evil-Con! Press \'e\' to continue.",
                "You can move around with the \'WASD\' keys.",
                "To look around, use your mouse.",
                "Hold shift to sprint",
                "Be on the lookout for enemies! Getting too close might trigger a battle..."
            };
            currDialogue = new Dialogue(testBG, testChar1, testChar2, dialogueOwners, characterOneDialogueColor,
                characterTwoDialogueColor, dialogueText);
            run();
            tutorialRan = true;
        }
        if (isRunning)
        {
            if (Input.GetKeyDown(KeyCode.E) && !PauseMenu.isPaused) {
                index++;
                if (index < currDialogue.getDialogueTexts().Length)
                {
                    dialogueProgress();
                }
                else
                {
                    isRunning = false;
                    dialogueCanvas.SetActive(false);
                    playerCont.enabled = true;
                    cam.enabled = true;
                }
            }
        }
    }

    void setDialogue(Dialogue dialogue)
    {
        currDialogue = dialogue;
    }
}
