using Conde.Zebra;
using Xunit;

namespace Conde.Zebra.Testes;

/// <summary>Todo número do README está preso aqui.</summary>
public class MedidasTestes
{
    [Fact]
    public void OPeixeEhDoAlemaoNaQuartaCasa()
    {
        var (dono, casa, rua, nos, _) = Medidas.QuemTemOPeixe();
        Assert.Equal("alemao", dono);
        Assert.Equal(3, casa);
        Assert.Equal(5_520L, nos);
        Assert.Contains("alemao verde cafe prince peixe", rua);
    }

    [Theory]
    [InlineData("como esta escrito", 5_520L)]
    [InlineData("cor primeiro", 5_520L)]
    [InlineData("animal primeiro", 149_880L)]
    [InlineData("bebida no fim", 14_880L)]
    [InlineData("animal e cigarro antes", 81_600L)]
    public void CadaLinhaDaOrdem(string ordem, long nos)
    {
        var linha = Medidas.PorOrdem().Single(l => l.Ordem == ordem);
        Assert.Equal(nos, linha.Nos);
        Assert.Equal(1, linha.Solucoes);
    }

    [Fact]
    public void APiorOrdemCustaVinteESeteVezesAMelhor()
    {
        var linhas = Medidas.PorOrdem();
        Assert.Equal(1.0, linhas.Single(l => l.Ordem == "como esta escrito").Vezes, 1);
        Assert.Equal(27.2, linhas.Single(l => l.Ordem == "animal primeiro").Vezes, 1);
    }

    [Theory]
    [InlineData(1, 6)]
    [InlineData(2, 2)]
    [InlineData(4, 8)]
    [InlineData(7, 7)]
    [InlineData(9, 17)]
    [InlineData(14, 12)]
    [InlineData(15, 1)]
    public void QuantasSolucoesSobramSemCadaPista(int numero, int semEla)
    {
        var linha = Medidas.UmaAMenos().Single(l => l.Numero == numero);
        Assert.Equal(semEla, linha.SemEla);
    }

    [Fact]
    public void AQuinzePistaEhADeMaisNoEnigma()
    {
        var linhas = Medidas.UmaAMenos();
        Assert.Equal(15, linhas.Count);
        Assert.Equal(14, linhas.Count(l => l.Necessaria));

        var sobrando = linhas.Single(l => !l.Necessaria);
        Assert.Equal(15, sobrando.Numero);
        Assert.Equal(1, sobrando.SemEla);
        Assert.Contains("agua", sobrando.Texto);
    }

    [Fact]
    public void ATirarANonaPistaEhOQueMaisEstraga()
    {
        var linhas = Medidas.UmaAMenos();
        Assert.Equal(17, linhas.Max(l => l.SemEla));
        Assert.Equal(9, linhas.OrderByDescending(l => l.SemEla).First().Numero);
    }

    [Fact]
    public void SoDoisSubconjuntosDasQuinzePistasBastam()
    {
        var linhas = Medidas.MenorConjuntoQueBasta();

        // Nenhum subconjunto de 13 pistas ou menos deixa solução única.
        foreach (var linha in linhas.Where(l => l.Pistas <= 13))
            Assert.Equal(0, linha.ComSolucaoUnica);

        var catorze = linhas.Single(l => l.Pistas == 14);
        Assert.Equal(15, catorze.Subconjuntos);
        Assert.Equal(1, catorze.ComSolucaoUnica);
        Assert.Equal("1 2 3 4 5 6 7 8 9 10 11 12 13 14", catorze.Exemplo);

        var quinze = linhas.Single(l => l.Pistas == 15);
        Assert.Equal(1, quinze.Subconjuntos);
        Assert.Equal(1, quinze.ComSolucaoUnica);
    }

    [Fact]
    public void ASomaDosSubconjuntosPorTamanhoFechaComTrintaEDoisMil()
    {
        // Só os tamanhos de 10 a 15 entram na tabela, e eles somam 4.944 dos
        // 32.768 subconjuntos varridos.
        var linhas = Medidas.MenorConjuntoQueBasta();
        Assert.Equal(4_944, linhas.Sum(l => l.Subconjuntos));
        Assert.Equal(new[] { 10, 11, 12, 13, 14, 15 }, linhas.Select(l => l.Pistas).ToArray());
    }

    [Theory]
    [InlineData("3 casas, 3 categorias", 216d, 216L, 24L, 2)]
    [InlineData("4 casas, 4 categorias", 331_776d, 331_776L, 1_512L, 32)]
    public void CadaLinhaDoJuiz(string nome, double ruas, long nosBruta, long nosBusca, int solucoes)
    {
        var linha = Medidas.ContraOJuiz().Single(l => l.Enigma == nome);
        Assert.Equal(ruas, linha.Atribuicoes);
        Assert.Equal(nosBruta, linha.NosForcaBruta);
        Assert.Equal(nosBusca, linha.NosBusca);
        Assert.Equal(solucoes, linha.Solucoes);
        Assert.True(linha.Bate);
    }

    [Fact]
    public void AForcaBrutaMontaExatamenteAsRuasQueExistem()
    {
        foreach (var linha in Medidas.ContraOJuiz())
            Assert.Equal(linha.Atribuicoes, linha.NosForcaBruta);
    }
}
