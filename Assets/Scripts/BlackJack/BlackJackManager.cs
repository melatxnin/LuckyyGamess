using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// On crée un enum appelé GameState qui représente
// les différents états du jeu
public enum StateBlackJack
{
    Start,
    NormalTurn,
    DoublerTurn,
    Assurance,
    DealerTurn,
    Wait
}

public class BlackJackManager : MonoBehaviour
{
    // On a besoin de DeckManager pour créer le deck, le mélanger et surtout tirer une carte
    public DeckManager deckManager;
    public OptionsManager optionsManager;
    public SliderWagerController sliderWagerController;

    public GameObject state_Normal;
    public GameObject state_Doubler;
    public GameObject state_Assurance;
    public GameObject state_Start;

    // On a besoin de la MAIN VISUELLE du joueur et celle du dealer
    public HandView playerHandView;
    public HandView dealerHandView;

    // On a besoin de la MAIN STRUCTURELLE du joueur et celle du dealer
    private BlackjackHand playerHand = new BlackjackHand();
    private BlackjackHand dealerHand = new BlackjackHand();

    // Carte cachée du dealer (assurance)
    private Card? dealerHiddenCard = null;  

    // On a besoin de trois textmeshpro :
    // Un pour afficher le score du joueur
    // Un pour afficher le score du dealer
    // Un pour afficher le résultat (perdu ou gagné)
    public TextMeshPro playerText;
    public TextMeshPro dealerText;
    public TextMeshPro resultText;
    public TextMeshProUGUI creditsText;

    [SerializeField] private AudioClip cardSound;
    [SerializeField] private AudioClip youWinSound;
    [SerializeField] private AudioClip youLoseSound;
    [SerializeField] private AudioClip pushSound;
    [SerializeField] private AudioClip blackJackMusic;

    private int coinBonus = 0;
    private bool hasDoubled = false;
    private int winStreak = 0;
    private int credits = 0;

    // On a besoin de l'état actuelle du jeu via l'enum GameState
    StateBlackJack currentState;

    private bool assurance = false;

    private void OnEnable()
    {
        if (ScoreManager.Instance.isBet == true)
        {
            credits = sliderWagerController.value;
            creditsText.text = "CREDITS : $" + sliderWagerController.value.ToString();
        }
        else
        {
            creditsText.text = "FREE MODE";
        }

        SoundManager.Instance.PlayMusic(blackJackMusic);

        // Au demarrage, on est dans l'etat WaitingForDeal, donc le jeu
        // ne fait rien tant qu'on appuie pas sur le bouton Distribuer
        currentState = StateBlackJack.Start;

        // À ce moment on a ni cartes, ni resultat, donc aucune raison d'afficher les textes
        playerText.enabled = true;
        dealerText.enabled = true;
        resultText.enabled = false;

        // On met a jour les buttons
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        // Le button distribuer devient interactif seulement si on est dans l'etat de jeu WaitingForDeal
        state_Start.SetActive(currentState == StateBlackJack.Start);
        // Le button tirer devient interactif seulement si on est dans l'etat de jeu PlayerTurn
        state_Normal.SetActive(currentState == StateBlackJack.NormalTurn);
        // Le button rester devient interactif seulement si on est dans l'etat de jeu PlayerTurn
        state_Doubler.SetActive(currentState == StateBlackJack.DoublerTurn);
        // Le button rester devient interactif seulement si on est dans l'etat de jeu PlayerTurn
        state_Assurance.SetActive(currentState == StateBlackJack.Assurance);
    }

    // Cette méthode est en public non pas pour être appelée via un 
    // autre script, mais pour être appelée via le bouton Distribuer
    // Donc à chaque fois qu'on appuie sur le button Distribuer, il se passe tout ca :
    public void Distribuer()
    {
        StartCoroutine(DistribuerRoutine());
    }

