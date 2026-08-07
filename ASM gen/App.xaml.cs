using ASM_gen.StartWindow;
using System.IO;
using System.Reflection;
using System.Windows;
using VMApplication;

namespace ASM_gen
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }
        public static VMHost VmHost { get; private set; } = null!;

        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    base.OnStartup(e);

        //    // 1. Создаём UI-сервисы (пока без пути проекта, путь будет задан позже)
        //    // Но для открытия проекта нужно знать путь, поэтому IFileService можно создать временно.
        //    // Лучше создать VMHost без IFileService, а потом задать проект.
        //    // Упростим: создадим WpfOutputView сразу, привяжем к outputBox главного окна.
        //    // Остальное создадим в IDEPage после загрузки проекта.

        //    // WpfOutputView будет создан после загрузки окна, так как нужен RichTextBox.
        //    // Пока просто запускаем окно.
        //    MainWindow mainWindow = new();
        //    mainWindow.Show();
        //}
        private static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            // Имя сборки, которую ищет среда
            string assemblyName = new AssemblyName(args.Name).Name + ".dll";

            // Путь к нашей кастомной папке (используем AppPaths)
            string probePath = Path.Combine(AppPaths.CurrentTemplatesPath, assemblyName);

            if (File.Exists(probePath))
            {
                return Assembly.LoadFrom(probePath);
            }

            return null;
        }
    }
}
