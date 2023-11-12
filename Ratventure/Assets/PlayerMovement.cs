using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpForceUP;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;
    private float jumpTime = 0.0f;
    private BoxCollider2D boxCollider;

    // Start is called before the first frame update
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        // движение вперед и назад
        float dirX = Input.GetAxis("Horizontal");
        
        body.velocity = new Vector2(dirX * speed, body.velocity.y);

        //компонент прыжка 
        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            StartJump();
        }
        if(Input.GetButton("Jump") && isGrounded () ) 
        {
            ContinueJump();
        }
        if (Input.GetButtonUp("Jump") && isGrounded())
        {
            EndJump();
        }

    }
    void StartJump()
    {
        jumpTime = 0.0f;
    }

    void ContinueJump()
    {
        jumpTime += Time.fixedDeltaTime;
    }

    void EndJump()
    {
        float finalJumpForce = (jumpForce * (1.0f + 1.0f / jumpForceUP)) * (-1 + jumpTime);
        body.velocity = new Vector2(body.velocity.x, finalJumpForce);
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
}
