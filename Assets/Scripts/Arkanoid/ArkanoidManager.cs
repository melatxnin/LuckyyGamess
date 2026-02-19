using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

public class ArkanoidManager : MonoBehaviour
{
    public bool isPaused = false;

    [SerializeField] private OptionsManager optionsManager;

    [SerializeField] private AudioClip sevenBonusSound;
    [SerializeField] private AudioClip flowerBonusSound;
    [SerializeField] private AudioClip coinBonusSound;
    [SerializeField] private AudioClip letterBonusSound;
    [SerializeField] private AudioClip arkanoidMusic;

    public SliderWagerController sliderWagerController;
    public TextMeshProUGUI creditsText;
    public TextMeshProUGUI resultText;

    [SerializeField] private List<SpriteRenderer> sevenGroup;
    [SerializeField] private List<SpriteRenderer> coinGroup;
    [SerializeField] private List<SpriteRenderer> flowerGroup;
    [SerializeField] private List<SpriteRenderer> letterGroup;

    [SerializeField] private List<Sprite> sprites;

    [SerializeField] private GameObject prefabBall;
    [SerializeField] private PaddleController paddleController;

    private int countLife = 0;
    private int credits = 0;
    public int startCredits = 0;
    private int remainingBricks = 0;
    private int coinBonus = 1;

    private void OnEnable()
    {
        resultText.enabled = false;

        if (ScoreManager.Instance.isBet == true)
        {
            startCredits = sliderWagerController.value;
            credits = sliderWagerController.value;
            creditsText.text = "CREDITS : €" + sliderWagerController.value.ToString();
        }
        else
        {
            creditsText.text = "FREE MODE";
        }

        remainingBricks = 36;
        optionsManager.arkanoid = false;
        Time.timeScale = 1f;
        coinBonus = 1;
        SoundManager.Instance.PlayMusic(arkanoidMusic);
        ResetAllSymbols();
        Instantiate(prefabBall, new Vector3 (0f, 0f, 0f), Quaternion.identity);
        Incrementation();
        StartCoroutine(DecrementationCredits());
    }

    public void UnregisterBrick()
    {
        remainingBricks--;
        Debug.Log(remainingBricks);

        if (remainingBricks <= 0)
        {
            StartCoroutine(EndGame(true));
        }
    }

    private IEnumerator EndGame(bool result)
    {
        yield return new WaitForSeconds(1f);
        isPaused = true;

        if (result == true)
        {
            resultText.text = "<color=green>WELL PLAYED</color>";
            optionsManager.arkanoid = true;
            
            if (ScoreManager.Instance.isBet &&
                credits >= startCredits * 2)
            {
                if (QuestsManager.Instance != null)
                {
                    QuestsManager.Instance.Notify(QuestType.DoubleCreditsArkanoidRoyale);
                }
            }
        }
        else
        {
            resultText.text = "<color=red>LOOOOOSER</color>";
            optionsManager.arkanoid = false;
        }

        resultText.enabled = true;

        optionsManager.GameOver();
    }

    private IEnumerator DecrementationCredits()
    {
        yield return new WaitForSeconds(1f);

        while (credits > 0)
        {
            yield return new WaitForSeconds(2.5f);
            RemoveCredit(sliderWagerController.value / 10);
        }
    }

    private void AddCredits(int x)
    {
        if (ScoreManager.Instance.isBet == true)
        {
            credits += x;
            ShowCredits();
        }
    }

    private void RemoveCredit(int x)
    {
        credits -= x;
        ShowCredits();
        ScoreManager.Instance.RemoveCredits(x);

        if (credits <= 0)
        {
            StartCoroutine(EndGame(false));
        }
    }

    public int GetCredits()
    {
        return credits;
    }

