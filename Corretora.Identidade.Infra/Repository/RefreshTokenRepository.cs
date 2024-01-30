using Corretora.Identidade.Core.Domain;
using Corretora.Identidade.Infra.Context;
using Corretora.Identidade.Infra.Interfaces;
using Delivery.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Corretora.Identidade.Infra.Repository
{
    public sealed class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IdentidadeContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public RefreshTokenRepository(IdentidadeContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetRefreshTokenByTokenAsync(Guid token)
        {
            return await _context.RefreshTokens.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Token == token);
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }

        public void DeleteByCpf(string cpf)
        {
            _context.RefreshTokens.RemoveRange(_context.RefreshTokens.Where(u => u.NomeUsuario == cpf));
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public Task<RefreshToken> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task Add(RefreshToken Item)
        {
            throw new NotImplementedException();
        }

        public void Update(RefreshToken Item)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Guid Id)
        {
            throw new NotImplementedException();
        }
    }
}
