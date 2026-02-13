using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderWagerController : MonoBehaviour
{
    public Slider betSlider;
    public TextMeshProUGUI wagerText;
    public Button betButton;
    public int value = 0;
    public int maxSteps = 0;
    public OptionsManager optionsManager;

    private void Start()
    {
        value = 0;
        betButton.interactable = false;
        int totalBalance = ScoreManager.Instance.GetCredits();

        maxSteps = totalBalance;
        betSlider.minValue = 10;
        betSlider.maxValue = maxSteps;
        betSlider.wholeNumbers = true;

        betSlider.value = 0;

        UpdateBetUI();
    }

    public void OnSliderChanged()
    {
        UpdateBetUI();
    }

    private void UpdateBetUI()
    {
        value = (int)betSlider.value;
        
        wagerText.text = "$" + value;
        betButton.interactable = optionsManager.canInteracteButton;
    }
}
