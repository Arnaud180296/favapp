using Microsoft.AspNetCore.Components.Authorization;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Bunit;
using Bunit.TestDoubles;
using favapp.Pages;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using favapp.Components;

namespace TestFavApp
{
    /// <summary>
    /// Tests d'interface utilisateur (UI) pour la page de connexion.
    /// Utilise bUnit pour générer le composant en mémoire et interagir avec ses éléments HTML.
    /// </summary>
    public class LoginTests : BunitContext 
    {
        /// <summary>
        /// Vérifie que la page de connexion génère correctement un bouton de validation.
        /// S'assure que le bouton contient le texte attendu ("Valider") et qu'il possède 
        /// les classes CSS Bootstrap (btn-primary) appropriées pour l'affichage visuel.
        /// </summary>
        [Fact]
        public void LoginPage_DevraitAfficherUnBoutonValider()
        {
            // La page Login a besoin du Douanier (AuthenticationStateProvider) pour se construire.
            // On lui en donne un faux (Mock) juste pour que la page accepte de s'afficher.
            var mockAuth = new Mock<AuthenticationStateProvider>();
            Services.AddSingleton<AuthenticationStateProvider>(mockAuth.Object);

            // On demande à bUnit de générer virtuellement le HTML du composant Login
            var composant = Render<Login>();

            // On demande à bUnit de chercher la balise <button> dans le code HTML généré
            var bouton = composant.Find("button");

            // On vérifie que ce bouton existe ET qu'il contient bien le texte "Valider"
            Assert.Equal("Valider", bouton.TextContent);

            // On vérifie qu'il a bien la classe Bootstrap bleue "btn-primary"
            Assert.Contains("btn-primary", bouton.ClassName);
        }

        /// <summary>
        /// Vérifie que la page de connexion contient bien un champ de saisie (input) 
        /// invitant l'utilisateur à entrer son nom, avec le bon texte indicatif (placeholder).
        /// </summary>
        [Fact]
        public void LoginPage_DevraitAvoirUnChampPourLeNomDUtilisateur()
        {
            
            // Comme pour le test précédent, la page a besoin du service d'authentification pour se charger.
            // On crée donc un faux "douanier" (Mock) pour éviter que la page ne plante au démarrage.
            var mockAuth = new Mock<AuthenticationStateProvider>();
            Services.AddSingleton<AuthenticationStateProvider>(mockAuth.Object);

            
            // On demande à bUnit de dessiner virtuellement la page Login en mémoire.
            var composant = Render<Login>();

            // On demande à bUnit de chercher la première balise <input> dans le code HTML généré.
            var champTexte = composant.Find("input");

            // On vérifie que ce champ texte possède bien l'attribut HTML "placeholder" 
            // et que sa valeur correspond exactement à ce qui est attendu.
            Assert.Equal("Entrez votre nom...", champTexte.GetAttribute("placeholder"));
        }
    }
}
