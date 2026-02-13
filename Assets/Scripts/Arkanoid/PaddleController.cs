using UnityEngine;
using System.Collections;

public class PaddleController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private Transform paddleTransform;

    private float screenHalfWidth;
    private float screenHalfHeight;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private float input;

    private float lastTouchX;

    // Sens de déplacement calculé depuis le touch
    private float touchInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        Camera cam = Camera.main;
        paddleTransform.localScale = new Vector3(1f, 1.1f, 1.1f);

        screenHalfHeight = cam.orthographicSize;
        screenHalfWidth = screenHalfHeight * cam.aspect;
    }

    public void UpgradeScale()
    {
        StartCoroutine(UpgradeScaleRoutine());
    }

    private IEnumerator UpgradeScaleRoutine()
    {
        paddleTransform.localScale = new Vector3(1.6f, 1.1f, 1.1f);
        yield return new WaitForSeconds(5f);
        paddleTransform.localScale = new Vector3(1f, 1.1f, 1.1f);
    }

    void Update()
    {
    #if UNITY_EDITOR || UNITY_STANDALONE
        input = Input.GetAxisRaw("Horizontal");
    #else
        HandleTouchInput();
        input = touchInput;
    #endif
    }

    private void HandleTouchInput()
    {
        touchInput = 0f;

        // Un seul doigt suffit
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        // Quand le doigt touche l'écran
        if (touch.phase == TouchPhase.Began)
        {
            lastTouchX = touch.position.x;
        }
        // Quand le doigt glisse
        else if (touch.phase == TouchPhase.Moved)
        {
            float deltaX = touch.position.x - lastTouchX;

            // Normalisation pour avoir une valeur exploitable
            touchInput = Mathf.Sign(deltaX);

            lastTouchX = touch.position.x;
        }
        // Quand le doigt est relâché
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            touchInput = 0f;
        }
    }


    void FixedUpdate()
    {
        Vector2 position = rb.position;

        position.x += input * speed * Time.fixedDeltaTime;

        float halfWidth = spriteRenderer.bounds.extents.x;

        position.x = Mathf.Clamp(
            position.x,
            -screenHalfWidth + halfWidth,
            screenHalfWidth - halfWidth
        );

        rb.MovePosition(position);
    }
}
