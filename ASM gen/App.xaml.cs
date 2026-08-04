using ASM_gen.ProjectManage.Data;
using System.IO;
using System.Reflection;
using System.Windows;

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
