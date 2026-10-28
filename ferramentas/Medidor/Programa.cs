using Conde.Zebra;

var comando = args.Length > 0 ? args[0] : "tudo";

if (comando is "peixe" or "tudo") Peixe();
if (comando is "ordem" or "tudo") Ordem();
if (comando is "pistas" or "tudo") Pistas();
if (comando is "minimo" or "tudo") Minimo();
if (comando is "juiz" or "tudo") Juiz();

static void Peixe()
{
    Console.WriteLine();
    var (dono, casa, rua, nos, segundos) = Medidas.QuemTemOPeixe();
    Console.WriteLine("QUEM TEM O PEIXE?");
    Console.WriteLine();
    Console.WriteLine($"  resposta    o {dono}, na casa {casa + 1}");
    Console.WriteLine($"  a rua       {rua}");
    Console.WriteLine($"  nos         {nos:n0}");
    Console.WriteLine($"  segundos    {segundos:0.000}");
    Console.WriteLine();
}

static void Ordem()
{
    Console.WriteLine();
    Console.WriteLine("SO A ORDEM DAS CATEGORIAS. A mesma busca, a mesma resposta, outro preco.");
    Console.WriteLine();
    Console.WriteLine($"  {"ordem",-24} {"solucoes",9} {"nos",12} {"vezes",8}");

    foreach (var l in Medidas.PorOrdem())
        Console.WriteLine($"  {l.Ordem,-24} {l.Solucoes,9} {l.Nos,12:n0} {l.Vezes,7:0.0}x");

    Console.WriteLine();
}

static void Pistas()
{
    Console.WriteLine();
    Console.WriteLine("TIRANDO UMA PISTA DE CADA VEZ: quantas solucoes sobram?");
    Console.WriteLine();
    Console.WriteLine($"  {"n",3} {"pista",-52} {"sem ela",8} {"necessaria",11}");

    foreach (var l in Medidas.UmaAMenos())
        Console.WriteLine($"  {l.Numero,3} {l.Texto,-52} {l.SemEla,8} {(l.Necessaria ? "sim" : "NAO"),11}");

    Console.WriteLine();
}

static void Minimo()
{
    Console.WriteLine();
    Console.WriteLine("TODOS OS 32.768 SUBCONJUNTOS DAS 15 PISTAS, por tamanho.");
    Console.WriteLine();
    Console.WriteLine($"  {"pistas",7} {"subconjuntos",13} {"deixam uma solucao so",22}  o primeiro que basta");

    foreach (var l in Medidas.MenorConjuntoQueBasta())
        Console.WriteLine($"  {l.Pistas,7} {l.Subconjuntos,13:n0} {l.ComSolucaoUnica,22:n0}  {l.Exemplo}");

    Console.WriteLine();
}

static void Juiz()
{
    Console.WriteLine();
    Console.WriteLine("A BUSCA CONTRA A FORCA BRUTA que monta toda rua e entrega ao juiz.");
    Console.WriteLine();
    Console.WriteLine($"  {"enigma",-24} {"ruas possiveis",16} {"nos da forca bruta",19} {"nos da busca",13} {"solucoes",9} {"bate",6}");

    foreach (var l in Medidas.ContraOJuiz())
        Console.WriteLine($"  {l.Enigma,-24} {l.Atribuicoes,16:n0} {l.NosForcaBruta,19:n0} {l.NosBusca,13:n0} {l.Solucoes,9} {(l.Bate ? "sim" : "NAO"),6}");

    Console.WriteLine();
}
