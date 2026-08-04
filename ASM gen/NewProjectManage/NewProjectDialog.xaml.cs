using ASM_gen.ProjectManage.Managers.Static;
using System.Windows;
using System.Windows.Controls;

namespace ASM_gen.NewProjectManage
{
    /// <summary>
    /// Логика взаимодействия для NewProjectDialog.xaml
    /// </summary>
    public partial class NewProjectDialog : Window
    {
        public string? CreatedProjectPath { get; private set; }

        public NewProjectDialog()
        {
            InitializeComponent();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            string name = ProjectNameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите имя проекта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isAsm = ((ComboBoxItem)LanguageBox.SelectedItem).Content.ToString() == "Ассемблер";
            try
            {
                CreatedProjectPath = DirectoryManager.CreateNewProject(name, isAsm);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}
