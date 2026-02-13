using UnityEngine;
using System.Collections.Generic;

public enum DeckType
{
    Queue,
    Stack
}

public class DeckManager : MonoBehaviour
{
    // Permet de choisir dans l'inspector si cette instance
    // fonctionne en Queue ou en Stack
    [SerializeField] private DeckType deckType;

    // On garde les deux structures en interne,
    // mais UNE SEULE sera utilisée selon deckType
    private Queue<Card> queueDeck;
    private Stack<Card> stackDeck;

    private void Awake()
    {
        // Initialisation des structures
        queueDeck = new Queue<Card>();
        stackDeck = new Stack<Card>();

        // Création + mélange au lancement
        CreateDeck();
        ShuffleDeck();
    }

    private void CreateDeck()
    {
        queueDeck.Clear();
        stackDeck.Clear();

        foreach (Suit s in System.Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank r in System.Enum.GetValues(typeof(Rank)))
            {
                Card newCard = new Card(s, r);

                if (deckType == DeckType.Queue)
                {
                    queueDeck.Enqueue(newCard);
                }
                else
                {
                    stackDeck.Push(newCard);
                }
            }
        }
    }

    private void ShuffleDeck()
    {
        List<Card> temp;

        if (deckType == DeckType.Queue)
        {
            temp = new List<Card>(queueDeck);
            queueDeck.Clear();
        }
        else
        {
            temp = new List<Card>(stackDeck);
            stackDeck.Clear();
        }

        while (temp.Count > 0)
        {
            int index = Random.Range(0, temp.Count);

            Card selectedCard = temp[index];

            if (deckType == DeckType.Queue)
            {
                queueDeck.Enqueue(selectedCard);
            }
            else
            {
                stackDeck.Push(selectedCard);
            }

            temp.RemoveAt(index);
        }
    }

    public Card DrawCard()
    {
        if (deckType == DeckType.Queue && queueDeck.Count == 0)
        {
            CreateDeck();
            ShuffleDeck();
        }
        else if (deckType == DeckType.Stack && stackDeck.Count == 0)
        {
            CreateDeck();
            ShuffleDeck();
        }

        if (deckType == DeckType.Queue)
        {
            return queueDeck.Dequeue();
        }
        else
        {
            return stackDeck.Pop();
        }
    }
}
