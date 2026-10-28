using Conde.Zebra;
using Xunit;

namespace Conde.Zebra.Testes;

public class ResolvedorTestes
{
    [Fact]
    public void OEnigmaClassicoTemUmaSolucaoSo()
    {
        var resultado = Resolvedor.Resolve(Classico.Enigma());
        Assert.Equal(1, resultado.Solucoes);
        Assert.True(resultado.Unica);
    }

    [Fact]
    public void ASolucaoAchadaPassaPeloJuiz()
    {
        var enigma = Classico.Enigma();
        var resultado = Resolvedor.Resolve(enigma);
        Assert.NotNull(resultado.Primeira);
        Assert.True(Juiz.Confere(enigma, resultado.Primeira!));
    }

    [Fact]
    public void OAlemaoTemOPeixe()
    {
        var rua = Resolvedor.Resolve(Classico.Enigma()).Primeira!;
        Assert.Equal(rua[Classico.Nacionalidade, 4], rua[Classico.Animal, 4]);
        Assert.Equal(3, rua[Classico.Animal, 4]);
    }

    [Fact]
    public void ANoruegaTemAguaEOEnigmaNaoPerguntaIsso()
    {
        var rua = Resolvedor.Resolve(Classico.Enigma()).Primeira!;
        Assert.Equal(rua[Classico.Nacionalidade, 3], rua[Classico.Bebida, 4]);
    }

    [Fact]
    public void AOrdemDasCategoriasNaoMudaAResposta()
    {
        var enigma = Classico.Enigma();
        var primeira = Resolvedor.Resolve(enigma).Primeira!.Texto(enigma);

        foreach (var ordem in new[]
                 {
                     new[] { 1, 0, 2, 3, 4 },
                     new[] { 4, 3, 2, 1, 0 },
                     new[] { 2, 4, 0, 3, 1 },
                 })
        {
            var resultado = Resolvedor.Resolve(enigma, ordem: ordem);
            Assert.Equal(1, resultado.Solucoes);
            Assert.Equal(primeira, resultado.Primeira!.Texto(enigma));
        }
    }

    [Fact]
    public void AOrdemDasCategoriasMudaOCusto()
    {
        var enigma = Classico.Enigma();
        var direto = Resolvedor.Resolve(enigma).Nos;
        var ruim = Resolvedor.Resolve(enigma, ordem: [4, 3, 2, 1, 0]).Nos;
        Assert.True(ruim > 20 * direto, $"{ruim} contra {direto}");
    }

    [Fact]
    public void ABuscaEAForcaBrutaAchamOMesmoNumeroDeSolucoes()
    {
        foreach (var enigma in new[] { Classico.Pequeno(), Classico.Medio() })
            Assert.Equal(Resolvedor.PorForcaBruta(enigma).Solucoes, Resolvedor.Resolve(enigma).Solucoes);
    }

    [Fact]
    public void AForcaBrutaAbreExatamenteAsRuasQueExistem()
    {
        Assert.Equal(216, Resolvedor.PorForcaBruta(Classico.Pequeno()).Nos);
        Assert.Equal(331_776, Resolvedor.PorForcaBruta(Classico.Medio()).Nos);
    }

    [Fact]
    public void SemPistaNenhumaTodaRuaServe()
    {
        var enigma = Classico.Pequeno().ComPistas([]);
        Assert.Equal(216, Resolvedor.PorForcaBruta(enigma).Solucoes);
    }

    [Fact]
    public void PararCedoEntregaAMesmaPrimeira()
    {
        var enigma = Classico.Medio();
        var tudo = Resolvedor.Resolve(enigma);
        var pouco = Resolvedor.Resolve(enigma, ate: 1);

        Assert.Equal(1, pouco.Solucoes);
        Assert.True(tudo.Solucoes > 1);
        Assert.Equal(tudo.Primeira!.Texto(enigma), pouco.Primeira!.Texto(enigma));
    }

    [Fact]
    public void OrdemQueNaoCitaTodaCategoriaNaoServe()
    {
        Assert.Throws<ArgumentException>(() => Resolvedor.Resolve(Classico.Enigma(), ordem: [0, 1]));
    }
}
