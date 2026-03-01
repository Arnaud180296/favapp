using System;
using System.Collections.Generic;
using System.Text;
using favapp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using System.Security.Claims;
using Xunit;

namespace TestFavApp
{
    /// <summary>
    /// Classe de tests unitaires (xUnit) dédiée au fournisseur d'authentification personnalisé.
    /// Vérifie que les sessions en mémoire sont bien créées et que les bons appels 
    /// sont faits vers le LocalStorage virtuel.
    /// </summary>
    public class CustomAuthStateProviderTests
    {
        // On garde une référence de notre acteur (Mock) au niveau de la classe 
        // pour pouvoir vérifier plus tard s'il a bien fait son travail.
        private Mock<IJSRuntime> _mockJsRuntime;

        /// <summary>
        /// Méthode utilitaire (Arrange) pour préparer le faux environnement.
        /// </summary>
        private CustomAuthStateProvider CreateProviderWithMocks()
        {
            // 1. On engage notre acteur pour simuler le navigateur
            _mockJsRuntime = new Mock<IJSRuntime>();

            // On lui dit : "Si on te demande de faire une action qui ne renvoie rien (InvokeVoidAsync, comme setItem ou removeItem), dis juste que c'est fait."
            _mockJsRuntime.Setup(js => js.InvokeAsync<IJSVoidResult>(It.IsAny<string>(), It.IsAny<object[]>()))
                          .ReturnsAsync(Mock.Of<IJSVoidResult>());

            // 2. On injecte ce faux navigateur dans notre vrai Douanier
            return new CustomAuthStateProvider(_mockJsRuntime.Object);
        }

        /// <summary>
        /// Vérifie que la méthode SeConnecter change bien l'état de l'utilisateur 
        /// pour le passer en "Connecté" avec le bon nom.
        /// </summary>
        [Fact]
        public async Task LoginAsync_ShouldAuthenticateUser_WithCorrectName()
        {
            // Arrange (Préparation)
            var provider = CreateProviderWithMocks();
            string testUsername = "Alice";

            // Act (Action)
            // On demande au douanier de connecter Alice
            await provider.SeConnecter(testUsername);

            // On demande ensuite au douanier : "C'est qui la personne connectée actuellement ?"
            var authState = await provider.GetAuthenticationStateAsync();
            var user = authState.User;

            // Assert (Vérification par le juge)
            // Le juge vérifie : "Est-ce que la personne est considérée comme authentifiée ?"
            Assert.True(user.Identity?.IsAuthenticated);
            // Le juge vérifie : "Est-ce que cette personne s'appelle bien Alice ?"
            Assert.Equal(testUsername, user.Identity?.Name);
        }

        /// <summary>
        /// Vérifie que la méthode SeDeconnecter efface bien l'identité de l'utilisateur 
        /// pour le rendre anonyme.
        /// </summary>
        [Fact]
        public async Task LogoutAsync_ShouldSetUserAsAnonymous()
        {
            // Arrange
            var provider = CreateProviderWithMocks();
            await provider.SeConnecter("Bob"); // On connecte Bob pour commencer

            // Act
            // On déconnecte brutalement !
            await provider.SeDeconnecter();
            var authState = await provider.GetAuthenticationStateAsync();
            var user = authState.User;

            // Assert
            // Le juge vérifie : "Est-ce que l'utilisateur est bien redevenu un fantôme (non authentifié) ?"
            Assert.False(user.Identity?.IsAuthenticated);
            Assert.Null(user.Identity?.Name); // Son nom doit être effacé
        }

        /// <summary>
        /// Vérifie que si on simule un rafraîchissement de page (F5), 
        /// le Douanier va bien lire le LocalStorage et restaurer la session.
        /// </summary>
        [Fact]
        public async Task GetAuthenticationStateAsync_OnFirstLoadWithSavedUser_ShouldRestoreSession()
        {
            // Arrange
            var provider = CreateProviderWithMocks();

            // MAGIE ICI : On donne un script spécifique à notre faux navigateur.
            // On lui dit : "Quand le douanier va te faire un getItem pour chercher l'utilisateur_connecte, 
            // mens-lui et réponds-lui qu'un certain 'Charlie' était enregistré sur le disque dur."
            _mockJsRuntime.Setup(js => js.InvokeAsync<string>("localStorage.getItem", It.IsAny<object[]>()))
                          .ReturnsAsync("Charlie");

            // Act
            // C'est le tout premier appel (comme un F5). Le douanier va interroger le faux navigateur.
            var authState = await provider.GetAuthenticationStateAsync();
            var user = authState.User;

            // Assert
            // Le juge vérifie : "Est-ce que le douanier a bien recréé la session de Charlie ?"
            Assert.True(user.Identity?.IsAuthenticated);
            Assert.Equal("Charlie", user.Identity?.Name);
        }
    }
}
