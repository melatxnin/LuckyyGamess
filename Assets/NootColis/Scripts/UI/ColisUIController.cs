using UnityEngine;
using UnityEngine.UIElements;
using NootColis.Logic;
using NootColis.Visuals;
using System.Collections.Generic;

namespace NootColis.UI
{
    public class ColisUIController : MonoBehaviour
    {
        private VisualElement _root;
        private TextField _expediteurField;
        private TextField _destinationField;
        private TextField _contenuField;
        private TextField _utilisateurField;
        private Label _statusLabel;
        private Button _btnAuto;

        private bool _isAutoPulling = false;
        private float _nextPullTime = 0f;
        private const float PULL_INTERVAL = 1.0f;

        private void OnEnable()
        {
            Debug.Log("<color=orange>[ColisUIController]</color> Initialisation de l'UI...");

            UIDocument uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null) return;

            _root = uiDocument.rootVisualElement;
            if (_root == null) return;

            _expediteurField = VerifyElement<TextField>("input-expediteur");
            _destinationField = VerifyElement<TextField>("input-destination");
            _contenuField = VerifyElement<TextField>("input-contenu");
            _utilisateurField = VerifyElement<TextField>("input-utilisateur");
            _statusLabel = VerifyElement<Label>("label-status");

            Button btnEnvoyer = VerifyElement<Button>("btn-envoyer");
            Button btnRecuperer = VerifyElement<Button>("btn-recuperer");
            _btnAuto = VerifyElement<Button>("btn-auto"); // Nouveau bouton à ajouter dans l'UXML

            if (btnEnvoyer != null) btnEnvoyer.clicked += () => { _ = OnEnvoyerClicked(); };
            if (btnRecuperer != null) btnRecuperer.clicked += () => { _ = OnRecupererClicked(); };
            if (_btnAuto != null) _btnAuto.clicked += ToggleAutoPull;

            Debug.Log("<color=green>[ColisUIController]</color> UI prête.");
        }

        private void Update()
        {
            if (_isAutoPulling && Time.time >= _nextPullTime)
            {
                _nextPullTime = Time.time + PULL_INTERVAL;
                _ = AutoPullTask();
            }
        }

        private void ToggleAutoPull()
        {
            _isAutoPulling = !_isAutoPulling;
            
            if (_isAutoPulling)
            {
                _btnAuto.text = "STOP AUTO";
                _btnAuto.AddToClassList("btn-auto-active"); // Style rouge pro
                SetStatus("Mode AUTO activé (1s)");
            }
            else
            {
                _btnAuto.text = "AUTO";
                _btnAuto.RemoveFromClassList("btn-auto-active");
                SetStatus("Mode AUTO désactivé");
            }
        }

        private async Awaitable AutoPullTask()
        {
            string user = _utilisateurField.value;
            if (string.IsNullOrEmpty(user)) return;

            await NootColisManager.Instance.RecupererProchain(user, (success, colis, message) => {
                if (success && colis != null)
                {
                    SetStatus($"[AUTO] Colis reçu !");
                    ColisVisualizer.Instance.SpawnColis(colis);
                }
            });
        }

        private T VerifyElement<T>(string name) where T : VisualElement
        {
            T element = _root.Q<T>(name);
            if (element == null) Debug.LogError($"[ColisUIController] Élément '{name}' introuvable.");
            return element;
        }

        private async Awaitable OnEnvoyerClicked()
        {
            string exp = _expediteurField.value;
            string dest = _destinationField.value;
            string cont = _contenuField.value;

            if (string.IsNullOrEmpty(exp) || string.IsNullOrEmpty(dest) || string.IsNullOrEmpty(cont)) return;

            SetStatus("Envoi en cours...");
            await NootColisManager.Instance.EnvoyerColis(exp, dest, cont, (result) => {
                SetStatus(result);
                if (result.Contains("Succès"))
                {
                    _expediteurField.value = "";
                    _destinationField.value = "";
                    _contenuField.value = "";
                }
            });
        }

        private async Awaitable OnRecupererClicked()
        {
            string user = _utilisateurField.value;
            if (string.IsNullOrEmpty(user)) return;

            SetStatus("Récupération en cours...");
            await NootColisManager.Instance.RecupererProchain(user, (success, colis, message) => {
                SetStatus(message);
                if (success && colis != null)
                {
                    ColisVisualizer.Instance.SpawnColis(colis);
                }
            });
        }

        private void SetStatus(string message)
        {
            if (_statusLabel != null) _statusLabel.text = message;
        }
    }
}