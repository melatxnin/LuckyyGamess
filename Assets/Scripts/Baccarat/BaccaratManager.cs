using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// On crée un enum appelé GameState qui représente
// les différents états du jeu
public enum StateBaccarat
{
    Start,
    Wait
}

public class BaccaratManager : MonoBehaviour
{
    // On a besoin de DeckManager pour créer le deck, le mélanger et surtout tirer une carte
    public DeckManager deckManager;
    // On a besoin de CoinStack pour gérer la logique de jetons
    public SliderWagerController sliderWagerController;
    public OptionsManager optionsManager;

    // On a besoin de la MAIN VISUELLE du joueur et celle du dealer
    public HandView playerHandView;
    public HandView dealerHandView;

    // On a besoin de la MAIN STRUCTURELLE du joueur et celle du dealer
    private BaccaratHand playerHand = new BaccaratHand();
    private BaccaratHand dealerHand = new BaccaratHand();

    // On a besoin de trois textmeshpro :
    // Un pour afficher le score du joueur
    // Un pour afficher le score du dealer
    // Un pour afficher le résultat (perdu ou gagné)
    public TextMeshPro playerText;
    public TextMeshPro dealerText;
    public TextMeshPro resultText;
    public TextMeshProUGUI creditsText;

    public TextMeshPro distribuerText;

    [SerializeField] private GameObject stateStart;

    [SerializeField] private AudioClip cardSound;
    [SerializeField] private AudioClip playerWinsSound;
    [SerializeField] private AudioClip bankerWinsSound;
    [SerializeField] private AudioClip tieSound;
    [SerializeField] private AudioClip baccaratMusic;

    [SerializeField] private Button[] buttons;

    [SerializeField] private Image playerButtonImage;
    [SerializeField] private Image bankerButtonImage;
    [SerializeField] private Image tieButtonImage;
    [SerializeField] private Image playerPairButtonImage;
    [SerializeField] private Image bankerPairButtonImage;

    [SerializeField] private Sprite[] RedSprites;
    [SerializeField] private Sprite[] GreenSprites;

    private int winStreak = 0;
    private int credits = 0;

    public int numberChoices = 0;
    private bool wait;
    private bool n;
    private bool m;

