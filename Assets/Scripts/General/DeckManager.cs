using UnityEngine;
using System.Collections.Generic;

// Pour la Queue et la Stack que l'on doit mettre dans le jeu,
// J'ai choisi de créer DeckManager.cs, un script général dans le
// sens où il fonctionne aussi bien pour le jeu du Baccarat que
// pour le jeu du BlackJack
// Soit il fonctionne comme une Queue FIFO, soit comme une Stack
// LIFO, car un packet de car peut coller à ces deux méthodes
// Dans l'inspector on choisit juste notre mode, soit Queue, soit
// Stack, et le déroulement de la partie est le même
public enum DeckType
{
    Queue,
    Stack
}

public class DeckManager : MonoBehaviour
{

    [SerializeField] private DeckType deckType;

    // On initialise une Queue et une Stack, mais on ne va en utilier
    // qu'une selon ce qu'on a mis dans l'inspector
    private Queue<Card> queueDeck;
    private Stack<Card> stackDeck;

    private void Awake()
    {
        queueDeck = new Queue<Card>();
        stackDeck = new Stack<Card>();

        CreateDeck();
        ShuffleDeck();
    }

    private void CreateDeck()
    {
        queueDeck.Clear();
        stackDeck.Clear();

        // On parcourt toutes les couleurs (Suit) et toutes les valeurs (Rank)
        // donc si 4 suits et 13 ranks, ca fait 52 cartes créées
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
