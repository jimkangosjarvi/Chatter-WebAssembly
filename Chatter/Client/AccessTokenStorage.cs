using Blazored.LocalStorage;
using System.Threading.Tasks;

namespace Chatter.Client
{
    public class AccessTokenStorage
    {
        private readonly ILocalStorageService _localStorage;
        public AccessTokenStorage(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }
        public async Task<string> GetTokenAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            return savedToken;
        }
    
    }
}
