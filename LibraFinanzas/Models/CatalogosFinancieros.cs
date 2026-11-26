namespace LibraFinanzas.Models;

/// <summary>
/// Listas de opciones y constantes del dominio financiero para evitar cadenas repetidas.
/// </summary>
public static class CatalogosFinancieros
{
    public const string TipoIngreso = "Ingreso";
    public const string TipoGasto = "Gasto";

    public const string ClasificacionIngreso = "Ingreso";
    public const string ClasificacionNecesario = "Necesario";
    public const string ClasificacionInnecesario = "Innecesario";

    public static readonly List<string> CategoriasGasto = new()
    {
        "Casa",
        "Transporte",
        "Comida",
        "Ropa",
        "Entretenimiento",
        "Suscripciones",
        "Salud",
        "Otro"
    };

    public static readonly List<string> CategoriasIngreso = new()
    {
        "Salario",
        "Freelance",
        "Beca",
        "Venta",
        "Otro"
    };

    public static readonly List<string> TodasLasCategorias = new()
    {
        "Salario",
        "Freelance",
        "Casa",
        "Transporte",
        "Comida",
        "Ropa",
        "Entretenimiento",
        "Suscripciones",
        "Salud",
        "Beca",
        "Venta",
        "Otro"
    };

    public static List<string> ObtenerCategoriasPorTipo(string tipo)
    {
        return tipo == TipoIngreso ? CategoriasIngreso : CategoriasGasto;
    }
}
