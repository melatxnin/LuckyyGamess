using System.Collections.Generic;

public abstract class HandBase
{
    public List<Card> cards = new List<Card>();

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public void Clear()
    {
        cards.Clear();
    }

    // Chaque jeu devra définir son propre calcul
    public abstract int GetScore();
}
