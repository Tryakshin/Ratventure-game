using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed; // Скорость передвижения персонажа
    [SerializeField] public float jumpHeight = 5; // Высота прыжка персонажа
    [SerializeField] public float gravityScale = 5; // Значение силы гравитации
    [SerializeField] private LayerMask groundLayer; // Слой для определения поверхности земли
    [SerializeField] private LayerMask wallLayer; // Слой для определения стен
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private SpriteRenderer sprite;
    private Animator animator;
    public float buttonTime = 0.2f; // Время удержания кнопки прыжка
    public float cancelRate = 100; // Скорость отмены прыжка
    float jumpTime;
    bool jumping;
    bool jumpCancelled;

    // Вызывается перед первым кадром
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Вызывается один раз за кадр
    private void Update()
    {
        // Управление движением влево и вправо
        float dirX = Input.GetAxis("Horizontal");

        if (Input.GetButton("Horizontal"))
        {
            // Проверка наличия стены перед движением влево или вправо
            if (dirX > 0 && !CheckWallCollision(Vector2.right))
            {
                body.velocity = new Vector2(dirX * speed, body.velocity.y);
            }
            else if (dirX < 0 && !CheckWallCollision(Vector2.left))
            {
                body.velocity = new Vector2(dirX * speed, body.velocity.y);
            }

            bool is_flipix = sprite.flipX; // переменная для фиксации момента разворота

            sprite.flipX = dirX < 0.0f;

            if (is_flipix && !sprite.flipX || !is_flipix && sprite.flipX) // если произошёл поворт персонажа, то работа с коллайдером
            {
                Vector2 colliderSize = boxCollider.size; // Получить текущие размеры коллайдера
                                                         // colliderSize.y *= -1; // Отразить размеры коллайдера по вертикали
                boxCollider.size = colliderSize; // Применить новые размеры к коллайдеру
            }
        }
            

        // Управление прыжком
        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * body.gravityScale));
            body.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumping = true;
            jumpCancelled = false;
            jumpTime = 0;
        }

        // анимация бега
        if (dirX > 0f)
        {
            animator.SetBool("running", true);
        }
        else if (dirX < 0f)
        {
            animator.SetBool("running", true);
        }
        else
        {
            animator.SetBool("running", false);
        }


        // Логика управления продолжительностью прыжка
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

    // Вызывается фиксированное количество раз в секунду
    private void FixedUpdate()
    {
        // Отмена прыжка, если кнопка была отпущена во время прыжка вверх
        if (jumpCancelled && jumping && body.velocity.y > 0)
        {
            body.AddForce(Vector2.down * cancelRate);
        }
    }

    // Проверка, находится ли персонаж на земле
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    // Проверка наличия стены
    private bool CheckWallCollision(Vector2 direction)
    {
        float extraWidth = 0.05f;
        //float extraHeight = 0.1f; // Добавим запас высоты для точного обнаружения стены

        // Используем Raycast с учетом дополнительной высоты для точного обнаружения стены
        RaycastHit2D hit = Physics2D.Raycast(boxCollider.bounds.center, direction, boxCollider.bounds.extents.x + extraWidth, wallLayer);

        // Проверяем, находится ли персонаж на земле и не произошло ли касание стены во время прыжка
        return hit.collider != null && (!jumping || hit.collider.CompareTag("Ground"));
    }
}
