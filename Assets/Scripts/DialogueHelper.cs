using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private bool ran = false;
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private Image backgroundImageRef;
    [SerializeField] private Image characterOneImageRef;
    [SerializeField] private Image characterTwoImageRef;
    // The max scale that one of the character image's dimensions
    // can have on the canvas
    [SerializeField] private float canvasMaxScale = 350f;
    public Dialogue currDialogue;

    // BIG HELP:
    // All Backgrounds to be used by this script should be of the same resolution
    // Place in Resources Folder
    // TODO: Decide on background resolution to properly scale the background!

    void Start()
    {
        //var testBG = Resources.Load<Sprite>("Crowd");
        var testChar1 = Resources.Load<Sprite>("Crowd");
        var testChar2 = Resources.Load<Sprite>("Tree");
        int[] dialogueOwners = { 1, 1, 2, 1, 2 };
        string[] dialogueText = { "hi", "you smell", "WHAT", "you heard me", "you'll pay!"};
        currDialogue = new Dialogue(null, testChar1, testChar2, dialogueOwners, dialogueText); 
    }

    void run()
    {
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

    }
    // Update is called once per frame
    void Update()
    {
        if (!ran)
        {
            run();
            ran = true;
        }
    }

    void setDialogue(Dialogue dialogue)
    {
        currDialogue = dialogue;
    }
}
