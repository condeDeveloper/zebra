# O enigma da zebra

Cinco casas em fila, cinco nacionalidades, cinco cores, cinco bebidas, cinco
cigarros, cinco animais, e quinze pistas. **Quem tem o peixe?**

O enigma apareceu na revista Life em 1962 e costuma ser atribuído a Einstein, que
provavelmente nunca o viu. Aqui ele é resolvido em C# e .NET 8, mas a resposta é a
parte chata: o interessante é que o conjunto de pistas também é finito e
enumerável, e dá para perguntar **quais delas são realmente necessárias**.

```bash
dotnet test testes/Zebra.Testes/Zebra.Testes.csproj -c Release    # 48 testes
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release
```

## A resposta

```
  resposta    o alemao, na casa 4
  a rua       noruegues amarela agua dunhill gato | dinamarques azul cha blend cavalo |
              ingles vermelha leite pallmall passaro | alemao verde cafe prince peixe |
              sueco branca cerveja bluemaster cachorro
  nos         5,520
  segundos    0.018
```

São 24.883.200.000 ruas possíveis — 120 elevado a 5 — e a busca abre 5.520 nós.

## Só a ordem das categorias

```
  ordem                     solucoes          nos    vezes
  como esta escrito                1        5,520     1.0x
  cor primeiro                     1        5,520     1.0x
  animal primeiro                  1      149,880    27.2x
  bebida no fim                    1       14,880     2.7x
  animal e cigarro antes           1       81,600    14.8x
```

A busca preenche uma categoria de cada vez. Nada é descartado e a resposta é
sempre a mesma; o que muda é **quando cada pista fica testável**. Uma pista que
liga nacionalidade e animal só pode ser conferida quando as duas estiverem postas,
e até lá ela não poda nada.

Começar pelos animais custa **27 vezes mais**, porque as pistas que falam de animal
são quase todas do tipo "mora ao lado de", que precisam de uma segunda categoria
para dizer alguma coisa. A ordem das variáveis não é detalhe de implementação: é o
algoritmo.

## Uma pista a menos

```
    n pista                                                 sem ela  necessaria
    1 o ingles mora na casa vermelha                              6         sim
    2 o sueco tem cachorro                                        2         sim
    3 o dinamarques bebe cha                                      2         sim
    4 a casa verde fica logo a esquerda da branca                 8         sim
    5 o dono da casa verde bebe cafe                              4         sim
    6 quem fuma Pall Mall cria passaros                           4         sim
    7 o dono da casa amarela fuma Dunhill                         7         sim
    8 quem mora na casa do meio bebe leite                        4         sim
    9 o noruegues mora na primeira casa                          17         sim
   10 quem fuma Blend mora ao lado de quem tem gato               2         sim
   11 quem tem cavalo mora ao lado de quem fuma Dunhill           2         sim
   12 quem fuma Blue Master bebe cerveja                          3         sim
   13 o alemao fuma Prince                                        4         sim
   14 o noruegues mora ao lado da casa azul                      12         sim
   15 quem fuma Blend tem um vizinho que bebe agua                1         NAO
```

**A última pista do enigma não serve para nada.** Tirando "quem fuma Blend tem um
vizinho que bebe água", o enigma continua com uma solução só. As outras catorze
são todas necessárias: tirar qualquer uma delas faz o número de respostas saltar
para 2, 4, 8, até 17.

A pista 9, "o norueguês mora na primeira casa", é a que mais segura o enigma: sem
ela são **17 ruas** compatíveis com tudo o mais.

## Todos os subconjuntos de pistas

Se a 15 sobra, será que dá para tirar duas? Três? O conjunto de pistas tem 15
elementos, então são 32.768 subconjuntos, e todos cabem numa varredura:

```
   pistas  subconjuntos  deixam uma solucao so  o primeiro que basta
       10         3,003                      0
       11         1,365                      0
       12           455                      0
       13           105                      0
       14            15                      1  1 2 3 4 5 6 7 8 9 10 11 12 13 14
       15             1                      1  1 2 3 4 5 6 7 8 9 10 11 12 13 14 15
```

**Dois.** De todos os 32.768 subconjuntos possíveis, exatamente dois deixam o
enigma com uma solução só: o conjunto completo e o conjunto sem a pista 15. Com 13
pistas ou menos, nenhum subconjunto basta — nem um dos 105 de tamanho 13, nem um
dos 455 de tamanho 12.

Então o enigma não é "quase mínimo": ele tem **exatamente uma pista a mais do que
precisa**, e essa pista é a última da lista.

## A busca contra a força bruta

```
  enigma                     ruas possiveis  nos da forca bruta  nos da busca  solucoes   bate
  3 casas, 3 categorias                 216                 216            24         2    sim
  4 casas, 4 categorias             331,776             331,776         1,512        32    sim
```

A força bruta monta **todas** as ruas possíveis, sem olhar pista nenhuma pelo
caminho, e entrega cada uma ao juiz. Ela abre exatamente o número de ruas que
existem, e acha o mesmo número de soluções que a busca com poda.

No enigma de cinco casas ela teria de montar 24,8 bilhões de ruas, e por isso a
conferência fica nos pequenos. Um juiz que só funciona em miniatura ainda é um
juiz: ele não compartilha uma linha de código com a busca.

## O que as medidas me corrigiram

**Compartilhei o vetor de casas ocupadas entre as categorias.** A busca preenche
cada categoria distribuindo seus valores pelas cinco casas, e eu tinha um único
vetor `usada` para o enigma inteiro. Depois que a primeira categoria terminava, as
cinco casas estavam "ocupadas" e nenhuma outra categoria achava onde morar: a
busca devolvia zero soluções, sem erro nenhum. As casas são as mesmas cinco para
toda categoria, e cada nível precisa do seu próprio vetor.

**Ia escrever que o enigma é mínimo.** A frase natural seria "quinze pistas, nem
uma a mais". São catorze. A pista da água não faz falta, e isso não se descobre
lendo o enunciado: só tirando uma de cada vez e contando as soluções.

**Esperava achar vários subconjuntos de 13 pistas que bastassem.** Se uma pista
sobra, parecia razoável que houvesse combinações mais enxutas. São zero, em 105
subconjuntos de tamanho 13 e 455 de tamanho 12. A redundância do enigma é de
exatamente uma pista, e não de uma margem confortável.

**Achei que a ordem das categorias mudasse pouco.** São 27 vezes entre a melhor e
a pior, com a mesma busca e o mesmo resultado. Um enigma de lógica não tem
"dificuldade" sozinho: tem dificuldade para uma ordem de perguntas.

## Os arquivos

| arquivo | o que faz |
|---|---|
| `src/Zebra/Enigma.cs` | casas, categorias, valores e os quatro tipos de pista |
| `src/Zebra/Juiz.cs` | a rua preenchida e a conferência pista por pista, pela definição |
| `src/Zebra/Resolvedor.cs` | a busca por categoria com poda, e a força bruta que monta toda rua |
| `src/Zebra/Classico.cs` | o enigma de 1962, e duas versões pequenas para a força bruta alcançar |
| `src/Zebra/Medidas.cs` | as cinco tabelas deste README |
| `ferramentas/Medidor` | imprime as tabelas, uma por comando |

## Rodando

```bash
dotnet test testes/Zebra.Testes/Zebra.Testes.csproj -c Release

dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- peixe
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- ordem
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- pistas
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- minimo
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- juiz
```

C# 12 e .NET 8. Zero dependências fora do xUnit, e só nos testes.

48 testes. Os números deste README estão presos em `MedidasTestes`.
