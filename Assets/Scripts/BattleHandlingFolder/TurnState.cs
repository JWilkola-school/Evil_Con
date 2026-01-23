public enum State
{
    ADDTOLIST,  // Adding character to queue
    WAITING,    // Waiting for turn (NEW)
    ACTION,     // This character is taking an action
    DEAD        // This character is dead
}