    private void ShowCredits()
    {
        if (credits < sliderWagerController.value)
        {
            if (credits < 0) credits = 0;
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

    private void ResetAllSymbols()
    {
        for (int i = 0; i < sevenGroup.Count; i++)
        {
            SpriteRenderer sr = sevenGroup[i];
            SetAlpha(sr, 0.2f);
        }
        for (int i = 0; i < coinGroup.Count; i++)
        {
            SpriteRenderer sr = coinGroup[i];
            SetAlpha(sr, 0.2f);
        }
        for (int i = 0; i < flowerGroup.Count; i++)
        {
            SpriteRenderer sr = flowerGroup[i];
            SetAlpha(sr, 0.2f);
        }
        for (int i = 0; i < letterGroup.Count; i++)
        {
            SpriteRenderer sr = letterGroup[i];
            SetAlpha(sr, 0.2f);
        }
    }

    private void SetAlpha(SpriteRenderer sr, float alpha)
    {
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }

    public void AddSprite(Sprite sprite)
    {
        UnregisterBrick();

        if (sprite == sprites[0])
        {
            if (sevenGroup[0].color.a == 0.2f)
            {
                SpriteRenderer sr = sevenGroup[0];
                SetAlpha(sr, 1f);
            }
            else if (sevenGroup[1].color.a == 0.2f)
            {
                SpriteRenderer sr = sevenGroup[1];
                SetAlpha(sr, 1f);
            }
            else if (sevenGroup[2].color.a == 0.2f)
            {
                SpriteRenderer sr = sevenGroup[2];
                SetAlpha(sr, 1f);
                SoundManager.Instance.PlaySFX(sevenBonusSound);
                Handheld.Vibrate();
                StartCoroutine(BonusSeven());
            }
        }
        else if (sprite == sprites[1])
        {
            if (coinGroup[0].color.a == 0.2f)
            {
                SpriteRenderer sr = coinGroup[0];
                SetAlpha(sr, 1f);
            }
            else if (coinGroup[1].color.a == 0.2f)
            {
                SpriteRenderer sr = coinGroup[1];
                SetAlpha(sr, 1f);
            }
            else if (coinGroup[2].color.a == 0.2f)
            {
                SpriteRenderer sr = coinGroup[2];
                SetAlpha(sr, 1f);
                SoundManager.Instance.PlaySFX(coinBonusSound);
                Handheld.Vibrate();
                StartCoroutine(BonusCoin());
            }
        }
        else if (sprite == sprites[2])
        {
            if (flowerGroup[0].color.a == 0.2f)
            {
                SpriteRenderer sr = flowerGroup[0];
                SetAlpha(sr, 1f);
            }
            else if (flowerGroup[1].color.a == 0.2f)
            {
                SpriteRenderer sr = flowerGroup[1];
                SetAlpha(sr, 1f);
            }
            else if (flowerGroup[2].color.a == 0.2f)
            {
                SpriteRenderer sr = flowerGroup[2];
                SetAlpha(sr, 1f);
                SoundManager.Instance.PlaySFX(flowerBonusSound);
                Handheld.Vibrate();
                StartCoroutine(BonusFlower());
            }
        }
        else if (sprite == sprites[3])
        {
            if (letterGroup[0].color.a == 0.2f)
            {
                SpriteRenderer sr = letterGroup[0];
                SetAlpha(sr, 1f);
            }
            else if (letterGroup[1].color.a == 0.2f)
            {
                SpriteRenderer sr = letterGroup[1];
                SetAlpha(sr, 1f);
            }
            else if (letterGroup[2].color.a == 0.2f)
            {
                SpriteRenderer sr = letterGroup[2];
                SetAlpha(sr, 1f);
                SoundManager.Instance.PlaySFX(letterBonusSound);
                Handheld.Vibrate();
                StartCoroutine(BonusLetter());
            }
        }
    }

    private IEnumerator BonusLetter()
    {
        isPaused = true;
        yield return new WaitForSeconds(1f);
        paddleController.UpgradeScale();
        AddCredits(Mathf.RoundToInt(sliderWagerController.value * 0.25f * coinBonus));
        isPaused = false;
        ResetAllSymbols();
    }

    private IEnumerator BonusCoin()
    {
        isPaused = true;
        yield return new WaitForSeconds(1f);
        AddCredits(Mathf.RoundToInt(sliderWagerController.value * 0.5f * coinBonus));
        isPaused = false;
        ResetAllSymbols();

        coinBonus = 2;
        yield return new WaitForSeconds(5f);
        coinBonus = 1;
    }

    private IEnumerator BonusFlower()
    {
        isPaused = true;
        yield return new WaitForSeconds(1f);
        Instantiate(prefabBall, new Vector3 (0f, 0f, 0f), Quaternion.identity);
        Incrementation();
        AddCredits(Mathf.RoundToInt(sliderWagerController.value * 1f * coinBonus));
        isPaused = false;
        ResetAllSymbols();

        if (countLife == 3)
        {
            QuestsManager.Instance.Notify(QuestType.ThreeBallsArkanoidRoyale);
        }
    }

    private IEnumerator BonusSeven()
    {
        isPaused = true;
        yield return new WaitForSeconds(1f);
        Instantiate(prefabBall, new Vector3 (0f, 0f, 0f), Quaternion.identity);
        Incrementation();
        paddleController.UpgradeScale();
        isPaused = false;
        AddCredits(Mathf.RoundToInt(sliderWagerController.value * 2 * coinBonus));
        ResetAllSymbols();
        QuestsManager.Instance.Notify(QuestType.Get777BonusArkanoidRoyale);

        coinBonus = 2;
        yield return new WaitForSeconds(5f);
        coinBonus = 1;
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    public void Decrementation()
    {
        countLife--;

        if (countLife == 0)
        {
            StartCoroutine(EndGame(false));
        }
    }

    public void Incrementation()
    {
        countLife++;
    }
}
