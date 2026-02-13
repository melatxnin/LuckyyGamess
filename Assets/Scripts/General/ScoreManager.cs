using System;
using NootColis.Logic;
using UnityEditor;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private int oldCredits = 100;
    private const string CreditsKey = "OldCredits";

    public bool isBet = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (!PlayerPrefs.HasKey(CreditsKey))
            {
                oldCredits = 100;
                PlayerPrefs.SetInt(CreditsKey, oldCredits);
                PlayerPrefs.Save();
            }
            else
            {
                oldCredits = PlayerPrefs.GetInt(CreditsKey);
            }
        }
        else
        {
            Destroy(gameObject);
        }

        NootColisAPI.GetStreamOfColis("Nicolas");
    }

    private void Update()
    {
        if (NootColisAPI.GetInboxCount("Nicolas") > 0)
        {
            Colis colis = NootColisAPI.PopColis("Nicolas");
            int argent = Int32.Parse(colis.contenu);
            AddCredits(argent);
        }
    }

    public int GetCredits()
    {
        return oldCredits;
    }

    public void SetCredits(int newCredits)
    {
        oldCredits = newCredits;
        PlayerPrefs.SetInt(CreditsKey, oldCredits);
        PlayerPrefs.Save();
    }

    public void AddCredits(int amount)
    {
        SetCredits(oldCredits + amount);
    }

    public void RemoveCredits(int amount)
    {
        SetCredits(Mathf.Max(0, oldCredits - amount));
    }
}
