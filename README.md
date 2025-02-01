# EcomPulse

EcomPulse est une application web e-commerce développée en .NET, permettant la gestion des rôles administrateur et client. Elle offre une expérience d'achat fluide avec un système de panier, de commande et de paiement, ainsi qu'un module de gestion des utilisateurs.

## Fonctionnalités

### Utilisateur
1. **Catalogue de produits** :
   - Affichage d'une liste de produits avec pagination.
   - Filtres de recherche par catégorie et prix.
   - Page de détails pour chaque produit incluant description, prix et images.

2. **Panier d'achat** :
   - Ajout de produits au panier avec modification des quantités ou suppression d'articles.
   - Affichage du total du panier.

3. **Commande et Paiement** :
   - Page de validation de commande avec saisie des informations d'expédition.
   - Confirmation de commande avec génération d'un numéro de commande.

4. **Gestion des utilisateurs** :
   - Inscription et connexion des utilisateurs.
   - Page de profil utilisateur permettant de modifier les informations personnelles et consulter l'historique des commandes.

### Admin
- Gestion des produits (ajout, modification, suppression).
- Suivi des commandes et gestion des paiements.

## Technologies et Outils
- **Backend** : ASP.NET Core MVC
- **Base de données** : PostgreSQL
- **Frontend** : Razor Pages

## Installation et Configuration

### Prérequis
- [.NET SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)

### Installation
1. **Cloner le dépôt** :
   ```bash
   git clone https://github.com/HaddarMelek/EcomPulse.git
   cd EcomPulse
   ```

2. **Configurer la base de données** :
   - Modifier la chaîne de connexion PostgreSQL dans `appsettings.json`
   - Appliquer les migrations :
     ```bash
     dotnet ef database update
     ```

3. **Lancer l'application** :
   ```bash
   dotnet run
   ```

