namespace Conde.Zebra;

/// <summary>O que uma busca achou e o que custou.</summary>
public readonly record struct Resultado(int Solucoes, Rua? Primeira, long Nos)
{
    public bool Unica => Solucoes == 1;
}

/// <summary>
/// A busca preenche uma categoria de cada vez, tentando todas as permutações de
/// valores pelas casas, e confere as pistas que já dá para conferir. Uma pista
/// que fala de duas categorias só pode ser testada quando as duas estão postas —
/// e é por isso que a ORDEM das categorias muda tanto o custo.
/// </summary>
public static class Resolvedor
{
    /// <summary>
    /// Resolve, contando no máximo <paramref name="ate"/> soluções. Contar só até
    /// duas basta para responder "é única?", que é a pergunta que a varredura de
    /// subconjuntos de pistas faz 32.768 vezes.
    /// </summary>
    public static Resultado Resolve(Enigma enigma, int ate = int.MaxValue, int[]? ordem = null)
    {
        ArgumentNullException.ThrowIfNull(enigma);

        var categorias = enigma.Categorias.Count;
        ordem ??= Enumerable.Range(0, categorias).ToArray();
        if (ordem.Length != categorias) throw new ArgumentException("a ordem precisa citar toda categoria", nameof(ordem));

        var rua = new Rua(enigma.Casas, categorias);
        var solucoes = 0;
        long nos = 0;
        Rua? primeira = null;

        // Para cada categoria, as pistas que ficam testáveis quando ela entra.
        var pistasPorNivel = new List<Pista>[categorias];
        var postas = new bool[categorias];
        for (var nivel = 0; nivel < categorias; nivel++)
        {
            postas[ordem[nivel]] = true;
            // A pista entra no nível em que a ÚLTIMA categoria dela é posta.
            pistasPorNivel[nivel] = enigma.Pistas
                .Where(p => Testavel(p, postas))
                .Where(p => p.CategoriaA == ordem[nivel] || p.CategoriaB == ordem[nivel])
                .ToList();
        }

        bool Nivel(int nivel)
        {
            if (nivel == categorias)
            {
                solucoes++;
                primeira ??= rua.Copia();
                return solucoes >= ate;
            }

            var categoria = ordem[nivel];

            // Cada nível tem a SUA lista de casas ocupadas: as casas são as
            // mesmas cinco para toda categoria, e compartilhar esse vetor entre
            // níveis faz a segunda categoria não achar casa livre nenhuma.
            var usada = new bool[enigma.Casas];

            bool Permuta(int valor)
            {
                if (valor == enigma.Casas)
                {
                    nos++;
                    foreach (var pista in pistasPorNivel[nivel])
                        if (!Juiz.Vale(pista, rua))
                            return false;
                    return Nivel(nivel + 1);
                }

                for (var casa = 0; casa < enigma.Casas; casa++)
                {
                    if (usada[casa]) continue;
                    usada[casa] = true;
                    rua[categoria, valor] = casa;
                    if (Permuta(valor + 1)) return true;
                    rua[categoria, valor] = -1;
                    usada[casa] = false;
                }

                return false;
            }

            return Permuta(0);
        }

        Nivel(0);
        return new Resultado(solucoes, primeira, nos);
    }

    private static bool Testavel(Pista pista, bool[] postas) =>
        postas[pista.CategoriaA] && (pista.CategoriaB < 0 || postas[pista.CategoriaB]);

    /// <summary>
    /// A força bruta pura: monta TODAS as atribuições possíveis, sem olhar pista
    /// nenhuma pelo caminho, e entrega cada uma ao juiz. São (casas!) elevado ao
    /// número de categorias, o que só é viável em enigma pequeno — e é justamente
    /// aí que ela serve de juiz da busca.
    /// </summary>
    public static Resultado PorForcaBruta(Enigma enigma)
    {
        ArgumentNullException.ThrowIfNull(enigma);

        var categorias = enigma.Categorias.Count;
        var rua = new Rua(enigma.Casas, categorias);
        var solucoes = 0;
        long nos = 0;
        Rua? primeira = null;

        void Categoria(int categoria)
        {
            if (categoria == categorias)
            {
                nos++;
                if (!Juiz.Confere(enigma, rua)) return;
                solucoes++;
                primeira ??= rua.Copia();
                return;
            }

            var usada = new bool[enigma.Casas];

            void Permuta(int valor)
            {
                if (valor == enigma.Casas) { Categoria(categoria + 1); return; }

                for (var casa = 0; casa < enigma.Casas; casa++)
                {
                    if (usada[casa]) continue;
                    usada[casa] = true;
                    rua[categoria, valor] = casa;
                    Permuta(valor + 1);
                    rua[categoria, valor] = -1;
                    usada[casa] = false;
                }
            }

            Permuta(0);
        }

        Categoria(0);
        return new Resultado(solucoes, primeira, nos);
    }
}
