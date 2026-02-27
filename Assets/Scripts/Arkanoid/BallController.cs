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
        // On a besoin de connaître ArkanoidManager pour la condition
        // de défaite plus bas
        arkanoidManager = FindFirstObjectByType<ArkanoidManager>();

        gameOver = false;
        // On place la balle au centre dès le debut
        transform.position = Vector3.zero;

        // On alcule les limites de l’écran avec la caméra orthographique
        Camera cam = Camera.main;
        screenHalfHeight = cam.orthographicSize;
        screenHalfWidth = screenHalfHeight * cam.aspect;

        // On récupère son rayon
        radius = GetComponent<CircleCollider2D>().bounds.extents.x;

        StartCoroutine(StartGame(timeToStart));
    }

    private IEnumerator StartGame(float timeToStart)
    {
        // On désactive le collider (pour éviter les collisions pendant l’attente)
        // Ensuite on fait clignoter la ball rapidement pour prendre
        // l'attention du joueur et ainsi le prévenir que la balle va commencer à bouger
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

        // Enfin on réactive le collider et on initialise la direction vers le bas
        c.enabled = true;
        direction = Vector2.down;
        ResetBall();
    }

    void ResetBall()
    {
        // On garantit que la balle commence toujours vers le bas, et on normalise le vecteur.
        transform.position = Vector3.zero;
        direction = new Vector2(direction.x, -Mathf.Abs(direction.y)).normalized;
    }

    void Update()
    {
        if (arkanoidManager.isPaused == true)
        {
            return;
        }
        
        // Chaque frame la position de la balle change en fonction de la vitesse qu'on
        // a fournit, multiplié par deltaTime et la direction normalisée surtout
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        CheckWallCollision();
        TurnVisual();
    }

    // Cette fonction fait tout simplement tourner la balle (axe Z) vers la gauche si
    // elle se déplace vers la gauche, sinon vers la droite avec une marge qui fait en
    // sorte que si la balle ne se déplace presque pas horizontalement, elle ne tourne pas
    // C'est purement visuel et n'affecte pas la physique
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

        // Si la balle touche un mur horizontal, à droite ou à gauche,
        // sa direction.x est simplement inversée
        if (pos.x <= -screenHalfWidth + radius && direction.x < 0f)
        {
            direction.x = -direction.x;
        }
        else if (pos.x >= screenHalfWidth - radius && direction.x > 0f)
        {
            direction.x = -direction.x;
        }

        // Même logique pour le plafond, on inverse juste l'axe y
        if (pos.y >= screenHalfHeight - radius && direction.y > 0f)
        {
            direction.y = -direction.y;
        }

        // Par contre si elle passe sous l'écran, elle ne rebondit pas mais
        // le joueur a logiquement perdu, donc on informe le joueur que cette
        // balle est en moins (et lui calculera si la partie est terminée maintenant
        // ou pas selon si c'était la dernière balle en jeu ou si il en reste d'autres)
        // et on détruit cette balle car on n'en a plus besoin
        if (pos.y <= -screenHalfHeight - radius)
        {
            arkanoidManager.Decrementation();
            Destroy(gameObject);
        }
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
            // La balle actuelle peut egalement toucher une autre balle
            // s'il y en a une, donc on lui donne juste la même méthode 
            // de rebond que pour une brique car c'est le même principe
            BounceOnBrick(other);
            SoundManager.Instance.PlaySFX(hitBallSound);
        }
    }

    void BounceOnPaddle(Collider2D paddle)
    {
        // la longueur du paddle
        float paddleHalfWidth = paddle.bounds.extents.x;

        // On calcule la distance horizontale entre le centre de la paddle et la balle
        float hitOffset = transform.position.x - paddle.transform.position.x;

        // On normalise par la largeur de la paddle
        float normalizedOffset = hitOffset / paddleHalfWidth;

        // On clamp
        normalizedOffset = Mathf.Clamp(normalizedOffset, -1f, 1f);

        // On recrée une direction
        direction = new Vector2(normalizedOffset, 1f).normalized;

        // Si la balle touche le centre → va presque verticalement
        // Si elle touche le bord gauche → repart vers la gauche
        // Si elle touche le bord droit → repart vers la droite
    }

    void BounceOnBrick(Collider2D brick)
    {
        // Méthode classique pour connaitre la distance entre les deux objets
        Vector2 ballPos = transform.position;
        Vector2 brickPos = brick.transform.position;
        Vector2 delta = ballPos - brickPos;

        // On sait que bounds.extents c'est la moitié de la taille
        // DOnc on récupère simplement la moitié de la largeur et de la hauteur
        // Et ensuite on divise car on veut savoir si la balle est-elle plus proche 
        // d’un bord horizontal ou vertical ? diviser par la demi-taille permet 
        // d’obtenir une valeur comparable entre X et Y.
        float halfWidth = brick.bounds.extents.x;
        float halfHeight = brick.bounds.extents.y;
        float normalizedX = delta.x / halfWidth;
        float normalizedY = delta.y / halfHeight;

        // On compare l’impact horizontal vs vertical
        // Si impact plus horizontal : on inverse X sinon : on inverse Y
        if (Mathf.Abs(normalizedX) > Mathf.Abs(normalizedY))
        {
            direction = new Vector2(Mathf.Sign(normalizedX),direction.y).normalized;
        }
        else
        {
            direction = new Vector2(direction.x,Mathf.Sign(normalizedY)).normalized;
        }
    }
}
