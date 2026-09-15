using DeliveryApp.Dominio.Compartilhado.Auth;

namespace DeliveryApp.Testes.Integracao.Compartilhado.Identity;

public sealed class ProvedorDeUsuarioFake(Guid userId) : IProvedorDeUsuario
{
    public Guid? Id => userId;

    public string? Email => null;

    public bool EstaAutenticado => true;

    public bool PossuiTipo(TipoUsuario tipoUsuario) => true;
}