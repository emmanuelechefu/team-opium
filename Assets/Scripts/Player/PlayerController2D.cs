using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float jumpForce = 12f;

    public Transform groundCheck;
    public float groundRadius = 0.15f;
    public LayerMask groundMask;

    Rigidbody2D rb;
    bool jumpQueued;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        // Jump = W (Space is shooting)
        if (Input.GetKeyDown(KeyCode.W)) jumpQueued = true;
    }

    void FixedUpdate()
    {
        float x = 0f;
        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x =  1f;

        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
        if (jumpQueued && grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        jumpQueued = false;
    }
}
