using UnityEngine;

public class DudebroDialogue : DialogueCharacter
{
    public DudebroDialogue()
    {
        var loadSprite = Resources.Load<Sprite>("Dudebro ManStrong No background");
        this.CharacterImage = loadSprite;
        this.CharacterColor = 0xFF9650;
    }
}
