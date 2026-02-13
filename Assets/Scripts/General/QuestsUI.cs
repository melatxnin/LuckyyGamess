using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestsUI : MonoBehaviour
{
    public TextMeshProUGUI[] challengeTexts;

    public Image[] challengeImages;

    public Sprite emptySlot;
    public Sprite filledSlot;

    private void OnEnable()
    {
        TryRefresh();
    }

    private void TryRefresh()
    {
        if (QuestsManager.Instance == null) 
            return;

        RefreshUI();
    }

    private void Start()
    {
        TryRefresh();
    }

    private string GetChallengeText(Quest challenge)
    {
        switch (challenge.type)
        {
            case QuestType.Get777BonusArkanoidRoyale:
                return "GET THE <color=red>777<color=white> BONUS IN ARKANOID ROYALE";

            case QuestType.Hit21Blackjack:
                return "HIT 21 WITH YOUR FIRST TWO CARDS IN BLACK JACK";

            case QuestType.BeatDealer3InARowBlackJack:
                return "BEAT THE DEALER 3 TIMES IN A ROW IN BLACK JACK";
            
            case QuestType.ThreeBallsArkanoidRoyale:
                return "REACH THREE BALLS IN PLAY IN ARKANOID ROYALE";

            case QuestType.WinAssuranceBlackJack:
                return "WIN WITH INSURANCE ON DEALER IN BLACK JACK";

            case QuestType.WinEgalityBaccarat:
                return "WIN BY BETTING ON TIE IN BACCARAT";

            case QuestType.WinPlayerPairBaccarat:
                return "WIN BY BETTING ON PLAYER PAIR IN BACCARAT";

            case QuestType.WinBankerPairBaccarat:
                return "WIN BY BETTING ON BANKER PAIR IN BACCARAT";

            default:
                return "Unknown Challenge";
        }
    }

    public void RefreshUI()
    {
        var challenges = QuestsManager.Instance.activeChallenges;

        for (int i = 0; i < challengeTexts.Length; i++)
        {
            if (i < challenges.Count)
            {
                Quest c = challenges[i];

                challengeTexts[i].text = GetChallengeText(c);
                challengeImages[i].sprite = c.IsCompleted() ? filledSlot : emptySlot;
            }
            else
            {
                challengeTexts[i].text = "";
                challengeImages[i].sprite = emptySlot;
            }
        }
    }
}
