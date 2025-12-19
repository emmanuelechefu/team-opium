using System.Collections;
using UnityEngine;

public class WeaponVisuals : MonoBehaviour
{
    [Header("Renderers")]
    public SpriteRenderer weaponRenderer;    
    public SpriteRenderer muzzleFlashRenderer; 

    [Header("Settings")]
    public float flashDuration = 0.05f;

    void Awake()
    {
        if (weaponRenderer) weaponRenderer.enabled = false;
        if (muzzleFlashRenderer) muzzleFlashRenderer.enabled = false;
    }

    public void TriggerFlash(WeaponData data, Vector2 direction)
    {
        // Rotate the entire FirePoint toward the mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Apply Weapon Flash visuals
        if (weaponRenderer && data.weaponFlashSprite)
        {
            weaponRenderer.transform.localPosition = new Vector3(data.weaponFlashOffset, 0, 0);
            weaponRenderer.transform.localScale = Vector3.one * data.weaponFlashScale;
            weaponRenderer.sprite = data.weaponFlashSprite;
        }

        // Apply Muzzle Flash visuals
        if (muzzleFlashRenderer)
        {
            muzzleFlashRenderer.transform.localPosition = new Vector3(data.muzzleFlashOffset, 0, 0);
            muzzleFlashRenderer.transform.localScale = Vector3.one * data.muzzleFlashScale;
        }

        StopAllCoroutines();
        StartCoroutine(FlashRoutine(data));
    }

    private IEnumerator FlashRoutine(WeaponData data)
    {
        if (weaponRenderer && data.weaponFlashSprite)
            weaponRenderer.enabled = true;

        if (muzzleFlashRenderer && data.enableMuzzleFlash)
            muzzleFlashRenderer.enabled = true;

        // Ensure at least one frame is rendered
        yield return new WaitForEndOfFrame(); 
    
        if (flashDuration > 0)
            yield return new WaitForSeconds(flashDuration);

        if (weaponRenderer) weaponRenderer.enabled = false;
        if (muzzleFlashRenderer) muzzleFlashRenderer.enabled = false;
    }
}