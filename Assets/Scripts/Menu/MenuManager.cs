using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] public int levelChoice = 1;
    [SerializeField] private int maxLevels = 3;

    [SerializeField] private ArkanoidAnimation arkanoidAnimation;

    [SerializeField] private GameObject[] visuals;
    [SerializeField] private GameObject[] names;

    [SerializeField] private TextMeshProUGUI creditsText;

    [SerializeField] private bool changingScene = false;

    [SerializeField] private GameObject HomeInterface;
    [SerializeField] private GameObject SettingsInterface;
    [SerializeField] private GameObject QuestsInterface;

    [SerializeField] private Image rendererMusic;
    [SerializeField] private Image rendererSFX;
    [SerializeField] private Sprite spriteOn;
    [SerializeField] private Sprite spriteOff;

    public bool canTurn = false;

    private void OnEnable()
    {
        changingScene = false;
        SoundManager.Instance.StopMusic();
        UpdateVisualAndName();
        GoToMenuInterface();
        levelChoice = 0;
        Time.timeScale = 1f;
        creditsText.text =  "$" + ScoreManager.Instance.GetCredits().ToString();
        Debug.Log(ScoreManager.Instance.GetCredits());
    }

    private void Update()
    {
        HandleKeyboard();
        creditsText.text =  "$" + ScoreManager.Instance.GetCredits().ToString();
    }

    private void HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            SelectLeft();
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            SelectRight();
    }

    private void UpdateVisualAndName()
    {
        for (int i = 0; i < maxLevels; i++)
        {
            bool active = i == levelChoice;
            visuals[i].SetActive(active);
            names[i].SetActive(active);
        }
    }

    public void ButtonStartPressed()
    {
        if (levelChoice == 0 && !changingScene)
        {
            SceneManager.LoadScene("ArkanoidScene");
        }
        else if (levelChoice == 1 && !changingScene)
        {
            SceneManager.LoadScene("BlackJackScene");
        }
        else if (levelChoice == 2 && !changingScene)
        {
            SceneManager.LoadScene("BaccaratScene");
        }

        changingScene = true;
    }

    public void SelectLeft()
    {
        levelChoice = (levelChoice + maxLevels - 1) % maxLevels;
        UpdateVisualAndName();
        arkanoidAnimation.SignChanged(true);
    }

    public void SelectRight()
    {
        levelChoice = (levelChoice + 1) % maxLevels;
        UpdateVisualAndName();
        arkanoidAnimation.SignChanged(false);
    }

    public void GoToOptionsInterface()
    {
        HomeInterface.SetActive(false);
        SettingsInterface.SetActive(false);
        QuestsInterface.SetActive(false);
    }

    public void GoToMenuInterface()
    {
        HomeInterface.SetActive(true);
        SettingsInterface.SetActive(false);
        QuestsInterface.SetActive(false);
    }

    public void GoToSettingsInterface()
    {
        HomeInterface.SetActive(false);
        SettingsInterface.SetActive(true);
        QuestsInterface.SetActive(false);
    }

    public void GoToQuestsInterface()
    {
        HomeInterface.SetActive(false);
        SettingsInterface.SetActive(false);
        QuestsInterface.SetActive(true);
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
}
