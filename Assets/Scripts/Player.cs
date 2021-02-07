using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;
[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool isDead = false;
    public bool _isDead
    {
        get { return isDead; }
        protected set { isDead = value; }
    }
    [SerializeField]
    private int maxHealth = 100;
    [SyncVar]
    private int currentHealth;
    [SerializeField]
    private Behaviour[] disableOnDeath;
    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    private bool[] wasEnabled;

    [SerializeField]
    private GameObject deathEffect;
    [SerializeField]
    private GameObject spawnEffect;
    public void Setup()
    {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i];
        }
        SetDefaults();
    }
    [ClientRpc]
    public void RpcTakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log(transform.name + " now has " + currentHealth + " health.");
        {
        if (currentHealth <= 0)
            Die();
        }
    }
    private void Die()
    {
        isDead = true;
        //disable components on the player object
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        //disable gameobjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        GameObject gfxIns = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gfxIns, 3f);
        Debug.Log(transform.name + " is DEAD!");
        StartCoroutine(Respawn());
        //switch cameras
        if (isLocalPlayer)
        {
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
            GameManager.singleton.SetSceneCameraActive(true);
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.singleton.matchSettings.respawnTime);
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        Debug.Log(transform.name + " respawned");
        SetDefaults();
    }
    public void SetDefaults()
    {
        isDead = false;
        currentHealth = maxHealth;
        //enable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }
        //enable gameobjects
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
        //switch cameras
        if (isLocalPlayer)
        {
            GameManager.singleton.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }
        //create spawn effect
        GameObject gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
    }
    private void Update()
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(9999);
        };
    }
}
