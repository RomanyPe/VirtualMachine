using System.Windows;
using System.Windows.Controls;
using VMApplication;

namespace ASM_gen
{
    /// <summary>
    /// Логика взаимодействия для NewFileDialog.xaml
    /// </summary>
    public partial class NewFileDialog : Window
    {
        public (string fileName, SourceLanguage language)? Result { get; private set; }

        public NewFileDialog()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            string name = FileNameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите имя файла.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            bool isC = ((ComboBoxItem)LanguageBox.SelectedItem).Content.ToString() == "C";
            Result = (name, isC ? SourceLanguage.C : SourceLanguage.Asm);
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
