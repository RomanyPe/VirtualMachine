using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ASM_gen;

public partial class NewDiskDialog : Window
{
    public string? SelectedFilePath { get; private set; }
    public string? NameDisk { get; private set; }
    public uint Port { get; private set; }
    public int SectorCount { get; private set; }
    public DiskCreationMode Mode { get; private set; }

    public NewDiskDialog(DiskCreationMode initialMode = DiskCreationMode.Empty)
    {
        InitializeComponent();
        ModeCombo.SelectedIndex = (int)initialMode;
        UpdateMode();
    }

    public enum DiskCreationMode
    {
        Empty,
        FromBin,
        FromImage
    }

    private void UpdateMode()
    {
        Mode = (DiskCreationMode)ModeCombo.SelectedIndex;
        BrowseButton.IsEnabled = Mode != DiskCreationMode.Empty;
        FilePathText.Text = "";
        SelectedFilePath = null;
    }

    private void Browse_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
        switch (Mode)
        {
            case DiskCreationMode.FromBin:
                dialog.Filter = "Program files (*.bin)|*.bin|All files (*.*)|*.*";
                dialog.Title = "Выберите .bin файл программы";
                break;
            case DiskCreationMode.FromImage:
                dialog.Filter = "Disk image files (*.vmg)|*.vmg|All files (*.*)|*.*";
                dialog.Title = "Выберите файл образа диска";
                break;
        }

        if (dialog.ShowDialog() == true)
        {
            SelectedFilePath = dialog.FileName;
            FilePathText.Text = Path.GetFileName(SelectedFilePath);
        }
    }

    private void Create_Click(object sender, RoutedEventArgs e)
    {
        if (!uint.TryParse(PortBox.Text.Trim(), out uint port))
        {
            MessageBox.Show("Введите корректный номер порта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        Port = port;

        if (Mode == DiskCreationMode.FromBin && string.IsNullOrEmpty(SelectedFilePath))
        {
            MessageBox.Show("Выберите .bin файл.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (Mode == DiskCreationMode.FromImage && string.IsNullOrEmpty(SelectedFilePath))
        {
            MessageBox.Show("Выберите .vmg файл.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        NameDisk = DiskName.Text;
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded)
            UpdateMode();
    }

    private void SectorCountBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SectorCountBox.Text)) return;

        if (!int.TryParse(SectorCountBox.Text.Trim(), out int sectors) || sectors <= 0)
        {
            MessageBox.Show("Введите положительное количество секторов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        SectorCount = 1;
        while (SectorCount < sectors)
            SectorCount <<= 1;

        SectorCountBox.Text = SectorCount.ToString();
    }

    private void SectorCountBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !char.IsDigit(e.Text, e.Text.Length - 1);
    }
}