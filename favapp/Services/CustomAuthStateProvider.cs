using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace favapp.Services
{
    /// <summary>
    /// Fournisseur d'état d'authentification personnalisé.
    /// Gère la session utilisateur sans nécessiter de serveur backend d'identité (comme IdentityServer).
    /// Maintient l'état en mémoire et utilise le LocalStorage pour persister la connexion lors du rafraîchissement de la page.
    /// </summary>
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        private bool _isInitialized = false; // Pour ne lire le LocalStorage qu'une seule fois au démarrage

        /// <summary>
        /// Initialise une nouvelle instance du fournisseur d'authentification.
        /// </summary>
        /// <param name="jsRuntime">L'outil permettant d'exécuter du JavaScript pour accéder au LocalStorage.</param>
        public CustomAuthStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Récupère l'état d'authentification actuel de l'application.
        /// Lors du tout premier appel (souvent au chargement initial ou après un F5), 
        /// vérifie le LocalStorage pour restaurer une éventuelle session précédente.
        /// </summary>
        /// <returns>Une tâche asynchrone contenant l'état d'authentification (<see cref="AuthenticationState"/>).</returns>    
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Au premier chargement de la page (F5), on va vérifier le LocalStorage
            if (!_isInitialized)
            {
                try
                {
                    var savedUser = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "utilisateur_connecte");
                    if (!string.IsNullOrEmpty(savedUser))
                    {
                        // On a trouvé un nom ! On recrée la session.
                        var claims = new[] { new Claim(ClaimTypes.Name, savedUser) };
                        var identity = new ClaimsIdentity(claims, "FakeAuth");
                        _currentUser = new ClaimsPrincipal(identity);
                    }
                }
                catch
                {
                    // Sécurité : si le JS plante, on ignore et on reste déconnecté
                }
                _isInitialized = true;
            }

            return new AuthenticationState(_currentUser);
        }

        /// <summary>
        /// Connecte virtuellement un utilisateur en créant son identité et en la sauvegardant dans le navigateur.
        /// Notifie ensuite l'application Blazor que l'état a changé pour rafraîchir l'interface (<see cref="AuthorizeView"/>).
        /// </summary>
        /// <param name="nomUtilisateur">Le nom de l'utilisateur qui se connecte.</param>
        public async Task SeConnecter(string nomUtilisateur)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, nomUtilisateur) };
            var identity = new ClaimsIdentity(claims, "FakeAuth");
            _currentUser = new ClaimsPrincipal(identity);

            // On sauvegarde dans le navigateur !
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "utilisateur_connecte", nomUtilisateur);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        /// <summary>
        /// Déconnecte l'utilisateur en réinitialisant son identité (Anonyme) et en nettoyant le LocalStorage.
        /// Notifie ensuite l'application pour rafraîchir l'interface.
        /// </summary>
        public async Task SeDeconnecter()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

            // On efface la mémoire du navigateur !
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "utilisateur_connecte");

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }
    }
}