using UnityEngine;
[System.Serializable]
public struct Dialogue
{
    [SerializeField] private Sprite backgroundImage;
    [SerializeField] private DialogueCharacter characterOne;
    [SerializeField] private DialogueCharacter characterTwo;
    [SerializeField] private int[] dialogueOwners;
    [SerializeField] private string[] dialogueText;

    public Dialogue(Sprite backgroundImage, DialogueCharacter characterOne, DialogueCharacter characterTwo,
        int[] dialogueOwners, string[] dialogueText)
    {
        this.backgroundImage = backgroundImage;
        this.characterOne = characterOne;
        this.characterTwo = characterTwo;
        this.dialogueOwners = dialogueOwners;
        this.dialogueText = dialogueText;
    }

    public Sprite getBackgroundImage()
    {
        return backgroundImage;
    }

    public Sprite getCharacterOneImage()
    {
        return characterOne.getCharacterImage();
    }

    public Sprite getCharacterTwoImage()
    {
        return characterTwo.getCharacterImage();
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
        return characterOne.getCharacterColor();
    }

    public int getDialogueColorTwo()
    {
        return characterTwo.getCharacterColor();
    }
}

