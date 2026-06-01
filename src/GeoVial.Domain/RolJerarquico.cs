namespace GeoVial.Domain;

/// <summary>
/// Nivel del usuario en la jerarquía raíz → jefe general → jefe de área → agente de campo.
/// Valores físicos según modelo-datos-logico §1.1 (RC-05). 1=raíz, 2=jefe general, 3=jefe de área, 4=agente.
/// </summary>
public enum RolJerarquico
{
    Raiz = 1,
    JefeGeneral = 2,
    JefeArea = 3,
    AgenteCampo = 4,
}
