using UnityEngine;
using System.Collections.Generic;

public class CardDatabase : MonoBehaviour
{
    // Je fais une instance de ce script pour une simplicité
    // d'accès même si ce n'est objectivement pas obligatoire
    public static CardDatabase Instance;

    // On crée un dictionnaire avec comme clés des string et
    // comme valeur un Sprite, donc on aura un dictionnaire 
    // représentant toutes nos cartes, aussi bien en terme
    // de valeur numérique que de visuel
    public Dictionary<string, Sprite> cardSprites = new Dictionary<string, Sprite>();

    // On utilise System.Serializable pour afficher cette structure 
    // dans l'inspector, et ainsi la remplir nous même dans Unity
    [System.Serializable]
    public struct CardSpriteEntry
    {
        public Suit suit;
        public Rank rank;
        public Sprite sprite;
    }

    // On crée un tableau de CardSpriteEntry, donc un tableau de structures
    // Je dois alors remplir dans l'inspector chaque structure, donc pour
    // chacune je met un Suit, un Rank et un Sprite bien a elle
    // Deux structures doivent ainsi avoir au maximum 1 point commun,
    // soit le Rank, soit le Suit, car elles peuvent être toutes les deux
    // des dix ou des carreaux, mais pas toutes les deux des dix de carreau
    public CardSpriteEntry[] entries;

    void Awake()
    {
        // Lexique de base pour une instance, si il n'y a pas encore
        // d'instance, ce script en devient une, sinon le gameObject qui
        // contient ce script est détruit
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Pour chaque CardSpriteEntry qu'on va appeler 'e' dans notre tableau :
        foreach (CardSpriteEntry e in entries)
        {
            // On définit une string 'key' qui n'est pas égale à n'importe quoi
            // Elle est égale à la couleur et à la valeur précise de la carte
            // On se sert alors de GetKey() plus bas pour l'écrire correctement
            string key = GetKey(e.suit, e.rank);
            // On remplit notre dictionnaire vide avec la clé 'key' que l'on
            // a obtenu juste au dessus, donc par exemple "Clubs_Five"
            // et on dit que cette clé est égale au Sprite correspondant
            // dans le tableau que j'ai remplit dans l'inspector
            cardSprites[key] = e.sprite;
        }
    }

    public Sprite GetSprite(Card card)
    {
        // Méthode appelée par CardView, elle sert a afficher
        // visuellement à l'écran le sprite de la carte choisie 
        string key = GetKey(card.suit, card.rank);
        return cardSprites[key];
    }

    private string GetKey(Suit suit, Rank rank)
    {
        // Cette méthode va renvoyer une string à chaque 'key' de la boucle
        // foreach dans Awake, avec cette écriture :
        // le Suit donné, par exemple "Clubs", suivit d'un tiret du bas,
        // suivit du Rank donné, par exemple "Five", ce qui donne dans cet exemple :
        // return "Clubs_Five"
        return suit.ToString() + "_" + rank.ToString();
    }
}