    private IEnumerator DistribuerRoutine()
    {
        // On affiche le score du joueur et du dealer, un affichage logique
        // car cette méthode donne bien un score à chacun, mais il n'y a pour
        // l'instant pas de résultat, on attend le second choix du joueur, donc
        // pas de texte Résultat disponible
        resultText.enabled = false;
        playerText.enabled = true;
        dealerText.enabled = true;

        coinBonus = 0;
        hasDoubled = false;
        assurance = false;

        currentState = StateBlackJack.Wait;
        UpdateButtons();

        if (ScoreManager.Instance.isBet == true)
        {
            RemoveCredit(sliderWagerController.value / 10);
        }

        // On vide la liste de Card du joueur et celle du dealer
        playerHand.Clear();
        dealerHand.Clear();
        // On vide la liste de CardView du joueur et celle du dealer
        playerHandView.ClearBlackJack();
        dealerHandView.ClearBlackJack();

        // La règle du Blackest simple, dans l'ordre :
        // On tire une première nouvelle carte pour le joueur
        // On met à jour le score du joueur
        // On tire une première nouvelle carte pour le dealer
        // On met à jour le score du dealer
        // On tire une deuxième carte pour le joueur
        // On met à jour le score du joueur
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.PlaySFX(cardSound);
        playerHand.AddCard(deckManager.DrawCard());
        playerHandView.AddCardBlackJack(playerHand.cards[^1]);
        playerText.text = "PLAYER : " + playerHand.GetScore();
        playerText.enabled = true;
        yield return new WaitForSeconds(1f);

        SoundManager.Instance.PlaySFX(cardSound);
        dealerHand.AddCard(deckManager.DrawCard());
        dealerHandView.AddCardBlackJack(dealerHand.cards[^1]);
        dealerText.text = "DEALER : " + dealerHand.GetScore();
        dealerText.enabled = true;
        yield return new WaitForSeconds(1f);

        SoundManager.Instance.PlaySFX(cardSound);
        playerHand.AddCard(deckManager.DrawCard());
        playerHandView.AddCardBlackJack(playerHand.cards[^1]);
        playerText.text = "PLAYER : " + playerHand.GetScore();

        dealerHiddenCard = deckManager.DrawCard();
        int scoreDealer = dealerHand.GetScore();
        int scorePlayer = playerHand.GetScore();

        if (scoreDealer == 11 && credits >= sliderWagerController.value / 10)
        {
            currentState = StateBlackJack.Assurance;
            UpdateButtons();
        }
        else if (scorePlayer == 21)
        {
            currentState = StateBlackJack.Wait;
            UpdateButtons();
            QuestsManager.Instance.Notify(QuestType.Hit21Blackjack);
            StartCoroutine(DealerPlay());
        }
        else
        {
            CheckPlayer();
        }
    }

    private void AddCredits(int x)
    {
        credits += x;
        ShowCredits();
        Handheld.Vibrate();

        ScoreManager.Instance.AddCredits(x);
    }

    private void RemoveCredit(int x)
    {
        credits -= x;
        ShowCredits();
        ScoreManager.Instance.RemoveCredits(x);
    }

    private void ShowCredits()
    {
        if (credits < sliderWagerController.value)
        {
            creditsText.text = "CREDITS : " + "<color=red>$" + credits + "</color>";
        }
        else if (credits > sliderWagerController.value)
        {
            creditsText.text = "CREDITS : " + "<color=green>$" + credits + "</color>";
        }
        else
        {
            creditsText.text = "CREDITS : $" + credits;
        }
    }

    public void Tirer()
    {
        StartCoroutine(TirerRoutine());
    }

    private IEnumerator TirerRoutine()
    {
        currentState = StateBlackJack.Wait;
        UpdateButtons();

        // Le joueur tire une nouvelle carte
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.PlaySFX(cardSound);
        playerHand.AddCard(deckManager.DrawCard());
        playerHandView.AddCardBlackJack(playerHand.cards[^1]);
        playerText.text = "PLAYER : " + playerHand.GetScore();

        CheckPlayer();
    }

    // À chaque fois qu'on appuie sur le button Rester, il se passe tout ca :
    public void Rester()
    {
        // GameState passe à DealerTurn, car on choisi de rester,
        // donc on le laisse tirer ses cartes 
        currentState = StateBlackJack.DealerTurn;

        // On met a jour les buttons
        UpdateButtons();

        // On lance DealerPlay()
        StartCoroutine(DealerPlay());
    }

    public void Assurance()
    {
        if (ScoreManager.Instance.isBet == true)
        {
            RemoveCredit(sliderWagerController.value / 10);
        }

        // On ajoute la carte cachée au dealer
        dealerHand.AddCard(dealerHiddenCard.Value);
        assurance = true;

        StartCoroutine(SuspenseForRester());
    }

    private IEnumerator SuspenseForRester()
    {
        currentState = StateBlackJack.Wait;
        UpdateButtons();

        yield return new WaitForSeconds(2f);

        if (dealerHand.GetScore() == 21)
        {
            dealerHandView.AddCardBlackJack(dealerHand.cards[^1]);
            dealerText.text = "DEALER : " + dealerHand.GetScore();
            dealerHiddenCard = null;
            ResolveRound();
        }
        else
        {
            CheckPlayer();
        }
    }

    public void RefuserAssurance()
    {
        // Le dealer révèle sa carte cachée en DONNÉE
        dealerHand.AddCard(dealerHiddenCard.Value);

        assurance = true;

        StartCoroutine(SuspenseForAssurance());
    }

    private IEnumerator SuspenseForAssurance()
    {
        currentState = StateBlackJack.Wait;
        UpdateButtons();

        yield return new WaitForSeconds(2f);

        if (dealerHand.GetScore() == 21)
        {
            dealerHandView.AddCardBlackJack(dealerHand.cards[^1]);
            dealerText.text = "DEALER : " + dealerHand.GetScore();
            dealerHiddenCard = null;
            ResolveRound();
        }
        else
        {
            CheckPlayer();
        }
    }

    public void Doubler()
    {
        StartCoroutine(DoublerRoutine());
    }

