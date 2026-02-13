using UnityEngine;
using System.Collections.Generic;

public enum QuestType
{
    Get777BonusArkanoidRoyale,
    DoubleCreditsArkanoidRoyale,
    ThreeBallsArkanoidRoyale,
    Hit21Blackjack,
    BeatDealer3InARowBlackJack,
    WinAssuranceBlackJack,
    WinEgalityBaccarat,
    WinPlayerPairBaccarat,
    WinBankerPairBaccarat
}

[System.Serializable]
public class Quest
{
    public QuestType type;

    // progression actuelle
    public int currentValue;

    // objectif à atteindre
    public int targetValue;

    public bool IsCompleted()
    {
        return currentValue >= targetValue;
    }
}

public class QuestsManager : MonoBehaviour
{
    public static QuestsManager Instance;

    public List<QuestType> allPossibleChallenges;
    public List<Quest> activeChallenges = new List<Quest>();

    private const string SHUFFLED_KEY = "ShuffledChallenges";
    private const string INDEX_KEY = "ChallengeIndex";

    private List<QuestType> shuffledPool;
    private int currentIndex;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadOrCreateChallengePool();
        
        if (PlayerPrefs.HasKey("Challenge_0_Type"))
        {
            LoadChallenges();
        }
        else
        {
            LoadActiveChallenges();
        }
    }

    private void LoadOrCreateChallengePool()
    {
        if (!PlayerPrefs.HasKey(SHUFFLED_KEY))
        {
            // première fois uniquement
            shuffledPool = new List<QuestType>(allPossibleChallenges);
            Shuffle(shuffledPool);
            currentIndex = 0;
            SavePool();
        }
        else
        {
            LoadPool();
        }
    }

    private void LoadActiveChallenges()
    {
        activeChallenges.Clear();

        // Si on arrive au bout → reshuffle
        if (currentIndex + 3 > shuffledPool.Count)
        {
            Shuffle(shuffledPool);
            currentIndex = 0;
            SavePool();
        }

        for (int i = 0; i < 3; i++)
        {
            Quest challenge = CreateChallenge(shuffledPool[currentIndex + i]);
            activeChallenges.Add(challenge);
        }
    }

    private void Shuffle(List<QuestType> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = UnityEngine.Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }

    private Quest CreateChallenge(QuestType type)
    {
        Quest challenge = new Quest();
        challenge.type = type;
        challenge.currentValue = 0;

        switch (type)
        {
            case QuestType.Get777BonusArkanoidRoyale:
                challenge.targetValue = 1;
                break;

            case QuestType.Hit21Blackjack:
                challenge.targetValue = 1;
                break;
            
            case QuestType.DoubleCreditsArkanoidRoyale:
                challenge.targetValue = 1;
                break;

            case QuestType.BeatDealer3InARowBlackJack:
                challenge.targetValue = 1;
                break;
            
            case QuestType.ThreeBallsArkanoidRoyale:
                challenge.targetValue = 1;
                break;

            case QuestType.WinAssuranceBlackJack:
                challenge.targetValue = 1;
                break;

            case QuestType.WinEgalityBaccarat:
                challenge.targetValue = 1;
                break;
            
            case QuestType.WinPlayerPairBaccarat:
                challenge.targetValue = 1;
                break;
            
            case QuestType.WinBankerPairBaccarat:
                challenge.targetValue = 1;
                break;
        }

        return challenge;
    }

    private void ClearSavedChallenges()
    {
        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.DeleteKey("Challenge_" + i + "_Type");
            PlayerPrefs.DeleteKey("Challenge_" + i + "_Value");
            PlayerPrefs.DeleteKey("Challenge_" + i + "_Target");
        }
    }

    public void Notify(QuestType type, int amount = 1)
    {
        if (!ScoreManager.Instance.isBet)
            return;

        foreach (Quest challenge in activeChallenges)
        {
            if (challenge.type == type && !challenge.IsCompleted())
            {
                challenge.currentValue = Mathf.Min(
                    challenge.currentValue + amount,
                    challenge.targetValue
                );

                SaveChallenges();
                break;
            }
        }

        CheckAllQuestsCompleted();
    }

    private void CheckAllQuestsCompleted()
    {
        foreach (var c in activeChallenges)
        {
            if (!c.IsCompleted())
                return;
        }

        currentIndex += 3;
        ScoreManager.Instance.AddCredits(100);
        SavePool();

        activeChallenges.Clear();
        LoadActiveChallenges();
        ClearSavedChallenges();
        SaveChallenges();
    }

    private void SavePool()
    {
        PlayerPrefs.SetInt(INDEX_KEY, currentIndex);

        PlayerPrefs.SetInt(SHUFFLED_KEY, shuffledPool.Count);

        for (int i = 0; i < shuffledPool.Count; i++)
        {
            PlayerPrefs.SetInt($"{SHUFFLED_KEY}_{i}", (int)shuffledPool[i]);
        }

        PlayerPrefs.Save();
    }

    private void LoadPool()
    {
        shuffledPool = new List<QuestType>();

        int count = PlayerPrefs.GetInt(SHUFFLED_KEY);

        for (int i = 0; i < count; i++)
        {
            shuffledPool.Add(
                (QuestType)PlayerPrefs.GetInt($"{SHUFFLED_KEY}_{i}")
            );
        }

        currentIndex = PlayerPrefs.GetInt(INDEX_KEY, 0);
    }

    private void SaveChallenges()
    {
        for (int i = 0; i < activeChallenges.Count; i++)
        {
            PlayerPrefs.SetInt(
                "Challenge_" + i + "_Type",
                (int)activeChallenges[i].type
            );

            PlayerPrefs.SetInt(
                "Challenge_" + i + "_Value",
                activeChallenges[i].currentValue
            );

            PlayerPrefs.SetInt(
                "Challenge_" + i + "_Target",
                activeChallenges[i].targetValue
            );
        }

        PlayerPrefs.Save();
    }

    private void LoadChallenges()
    {
        activeChallenges.Clear();

        for (int i = 0; i < 3; i++)
        {
            Quest challenge = new Quest();

            challenge.type = (QuestType)PlayerPrefs.GetInt("Challenge_" + i + "_Type");
            challenge.currentValue = PlayerPrefs.GetInt("Challenge_" + i + "_Value");
            challenge.targetValue = PlayerPrefs.GetInt("Challenge_" + i + "_Target");

            activeChallenges.Add(challenge);
        }
    }
}
