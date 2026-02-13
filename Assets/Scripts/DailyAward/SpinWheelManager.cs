using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class SpinWheelManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI awardText;
    [SerializeField] private TextMeshProUGUI numberAwardText;

    [SerializeField] private GameObject spinButton;
    [SerializeField] private GameObject menuButton;

    [SerializeField] private Transform wheelTransform;

    private int caseCount = 6;
    private float spinDuration = 5.0f;
    private int fullRotations = 10;
    private int bonusAward = 0;

    private void Start()
    {
        spinButton.SetActive(true);
        menuButton.SetActive(false);

        awardText.enabled = false;
        numberAwardText.enabled = false;
    }

    public void SpinPressed()
    {
        spinButton.SetActive(false);
        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        int winningIndex = Random.Range(0, caseCount);
        float angle = 360f / caseCount;
        float targetAngle = winningIndex * angle;
        float totalRotation = fullRotations * 360f + targetAngle;
        float startRotation = wheelTransform.eulerAngles.z;
        float t = 0f;

        while (t < spinDuration)
        {
            t += Time.deltaTime;
            float x = t / spinDuration;
            float ralentissement = Mathf.SmoothStep(0f, 1f, x);
            float currentAngle = Mathf.Lerp(0f, totalRotation, ralentissement);

            wheelTransform.eulerAngles = new Vector3(0f, 0f, startRotation - currentAngle);
            yield return null;
        }

        wheelTransform.eulerAngles = new Vector3(0f, 0f, startRotation - totalRotation);

        bonusAward = (winningIndex + 1);

        if (bonusAward == 1)
        {
            bonusAward = 200;
        }
        else if (bonusAward == 2 || bonusAward == 4 || bonusAward == 6)
        {
            bonusAward = 50;
        }
        else
        {
            bonusAward = 100;
        }

        numberAwardText.text = "<color=green>" + bonusAward + "</color>";
        ScoreManager.Instance.AddCredits(bonusAward);
        StartCoroutine(ClignoterNumberText());
        awardText.enabled = true;
        menuButton.SetActive(true);
    }

    private IEnumerator ClignoterNumberText()
    {
        numberAwardText.enabled = true;
        yield return new WaitForSeconds(0.25f);
        numberAwardText.enabled = false;
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(ClignoterNumberText());
    }

    public void MenuPressed()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