    private IEnumerator DoublerRoutine()
    {
        currentState = StateBlackJack.Wait;
        UpdateButtons();

        if (ScoreManager.Instance.isBet == true)
        {
            RemoveCredit(sliderWagerController.value / 10);
        }
        coinBonus = 1;
        
        // Le joueur tire une nouvelle carte
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.PlaySFX(cardSound);
        playerHand.AddCard(deckManager.DrawCard());
        playerHandView.AddCardBlackJack(playerHand.cards[^1]);
        playerText.text = "PLAYER : " + playerHand.GetScore();

        yield return new WaitForSeconds(1f);
        hasDoubled = true;
        CheckPlayer();
    }

    private void CheckPlayer()
    {
        int scorePlayer = playerHand.GetScore();

        // Si le score du joueur est supérieur ou égal
        // à 21, on passe directement au tour du dealer
        // GameState passe donc à DealerTurn, on update  
        // les buttons et on lance la DealerPlay()
        if (scorePlayer > 21)
        {
            ResolveRound();
            return;
        }

        if (scorePlayer == 21 || hasDoubled)
        {
            currentState = StateBlackJack.Wait;
            UpdateButtons();
            StartCoroutine(DealerPlay());
            return;
        }

        if (credits >= sliderWagerController.value / 10 && playerHand.cards.Count == 2)
        {
            currentState = StateBlackJack.DoublerTurn;
            UpdateButtons();
            return;
        }

        currentState = StateBlackJack.NormalTurn;
        UpdateButtons();
    }

    // Cette méthode est appelée soit lorsque le joueur a BlackJack,
    // soit lorsque le score du joueur est supérieur à 21, soit
    // lorsque l'on appuie sur le button Rester
    private IEnumerator DealerPlay()
    {
        if (dealerHiddenCard != null && assurance)
        {
            SoundManager.Instance.PlaySFX(cardSound);
            dealerHandView.AddCardBlackJack(dealerHand.cards[^1]);
            dealerText.text = "DEALER : " + dealerHand.GetScore();
            dealerHiddenCard = null;
            assurance = false;
        }

        // Simplement, tant que le score du dealer est inférieur à 17,
        // celui-ci continue de tirer une carte. Il arrête donc de tirer 
        // dès que son score est entre 17 et 21, ou dès que son score dépasse 21
        // On oublie pas de mettre son score visuel (dealerText) à jour à chaque tirage
        while (dealerHand.GetScore() < 17)
        {
            yield return new WaitForSeconds(1f);
            SoundManager.Instance.PlaySFX(cardSound);
            dealerHand.AddCard(deckManager.DrawCard());
            dealerHandView.AddCardBlackJack(dealerHand.cards[^1]);
            dealerText.text = "DEALER : " + dealerHand.GetScore();
        }

        ResolveRound();
    }

    private void ResolveRound()
    {
        int playerScore = playerHand.GetScore();
        int dealerScore = dealerHand.GetScore();

        if (assurance)
        {
            SoundManager.Instance.PlaySFX(youWinSound);
            resultText.text = "<color=green>YOU WIN</color>";
            if (ScoreManager.Instance.isBet == true)
            {
                AddCredits(sliderWagerController.value / 10 * 2);
            }
            winStreak++;
            QuestsManager.Instance.Notify(QuestType.WinAssuranceBlackJack, 1);
            if (winStreak == 3)
            {
                QuestsManager.Instance.Notify(QuestType.BeatDealer3InARowBlackJack, 1);
            }
            resultText.enabled = true;
            currentState = StateBlackJack.Start;
            UpdateButtons();
            return;
        }

        if (playerScore == dealerScore)
        {
            if (ScoreManager.Instance.isBet == true)
            {
                if (coinBonus == 0)
                {
                    AddCredits(sliderWagerController.value / 10);
                }
                else
                {
                    AddCredits(sliderWagerController.value / 10 * 2);
                }
            }

            SoundManager.Instance.PlaySFX(pushSound);
            resultText.text = "PUSH";
        }
        else if (playerScore <= 21 && (dealerScore > 21 || playerScore > dealerScore))
        {
            SoundManager.Instance.PlaySFX(youWinSound);
            resultText.text = "<color=green>YOU WIN</color>";

            if (ScoreManager.Instance.isBet == true)
            {
                if (coinBonus == 0)
                {
                    AddCredits(sliderWagerController.value / 10 * 2);
                }
                else
                {
                    AddCredits(sliderWagerController.value / 10 * 4);
                }
            }

            winStreak++;
            if (winStreak == 3)
            {
                QuestsManager.Instance.Notify(QuestType.BeatDealer3InARowBlackJack, 1);
            }
        }
        else
        {
            SoundManager.Instance.PlaySFX(youLoseSound);
            resultText.text = "<color=red>YOU LOSE</color>";
            winStreak = 0;
        }

        resultText.enabled = true;

        if (credits < sliderWagerController.value / 10)
        {
            currentState = StateBlackJack.Wait;
            UpdateButtons();
            optionsManager.GameOver();
            return;
        }

        StartCoroutine(YouWinRoutine());
    }

    private IEnumerator YouWinRoutine()
    {
        yield return new WaitForSeconds(2.5f);
        resultText.enabled = false;
        currentState = StateBlackJack.Start;
        UpdateButtons();
    }
}
