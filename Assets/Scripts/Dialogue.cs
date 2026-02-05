using UnityEngine;
[System.Serializable]
public struct Dialogue
{
    [SerializeField] private Sprite backgroundImage;
    [SerializeField] private Sprite characterOneImage;
    [SerializeField] private Sprite characterTwoImage;
    [SerializeField] private int[] dialogueOwners;
    [SerializeField] private string[] dialogueText;

    public Dialogue(Sprite backgroundImage, Sprite characterOneImage, Sprite characterTwoImage,
        int[] dialogueOwners, string[] dialogueText)
    {
        this.backgroundImage = backgroundImage;
        this.characterOneImage = characterOneImage;
        this.characterTwoImage = characterTwoImage;
        this.dialogueOwners = dialogueOwners;
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
