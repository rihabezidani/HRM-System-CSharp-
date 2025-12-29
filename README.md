# HRM PRO - Système de Gestion des Ressources Humaines

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512bd4)
![Entity Framework](https://img.shields.io/badge/EF%20Core-8.0-6c3385)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952b3)

**HRM PRO** est une solution logicielle robuste pour la gestion des ressources humaines, développée en **C#** avec **ASP.NET Core MVC**. Le système centralise la gestion des employés, le suivi des absences, le calcul automatisé des paies et propose une assistance intelligente via un chatbot.

---

##  Fonctionnalités Clés

###  Tableau de Bord (Dashboard)
- **Indicateurs Temps Réel** : Effectif total, nombre d'absents aujourd'hui, et volume de demandes de congés à traiter.
- **Actions Rapides** : Raccourcis ergonomiques pour ajouter un employé ou valider des absences.

###  Gestion des Collaborateurs
- **Annuaire RH** : Gestion complète (CRUD) des fiches employés avec gestion des rôles (Admin/Employé).
- **Espace Personnel** : Chaque employé peut consulter son propre profil et ses informations contractuelles.

###  Gestion des Congés & Absences
- **Workflow de Validation** : Soumission par l'employé et interface de décision (Approuver/Refuser) pour l'Admin.
- **Suivi Dynamique** : Visualisation claire des statuts des demandes.

###  Module de Paie Automatisé
- **Calcul Intelligent** : Génération des bulletins de paie avec déduction automatique en cas de jours de maladie ou d'absence.
- **Historique** : Archivage des bulletins consultables et téléchargeables par les employés.

###  Système de Notifications
- **Badge Dynamique** : Alerte visuelle immédiate sur la cloche pour les nouvelles demandes de congés.
- **Gestion du statut "Lu"** : Possibilité de marquer les notifications comme lues via une interaction AJAX.

###  Assistant IA (Chatbot)
- **Support Utilisateur** : Chatbot intégré permettant de répondre aux questions RH courantes.
- **Interface Fluide** : Communication asynchrone sans rechargement de page pour une expérience utilisateur moderne.

---

## 🛠️ Stack Technique

- **Backend** : C# / ASP.NET Core 8 MVC
- **ORM** : Entity Framework Core (SQL Server)
- **Frontend** : 
    - Design : Bootstrap 5 & CSS personnalisé (Thème Ultra Violet)
    - Icônes : FontAwesome 6
    - Interactivité : jQuery, AJAX, SweetAlert2
- **Source Control** : Git & GitHub

---

## ⚙️ Installation

1. **Cloner le projet**
   ```bash
   git clone [https://github.com/ton-pseudo/syst-me_de_gestion_RH.git](https://github.com/ton-pseudo/syst-me_de_gestion_RH.git)
2. **Configurer la base de données**
Configurer la base de données Mettez à jour la chaîne de connexion dans appsettings.json :
"ConnectionStrings": {
  "DefaultConnection": "Server=VOTRE_SERVEUR;Database=HrmProDb;Trusted_Connection=True;"
}
3. **Migrations et Lancement**
   Update-Database
# Puis lancer le projet (F5 dans Visual Studio)
## 📸 Aperçu de l'Application
<img width="1901" height="915" alt="image" src="https://github.com/user-attachments/assets/01871a44-d09b-4246-9a40-2aeaffbe3c98" />

<img width="1907" height="920" alt="image" src="https://github.com/user-attachments/assets/58aca1bd-64a0-4d04-b9bc-6c821f71fd37" />
