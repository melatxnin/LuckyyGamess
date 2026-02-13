using UnityEngine;

public class BrickData : MonoBehaviour
{
    public Sprite sprite;

    [SerializeField] private ArkanoidManager arkanoidManager;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            arkanoidManager.AddSprite(sprite);
        }
    }
}
