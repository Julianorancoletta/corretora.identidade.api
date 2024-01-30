using Corretora.Identidade.Core.Domain;
using Delivery.Core.Data;

namespace Corretora.Identidade.Infra.Interfaces
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        IUnitOfWork UnitOfWork { get; }
        Task<RefreshToken?> GetRefreshTokenByTokenAsync(Guid token);
        Task AddAsync(RefreshToken refreshToken);
        void DeleteByCpf(string cpf);
    }
}
