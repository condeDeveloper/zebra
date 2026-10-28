using Conde.Zebra;
using Xunit;

namespace Conde.Zebra.Testes;

public class EnigmaTestes
{
    [Fact]
    public void OClassicoTemCincoCasasCincoCategoriasEQuinzePistas()
    {
        var enigma = Classico.Enigma();
        Assert.Equal(5, enigma.Casas);
        Assert.Equal(5, enigma.Categorias.Count);
        Assert.Equal(15, enigma.Pistas.Count);
        Assert.Equal(15, Classico.Textos.Length);
    }

    [Fact]
    public void CadaCategoriaTemUmValorPorCasa()
    {
        var enigma = Classico.Enigma();
        foreach (var valores in enigma.Valores)
        {
            Assert.Equal(enigma.Casas, valores.Length);
            Assert.Equal(valores.Length, valores.Distinct().Count());
        }
    }

    [Fact]
    public void SaoVinteEQuatroBilhoesDeRuasPossiveis()
    {
        // 120 elevado a 5: é por isso que a força bruta fica nos enigmas pequenos.
        Assert.Equal(24_883_200_000d, Classico.Enigma().Atribuicoes);
        Assert.Equal(216d, Classico.Pequeno().Atribuicoes);
        Assert.Equal(331_776d, Classico.Medio().Atribuicoes);
    }

    [Fact]
    public void CategoriaSemValorParaCadaCasaNaoMontaEnigma()
    {
        Assert.Throws<ArgumentException>(() =>
            new Enigma(3, ["a"], [["x", "y"]], []));
    }

    [Fact]
    public void OValorEhAchadoPeloNome()
    {
        var enigma = Classico.Enigma();
        Assert.Equal(0, enigma.Valor(Classico.Nacionalidade, "ingles"));
        Assert.Equal(4, enigma.Valor(Classico.Animal, "peixe"));
        Assert.Throws<ArgumentException>(() => enigma.Valor(Classico.Animal, "tartaruga"));
    }

    [Fact]
    public void TrocarAsPistasNaoMudaOResto()
    {
        var enigma = Classico.Enigma();
        var vazio = enigma.ComPistas([]);
        Assert.Equal(enigma.Casas, vazio.Casas);
        Assert.Empty(vazio.Pistas);
        Assert.Equal(15, enigma.Pistas.Count);
    }

    [Fact]
    public void AsPistasApontamParaCategoriasQueExistem()
    {
        var enigma = Classico.Enigma();
        foreach (var pista in enigma.Pistas)
        {
            Assert.InRange(pista.CategoriaA, 0, enigma.Categorias.Count - 1);
            Assert.InRange(pista.ValorA, 0, enigma.Casas - 1);
            if (pista.Tipo == Tipo.CasaFixa)
            {
                Assert.InRange(pista.Casa, 0, enigma.Casas - 1);
                continue;
            }
            Assert.InRange(pista.CategoriaB, 0, enigma.Categorias.Count - 1);
            Assert.InRange(pista.ValorB, 0, enigma.Casas - 1);
        }
    }
}
