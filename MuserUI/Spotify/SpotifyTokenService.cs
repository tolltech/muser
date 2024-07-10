using System;
using System.Threading.Tasks;
using Tolltech.MuserUI.Sync;
using Tolltech.SpotifyClient;
using Tolltech.SqlEF;

namespace Tolltech.MuserUI.Spotify
{
    public class SpotifyTokenService : ISpotifyTokenService
    {
        private readonly IQueryExecutorFactory queryExecutorFactory;

        public SpotifyTokenService(IQueryExecutorFactory queryExecutorFactory)
        {
            this.queryExecutorFactory = queryExecutorFactory;
        }
        
        public async Task CreateOrUpdate(TokenInfo token, Guid userId)
        {
            using var queryExecutor = queryExecutorFactory.Create<SpotifyTokenHandler, SpotifyTokenDbo>();
            var result = await queryExecutor.ExecuteAsync(x => x.FindAsync(userId)).ConfigureAwait(false);
            if (result == null)
            {
                var newToken = new SpotifyTokenDbo
                {
                    AccessToken = token.AccessToken,
                    ExpiresUtc = token.ExpiresUtc,
                    RefreshToken = token.RefreshToken,
                    Scope = token.Scope,
                    TokenType = token.TokenType,
                    UserId = userId
                };
                await queryExecutor.ExecuteAsync(x => x.CreateAsync(newToken)).ConfigureAwait(false);
            }
            else
            {
                result.AccessToken = token.AccessToken;
                result.ExpiresUtc = token.ExpiresUtc;
                result.RefreshToken = token.RefreshToken;
                result.Scope = token.Scope;
                result.TokenType = token.TokenType;
                
                await queryExecutor.ExecuteAsync(x => x.UpdateAsync()).ConfigureAwait(false);    
            }
        }

        public async Task<TokenInfo> Find(Guid userId)
        {
            using var queryExecutor = queryExecutorFactory.Create<SpotifyTokenHandler, SpotifyTokenDbo>();
            var token = await queryExecutor.ExecuteAsync(x => x.FindAsync(userId)).ConfigureAwait(false);

            if (token == null) return null;
            
            return new TokenInfo
            {
                Scope = token.Scope,
                TokenType = token.TokenType,
                ExpiresUtc = token.ExpiresUtc,
                RefreshToken = token.RefreshToken,
                AccessToken = token.AccessToken
            };
        }

        public async Task Delete(Guid userId)
        {
            using var queryExecutor = queryExecutorFactory.Create<SpotifyTokenHandler, SpotifyTokenDbo>();
            var result = await queryExecutor.ExecuteAsync(x => x.FindAsync(userId)).ConfigureAwait(false);
            if (result == null) return;
            
            await queryExecutor.ExecuteAsync(x => x.DeleteAsync(result)).ConfigureAwait(false);
        }
    }
}