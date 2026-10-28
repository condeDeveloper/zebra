namespace Conde.Zebra;

/// <summary>
/// O enigma como ele é contado desde 1962, quando apareceu na revista Life: cinco
/// casas, cinco nacionalidades, cinco cores, cinco bebidas, cinco cigarros e
/// cinco animais. Quem tem o peixe?
/// </summary>
public static class Classico
{
    public const int Nacionalidade = 0;
    public const int Cor = 1;
    public const int Bebida = 2;
    public const int Cigarro = 3;
    public const int Animal = 4;

    public static readonly string[] Categorias = ["nacionalidade", "cor", "bebida", "cigarro", "animal"];

    public static readonly string[][] Valores =
    [
        ["ingles", "sueco", "dinamarques", "noruegues", "alemao"],
        ["vermelha", "verde", "branca", "amarela", "azul"],
        ["cha", "cafe", "leite", "cerveja", "agua"],
        ["pallmall", "dunhill", "blend", "bluemaster", "prince"],
        ["cachorro", "passaro", "gato", "cavalo", "peixe"],
    ];

    /// <summary>Os textos de cada pista, na mesma ordem de <see cref="Pistas"/>.</summary>
    public static readonly string[] Textos =
    [
        "o ingles mora na casa vermelha",
        "o sueco tem cachorro",
        "o dinamarques bebe cha",
        "a casa verde fica logo a esquerda da branca",
        "o dono da casa verde bebe cafe",
        "quem fuma Pall Mall cria passaros",
        "o dono da casa amarela fuma Dunhill",
        "quem mora na casa do meio bebe leite",
        "o noruegues mora na primeira casa",
        "quem fuma Blend mora ao lado de quem tem gato",
        "quem tem cavalo mora ao lado de quem fuma Dunhill",
        "quem fuma Blue Master bebe cerveja",
        "o alemao fuma Prince",
        "o noruegues mora ao lado da casa azul",
        "quem fuma Blend tem um vizinho que bebe agua",
    ];

    public static readonly Pista[] Pistas =
    [
        Pista.Mesma(Nacionalidade, 0, Cor, 0),       // inglês, vermelha
        Pista.Mesma(Nacionalidade, 1, Animal, 0),    // sueco, cachorro
        Pista.Mesma(Nacionalidade, 2, Bebida, 0),    // dinamarquês, chá
        Pista.Esquerda(Cor, 1, Cor, 2),              // verde à esquerda da branca
        Pista.Mesma(Cor, 1, Bebida, 1),              // verde, café
        Pista.Mesma(Cigarro, 0, Animal, 1),          // Pall Mall, pássaro
        Pista.Mesma(Cor, 3, Cigarro, 1),             // amarela, Dunhill
        Pista.Fixa(Bebida, 2, 2),                    // leite na casa do meio
        Pista.Fixa(Nacionalidade, 3, 0),             // norueguês na primeira
        Pista.Vizinho(Cigarro, 2, Animal, 2),        // Blend, gato
        Pista.Vizinho(Animal, 3, Cigarro, 1),        // cavalo, Dunhill
        Pista.Mesma(Cigarro, 3, Bebida, 3),          // Blue Master, cerveja
        Pista.Mesma(Nacionalidade, 4, Cigarro, 4),   // alemão, Prince
        Pista.Vizinho(Nacionalidade, 3, Cor, 4),     // norueguês, casa azul
        Pista.Vizinho(Cigarro, 2, Bebida, 4),        // Blend, água
    ];

    public static Enigma Enigma() => new(5, Categorias, Valores, Pistas);

    /// <summary>
    /// Uma versão pequena com as mesmas regras: três casas, três categorias e
    /// quatro pistas. Serve para a força bruta poder rodar inteira e conferir a
    /// busca, porque no enigma de cinco casas ela teria de montar 24,8 bilhões de
    /// ruas.
    /// </summary>
    public static Enigma Pequeno()
    {
        string[] categorias = ["nacionalidade", "cor", "bebida"];
        string[][] valores =
        [
            ["ingles", "sueco", "noruegues"],
            ["vermelha", "verde", "branca"],
            ["cha", "cafe", "leite"],
        ];

        Pista[] pistas =
        [
            Pista.Mesma(0, 0, 1, 0),
            Pista.Esquerda(1, 1, 1, 2),
            Pista.Mesma(1, 1, 2, 1),
            Pista.Fixa(0, 2, 0),
        ];

        return new Enigma(3, categorias, valores, pistas);
    }

    /// <summary>
    /// Uma versão de quatro casas e quatro categorias, ainda ao alcance da força
    /// bruta: 331.776 ruas.
    /// </summary>
    public static Enigma Medio()
    {
        string[] categorias = ["nacionalidade", "cor", "bebida", "animal"];
        string[][] valores =
        [
            ["ingles", "sueco", "noruegues", "alemao"],
            ["vermelha", "verde", "branca", "azul"],
            ["cha", "cafe", "leite", "agua"],
            ["cachorro", "passaro", "gato", "peixe"],
        ];

        Pista[] pistas =
        [
            Pista.Mesma(0, 0, 1, 0),
            Pista.Esquerda(1, 1, 1, 2),
            Pista.Mesma(1, 1, 2, 1),
            Pista.Fixa(0, 2, 0),
            Pista.Mesma(0, 1, 3, 0),
            Pista.Vizinho(3, 2, 2, 3),
            Pista.Mesma(0, 3, 3, 3),
        ];

        return new Enigma(4, categorias, valores, pistas);
    }
}
