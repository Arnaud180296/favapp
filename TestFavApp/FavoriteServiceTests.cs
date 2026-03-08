using favapp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.JSInterop.Infrastructure;

namespace TestFavApp
{
    /// <summary>
    /// Classe de tests unitaires (xUnit) dédiée à la vérification de la logique métier du FavoriteService.
    /// Utilise la bibliothèque Moq pour simuler le comportement du navigateur (JSInterop) 
    /// et du système d'authentification sans nécessiter une véritable application en cours d'exécution.
    /// </summary>
    public class FavoriteServiceTests
    {
        /// <summary>
        /// Méthode utilitaire (Phase "Arrange") permettant de configurer l'environnement de test.
        /// Crée de fausses implémentations (Mocks) pour IJSRuntime et AuthenticationStateProvider,
        /// afin de tester le service de manière totalement isolée.
        /// </summary>
        /// <returns>Une instance de FavoriteService prête à être testée.</returns>
        private FavoriteService CreateServiceWithMocks()
        {
            // On engage un acteur (Mock) pour jouer le rôle du navigateur. 
            // Pourquoi ? Parce que pendant un test unitaire, il n'y a pas de "vrai" Google Chrome ou Firefox d'ouvert
            var mockJsRuntime = new Mock<IJSRuntime>();

            // Si le code te demande d'écrire dans le LocalStorage (InvokeAsync), 
            // hoche la tête et fais comme si ça avait marché, sans faire d'erreur.
            mockJsRuntime.Setup(js => js.InvokeAsync<IJSVoidResult>(It.IsAny<string>(), It.IsAny<object[]>()))
                         .ReturnsAsync(Mock.Of<IJSVoidResult>());

            // On engage un deuxième acteur pour jouer le rôle du Douanier (AuthenticationStateProvider).
            var mockAuthState = new Mock<AuthenticationStateProvider>();

            // On fabrique une fausse carte d'identité de toutes pièces...
            var claims = new[] { new Claim(ClaimTypes.Name, "TestUser") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity); // Voilà, on a créé un faux humain virtuel !

            // On dit au faux Douanier : "Quand on te demande qui est connecté, réponds que c'est notre faux humain".
            mockAuthState.Setup(a => a.GetAuthenticationStateAsync())
                         .ReturnsAsync(new AuthenticationState(user));

            // On injecte nos deux acteurs menteurs dans le vrai service pour voir comment il réagit.
            return new FavoriteService(mockJsRuntime.Object, mockAuthState.Object);
        }

        /// <summary>
        /// Vérifie que l'ajout d'un nouvel identifiant de film fonctionne correctement 
        /// et qu'il est bien enregistré dans la liste en mémoire.
        /// </summary>
        [Fact]
        public async Task AddToFavoritesAsync_WhenNewMovie_ShouldAddMovie()
        {
            
            var service = CreateServiceWithMocks();
            int testMovieId = 123;

           
            await service.AddToFavoritesAsync(testMovieId);

            
            // Le juge vérifie : "Est-ce que le film 123 est bien vu comme un favori ?"
            Assert.True(service.IsFavorite(testMovieId));
            // Le juge vérifie : "Est-ce qu'il y a bien UN SEUL film dans le panier de l'utilisateur ?"
            Assert.Single(service.GetFavorites());
        }

        /// <summary>
        /// Vérifie la règle métier anti-doublon : si on tente d'ajouter un film 
        /// qui est déjà dans les favoris, la liste ne doit pas s'allonger.
        /// </summary>
        [Fact]
        public async Task AddToFavoritesAsync_WhenMovieAlreadyExists_ShouldNotAddDuplicate()
        {
          
            var service = CreateServiceWithMocks();
            int testMovieId = 456;

            
            await service.AddToFavoritesAsync(testMovieId); // On clique une fois sur Ajouter...
            await service.AddToFavoritesAsync(testMovieId); // ...puis on clique une deuxième fois !

            
            // Même si on a cliqué deux fois, le juge vérifie que la sécurité anti-doublon a fonctionné : il ne doit y avoir qu'UN film.
            Assert.Single(service.GetFavorites());
        }

        /// <summary>
        /// Vérifie que la suppression d'un film existant le retire bien de la liste des favoris.
        /// </summary>
        [Fact]
        public async Task RemoveFromFavoritesAsync_WhenMovieIsPresent_ShouldRemoveMovie()
        {
            
            var service = CreateServiceWithMocks();
            int testMovieId = 789;
            await service.AddToFavoritesAsync(testMovieId); // On s'assure que le film est bien dedans au départ
            
            await service.RemoveFromFavoritesAsync(testMovieId); // On le retire

            // Le juge vérifie : "Le film 789 ne doit plus être un favori".
            Assert.False(service.IsFavorite(testMovieId));
            // Le panier doit être totalement vide.
            Assert.Empty(service.GetFavorites());
        }
    }
}
