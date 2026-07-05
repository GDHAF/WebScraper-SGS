# WebScraper-SGS

Coletor de dados do Banco Central do Brasil, focado em dados de crédito veicular.

O projeto navega automaticamente pelo portal público do Banco Central (`https://www3.bcb.gov.br/sgspub/`), insere os códigos das séries desejadas, extrai os valores históricos e exporta tudo em arquivos `.csv` individuais, além de um arquivo `dicionario.csv` com os metadados de cada série baixada.

---

## Sumário

- [Requisitos](#requisitos)
- [Instalação](#instalação)
- [Como executar](#como-executar)
- [Séries coletadas](#séries-coletadas)
- [Estrutura do projeto](#estrutura-do-projeto)
- [Formato dos arquivos de saída](#formato-dos-arquivos-de-saída)
- [Amostra de saída](#amostra-de-saída)
- [Limitações conhecidas](#limitações-conhecidas)

---

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) ou superior
- Google Chrome instalado na máquina
- ChromeDriver compatível com a versão do Chrome instalada

Pacotes NuGet utilizados (restaurados automaticamente via `dotnet restore`):

| Pacote | Uso |
|---|---|
| `Selenium.WebDriver` | Automação do navegador para navegar no portal do SGS |
| `CsvHelper` | Escrita dos arquivos `.csv` de saída |

---

## Instalação

```bash
# Clone o repositório
git clone https://github.com/GDHAF/WebScraper-SGS.git
cd WebScraper-SGS

# Restaure as dependências
dotnet restore
```

---

## Como executar

```bash
dotnet run
```

Ao rodar, o programa:

1. Abre uma janela do Chrome via Selenium e acessa o portal do Banco Central.
2. Insere sequencialmente o código de cada série configurada em `Program.cs` (lista `series`).
3. Solicita a consulta de todos os valores históricos das séries selecionadas.
4. Percorre a paginação de resultados extraindo data e valor de cada série.
5. Salva um arquivo `.csv` por série na pasta `Output/`.
6. Gera o arquivo `Output/dicionario.csv` com os metadados de todas as séries processadas de acordo com o que já foi definido.

> A janela do navegador precisa permanecer aberta e visível durante toda a execução — não feche o Chrome manualmente enquanto o processo estiver rodando.

---

## Séries coletadas

| Código SGS | Nome | Descrição | Arquivo de saída |
|---|---|---|---|
| 20553 | Saldo PJ | Saldo de financiamento de veículos — Pessoas Jurídicas | `CreditBalanceCompanies.csv` |
| 20581 | Saldo PF | Saldo de financiamento de veículos — Pessoas Físicas | `CreditBalanceHouseholds.csv` |
| 20673 | Concessões PF | Novas concessões de financiamento — Pessoas Físicas | `CreditConcessionsHouseholds.csv` |
| 20728 | Juros PJ | Taxa média anualizada de juros — Pessoas Jurídicas | `InterestRatesCompanies.csv` |
| 20749 | Juros PF | Taxa média anualizada de juros — Pessoas Físicas | `InterestRatesHouseholds.csv` |
| 21084 | Inadimplência PF | Percentual de operações com atraso acima de 90 dias — Pessoas Físicas | `NonPerformingLoansHouseholds.csv` |

Para adicionar uma nova série, inclua um novo item na lista `series` em `Program.cs`, informando `num`, `id_unico` (código SGS), `Name`, `Description` e `OutputFile`.

---

## Estrutura do projeto

```
WebScraper-SGS/
├── Models/
│   ├── SeriesInfo.cs       # Metadados de cada série (código, nome, descrição, arquivo de saída)
│   └── SeriesValues.cs     # Par (data, valor) de cada observação da série
├── Output/
│   ├── outputs_series.csv
|   ├──/...
│   └── dicionario.csv
├── Program.cs              # Ponto de entrada: automação do scraping e geração dos CSVs
├── scraper_sgs.csproj
└── README.md
```

---

## Formato dos arquivos de saída

Cada série é salva em um `.csv` separado com duas colunas:

```
data,value
jun/2000,35.54
jul/2000,35.95
ago/2000,34.79
```

O arquivo `dicionario.csv` indexa os metadados de todas as séries baixadas na mesma execução:

```
id_unico,description,file_path
CreditBalanceCompanies,Vehicle financing balance for companies,Output/CreditBalanceCompanies.csv
CreditBalanceHouseholds,Vehicle financing balance for households,Output/CreditBalanceHouseholds.csv
...
```

---

## Amostra de saída

`Output/InterestRatesHouseholds.csv`:

| data | value |
|---|---|
| jun/2000 | 35.54 |
| jul/2000 | 35.95 |
| ago/2000 | 34.79 |
| set/2000 | 35.22 |

---

## Limitações conhecidas

- **Dependência de interface visual:** a coleta é feita via automação de navegador (Selenium) sobre a interface pública do Banco Central. Dessa forma, o processo se torna dependente de mudanças no layout do site utilizado.
- **Tempos de espera fixos:** o código usa pausas fixas (`Task.Delay`) para aguardar carregamento de página, o que pode causar falhas em conexões mais lentas ou instabilidades no navegador.
