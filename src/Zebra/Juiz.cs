namespace Conde.Zebra;

/// <summary>
/// Uma rua preenchida: para cada categoria e cada valor, em que casa ele está.
/// </summary>
public sealed class Rua
{
    private readonly int[][] _casaDoValor;

    public Rua(int casas, int categorias)
    {
        Casas = casas;
        _casaDoValor = new int[categorias][];
        for (var c = 0; c < categorias; c++)
        {
            _casaDoValor[c] = new int[casas];
            Array.Fill(_casaDoValor[c], -1);
        }
    }

    public int Casas { get; }

    public int Categorias => _casaDoValor.Length;

    public int this[int categoria, int valor]
    {
        get => _casaDoValor[categoria][valor];
        set => _casaDoValor[categoria][valor] = value;
    }

    public bool Completa()
    {
        foreach (var linha in _casaDoValor)
            foreach (var casa in linha)
                if (casa < 0)
                    return false;
        return true;
    }

    public Rua Copia()
    {
        var saida = new Rua(Casas, Categorias);
        for (var c = 0; c < Categorias; c++) Array.Copy(_casaDoValor[c], saida._casaDoValor[c], Casas);
        return saida;
    }

    /// <summary>Quem está nesta casa, em cada categoria.</summary>
    public int[] Casa(int casa)
    {
        var saida = new int[Categorias];
        for (var c = 0; c < Categorias; c++)
            saida[c] = Array.IndexOf(_casaDoValor[c], casa);
        return saida;
    }

    public string Texto(Enigma enigma)
    {
        var linhas = new List<string>();
        for (var casa = 0; casa < Casas; casa++)
        {
            var quem = Casa(casa);
            linhas.Add(string.Join(" ", quem.Select((v, c) => v < 0 ? "?" : enigma.Valores[c][v])));
        }
        return string.Join(" | ", linhas);
    }
}

/// <summary>
/// O juiz confere uma rua PRONTA contra cada pista, uma por uma, pela definição.
/// Ele não sabe podar, não sabe buscar e não é consultado durante a busca.
/// </summary>
public static class Juiz
{
    public static bool Confere(Enigma enigma, Rua rua)
    {
        ArgumentNullException.ThrowIfNull(enigma);
        ArgumentNullException.ThrowIfNull(rua);
        if (!rua.Completa()) return false;

        // Cada categoria tem de ser uma permutação: um valor por casa.
        for (var c = 0; c < rua.Categorias; c++)
        {
            var visto = new bool[rua.Casas];
            for (var v = 0; v < rua.Casas; v++)
            {
                var casa = rua[c, v];
                if (casa < 0 || casa >= rua.Casas || visto[casa]) return false;
                visto[casa] = true;
            }
        }

        foreach (var pista in enigma.Pistas)
            if (!Vale(pista, rua))
                return false;

        return true;
    }

    /// <summary>Esta pista é obedecida por esta rua?</summary>
    public static bool Vale(Pista pista, Rua rua)
    {
        ArgumentNullException.ThrowIfNull(rua);
        var a = rua[pista.CategoriaA, pista.ValorA];

        return pista.Tipo switch
        {
            Tipo.CasaFixa => a == pista.Casa,
            Tipo.MesmaCasa => a == rua[pista.CategoriaB, pista.ValorB],
            Tipo.EsquerdaImediata => a + 1 == rua[pista.CategoriaB, pista.ValorB],
            Tipo.Vizinhos => Math.Abs(a - rua[pista.CategoriaB, pista.ValorB]) == 1,
            _ => throw new ArgumentOutOfRangeException(nameof(pista)),
        };
    }

    /// <summary>
    /// Quais pistas esta rua quebra. Serve para a mensagem dizer qual é o
    /// problema, em vez de só dizer que existe um.
    /// </summary>
    public static List<int> Quebradas(Enigma enigma, Rua rua)
    {
        ArgumentNullException.ThrowIfNull(enigma);
        var saida = new List<int>();
        for (var i = 0; i < enigma.Pistas.Count; i++)
            if (!Vale(enigma.Pistas[i], rua))
                saida.Add(i);
        return saida;
    }
}
