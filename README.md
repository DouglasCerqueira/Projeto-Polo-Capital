# Projeto-Polo-Capital

Visão Geral:
- Este projeto é feito em WPF (Windows Presentation Foundation) utilizando a arquitetura MVVM (Model-View-ViewModel). Este programa foi feito para exibir as séries históricas da expectativa do mercado mensal fornecida pelo Banco Central. Ele consiste em o usuário escolher entre três tipos de indicadores (IPCA, IGP-M, Selic) e um intervalo de tempo entre início e fim. Após as escolhas, o programa consulta uma API e exibe os dados escolhidos na sua interface e também sendo possível o usuário exportar estas informações em um arquivo CSV.

Ambiente:
- O programa foi feito no Visual Studio Community, utilizando o .NET 8.0. Com alguns NuGet Packages (Newtonsoft.json e CommunityToolkit.Mvvm)

Estrutura:

Models: A pasta Models possui dois arquivos.
 - OptionsModel: Representa os dados do indicador, data de início e data do fim
 - DataAPI: Representa os dados recebidos pela resposta da API.

ViewModel: A pasta ViewModel possui um arquivo.
 - MainViewModel: Representa a lógica da interação com o usuário e a conexão/resposta com a API
 
View: A pasta View possui um arquivo.
 - MainWindow: Representa a tela principal da aplicação e é onde o usuário consegue selecionar suas interações.

Commands: A pasta Commands possui um arquivo.
 - RelayCommand: Representa uma classe que implementa a interface ICommand do WPF e permite que os comandos sejam associados a ações na ViewModels facilitando a execução de ações em resposta a eventos de interface do usuário.

Funcionalidade:
- O sistema tem um funcionamento simples, View: o usuário escolhe o indicador, a data de início e fim . O programa executa essas informações mandando para MainViewModel que recebe essas informações e manda direto para API, então a API retorna os dados solicitados para a MainViewModel e ela retorna através de um DataGrid as informações na View e caso o usuário deseje, ele poderá exportar os dados em um arquivo CSV.

Observações:
- É um projeto simples que pode vir ser modificado no futuro, caso necessário, contendo mais conhecimento e informações.

<img src="https://prnt.sc/p04PqQpbNJ3e" alt="Imagem APP">




