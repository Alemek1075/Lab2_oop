using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.Xsl;
using Lab2_oop.Models;
using Lab2_oop.Analyzers;

namespace Lab2_oop.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string? _filePath;
        private string _selectedAnalyzerName = "LINQ to XML";
        private string? _searchKeyword;
        private string? _selectedRole;
        private string? _selectedSection;
        private string? _selectedGroup;
        private string? _selectedRank;
        private string _statusMessage = "Очікування файлу...";

        private ObservableCollection<MemberInfo> _results;
        private ObservableCollection<string> _roles;
        private ObservableCollection<string> _sections;
        private ObservableCollection<string> _groups;
        private ObservableCollection<string> _ranks;

        private IXmlAnalyzer _currentAnalyzer;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel()
        {
            _results = new ObservableCollection<MemberInfo>();
            _roles = new ObservableCollection<string>();
            _sections = new ObservableCollection<string>();
            _groups = new ObservableCollection<string>();
            _ranks = new ObservableCollection<string>();

            AnalyzersList = new List<string> { "SAX API", "DOM API", "LINQ to XML" };
            _currentAnalyzer = new LinqAnalyzer();

            LoadFileCommand = new Command(async () => await LoadFileAsync());
            SearchCommand = new Command(PerformSearch);
            ClearCommand = new Command(ClearForm);
            TransformCommand = new Command(async () => await TransformToHtmlAsync());
            ExitCommand = new Command(async () => await ExitAppAsync());
        }

        public List<string> AnalyzersList { get; }

        public string SelectedAnalyzerName
        {
            get => _selectedAnalyzerName;
            set
            {
                if (_selectedAnalyzerName != value)
                {
                    _selectedAnalyzerName = value;
                    OnPropertyChanged();
                    switch (value)
                    {
                        case "SAX API": _currentAnalyzer = new SaxAnalyzer(); break;
                        case "DOM API": _currentAnalyzer = new DomAnalyzer(); break;
                        case "LINQ to XML": _currentAnalyzer = new LinqAnalyzer(); break;
                    }
                    if (!string.IsNullOrEmpty(_filePath)) PerformSearch();
                }
            }
        }

        public string? SearchKeyword
        {
            get => _searchKeyword;
            set { if (_searchKeyword != value) { _searchKeyword = value; OnPropertyChanged(); PerformSearch(); } }
        }

        public string? SelectedRole
        {
            get => _selectedRole;
            set { if (_selectedRole != value) { _selectedRole = value; OnPropertyChanged(); PerformSearch(); } }
        }

        public string? SelectedSection
        {
            get => _selectedSection;
            set { if (_selectedSection != value) { _selectedSection = value; OnPropertyChanged(); PerformSearch(); } }
        }

        public string? SelectedGroup
        {
            get => _selectedGroup;
            set { if (_selectedGroup != value) { _selectedGroup = value; OnPropertyChanged(); PerformSearch(); } }
        }

        public string? SelectedRank
        {
            get => _selectedRank;
            set { if (_selectedRank != value) { _selectedRank = value; OnPropertyChanged(); PerformSearch(); } }
        }

        public ObservableCollection<string> Roles { get => _roles; set { _roles = value; OnPropertyChanged(); } }
        public ObservableCollection<string> Sections { get => _sections; set { _sections = value; OnPropertyChanged(); } }
        public ObservableCollection<string> Groups { get => _groups; set { _groups = value; OnPropertyChanged(); } }
        public ObservableCollection<string> Ranks { get => _ranks; set { _ranks = value; OnPropertyChanged(); } }

        public ObservableCollection<MemberInfo> Results
        {
            get => _results;
            set { _results = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoadFileCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand TransformCommand { get; }
        public ICommand ExitCommand { get; }

        private async Task LoadFileAsync()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Оберіть XML файл",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { ".xml" } },
                        { DevicePlatform.macOS, new[] { "xml" } },
                        { DevicePlatform.Android, new[] { "application/xml", "text/xml" } }
                    })
                });

                if (result != null)
                {
                    _filePath = result.FullPath;
                    StatusMessage = $"Завантажено: {result.FileName}";
                    PopulateDynamicFilters(_filePath);
                    PerformSearch();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Помилка: {ex.Message}";
            }
        }

        private void PopulateDynamicFilters(string path)
        {
            Roles.Clear(); Roles.Add("");
            Sections.Clear(); Sections.Add("");
            Groups.Clear(); Groups.Add("");
            Ranks.Clear(); Ranks.Add("");

            try
            {
                XDocument doc = XDocument.Load(path);

                var roles = doc.Descendants("Participant").Select(p => p.Attribute("type")?.Value).Where(x => x != null).Distinct();
                foreach (var r in roles) if (r != null) Roles.Add(r);

                var sections = doc.Descendants("Section").Select(p => p.Attribute("name")?.Value).Where(x => x != null).Distinct();
                foreach (var s in sections) if (s != null) Sections.Add(s);

                var groups = doc.Descendants("Participant").Select(p => p.Attribute("group")?.Value).Where(x => x != null).Distinct();
                foreach (var g in groups) if (g != null) Groups.Add(g);

                var ranks = doc.Descendants("Participant").Select(p => p.Attribute("rank")?.Value).Where(x => x != null).Distinct();
                foreach (var r in ranks) if (r != null) Ranks.Add(r);
            }
            catch { }
        }

        private void PerformSearch()
        {
            if (string.IsNullOrEmpty(_filePath)) return;

            try
            {
                var criteria = new SearchCriteria
                {
                    Keyword = SearchKeyword ?? "",
                    Role = SelectedRole,
                    Section = SelectedSection,
                    Group = SelectedGroup,
                    Rank = SelectedRank
                };

                var data = _currentAnalyzer.Search(_filePath, criteria);

                Results.Clear();
                foreach (var item in data) Results.Add(item);

                StatusMessage = $"Знайдено: {Results.Count}. Метод: {SelectedAnalyzerName}";
            }
            catch (Exception ex) { StatusMessage = $"Помилка пошуку: {ex.Message}"; }
        }

        private void ClearForm()
        {
            SearchKeyword = string.Empty;
            SelectedRole = null;
            SelectedSection = null;
            SelectedGroup = null;
            SelectedRank = null;

            Results.Clear();
            if (!string.IsNullOrEmpty(_filePath)) PerformSearch();
            StatusMessage = "Форму очищено.";
        }

        private async Task TransformToHtmlAsync()
        {
            string? currentPath = _filePath;
            if (string.IsNullOrEmpty(currentPath)) return;

            try
            {
                string? directory = Path.GetDirectoryName(currentPath);
                if (directory == null) return;

                string xslPath = Path.Combine(directory, "transform.xsl");

                if (!File.Exists(xslPath))
                {
                    StatusMessage = "Не знайдено файл transform.xsl.";
                    return;
                }

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(xslPath);

                string outputHtml = Path.Combine(FileSystem.CacheDirectory, "report.html");
                transform.Transform(currentPath, outputHtml);

                await Launcher.Default.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(outputHtml) });
                StatusMessage = "HTML згенеровано.";
            }
            catch (Exception ex) { StatusMessage = $"Помилка HTML: {ex.Message}"; }
        }

        private async Task ExitAppAsync()
        {

            if (Shell.Current != null)
            {
                bool answer = await Shell.Current.DisplayAlert("Вихід", "Завершити роботу?", "Так", "Ні");
                if (answer)
                {
                    Application.Current?.Quit();
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}