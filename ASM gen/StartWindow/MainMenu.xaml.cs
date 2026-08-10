using ASM_gen.NewProjectManage;
using ASM_gen.ProjectManage.Managers.Static;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using VMApplication.Project;

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
            _projects = await DirManager.SearchProjects();

            // Привязываем словарь к ListBox
            LstFiles.ItemsSource = _projects;
        }

        private void ProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                string? filePath = clickedButton.Tag as string;
                if (!string.IsNullOrEmpty(filePath) && _projects.TryGetValue(filePath, out FileReadResult result))
                {
                    if (result.Error != ErrorFile.None)
                    {
                        MessageBox.Show($"Невозможно открыть проект: {result.FileName}\nПричина: {result.Error}",
                                        "Ошибка чтения", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string projectFolder = System.IO.Path.GetDirectoryName(filePath)!;
                    NavigationService.Navigate(new IDEPage(projectFolder));
                }
            }
        }
        private async void BtnNewProj_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NewProjectDialog { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() == true && dialog.CreatedProjectPath != null)
            {
                // Обновляем список проектов
                _projects = await DirManager.SearchProjects();
                LstFiles.ItemsSource = _projects;
            }
        }
    }
}
