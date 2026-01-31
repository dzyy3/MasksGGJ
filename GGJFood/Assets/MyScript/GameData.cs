using UnityEngine;


public static class GameData
{
    //static & singleton
    public static string username = string.Empty;
    // public static string username2 = string.Empty;

    // Scene Data
    public static int objectTouchCount = 0;
    public static bool ObjectDestroyed = false;
    public static bool ObjectAlive = true;
    public static bool ObjectActivated = true;
    public static bool PopupImageActive = false;
    
    public static bool item_1_Active = false;
    public static bool item_2_Active = false;   
    public static bool item_3_Active = false;
    public static bool item_4_Active = false;
    public static bool item_5_Active = false;

    public static bool DialogueActive = false;
    // public static int CurrentDialogueIndex = 0;
    public static string[] DialogueLines;

    // public static int SceneMoveCount = 0;
    public static int spawnIndex;
    public static Vector3 PlayerSpawnPoint = new Vector3(0, 0, 0);
    public static bool SceneMoveEnabled = false;

    public static string cabinetItemName;
}
