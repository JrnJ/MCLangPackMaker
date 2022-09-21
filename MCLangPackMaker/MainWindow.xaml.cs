using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using MCLangPackMaker.Classes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MCLangPackMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<LangPackValue> _searches;

        public ObservableCollection<LangPackValue> Searches
        {
            get { return _searches; }
            set { _searches = value; OnPropertyChanged(); }
        }

        private string _searchValue;

        public string SearchValue
        {
            get { return _searchValue; }
            set { _searchValue = value; OnPropertyChanged(); }
        }

        public MainWindow()
        {
            InitializeComponent();

            AsyncMainWindow();

            Searches = new();

            DataContext = this;
        }

        private ObservableCollection<LangPackValue> _langPack;

        public ObservableCollection<LangPackValue> LangPack
        {
            get { return _langPack; }
            set { _langPack = value; }
        }

        public async void AsyncMainWindow()
        {
            // Get Pack
            LangPack = await LangPackFileHandler.GetLangPack(MCVersion.V1x19x2, LangPackFileHandler.Default1x19x2LangPackPath);
            //LangPack = await LangPackFileHandler.GetLangPack(MCVersion.V1x8x9, LangPackFileHandler.Default1x8x9LangPackPath);
        }

        public void SearchFromKey(string key)
        {
            Searches = new();

            for (int i = 0; i < LangPack.Count; i++)
            {
                // Look for Matching Key
                if (LangPack[i].Key.ToLower().Contains(key.ToLower()))
                {
                    Searches.Add(LangPack[i]);
                }
            }
        }

        public void SearchFromValue(string value)
        {
            // ?
        }

        private void SearchClick(object sender, RoutedEventArgs e)
        {
            SearchFromKey(SearchValue);
        }

        private void SearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchFromKey(((TextBox)sender).Text);
            }
        }

        private async void SaveLangPackClick(object sender, RoutedEventArgs e)
        {
            await LangPackFileHandler.SaveLangPack(MCVersion.V1x19x2, MCVersion.V1x8x9, LangPack);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
