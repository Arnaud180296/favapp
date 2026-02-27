using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace favapp.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        private bool _isInitialized = false; // Pour ne lire le LocalStorage qu'une seule fois au démarrage

        // On injecte le JSInterop pour parler au navigateur
        public CustomAuthStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

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

        // Attention : c'est devenu "async Task" au lieu de "void"
        public async Task SeConnecter(string nomUtilisateur)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, nomUtilisateur) };
            var identity = new ClaimsIdentity(claims, "FakeAuth");
            _currentUser = new ClaimsPrincipal(identity);

            // On sauvegarde dans le navigateur !
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "utilisateur_connecte", nomUtilisateur);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        // Attention : c'est devenu "async Task" aussi
        public async Task SeDeconnecter()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

            // On efface la mémoire du navigateur !
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "utilisateur_connecte");

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }
    }
}