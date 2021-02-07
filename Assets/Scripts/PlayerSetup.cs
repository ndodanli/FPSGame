
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;
    [SerializeField]
    string remoteLayerName = "RemotePlayer";
    [SerializeField]
    string dontDrawLaterName = "DontDraw";
    [SerializeField]
    GameObject playerGraphics;
    [SerializeField]
    GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;
    private void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            //disable palyer graphics for local player
            Util.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLaterName));
            //create playerUI
            playerUIInstance =  Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;
            //configure PlayerUI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if (ui == null) Debug.LogError("No PlayerUI component on PlayerUI prefab.");
            else ui.SetController(GetComponent<PlayerController>());
        }
        GetComponent<Player>().Setup();
    }

    public override void OnStartClient()    //called every time a client is set up locally
    {
        base.OnStartClient();
        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();
        GameManager.RegisterPlayer(netID, player);
    }
    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }
    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }
    private void OnDisable()
    {
        Destroy(playerUIInstance);
        GameManager.singleton.SetSceneCameraActive(true);
        GameManager.UnRegisterPlayer(transform.name);
    }
}
