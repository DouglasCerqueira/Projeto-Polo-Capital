using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Verificar_Historico_Mensal.Commands;
using Verificar_Historico_Mensal.Models;

namespace Verificar_Historico_Mensal.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            Indicator = new OptionsModel();
            IndicatorsType = new List<string> { "IPCA", "IGP-M", "Selic" };
            DataList = new ObservableCollection<DataAPI>();
            ExportCommand = new RelayCommand(Csv);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private OptionsModel _indicator;
        public OptionsModel Indicator
        {
            get { return _indicator; }
            set
            {
                _indicator = value;
                OnPropertyChanged("Indicator");
            }
        }

        public ObservableCollection<DataAPI> DataList { get; set; }
        public List<string> IndicatorsType { get; set; }

        private string _selectedIndicatorsType;
        public string SelectedIndicatorsType
        {
            get { return _selectedIndicatorsType; }
            set
            {
                _selectedIndicatorsType = value;
                OnPropertyChanged("SelectedIndicatorsType");
            }
        }

        private RelayCommand _dataCommand;
        public RelayCommand DataCommand
        {
            get
            {
                if (_dataCommand == null)
                {
                    _dataCommand = new RelayCommand(async (param) => await FetchDataAsync(), (param) => CanFetchData());
                }
                return _dataCommand;
            }
        }

        public ICommand ExportCommand { get; private set; }

        private bool CanFetchData()
        {
            return !string.IsNullOrEmpty(SelectedIndicatorsType);
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task FetchDataAsync()
        {
            using (var client = new HttpClient())
            {
                string baseUrl = "https://olinda.bcb.gov.br/olinda/servico/Expectativas/versao/v1/odata/ExpectativaMercadoMensais?$top=10000&";
                string url = $"{baseUrl}$filter=Indicador%20eq%20'{SelectedIndicatorsType}'";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var indicadorDataResponse = JsonConvert.DeserializeObject<IndicadorDataResponse>(json);

                        var filteredData = indicadorDataResponse.Value.Where(data => DateTime.Parse(data.Data) >= Indicator.StartDate && DateTime.Parse(data.Data) <= Indicator.EndDate);

                        DataList.Clear();
                        foreach (var data in filteredData)
                        {
                            DataList.Add(data);
                        }

                        if (DataList.Count == 0)
                        {
                            ShowErrorMessage("Nenhum dado encontrado.");
                        }
                    }
                    else
                    {
                        ShowErrorMessage($"Falha na requisição: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage($"Erro ao acessar a API: {ex.Message}");
                }
            }
        }

        private void Csv(object parameter)
        {
            if (DataList == null || DataList.Count == 0)
            {
                MessageBox.Show("Nenhum dado para exportar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string defaultFilePath = Path.Combine(documentsPath, "ArquivoExportado.csv");

            StringBuilder csvContent = new StringBuilder();

            csvContent.AppendLine("Indicador , Data , DataReferencia , Media , Mediana , DesvioPadrao , Minimo , Maximo , NumeroRespondentes , BaseCalculo");

            foreach (var item in DataList)
            {
                csvContent.AppendLine($"{item.Indicador},{item.Data},{item.DataReferencia},{item.Media},{item.Mediana},{item.DesvioPadrao},{item.Minimo},{item.Maximo},{item.NumeroRespondentes},{item.BaseCalculo}");
            }

            // Escreve a string CSV em um arquivo
            File.WriteAllText(defaultFilePath, csvContent.ToString());

            MessageBox.Show("Os dados foram exportados com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public class IndicadorDataResponse
        {
            public List<DataAPI> Value { get; set; }
        }
    }
}
