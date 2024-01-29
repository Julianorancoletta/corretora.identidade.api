using System.Diagnostics.CodeAnalysis;

namespace Corretora.Identidade.API.Extensions
{
    [ExcludeFromCodeCoverage]
    public class AppTokenSettings
    {
        public int HorasExpiracaoRefreshToken { get; init; }
    }
}
