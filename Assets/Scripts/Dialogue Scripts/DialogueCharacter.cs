using UnityEngine;

public class DialogueCharacter
{
    protected Sprite CharacterImage;
    // Colors in Hex, textbox transitions represented by color
    protected int CharacterColor;

    public Sprite getCharacterImage()
    {
        return CharacterImage;
    }
    public int getCharacterColor()
    {
        return CharacterColor;
    }
}
