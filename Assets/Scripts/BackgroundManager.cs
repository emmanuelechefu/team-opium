using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public Sprite backgroundImage;
    
    void Start()
    {
        // Create a new GameObject for the background
        GameObject background = new GameObject("Background");
        
        // Add a SpriteRenderer component
        SpriteRenderer renderer = background.AddComponent<SpriteRenderer>();
        
        // Set the sprite
        renderer.sprite = backgroundImage;
        
        // Set the sorting layer to be behind everything else
        renderer.sortingLayerName = "Background";
        
        // Scale the background to fit the camera view
        float cameraHeight = Camera.main.orthographicSize * 2;
        float cameraWidth = cameraHeight * Camera.main.aspect;
        
        float spriteWidth = renderer.sprite.bounds.size.x;
        float spriteHeight = renderer.sprite.bounds.size.y;
        
        // Scale the background to cover the camera view
        background.transform.localScale = new Vector3(
            cameraWidth / spriteWidth,
            cameraHeight / spriteHeight,
            1
        );
        
        // Position the background at the camera's position
        background.transform.position = new Vector3(
            Camera.main.transform.position.x,
            Camera.main.transform.position.y,
            10 // Make sure it's behind other objects
        );
    }
}
