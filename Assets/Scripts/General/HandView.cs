using UnityEngine;
using System.Collections.Generic;

// Ce script sera sur deux gameObjects : un pour le joueur,
// et un autre pour le dealer, respectivement a gauche et a droite
// Chaque personne a besoin d'un main visuelle, d'où deux utilisation
public class HandView : MonoBehaviour
{
    // On a besoin du prefab du script CardView pour afficher les cartes,
    // un script que l'on retrouve pour rappel sur le prefab de carte
    public CardView cardPrefab;

    // Valeur réglable dans l'inspector, on s'en sert dans AddCard
    // Pour ajouter visuellement une nouvelle carte à notre main
    // de facon assez espacée pour voir suffisament la couleur
    // et la valeur de chaque carte lorsqu'il y en a plusieurs
    public float spacing = 1.2f;

    // On crée une liste appelée currentCards qui représente les cartes
    // actuelle du joueur (ou du dealer). C'est une liste de CardView,
    // donc une liste de prefab qui changera de taille suivant le nombre
    // de carte possédée dans leur main
    private List<CardView> currentCards = new List<CardView>();

    // Méthode appelée par GameManager, lorsqu'un tour recommence, on a besoin d'effacer
    // les cartes dans la main du joueur et du dealer, d'un part en termes de données, chose
    // que l'on fait dans Hand, et d'autre part en termes de visuel, chose que l'on fait
    // dans ce script via cette méthode super incroyablement géniale
    public void ClearBlackJack()
    {
        // Tout simplement, pour chaque CardView dans la liste de currentCards,
        // autrement dit pour chaque prefab dans la liste de prefab,
        // on detruit ces prefabs, le visuel n'existe plus
        foreach (CardView c in currentCards)
            Destroy(c.gameObject);
        
        // Pour être sûr que la main est vide, qu'elle ne possède plus
        // aucune carte, on la Clear(), donc on vide tout ce qu'il y a dedans
        currentCards.Clear();
        transform.position = new Vector3 (0, transform.position.y, 0f);
    }

    public void ClearBaccarat()
    {
        // Tout simplement, pour chaque CardView dans la liste de currentCards,
        // autrement dit pour chaque prefab dans la liste de prefab,
        // on detruit ces prefabs, le visuel n'existe plus
        foreach (CardView c in currentCards)
            Destroy(c.gameObject);
        
        // Pour être sûr que la main est vide, qu'elle ne possède plus
        // aucune carte, on la Clear(), donc onn vide tout ce qu'il y a dedans
        currentCards.Clear();
        transform.position = new Vector3 (transform.position.x, 1.65f, 0f);
    }

    // Méthode également appelée par GameManager, cette fois ci non pas pour vider la main
    // mais pour au contraire lui rajouter une carte (le même système que dans Hand.cs avec
    // AddCard et Clear, mais ici côté visuel seulememnt)
    public void AddCardBlackJack(Card card)
    {
        // Création de la carte
        CardView nouvelleCarte = Instantiate(cardPrefab, transform);

        // Attribution du visuel
        nouvelleCarte.SetCard(card);

        // IMPORTANT : on ajoute d'abord la carte à la liste
        currentCards.Add(nouvelleCarte);

        // Maintenant le count est correct
        int count = currentCards.Count;

        float totalWidth = (count - 1) * spacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            currentCards[i].transform.localPosition =
                new Vector3(startX + i * spacing, 0f, 0f);
        }
    }

    public void AddCardBaccarat(Card card)
    {
        // On crée une nouvelle prefab à la position du 
        // GameObject qui possède ce script
        CardView nouvelleCarte = Instantiate(cardPrefab, transform);

        // On doit donner un visuel à cette carte, mais pas un visuel aléatoire, 
        // on appelle alors la méthode SetCard dans CardView.cs en lui transferant 
        // les données de la carte qui viennent de GameManager.cs
        nouvelleCarte.SetCard(card);

        // Pour ne pas monter la nouvelle carte n'importe où sur l'écran, 
        // on a besoin de connaitre le nombre actuelle de carte dans la liste currents,
        // Par exemple, si le joueur a 3 cartes dans sa main, index vaut 3
        int index = currentCards.Count;

        // On place la carte sur l'axe x à index, donc 3 si on reprend l'exemple juste au dessus
        // on la place à x = 3, mais on ajoute spacing, pour comme son nom l'indique l'espacer
        // suffisament des autres cartes, ce qui donne 3 * 1.2f = 3.6f. La carte suivante sera 
        // à 4 * 1.2f = 4.8f, ce qui correspond bien à un espacement de 1.2f à chaque fois
        // Pour l'axe Y, on met zero car on veut que les cartes soient bien au milieu de la main
        // du joueur et du dealer, et pour l'axe Z, on met zero car on est en 2D
        nouvelleCarte.transform.localPosition = new Vector3(0f, index * (spacing * -1), 0f);

        // On ajoute à notre liste de prefab ce dernier une fois qu'il est bien placé
        currentCards.Add(nouvelleCarte);
    }
}
