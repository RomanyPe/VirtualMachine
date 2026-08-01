using System.Runtime.InteropServices;

namespace ASM_gen
{
    public static partial class IDEConsoleManager
    {
        // Константы Win32 API
        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint SC_CLOSE = 0xF060;
        // Константы для управления видимостью
        private const int SW_HIDE = 0;     // Скрыть окно
        private const int SW_SHOW = 5;     // Показать окно

        public static void DisableCloseButton()
        {
            
            IntPtr hwnd = GetConsoleWindow();
            if (hwnd != IntPtr.Zero)
            {
                IntPtr hMenu = GetSystemMenu(hwnd, false);
                if (hMenu != IntPtr.Zero)
                {

                    // Удаляем пункт "Закрыть", кнопка 'X' станет серой и неактивной
                    RemoveMenu(hMenu, SC_CLOSE, MF_BYCOMMAND);
                }
            }
        }

        public static void InitConsole(bool useConsole)
        {
            nint handle = GetConsoleWindow();
            DisableCloseButton();
            if (handle != nint.Zero)
            {
                ShowWindow(handle, useConsole ? SW_SHOW : SW_HIDE);
            }

            if (useConsole)
            {
                Console.Clear();
            }

        }

        [LibraryImport("kernel32.dll")]
        private static partial nint GetConsoleWindow();

        [LibraryImport("user32.dll")]
        private static partial IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}