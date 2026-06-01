using GeoVial.Application.Abstracciones;

namespace GeoVial.Infrastructure;

public sealed class RelojUtc : IRelojUtc
{
    public DateTime AhoraUtc => DateTime.UtcNow;
}
