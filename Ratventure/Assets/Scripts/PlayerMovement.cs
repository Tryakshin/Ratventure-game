using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed; // �������� ������������ ���������
    [SerializeField] public float jumpHeight = 5; // ������ ������ ���������
    [SerializeField] public float gravityScale = 5; // �������� ���� ����������
    [SerializeField] private LayerMask groundLayer; // ���� ��� ����������� ����������� �����
    [SerializeField] private LayerMask wallLayer; // ���� ��� ����������� ����
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private SpriteRenderer sprite;
    private Animator animator;
    
    private float dirX = 0f;
    public float buttonTime = 0.2f; // ����� ��������� ������ ������
    public float cancelRate = 100; // �������� ������ ������
    
    private enum MovementState {standing, running, jumping, falling }

    float jumpTime;
    bool jumping;
    bool jumpCancelled;

    // ���������� ����� ������ ������
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // ���������� ���� ��� �� ����
    private void Update()
    {
        // ���������� ��������� ����� � ������
        dirX = Input.GetAxis("Horizontal");

        if (Input.GetButton("Horizontal"))
        {
            // �������� ������� ����� ����� ��������� ����� ��� ������
            if (dirX > 0 && !CheckWallCollision(Vector2.right))
            {
                body.velocity = new Vector2(dirX * speed, body.velocity.y);
            }
            else if (dirX < 0 && !CheckWallCollision(Vector2.left))
            {
                body.velocity = new Vector2(dirX * speed, body.velocity.y);
            }

        }

        //   if (is_flipix && !sprite.flipX || !is_flipix && sprite.flipX) // ���� ��������� ������ ���������, �� ������ � �����������
        //   {
        //        Vector2 colliderSize = boxCollider.size; // �������� ������� ������� ����������
        //                                                // colliderSize.y *= -1; // �������� ������� ���������� �� ���������
        //        boxCollider.size = colliderSize; // ��������� ����� ������� � ����������
        //   }
        
            

        // ���������� �������
        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * body.gravityScale));
            body.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumping = true;
            jumpCancelled = false;
            jumpTime = 0;
        }

        // ������ ���������� ������������������ ������
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

        // ������� ��������
        UpdateAnimationState();
    }

    private void UpdateAnimationState() //��������� �������� ���������� 
    {
        MovementState state;

        // �������� ����
        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false; //�������� ������
        //    sprite.flip
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sprite.flipX = true; //�������� ������
        }
        else
        {
            state = MovementState.standing;
        }

        // �������� �������� ������� 
        if (body.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (body.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        animator.SetInteger("state", (int)state);
    }

    // ���������� ������������� ���������� ��� � �������
    private void FixedUpdate()
    {
        // ������ ������, ���� ������ ���� �������� �� ����� ������ �����
        if (jumpCancelled && jumping && body.velocity.y > 0)
        {
            body.AddForce(Vector2.down * cancelRate);
        }
    }

    // ��������, ��������� �� �������� �� �����
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    // �������� ������� �����
    private bool CheckWallCollision(Vector2 direction)
    {
        float extraWidth = 0.05f;
        //float extraHeight = 0.1f; // ������� ����� ������ ��� ������� ����������� �����

        // ���������� Raycast � ������ �������������� ������ ��� ������� ����������� �����
        RaycastHit2D hit = Physics2D.Raycast(boxCollider.bounds.center, direction, boxCollider.bounds.extents.x + extraWidth, wallLayer);

        // ���������, ��������� �� �������� �� ����� � �� ��������� �� ������� ����� �� ����� ������
        return hit.collider != null && (!jumping || hit.collider.CompareTag("Ground"));
    }
}
