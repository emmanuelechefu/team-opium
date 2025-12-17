using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Gold touched by {other.name} tag={other.tag}");

        if (!other.CompareTag("Player"))
            return;

        if (GameSession.Instance == null)
        {
            Debug.LogError("GameSession.Instance is null - no GameSession in scene!");
            return;
        }

        GameSession.Instance.gold += amount;
        Debug.Log($"Picked up gold. New total = {GameSession.Instance.gold}");
        Destroy(gameObject);
    }
}
