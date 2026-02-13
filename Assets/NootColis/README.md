# NootColis Core

Système simple de transfert de colis via serveur pour Unity.

## Installation
1. Glissez le dossier `NootColis` dans votre dossier `Assets`.
2. Ajoutez le prefab `NootColisSystem` (dans `NootColis/Prefabs`) dans votre scène.
3. Configurez l'URL de votre serveur sur le composant `NootColisManager`.

## Utilisation

### Envoyer un colis
```csharp
using NootColis.Logic;

// ...
await NootColisAPI.SendColis("Alice", "Bob", "Épée de légende");
```

### Récupérer un colis
```csharp
using NootColis.Logic;

// ...
// Pull automatique : récupère le colis et le met dans la stack Inbox
await NootColisAPI.GetColisOf("Bob");

// Vérifier si un colis est arrivé
if (NootColisAPI.Inbox.Count > 0)
{
    var colis = NootColisAPI.PopColis();
    Debug.Log($"Reçu : {colis.contenu} de la part de {colis.expediteur}");
}
```

## Structure
- `NootColisAPI` : Point d'entrée statique.
- `NootColisManager` : Gère les communications réseau (Singleton).
- `NootColisStore` : Gère la persistance locale (si besoin).
- `Inbox` : Stack statique stockant les colis récupérés.
