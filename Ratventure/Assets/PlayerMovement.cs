using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed; // скарость персонажа
    [SerializeField] public float jumpHeight = 5; // начальная сила прыжка
    [SerializeField] public float gravityScale = 5; // сила гравитация
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    public float buttonTime = 0.2f;
    public float cancelRate = 100;
    float jumpTime;
    bool jumping;
    bool jumpCancelled;


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
            float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * body.gravityScale));
            body.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumping = true;
            jumpCancelled = false;
            jumpTime = 0;
        }
        if (jumping)
        {
            jumpTime += Time.deltaTime;
            if (Input.GetButtonUp("Jump"))
            {
                jumpCancelled = true;
            }
            if (jumpTime > buttonTime)
            {
                jumping = false;
            }
        }
    }
    private void FixedUpdate()
    {
        if (jumpCancelled && jumping && body.velocity.y > 0)
        {
            body.AddForce(Vector2.down * cancelRate);
        }
    }

    // функция для отслеживания персонажа на земле ли он
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

}

    
