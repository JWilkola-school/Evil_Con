using UnityEngine;
[System.Serializable]
public struct Dialogue
{
    [SerializeField] private Sprite backgroundImage;
    [SerializeField] private Sprite characterOneImage;
    [SerializeField] private Sprite characterTwoImage;
    [SerializeField] private int[] dialogueOwners;
    // Colors in Hex, textbox transitions represented by color
    [SerializeField] private int characterOneDialogueColor;
    [SerializeField] private int characterTwoDialogueColor;
    [SerializeField] private string[] dialogueText;
    
    public Dialogue(Sprite backgroundImage, Sprite characterOneImage, Sprite characterTwoImage,
        int[] dialogueOwners, int characterOneDialogueColor, int characterTwoDialogueColor,
        string[] dialogueText)
    {
        this.backgroundImage = backgroundImage;
        this.characterOneImage = characterOneImage;
        this.characterTwoImage = characterTwoImage;
        this.dialogueOwners = dialogueOwners;
        this.characterOneDialogueColor = characterOneDialogueColor;
        this.characterTwoDialogueColor = characterTwoDialogueColor;
        this.dialogueText = dialogueText;
    }

    public Sprite getBackgroundImage()
    {
        return backgroundImage;
    }

    public Sprite getCharacterOneImage()
    {
        return characterOneImage;
    }

    public Sprite getCharacterTwoImage()
    {
        return characterTwoImage;
    }

    public int[] getDialogueOwners()
    {
        return dialogueOwners;
    }

    public string[] getDialogueTexts()
    {
        return dialogueText;
    }

    public int getDialogueColorOne()
    {
        return characterOneDialogueColor;
    }

    public int getDialogueColorTwo()
    {
        return characterTwoDialogueColor;
    }
}
/*
public class Dialogue : MonoBehaviour
{
    [SerializeField] private Sprite backgroundImage;
    [SerializeField] private Sprite characterOneImage;
    [SerializeField] private Sprite characterTwoImage;
    // Where will the speech bubble point to?
    // Character 1 or Character 2
    [SerializeField] private int[] dialogueOwners;
    [SerializeField] private string[] dialogueText;

    /*
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}*/
