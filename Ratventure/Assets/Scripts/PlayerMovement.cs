using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] public float jumpHeight = 5;
    [SerializeField] public float gravityScale = 5;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private SpriteRenderer sprite;
    private Animator animator;

    private float dirX = 0f;
    public float buttonTime = 0.2f;
    public float cancelRate = 100;

    private enum MovementState { standing, running, jumping, falling }

    float jumpTime;
    bool jumping;
    bool jumpCancelled;

    private Vector2 initialPosition = new Vector2(1, 1.16f);
    //private Vector2 checkpointPosition;
    private const string CheckpointXKey = "CheckpointX";
    private const string CheckpointYKey = "CheckpointY";
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        LoadCheckpointPosition();
    }

    private void Update()
    {
        dirX = Input.GetAxis("Horizontal");

        if (Input.GetButton("Horizontal"))
        {
            if (dirX > 0 && !CheckWallCollision(Vector2.right))
            {
                body.velocity = new Vector2(dirX * speed, body.velocity.y);
            }
            else if (dirX < 0 && !CheckWallCollision(Vector2.left))
            {
                body.velocity = new Vector2(dirX * speed, body.velocity.y);
            }
        }

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

        UpdateAnimationState();

        if (Input.GetKeyDown(KeyCode.E))
        {
            ResetCheckpoint();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }
    }

    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.standing;
        }

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

    private void FixedUpdate()
    {
        if (jumpCancelled && jumping && body.velocity.y > 0)
        {
            body.AddForce(Vector2.down * cancelRate);
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool CheckWallCollision(Vector2 direction)
    {
        float extraWidth = 0.05f;
        RaycastHit2D hit = Physics2D.Raycast(boxCollider.bounds.center, direction, boxCollider.bounds.extents.x + extraWidth, wallLayer);
        return hit.collider != null && (!jumping || hit.collider.CompareTag("Ground"));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CheckPoint"))
        {

            PlayerPrefs.SetFloat("CheckpointX", transform.position.x);
            PlayerPrefs.SetFloat("CheckpointY", transform.position.y);
            PlayerPrefs.Save();

            Debug.Log("Checkpoint saved!");
        }
    }

    private void ReloadScene()
    {
        // Перезагружаем текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadCheckpointPosition()
    {
        // Загружаем позицию чекпоинта из PlayerPrefs
        float checkpointX = PlayerPrefs.GetFloat("CheckpointX", initialPosition.x);
        float checkpointY = PlayerPrefs.GetFloat("CheckpointY", initialPosition.y);

        // Устанавливаем игрока на позицию чекпоинта
        transform.position = new Vector2(checkpointX, checkpointY);
    }


    private void ResetCheckpoint()
    {
        // Устанавливаем игрока на изначальные координаты
        transform.position = initialPosition;

        // Сбрасываем сохраненные позиции чекпоинта
        PlayerPrefs.DeleteKey("CheckpointX");
        PlayerPrefs.DeleteKey("CheckpointY");

        Debug.Log("Checkpoint reset!");
    }
}
