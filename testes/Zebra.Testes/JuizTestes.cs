using Conde.Zebra;
using Xunit;

namespace Conde.Zebra.Testes;

public class JuizTestes
{
    private static Rua ARespostaCerta()
    {
        var rua = new Rua(5, 5);
        // norueguês amarela água dunhill gato | dinamarquês azul chá blend cavalo |
        // inglês vermelha leite pallmall pássaro | alemão verde café prince peixe |
        // sueco branca cerveja bluemaster cachorro
        int[][] casas =
        [
            [3, 3, 4, 1, 2],
            [2, 4, 0, 2, 3],
            [0, 0, 2, 0, 1],
            [4, 1, 1, 4, 4],
            [1, 2, 3, 3, 0],
        ];

        for (var casa = 0; casa < 5; casa++)
            for (var categoria = 0; categoria < 5; categoria++)
                rua[categoria, casas[casa][categoria]] = casa;

        return rua;
    }

    [Fact]
    public void ARespostaConhecidaPassaPeloJuiz()
    {
        Assert.True(Juiz.Confere(Classico.Enigma(), ARespostaCerta()));
    }

    [Fact]
    public void NaRespostaOAlemaoTemOPeixe()
    {
        var rua = ARespostaCerta();
        Assert.Equal(rua[Classico.Nacionalidade, 4], rua[Classico.Animal, 4]);
    }

    [Fact]
    public void UmaTrocaQuebraOJuiz()
    {
        var rua = ARespostaCerta();
        (rua[Classico.Animal, 4], rua[Classico.Animal, 0]) = (rua[Classico.Animal, 0], rua[Classico.Animal, 4]);
        Assert.False(Juiz.Confere(Classico.Enigma(), rua));
        Assert.NotEmpty(Juiz.Quebradas(Classico.Enigma(), rua));
    }

    [Fact]
    public void RuaIncompletaNaoPassa()
    {
        Assert.False(Juiz.Confere(Classico.Enigma(), new Rua(5, 5)));
    }

    [Fact]
    public void DoisValoresNaMesmaCasaNaoPassam()
    {
        var rua = ARespostaCerta();
        rua[Classico.Cor, 0] = rua[Classico.Cor, 1];
        Assert.False(Juiz.Confere(Classico.Enigma(), rua));
    }

    [Fact]
    public void ARespostaCertaNaoQuebraPistaNenhuma()
    {
        Assert.Empty(Juiz.Quebradas(Classico.Enigma(), ARespostaCerta()));
    }

    [Fact]
    public void AEsquerdaImediataEhOrientada()
    {
        var rua = new Rua(3, 1);
        rua[0, 0] = 0;
        rua[0, 1] = 1;
        rua[0, 2] = 2;

        Assert.True(Juiz.Vale(Pista.Esquerda(0, 0, 0, 1), rua));
        Assert.False(Juiz.Vale(Pista.Esquerda(0, 1, 0, 0), rua));
    }

    [Fact]
    public void VizinhoValeDosDoisLados()
    {
        var rua = new Rua(3, 1);
        rua[0, 0] = 0;
        rua[0, 1] = 1;
        rua[0, 2] = 2;

        Assert.True(Juiz.Vale(Pista.Vizinho(0, 0, 0, 1), rua));
        Assert.True(Juiz.Vale(Pista.Vizinho(0, 1, 0, 0), rua));
        Assert.False(Juiz.Vale(Pista.Vizinho(0, 0, 0, 2), rua));
    }

    [Fact]
    public void OTextoDaRuaListaCasaPorCasa()
    {
        var texto = ARespostaCerta().Texto(Classico.Enigma());
        Assert.StartsWith("noruegues amarela agua dunhill gato", texto);
        Assert.EndsWith("sueco branca cerveja bluemaster cachorro", texto);
    }
}
