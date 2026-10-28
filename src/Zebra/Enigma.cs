namespace Conde.Zebra;

/// <summary>O tipo de cada pista do enigma.</summary>
public enum Tipo
{
    /// <summary>Os dois valores moram na mesma casa.</summary>
    MesmaCasa,

    /// <summary>O valor está numa casa fixa, contada da esquerda a partir de zero.</summary>
    CasaFixa,

    /// <summary>O primeiro valor está na casa imediatamente à esquerda do segundo.</summary>
    EsquerdaImediata,

    /// <summary>Os dois valores moram em casas vizinhas, de qualquer lado.</summary>
    Vizinhos,
}

/// <summary>
/// Uma pista. Guarda índices, não textos: a categoria é a linha da tabela, o
/// valor é a coluna, e a casa é a posição na rua.
/// </summary>
public readonly record struct Pista(Tipo Tipo, int CategoriaA, int ValorA, int CategoriaB, int ValorB, int Casa)
{
    public static Pista Mesma(int categoriaA, int valorA, int categoriaB, int valorB) =>
        new(Tipo.MesmaCasa, categoriaA, valorA, categoriaB, valorB, -1);

    public static Pista Fixa(int categoria, int valor, int casa) =>
        new(Tipo.CasaFixa, categoria, valor, -1, -1, casa);

    public static Pista Esquerda(int categoriaA, int valorA, int categoriaB, int valorB) =>
        new(Tipo.EsquerdaImediata, categoriaA, valorA, categoriaB, valorB, -1);

    public static Pista Vizinho(int categoriaA, int valorA, int categoriaB, int valorB) =>
        new(Tipo.Vizinhos, categoriaA, valorA, categoriaB, valorB, -1);
}

/// <summary>
/// O enigma: quantas casas, quais categorias, quais valores, e a lista de pistas.
/// Nada aqui sabe resolver nada.
/// </summary>
public sealed class Enigma
{
    public Enigma(int casas, IReadOnlyList<string> categorias, IReadOnlyList<string[]> valores, IReadOnlyList<Pista> pistas)
    {
        ArgumentNullException.ThrowIfNull(categorias);
        ArgumentNullException.ThrowIfNull(valores);
        ArgumentNullException.ThrowIfNull(pistas);
        if (casas < 1) throw new ArgumentOutOfRangeException(nameof(casas));
        if (categorias.Count != valores.Count)
            throw new ArgumentException("uma lista de valores por categoria", nameof(valores));
        foreach (var lista in valores)
            if (lista.Length != casas)
                throw new ArgumentException("cada categoria precisa de um valor por casa", nameof(valores));

        Casas = casas;
        Categorias = categorias;
        Valores = valores;
        Pistas = pistas;
    }

    public int Casas { get; }

    public IReadOnlyList<string> Categorias { get; }

    public IReadOnlyList<string[]> Valores { get; }

    public IReadOnlyList<Pista> Pistas { get; }

    /// <summary>O mesmo enigma com outra lista de pistas. É o que a varredura usa.</summary>
    public Enigma ComPistas(IReadOnlyList<Pista> pistas) => new(Casas, Categorias, Valores, pistas);

    /// <summary>Quantas atribuições existem ao todo: (casas!) elevado ao número de categorias.</summary>
    public double Atribuicoes
    {
        get
        {
            double fatorial = 1;
            for (var k = 2; k <= Casas; k++) fatorial *= k;
            return Math.Pow(fatorial, Categorias.Count);
        }
    }

    public int Valor(int categoria, string nome)
    {
        var indice = Array.IndexOf(Valores[categoria], nome);
        if (indice < 0) throw new ArgumentException($"{nome} não é valor de {Categorias[categoria]}", nameof(nome));
        return indice;
    }
}
