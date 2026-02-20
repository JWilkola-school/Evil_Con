using UnityEngine;

public class SecurityDialogue : DialogueCharacter
{
    public SecurityDialogue()
    {
        var loadSprite = Resources.Load<Sprite>("Security Guard No background");
        this.CharacterImage = loadSprite;
        this.CharacterColor = 0xA5A5A5;
    }
}
