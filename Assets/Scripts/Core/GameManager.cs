using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Optional: assign a GameSession prefab here")]
    public GameSession gameSessionPrefab;

    public Transform Player { get; private set; }
    public PlayerHealth PlayerHealth { get; private set; }
    public Inventory PlayerInventory { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        EnsureSessionExists();
    }

    void Start()
    {
        CachePlayer();
    }

    void EnsureSessionExists()
    {
        if (GameSession.Instance != null) return;

        if (gameSessionPrefab != null)
        {
            Instantiate(gameSessionPrefab);
        }
        else
        {
            // fallback: create one
            var go = new GameObject("GameSession");
            go.AddComponent<GameSession>();
        }
    }

    public void CachePlayer()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        Player = p != null ? p.transform : null;

        PlayerHealth = p != null ? p.GetComponent<PlayerHealth>() : null;
        PlayerInventory = p != null ? p.GetComponent<Inventory>() : null;
    }
}
