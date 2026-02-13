// Représente l'ensemble fini des couleurs possibles d'une carte
public enum Suit
{
    Clubs,
    Diamonds,
    Hearts,
    Spades
}

// Représente l'ensemble fini des valeurs possibles d'une carte
public enum Rank
{
    // Un enum est un ensemble de nombres entiers, dont le premier vaut 0 par défaut
    // Mais ici j'indique explicitement que le premier int (Ace) vaudra 1 avec "= 1"
    // Les valeurs suivantes s'ajustent, donc Two vaut 2, Three vaut 3, etc
    // Ce choix permet d'aligner les valeurs de cet enum avec les indices du tableau
    // de valeurs utilisé dans Hand.cs
    Ace = 1,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King
}

public struct Card
{
    // Cette structure contient Suit et Rank, donc tout ce qu'il faut
    // pour décrire complètement chacune des cartes du jeu
    // Preuve mathématique : Suit a 4 valeurs, Rank en a 13
    // ce qui donne (4 * 13) cartes différentes donc 52 cartes
    public Suit suit;
    public Rank rank;

    // Je définit à l'intérieur de la structure Card un constructeur
    // qui sera appelé automatiquement lors de la création d'une carte
    public Card(Suit s, Rank r)
    {
        suit = s;
        rank = r;
    }
}
