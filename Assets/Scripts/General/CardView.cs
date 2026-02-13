using UnityEngine;

public class CardView : MonoBehaviour
{
    // Chaque prefab de carte possède un sprite Renderer
    // et ce script. Ce script attaché sur la prefab va 
    // alors se charger de faire correspondre l'apparence
    // de la prefab avec sa valeur dans le dictionnaire
    public SpriteRenderer spriteRenderer;

    // Cette méthode va être appelée par HandView.cs
    public void SetCard(Card card)
    {
        // On fait correspondre le sprite du spriteRenderer du prefab
        // par rapport à ce que transmet GetSprite() de HandView.cs
        spriteRenderer.sprite = CardDatabase.Instance.GetSprite(card);
    }
}
