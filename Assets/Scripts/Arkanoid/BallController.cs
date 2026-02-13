using System;
using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer ballSprite;
    
    private ArkanoidManager arkanoidManager;

    [SerializeField] private float signForSpeed;

    [SerializeField] private float speed = 8f;
    [SerializeField] private float timeToStart = 2f;
    [SerializeField] private float turnSpeed = 200f;

    [SerializeField] private AudioClip hitBallSound;
    
    public bool gameOver;

    private Vector2 direction;

    private float screenHalfWidth;
    private float screenHalfHeight;
    private float radius;

    void Start()
    {
        arkanoidManager = FindFirstObjectByType<ArkanoidManager>();

        gameOver = false;
        transform.position = Vector3.zero;
        Camera cam = Camera.main;

        screenHalfHeight = cam.orthographicSize;
        screenHalfWidth = screenHalfHeight * cam.aspect;

        radius = GetComponent<CircleCollider2D>().bounds.extents.x;

        StartCoroutine(StartGame(timeToStart));
    }

    private IEnumerator StartGame(float timeToStart)
    {
        Collider2D c = gameObject.GetComponent<Collider2D>();
        c.enabled = false;
        yield return new WaitForSeconds(timeToStart);
        ballSprite.enabled = false;
        yield return new WaitForSeconds(0.25f);
        ballSprite.enabled = true;
        yield return new WaitForSeconds(0.25f);
        ballSprite.enabled = false;
        yield return new WaitForSeconds(0.25f);
        ballSprite.enabled = true;
        yield return new WaitForSeconds(0.25f);
        ballSprite.enabled = false;
        yield return new WaitForSeconds(0.25f);
        ballSprite.enabled = true;
        yield return new WaitForSeconds(1f);

        c.enabled = true;
        direction = Vector2.down;
        ResetBall();
    }

    void Update()
    {
        if (arkanoidManager.isPaused == true)
        {
            return;
        }
        
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        CheckWallCollision();
        TurnVisual();
    }

    private void TurnVisual()
    {
        if (Mathf.Abs(direction.x) < 0.05f)
            return;

        signForSpeed = direction.x > 0 ? -1f : 1f;
        transform.Rotate(new Vector3(0, 0, turnSpeed * Time.deltaTime * signForSpeed));
    }

    void CheckWallCollision()
    {
        Vector3 pos = transform.position;

        if (pos.x <= -screenHalfWidth + radius && direction.x < 0f)
        {
            direction.x = -direction.x;
        }
        else if (pos.x >= screenHalfWidth - radius && direction.x > 0f)
        {
            direction.x = -direction.x;
        }

        if (pos.y >= screenHalfHeight - radius && direction.y > 0f)
        {
            direction.y = -direction.y;
        }

        if (pos.y <= -screenHalfHeight - radius)
        {
            arkanoidManager.Decrementation();
            Destroy(gameObject);
        }
    }

    void ResetBall()
    {
        transform.position = Vector3.zero;
        direction = new Vector2(direction.x, -Mathf.Abs(direction.y)).normalized;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BounceOnPaddle(other);
            SoundManager.Instance.PlaySFX(hitBallSound);
        }
        else if (other.CompareTag("Brick"))
        {
            BounceOnBrick(other);
            SoundManager.Instance.PlaySFX(hitBallSound);
        }
        else if (other.CompareTag("Ball"))
        {
            BounceOnBrick(other);
            SoundManager.Instance.PlaySFX(hitBallSound);
        }
    }

    void BounceOnPaddle(Collider2D paddle)
    {
        float paddleHalfWidth = paddle.bounds.extents.x;

        float hitOffset = transform.position.x - paddle.transform.position.x;

        float normalizedOffset = hitOffset / paddleHalfWidth;

        normalizedOffset = Mathf.Clamp(normalizedOffset, -1f, 1f);

        direction = new Vector2(
            normalizedOffset,
            1f
        ).normalized;
    }

    void BounceOnBrick(Collider2D brick)
    {
        Vector2 ballPos = transform.position;
        Vector2 brickPos = brick.transform.position;

        Vector2 delta = ballPos - brickPos;

        float halfWidth = brick.bounds.extents.x;
        float halfHeight = brick.bounds.extents.y;

        float normalizedX = delta.x / halfWidth;
        float normalizedY = delta.y / halfHeight;

        if (Mathf.Abs(normalizedX) > Mathf.Abs(normalizedY))
        {
            direction = new Vector2(
                Mathf.Sign(normalizedX),
                direction.y
            ).normalized;
        }
        else
        {
            direction = new Vector2(
                direction.x,
                Mathf.Sign(normalizedY)
            ).normalized;
        }
    }
}
