/// <summary>
/// Different possible states of a level.
/// </summary>
public enum GameState
{
    WIN, // The player won.
    LOSE, // The player lost.
    ONGOING, // Main gameplay; in a level.
    TIE, // The player neither won nor lost.
    MENU, // The player is on the Main Menu.
    SKILL_SELECT, // The player is on the Skill Select Menu.
    INVALID // Something went wrong.
}
