using UnityEngine;
using System.Collections;

public class HitBrick : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D slotCollider;

    private bool isDisabled = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        slotCollider = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDisabled)
            return;

        if (other.CompareTag("Ball"))
        {
            StartCoroutine(HandleHit());
        }
    }

    private IEnumerator HandleHit()
    {
        isDisabled = true;
        slotCollider.enabled = false;
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(0.075f);
        spriteRenderer.enabled = true;
        yield return new WaitForSeconds(0.075f);
        spriteRenderer.enabled = false;
    }
}
