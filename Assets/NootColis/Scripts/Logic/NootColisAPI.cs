using System.Collections.Generic;
using UnityEngine;
using System;

namespace NootColis.Logic
{
    /// <summary>
    /// Point d'entrée statique simple pour utiliser le système NootColis.
    /// </summary>
    public static class NootColisAPI
    {
        /// <summary>
        /// Dictionnaire contenant les stacks de colis par destinataire.
        /// </summary>
        private static Dictionary<string, Stack<Colis>> _inboxes = new Dictionary<string, Stack<Colis>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Accès aux stacks de colis (lecture seule).
        /// </summary>
        public static IReadOnlyDictionary<string, Stack<Colis>> Inboxes => _inboxes;

        /// <summary>
        /// Envoie un colis au serveur.
        /// </summary>
        public static async Awaitable SendColis(string expediteur, string destination, string contenu)
        {
            if (NootColisManager.Instance == null)
            {
                Debug.LogError("[NootColisAPI] NootColisManager absent de la scène ! Ajoutez le prefab NootColisSystem.");
                return;
            }

            await NootColisManager.Instance.EnvoyerColis(expediteur, destination, contenu, (result) => {
                Debug.Log($"[NootColisAPI] Envoi : {result}");
            });
        }

        private static System.Threading.CancellationTokenSource _streamCts;

        /// <summary>
        /// Lance une écoute périodique (toutes les 0.5s) pour récupérer les colis d'un destinataire.
        /// Les colis trouvés sont automatiquement ajoutés à l'Inbox du destinataire.
        /// </summary>
        public static async void GetStreamOfColis(string destinataire, float interval = 0.5f)
        {
            // On arrête un flux précédent s'il existe
            StopStream();

            _streamCts = new System.Threading.CancellationTokenSource();
            var token = _streamCts.Token;

            Debug.Log($"[NootColisAPI] Stream démarré pour {destinataire} (Interval: {interval}s)");

            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (NootColisManager.Instance == null) break;

                    await GetColisOf(destinataire);

                    // Attente asynchrone native Unity 6
                    await Awaitable.WaitForSecondsAsync(interval, token);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("[NootColisAPI] Stream arrêté proprement.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[NootColisAPI] Erreur dans le stream : {e.Message}");
            }
        }

        /// <summary>
        /// Arrête le flux d'écoute en cours.
        /// </summary>
        public static void StopStream()
        {
            if (_streamCts != null)
            {
                _streamCts.Cancel();
                _streamCts.Dispose();
                _streamCts = null;
            }
        }

        /// <summary>
        /// Tente de récupérer le prochain colis pour un destinataire et l'ajoute à sa stack dédiée.
        /// </summary>
        public static async Awaitable GetColisOf(string destinataire)
        {
            if (NootColisManager.Instance == null)
            {
                Debug.LogError("[NootColisAPI] NootColisManager absent de la scène ! Ajoutez le prefab NootColisSystem.");
                return;
            }

            await NootColisManager.Instance.RecupererProchain(destinataire, (success, colis, message) => {
                if (success && colis != null)
                {
                    if (!_inboxes.ContainsKey(destinataire))
                    {
                        _inboxes[destinataire] = new Stack<Colis>();
                    }

                    _inboxes[destinataire].Push(colis);
                    Debug.Log($"[NootColisAPI] Colis pour {destinataire} reçu et ajouté (Total: {_inboxes[destinataire].Count})");
                }
                else
                {
                   // if (!success) Debug.LogWarning($"[NootColisAPI] Échec récupération pour {destinataire} : {message}");
                }
            });
        }

        /// <summary>
        /// Récupère le dernier colis arrivé pour un destinataire spécifique.
        /// </summary>
        public static Colis PopColis(string destinataire)
        {
            if (_inboxes.TryGetValue(destinataire, out Stack<Colis> stack) && stack.Count > 0)
            {
                return stack.Pop();
            }
            return null;
        }

        /// <summary>
        /// Vérifie le nombre de colis en attente pour un destinataire.
        /// </summary>
        public static int GetInboxCount(string destinataire)
        {
            if (_inboxes.TryGetValue(destinataire, out Stack<Colis> stack))
            {
                return stack.Count;
            }
            return 0;
        }
    }
}
