using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private GameObject SettingsInterface;
    [SerializeField] public GameObject OptionsInterface;
    [SerializeField] private GameObject GameInterface;
    [SerializeField] private GameObject UIGameInterface;
    [SerializeField] private GameObject OptionsButton;
    [SerializeField] private GameObject GameOverInterface;
    [SerializeField] private GameObject StartInterface;
    [SerializeField] private GameObject manager;

    [SerializeField] private GameObject noAnyMoney;
    [SerializeField] private GameObject anyMoney;

    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private Image rendererMusic;
    [SerializeField] private Image rendererSFX;
    [SerializeField] private Sprite spriteOn;
    [SerializeField] private Sprite spriteOff;

    [SerializeField] public string sceneName;
    [SerializeField] public bool canInteracteButton;

    public bool arkanoid;

    private void Awake()
    {
        PauseGame();

        OptionsInterface.SetActive(false);
        SettingsInterface.SetActive(false);
        UIGameInterface.SetActive(false);
        GameInterface.SetActive(false);
        OptionsButton.SetActive(false);
        GameOverInterface.SetActive(false);
        manager.SetActive(false);

        StartInterface.SetActive(true);
        textMeshProUGUI.text = "SOLDE : $" + ScoreManager.Instance.GetCredits();
        arkanoid = false;

        if (ScoreManager.Instance.GetCredits() >= 10)
        {
            anyMoney.SetActive(true);
            noAnyMoney.SetActive(false);
            canInteracteButton = true;
        }
        else
        {
            anyMoney.SetActive(false);
            noAnyMoney.SetActive(true);
            canInteracteButton = false;
        }
    }

    public void BetPressed()
    {
        OptionsInterface.SetActive(false);
        SettingsInterface.SetActive(false);
        UIGameInterface.SetActive(true);
        GameInterface.SetActive(true);
        OptionsButton.SetActive(true);
        GameOverInterface.SetActive(false);

        ScoreManager.Instance.isBet = true;
        StartInterface.SetActive(false);
        manager.SetActive(true);

        ResumeGameTime();
    }

    public void FreePressed()
    {
        OptionsInterface.SetActive(false);
        SettingsInterface.SetActive(false);
        UIGameInterface.SetActive(true);
        GameInterface.SetActive(true);
        OptionsButton.SetActive(true);
        GameOverInterface.SetActive(false);

        ScoreManager.Instance.isBet = false;
        StartInterface.SetActive(false);
        manager.SetActive(true);

        ResumeGameTime();
    }

    public void OptionsPressed()
    {
        PauseGame();

        OptionsInterface.SetActive(true);
        SettingsInterface.SetActive(false);
        UIGameInterface.SetActive(false);
        GameInterface.SetActive(false);
        OptionsButton.SetActive(false);
        GameOverInterface.SetActive(false);
        StartInterface.SetActive(false);
    }

    public void SettingsPressed()
    {
        OptionsInterface.SetActive(false);
        SettingsInterface.SetActive(true);
        UIGameInterface.SetActive(false);
        GameInterface.SetActive(false);
        OptionsButton.SetActive(false);
        StartInterface.SetActive(false);
    }

    public void MenuArkanoidPressed()
    {
        OptionsInterface.SetActive(false);
        SettingsInterface.SetActive(false);
        UIGameInterface.SetActive(false);
        GameInterface.SetActive(false);
        OptionsButton.SetActive(false);
        GameOverInterface.SetActive(false);
        StartInterface.SetActive(false);

        ArkanoidManager x = FindFirstObjectByType<ArkanoidManager>();

        if (arkanoid)
        {
            ScoreManager.Instance.AddCredits(x.GetCredits());
        }
        else
        {
            ScoreManager.Instance.RemoveCredits(x.startCredits);
        }

        SceneManager.LoadScene("MenuScene");
    }

    public void MenuPressed()
    {
        OptionsInterface.SetActive(false);
        SettingsInterface.SetActive(false);
        UIGameInterface.SetActive(false);
        GameInterface.SetActive(false);
        OptionsButton.SetActive(false);
        GameOverInterface.SetActive(false);
        StartInterface.SetActive(false);

        SceneManager.LoadScene("MenuScene");
    }

    public void RestartPressed()
    {
        OptionsInterface.SetActive(false);
        SettingsInterface.SetActive(false);
        UIGameInterface.SetActive(false);
        GameInterface.SetActive(false);
        OptionsButton.SetActive(false);
        GameOverInterface.SetActive(false);
        StartInterface.SetActive(false);

        SceneManager.LoadScene(sceneName);
    }

    public void RestartArkanoidPressed()
    {
        OptionsInterface.SetActive(false);
        SettingsInterface.SetActive(false);
        UIGameInterface.SetActive(false);
        GameInterface.SetActive(false);
        OptionsButton.SetActive(false);
        GameOverInterface.SetActive(false);
        StartInterface.SetActive(false);

        ArkanoidManager x = FindFirstObjectByType<ArkanoidManager>();

        if (arkanoid)
        {
            ScoreManager.Instance.AddCredits(x.GetCredits());
        }
        else
        {
            ScoreManager.Instance.RemoveCredits(x.startCredits);
        }

        SceneManager.LoadScene(sceneName);
    }

    public void ResumePressed()
    {
        ResumeGameTime();
        
        OptionsInterface.SetActive(false);
        SettingsInterface.SetActive(false);
        UIGameInterface.SetActive(true);
        GameInterface.SetActive(true);
        OptionsButton.SetActive(true);
        GameOverInterface.SetActive(false);
        StartInterface.SetActive(false);
    }

    public void TurnMusic()
    {
        SoundManager.Instance.TurnMusic();

        if (rendererMusic.sprite == spriteOff)
        {
            rendererMusic.sprite = spriteOn;
        }
        else
        {
            rendererMusic.sprite = spriteOff;
        }
    }

    public void TurnSFX()
    {
        SoundManager.Instance.TurnSFX();

        if (rendererSFX.sprite == spriteOff)
        {
            rendererSFX.sprite = spriteOn;
        }
        else
        {
            rendererSFX.sprite = spriteOff;
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGameTime()
    {
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        yield return new WaitForSeconds(1f);
        
        UIGameInterface.SetActive(false);
        GameInterface.SetActive(false);
        OptionsInterface.SetActive(false);
        GameOverInterface.SetActive(true);
    }
}
