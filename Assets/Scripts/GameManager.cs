using UnityEngine;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public MatchSettings matchSettings;
    public static GameManager singleton;
    [SerializeField]
    private GameObject sceneCamera;
    private void Awake()
    {
        if (singleton != null)
        {
            Debug.LogError("More than one GameManager in scene.");
        }
        else
        {
            singleton = this;
        }
    }
    #region Player Tracking
    private const string PLAYER_ID_PREFIX = "Player";
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public void SetSceneCameraActive(bool isActive)
    {
        if (sceneCamera == null) return;
        sceneCamera.SetActive(isActive);
    }
    public static void RegisterPlayer(string netID, Player player)
    {
        string playerID = PLAYER_ID_PREFIX + netID;
        player.transform.name = playerID;
        players.Add(playerID, player);

    }

    public static void UnRegisterPlayer(string playerID)
    {
        players.Remove(playerID);
    }

    public static Player GetPlayer(string playerID)
    {
        return players[playerID];
    }

    //private void OnGUI()
    //{
    //    GUILayout.BeginArea(new Rect(200,200, 200, 500));
    //    GUILayout.BeginVertical();
    //    foreach (string playerID in players.Keys)
    //    {
    //        GUILayout.Label(playerID + "    -    " + players[playerID].transform.name);
    //    }

    //    GUILayout.EndVertical();
    //    GUILayout.EndArea();
    //}
    #endregion
}
