using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Globalization;
using System.Collections;

public class DailyReward : MonoBehaviour
{
    // clé stockée par Player Prefs pour stocker la date de la dernière récompense
    private const string LastRewardKey = "LAST_REWARD_DATE";

    // Un texte pour informer le joueur que la récompense n'est pas disponible
    public TextMeshProUGUI sorryText;

    // Booléen pour empêcher le spam de sorryText pendant que la coroutine tourne
    private bool wait = false;

    // Au démarrage le texte est caché (il apparaîtra plus bas uniquement lorsque
    // le joueur appuyera sur le bouton), et wait est false, autrement dit le script
    // est à cet instant prêt à afficher le message si nécessaire
    private void Start()
    {
        sorryText.enabled = false;
        wait = false;
    }

    // Lorsque le joueur clique sur le button DailyReward (le petit button  avec une
    // image de cadeau), il appelle cette fonction
    public void OnRewardButtonClicked()
    {
        // On commence par prendre la date en UTC, sans l'heure, donc uniquement l'année
        // avec le mois et le jour. Le défault avec cette pratique est que le décalage
        // horaire n'est pas pris en compte, par exemple à la Réunion ce ne sera pas à
        // minuit que le cadeau va se réinitialiser, mais à trois heures du matin (ou deux).
        // Ce n'est tout de même pas très gênant, car beaucoup de jeux utilisent ce système.
        DateTime today = DateTime.UtcNow.Date;

        // Ce if sert à dire : si c'est la toute première fois que le joueur clique
        // sur le button, on lui donne forcément la récompense, on enregistre la date,
        // et on quitte la fonction, sinon, le joueur a déjà cliqué sur le button, donc
        // on compare les dates en dessous du if
        if (!PlayerPrefs.HasKey(LastRewardKey))
        {
            GiveReward(today);
            return;
        }

        // On récupère la date stockée dans PlayerPrefs
        // Puis on la reconvertit en DateTime
        string savedDate = PlayerPrefs.GetString(LastRewardKey);
        DateTime lastRewardDate = DateTime.ParseExact(savedDate,
                                    "yyyy-MM-dd",
                                    CultureInfo.InvariantCulture);

        // Si today, donc la date d'aujourdhui que l'on a récupéré, est strictement
        // supérieure à lastRewardDate, donc à la dernière date stockée, alors la
        // récompense est autorisée, sinon la date d'aujourdhui n'est pas supérieure,
        // autrement dit on est logiquement le même jour, donc on lance la coroutine
        // TextEnable qui affichera sorryText (seulement si à cet instant la wait est false)
        if (today > lastRewardDate)
        {
            GiveReward(today);
        }
        else
        {
            if (!wait) StartCoroutine(TextEnable());
        }
    }

    // Si cette fonction est lancée, c'est que la date d'aujourdhui est bien supérieure à
    // la dernière, alors on peut offrir la récompense au joueur.
    private void GiveReward(DateTime today)
    {
        // J'ai choisi de créer une scène spécialement pour la récompense quotidienne,
        // car peut-être qu'elle deviendra beaucoup plus complexe qu'elle ne l'est actuellement
        SceneManager.LoadScene("DailyRewardScene");

        // On n'oublie pas d'enregistrer la date du jour, au format ISO : 2026-02-13
        // Cela permet, même si le joueur n'en saura rien, de comprendre sans difficulté
        // cette clé
        PlayerPrefs.SetString(LastRewardKey, today.ToString("yyyy-MM-dd"));
        PlayerPrefs.Save();
    }

    // Si cette coroutine se lance cela signifie que le joueur clique sur le button
    // DailyReward, mais il l'a déjà fait aujourdhui, donc avec une petite animation
    // on affiche sorryText lui disant : "COME BACK TOMMOROW"
    private IEnumerator TextEnable()
    {
        // Dès le début, on met wait à true, et à la toute fin on le remet à false,
        // c'est ce qui permet tout simplement comme dit plus haut d'empêcher le joueur
        // de spammer le lancement de la coroutine TextEnable.
        // Elle fonctionne très simplement : on affiche le texte, on attend, on le cache,
        // on attend, on le réaffiche, on attend plus longtemps pour laisser le temps
        // au joueur de lire quand même, et on le re-cache, car un texte aussi petit et
        // futile n'a pas besoin de rester à l'écran
        wait = true;
        sorryText.enabled = true;
        yield return new WaitForSeconds(0.1f);
        sorryText.enabled = false;
        yield return new WaitForSeconds(0.1f);
        sorryText.enabled = true;
        yield return new WaitForSeconds(2f);
        sorryText.enabled = false;
        wait = false;
    }
}
