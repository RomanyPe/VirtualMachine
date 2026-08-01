using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ASM_gen.StartWindow
{
    public partial class MainMenu : Page
    {
        private Dictionary<string, FileReadResult> _projects = null!;
        public MainMenu()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Вызываем ваш метод из DirectoryManager
            _projects = await DirectoryManager.SearchProjects();

            // Привязываем словарь к ListBox
            LstFiles.ItemsSource = _projects;
        }

        private void ProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                // Извлекаем путь к файлу (это наш Key из словаря)
                string? filePath = clickedButton.Tag as string;

                if (!string.IsNullOrEmpty(filePath) && _projects.TryGetValue(filePath, out FileReadResult result))
                {
                    // Проверяем, не было ли ошибки при сканировании этого файла
                    if (result.Error != ErrorFile.None)
                    {
                        MessageBox.Show($"Невозможно открыть проект: {result.FileName}\nПричина: {result.Error}",
                                        "Ошибка чтения", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Если ошибок нет — запускаем логику открытия
                    MessageBox.Show($"Открываем валидный проект!\nИмя: {result.FileName}\nПуть: {result.FinalFilePath}",
                                    "Успех");

                    // TODO: Ваша логика перехода в рабочую область IDE
                    // OpenProjectWorkspace(result);
                }
            }
        }
        private void BtnNewProj_Click(object sender, RoutedEventArgs e)
        {
            //Dictionary<string, FileReadResult> files = DirectoryManager.SearchProjects().Result;

            //// Добавляем найденные пути в список
            //foreach (string file in files.Keys)
            //{
            //    LstFiles.Items.Add(file);
            //}
            // Навигация на новую страницу по её типу
            NavigationService.Navigate(new IDEPage());
        }
    }
}
