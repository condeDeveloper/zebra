using System.Diagnostics;

namespace Conde.Zebra;

/// <summary>
/// As medidas do README. Os testes conferem o que estes métodos devolvem; o
/// medidor só imprime.
/// </summary>
public static class Medidas
{
    public readonly record struct LinhaOrdem(string Ordem, int Solucoes, long Nos, double Vezes);

    public readonly record struct LinhaPista(int Numero, string Texto, int SemEla, bool Necessaria);

    public readonly record struct LinhaMinimo(int Pistas, int Subconjuntos, int ComSolucaoUnica, string Exemplo);

    public readonly record struct LinhaJuiz(string Enigma, double Atribuicoes, long NosForcaBruta, long NosBusca, int Solucoes, bool Bate);

    /// <summary>A resposta: quem tem o peixe, e em que casa.</summary>
    public static (string Dono, int Casa, string Rua, long Nos, double Segundos) QuemTemOPeixe()
    {
        var enigma = Classico.Enigma();
        var relogio = Stopwatch.StartNew();
        var resultado = Resolvedor.Resolve(enigma);
        relogio.Stop();

        var rua = resultado.Primeira!;
        var casa = rua[Classico.Animal, 4];
        var dono = Array.IndexOf(Enumerable.Range(0, 5).Select(n => rua[Classico.Nacionalidade, n]).ToArray(), casa);

        return (Classico.Valores[Classico.Nacionalidade][dono], casa, rua.Texto(enigma), resultado.Nos, relogio.Elapsed.TotalSeconds);
    }

    /// <summary>
    /// A MESMA busca, mudando só a ordem em que as categorias são preenchidas.
    /// Nada é descartado e o número de soluções é o mesmo: muda só quando cada
    /// pista fica testável.
    /// </summary>
    public static List<LinhaOrdem> PorOrdem()
    {
        var enigma = Classico.Enigma();
        var ordens = new (string Nome, int[] Ordem)[]
        {
            ("como esta escrito", [0, 1, 2, 3, 4]),
            ("cor primeiro", [1, 0, 2, 3, 4]),
            ("animal primeiro", [4, 3, 2, 1, 0]),
            ("bebida no fim", [0, 1, 3, 4, 2]),
            ("animal e cigarro antes", [4, 3, 0, 1, 2]),
        };

        var linhas = new List<LinhaOrdem>();
        long melhor = long.MaxValue;
        foreach (var (nome, ordem) in ordens)
        {
            var resultado = Resolvedor.Resolve(enigma, ordem: ordem);
            melhor = Math.Min(melhor, resultado.Nos);
            linhas.Add(new LinhaOrdem(nome, resultado.Solucoes, resultado.Nos, 0));
        }

        return linhas.Select(l => l with { Vezes = (double)l.Nos / melhor }).ToList();
    }

    /// <summary>
    /// Pista por pista: tirando essa, quantas soluções o enigma passa a ter? Se
    /// continuar uma só, a pista não estava fazendo falta.
    /// </summary>
    public static List<LinhaPista> UmaAMenos()
    {
        var enigma = Classico.Enigma();
        var linhas = new List<LinhaPista>();

        for (var i = 0; i < enigma.Pistas.Count; i++)
        {
            var semEla = enigma.Pistas.Where((_, k) => k != i).ToList();
            var resultado = Resolvedor.Resolve(enigma.ComPistas(semEla), ate: 100);
            linhas.Add(new LinhaPista(i + 1, Classico.Textos[i], resultado.Solucoes, resultado.Solucoes != 1));
        }

        return linhas;
    }

    /// <summary>
    /// Todos os 32.768 subconjuntos das 15 pistas, do menor para o maior, à
    /// procura do menor que ainda deixa uma solução só. Contar até duas basta
    /// para decidir.
    /// </summary>
    public static List<LinhaMinimo> MenorConjuntoQueBasta()
    {
        var enigma = Classico.Enigma();
        var total = enigma.Pistas.Count;
        var porTamanho = new Dictionary<int, (int Subconjuntos, int Unicos, string Exemplo)>();

        for (var mascara = 0; mascara < 1 << total; mascara++)
        {
            var tamanho = System.Numerics.BitOperations.PopCount((uint)mascara);
            var escolhidas = new List<Pista>();
            for (var i = 0; i < total; i++)
                if ((mascara >> i & 1) != 0)
                    escolhidas.Add(enigma.Pistas[i]);

            var resultado = Resolvedor.Resolve(enigma.ComPistas(escolhidas), ate: 2);
            var (subconjuntos, unicos, exemplo) = porTamanho.GetValueOrDefault(tamanho, (0, 0, string.Empty));
            subconjuntos++;

            if (resultado.Unica)
            {
                unicos++;
                if (exemplo.Length == 0)
                {
                    var numeros = new List<int>();
                    for (var i = 0; i < total; i++)
                        if ((mascara >> i & 1) != 0)
                            numeros.Add(i + 1);
                    exemplo = string.Join(" ", numeros);
                }
            }

            porTamanho[tamanho] = (subconjuntos, unicos, exemplo);
        }

        return porTamanho
            .OrderBy(p => p.Key)
            .Where(p => p.Value.Unicos > 0 || p.Key >= 10)
            .Select(p => new LinhaMinimo(p.Key, p.Value.Subconjuntos, p.Value.Unicos, p.Value.Exemplo))
            .ToList();
    }

    /// <summary>
    /// A busca contra a força bruta que monta toda rua possível e entrega ao juiz.
    /// Só nos enigmas pequenos, que é onde a força bruta termina.
    /// </summary>
    public static List<LinhaJuiz> ContraOJuiz()
    {
        var linhas = new List<LinhaJuiz>();
        foreach (var (nome, enigma) in new[]
                 {
                     ("3 casas, 3 categorias", Classico.Pequeno()),
                     ("4 casas, 4 categorias", Classico.Medio()),
                 })
        {
            var bruta = Resolvedor.PorForcaBruta(enigma);
            var busca = Resolvedor.Resolve(enigma);
            linhas.Add(new LinhaJuiz(
                nome, enigma.Atribuicoes, bruta.Nos, busca.Nos, busca.Solucoes, bruta.Solucoes == busca.Solucoes));
        }
        return linhas;
    }
}