    // On a besoin de l'état actuelle du jeu via l'enum GameState
    StateBaccarat currentState;

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

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = true;
        }

        SoundManager.Instance.PlayMusic(baccaratMusic);

        // Au demarrage, on est dans l'etat WaitingForDeal, donc le jeu
        // ne fait rien tant qu'on appuie pas sur le bouton Distribuer
        currentState = StateBaccarat.Start;

        numberChoices = 0;

        // À ce moment on a ni cartes, ni resultat, donc aucune raison d'afficher les textes
        playerText.enabled = true;
        dealerText.enabled = true;
        resultText.enabled = false;
        distribuerText.enabled = false;

        // On met a jour les buttons
        UpdateButtons();
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

    public void PlayerButton()
    {
        if (playerButtonImage.sprite == GreenSprites[2])
        {
            playerButtonImage.sprite = RedSprites[2];
            buttons[0].interactable = true;
            numberChoices--;
            n = false;
        }
        else
        {
            playerButtonImage.sprite = GreenSprites[2];
            buttons[0].interactable = false;
            numberChoices++;
            n = true;
        }
    }

    public void BankerButton()
    {
        if (bankerButtonImage.sprite == GreenSprites[0])
        {
            bankerButtonImage.sprite = RedSprites[0];
            buttons[2].interactable = true;
            numberChoices--;
            m = false;
        }
        else
        {
            bankerButtonImage.sprite = GreenSprites[0];
            buttons[2].interactable = false;
            numberChoices++;
            m = true;
        }
    }

    public void TieButton()
    {
        if (tieButtonImage.sprite == GreenSprites[1])
        {
            tieButtonImage.sprite = RedSprites[1];
            numberChoices--;
        }
        else
        {
            tieButtonImage.sprite = GreenSprites[1];
            numberChoices++;
        }
    }

    public void PlayerPairButton()
    {
        if (playerPairButtonImage.sprite == GreenSprites[4])
        {
            playerPairButtonImage.sprite = RedSprites[4];
            numberChoices--;
        }
        else
        {
            playerPairButtonImage.sprite = GreenSprites[4];
            numberChoices++;
        }
    }

    public void BankerPairButton()
    {
        if (bankerPairButtonImage.sprite == GreenSprites[3])
        {
            bankerPairButtonImage.sprite = RedSprites[3];
            numberChoices--;
        }
        else
        {
            bankerPairButtonImage.sprite = GreenSprites[3];
            numberChoices++;
        }
    }

    private void UpdateButtons()
    {
        stateStart.SetActive(currentState == StateBaccarat.Start);
    }

    // Cette méthode est en public non pas pour être appelée via un 
    // autre script, mais pour être appelée via le bouton Distribuer
    // Donc à chaque fois qu'on appuie sur le button Distribuer, il se passe tout ca :
    public void Distribuer()
    {
        if (numberChoices < 1)
        {
            if (!wait) StartCoroutine(TextEnable());
            return;
        }

        int bet = (sliderWagerController.value / 10) * numberChoices;

        RemoveCredit(bet);

        currentState = StateBaccarat.Wait;
        UpdateButtons();

        StartCoroutine(PlayBaccaratRound(bet));
    }

    private IEnumerator TextEnable()
    {
        wait = true;
        distribuerText.text = "AT LEAST ONE BET REQUIRED";
        distribuerText.enabled = true;
        yield return new WaitForSeconds(0.1f);
        distribuerText.enabled = false;
        yield return new WaitForSeconds(0.1f);
        distribuerText.enabled = true;
        yield return new WaitForSeconds(2f);
        distribuerText.enabled = false;
        wait = false;
    }

    private Card DealCard(BaccaratHand hand, HandView view)
    {
        // On joue le son de carte
        SoundManager.Instance.PlaySFX(cardSound);

        // On tire une carte depuis le deck
        Card card = deckManager.DrawCard();

        // On l'ajoute à la logique
        hand.AddCard(card);

        // On l'ajoute à l'affichage
        view.AddCardBaccarat(card);

        // On retourne la carte (utile pour la règle du banker)
        return card;
    }

    private IEnumerator PlayBaccaratRound(int bet)
    {
        playerHand.Clear();
        dealerHand.Clear();
        playerHandView.ClearBaccarat();
        dealerHandView.ClearBaccarat();
        
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }

        resultText.enabled = false;
        yield return new WaitForSeconds(1f);

        // ----- INITIAL DEAL -----

        DealCard(playerHand, playerHandView);
        playerText.text = "PLAYER : " + playerHand.GetScore();
        yield return new WaitForSeconds(1f);

        DealCard(dealerHand, dealerHandView);
        dealerText.text = "DEALER : " + dealerHand.GetScore();
        yield return new WaitForSeconds(1f);

        DealCard(playerHand, playerHandView);
        playerText.text = "PLAYER : " + playerHand.GetScore();
        yield return new WaitForSeconds(1f);

        DealCard(dealerHand, dealerHandView);
        dealerText.text = "DEALER : " + dealerHand.GetScore();
        yield return new WaitForSeconds(1f);

        int playerScore = playerHand.GetScore();
        int bankerScore = dealerHand.GetScore();

        // ----- NATURAL CHECK -----

        if (playerScore >= 8 || bankerScore >= 8)
        {
            ResolveRound(bet);
            yield break;
        }

        Card? playerThirdCard = null;

        // ----- PLAYER RULE -----

        if (playerScore <= 5)
        {
            playerThirdCard = DealCard(playerHand, playerHandView);
            playerText.text = "PLAYER : " + playerHand.GetScore();
            yield return new WaitForSeconds(1f);
        }

        // ----- BANKER RULE -----

        BankerDrawRule(playerThirdCard);
        yield return new WaitForSeconds(1f);

        ResolveRound(bet);
    }

    private void ResolveRound(int bet)
    {
        int playerScore = playerHand.GetScore();
        int bankerScore = dealerHand.GetScore();

        if (playerScore > bankerScore)
        {
            HandlePlayerWin(bet);
        }
        else if (bankerScore > playerScore)
        {
            HandleBankerWin(bet);
        }
        else
        {
            HandleTie(bet);
        }

        StartCoroutine(YouWinRoutine());
    }

    private void HandlePlayerWin(int bet)
    {
        SoundManager.Instance.PlaySFX(playerWinsSound);

        resultText.text = "<color=blue>PLAYER WINS</color>";
        resultText.enabled = true;

        if (playerButtonImage.sprite == GreenSprites[2])
        {
            AddCredits(bet * 2);
        }

        if (IsPlayerPair() && playerPairButtonImage.sprite == GreenSprites[4])
        {
            AddCredits(bet * 12);
            QuestsManager.Instance.Notify(QuestType.WinPlayerPairBaccarat);
        }
    }

    private void BankerDrawRule(Card? playerThirdCard)
    {
        int bankerScore = dealerHand.GetScore();

        if (playerThirdCard == null)
        {
            if (bankerScore <= 5)
            {
                DealCard(dealerHand, dealerHandView);
                dealerText.text = "DEALER : " + dealerHand.GetScore();
            }
            return;
        }

        int value = (int)playerThirdCard.Value.rank % 10;

        if (bankerScore <= 2)
        {
            DealCard(dealerHand, dealerHandView);
            dealerText.text = "DEALER : " + dealerHand.GetScore();
        }
        else if (bankerScore == 3 && value != 8)
        {
            DealCard(dealerHand, dealerHandView);
            dealerText.text = "DEALER : " + dealerHand.GetScore();
        }
        else if (bankerScore == 4 && value >= 2 && value <= 7)
        {
            DealCard(dealerHand, dealerHandView);
            dealerText.text = "DEALER : " + dealerHand.GetScore();
        }
        else if (bankerScore == 5 && value >= 4 && value <= 7)
        {
            DealCard(dealerHand, dealerHandView);
            dealerText.text = "DEALER : " + dealerHand.GetScore();
        }
        else if (bankerScore == 6 && (value == 6 || value == 7))
        {
            DealCard(dealerHand, dealerHandView);
            dealerText.text = "DEALER : " + dealerHand.GetScore();
        }
    }

    private bool IsPlayerPair()
    {
        if (playerHand.cards.Count < 2) return false;

        return playerHand.cards[0].rank == playerHand.cards[1].rank;
    }

    private bool IsBankerPair()
    {
        if (dealerHand.cards.Count < 2) return false;

        return dealerHand.cards[0].rank == dealerHand.cards[1].rank;
    }

    private void HandleBankerWin(int bet)
    {
        SoundManager.Instance.PlaySFX(bankerWinsSound);

        resultText.text = "<color=red>BANKER WINS</color>";
        resultText.enabled = true;

        if (bankerButtonImage.sprite == GreenSprites[0])
        {
            int payout = Mathf.FloorToInt(bet * 1.95f);
            AddCredits(payout);
        }

        if (IsBankerPair() && bankerPairButtonImage.sprite == GreenSprites[3])
        {
            AddCredits(bet * 12);
            QuestsManager.Instance.Notify(QuestType.WinBankerPairBaccarat);
        }
    }

    private void HandleTie(int bet)
    {
        SoundManager.Instance.PlaySFX(tieSound);

        resultText.text = "TIE";
        resultText.enabled = true;

        if (tieButtonImage.sprite == GreenSprites[1])
        {
            AddCredits(bet * 9);
            QuestsManager.Instance.Notify(QuestType.WinEgalityBaccarat);
        }
    }

    private IEnumerator YouWinRoutine()
    {
        yield return new WaitForSeconds(2.5f);

        resultText.enabled = false;

        currentState = StateBaccarat.Start;
        UpdateButtons();

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == 0 && n) i++;
            if (i == 2 && m) i++;
            buttons[i].interactable = true;
        }
    }
}
