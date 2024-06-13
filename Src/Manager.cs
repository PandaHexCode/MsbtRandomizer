﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using MSBTRando.Windows;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Win32;
using MSBTRando.Dialogs;
using System.Numerics;
using ImGuiNET;
using System.Security.Cryptography;
using FolderBrowserEx;
using System.Windows.Forms;
using Silk.NET.Core;
using System.Drawing.Imaging;
using System.Drawing;
using Image = SixLabors.ImageSharp.Image;
using System.Net;
using System.Management;

namespace MSBTRando{ 

    public class Manager
    {

        [DllImport("user32.dll")]
        public static extern int FindWindow(string ClassName, string WindowName);

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowCursor(bool bShow);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT{
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private const uint SWP_SHOWWINDOW = 0x0040;
        private const int SWP_FRAMECHANGED = 0x0020;
        private const int SW_MAXIMIZE = 3;
        private const int GWL_STYLE = -16;
        private const int GWL_EXSTYLE = -20;
        private const int WS_OVERLAPPEDWINDOW = 0x00CF0000;
        private const uint WS_POPUP = 0x80000000;
        private const int WS_EX_TOPMOST = 0x00000008;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int SW_RESTORE = 9;
        private const uint PROCESS_QUERY_INFORMATION = 0x0400;

        public static bool IsAdministrator(){
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static void CheckVersion(){
            try{
                string url = "https://raw.githubusercontent.com/PandaHexCode/TheIdkTool/master/VersionIndex?token=GHSAT0AAAAAACKQHTA7HASQYGXGRRAYBL4MZK2MCCA";
                using (WebClient client = new WebClient()){
                    string version = client.DownloadString(url);

                    version = version.Trim();

                    float versionFloat = StringToFloat(version);
                    if(versionFloat > MainWindow.currentVersion)
                        DrawUtilRender.AddDrawUtil(new WarningDialog(), "Your version is outdated.\nCheckout the newest release on https://github.com/PandaHexCode/TheIdkTool");
                }
            }catch (Exception ex){
                DrawUtilRender.AddDrawUtil(new WarningDialog(), "Something went wrong.\n" + ex.Message);
            }
        }

        public static RawImage GetRawImage(string path){
            if (!File.Exists(path))
                throw new FileNotFoundException(path + " not found.");

            using (var image2 = Image.Load<Rgba32>(path)){
                byte[] pixelData = new byte[image2.Width * image2.Height * 4];

                image2.CopyPixelDataTo(pixelData);

                Memory<byte> memory = new Memory<byte>(pixelData);
                RawImage image = new RawImage(553, 492, memory);
                return image;
            }
        }

        public static ReadOnlySpan<RawImage> GetRawimageSpan(string path){
            if (!File.Exists(path))
                throw new FileNotFoundException(path + " not found.");
            using (var image2 = Image.Load<Rgba32>(path)){
                byte[] pixelData = new byte[image2.Width * image2.Height * 4];

                image2.CopyPixelDataTo(pixelData);

                Memory<byte> memory = new Memory<byte>(pixelData);
                RawImage image = new RawImage(553, 492, memory);
                var imageSpan = new ReadOnlySpan<RawImage>(new[] { image });
                return imageSpan;
            }
        }

        public static void SelectFolderButton(ref string input, string text, int id = 0){
            ImGui.SameLine();
            if (ImGui.Button("..##" + id)){
                string output = SelectFolder(input);
                if (output == string.Empty)
                    output = input;
                input = output;
            }
            ImGui.SameLine();
            ImGui.Text(text);
        }

        public static void ToggleLockTaskbar(bool state){
            const string keyName = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
            const string valueName = "TaskbarSizeMove";

            try{
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true)){
                    if (key != null){
                        int currentValue = (int)key.GetValue(valueName);

                        if (state){
                            key.SetValue(valueName, 0, RegistryValueKind.DWord);
                        }else{
                            key.SetValue(valueName, 1, RegistryValueKind.DWord);
                        }
                    }else{
                        Console.WriteLine("Registry key not found.");
                    }
                }
            }catch (Exception ex){
                Console.WriteLine("Error toggling taskbar lock: " + ex.Message);
            }
        }

        public static void ToggleAutomaticallyHideTaskbar(bool state){
            const string keyName = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StuckRects3";
            const string valueName = "Settings";

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true))
                {
                    if (key != null)
                    {
                        byte[] settings = (byte[])key.GetValue(valueName);

                        // Bit position 24 represents the "Automatically hide the taskbar in desktop mode" option
                        if (state)
                        {
                            // Set bit 24 to 1 to enable automatic hiding
                            settings[24] |= 0x02;
                        }
                        else
                        {
                            // Set bit 24 to 0 to disable automatic hiding
                            settings[24] &= unchecked((byte)~0x02);
                        }

                        key.SetValue(valueName, settings, RegistryValueKind.Binary);
                    }
                    else
                    {
                        Console.WriteLine("Registry key not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error toggling automatic hiding of taskbar in desktop mode: " + ex.Message);
            }
        }

        public static string SelectFolder(string trySetDefault){
            FolderBrowserEx.FolderBrowserDialog folderBrowserDialog = new FolderBrowserEx.FolderBrowserDialog();
            folderBrowserDialog.Title = "Select a folder";
            if (Directory.Exists(trySetDefault))
                folderBrowserDialog.InitialFolder = trySetDefault;
            else
                folderBrowserDialog.InitialFolder = @"C:\";
            folderBrowserDialog.AllowMultiSelect = false;
            string result = string.Empty;
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK){
                result += folderBrowserDialog.SelectedFolder;
                return result;
            }else
                return string.Empty;
        }

        public static void SelectFileButton(ref string input, string text){
            ImGui.SameLine();
            if (ImGui.Button("..##4")){
                string temp = input;
                Thread fileSelectThread = new Thread(() => SelectFile(ref temp, temp));
                fileSelectThread.SetApartmentState(ApartmentState.STA);//Because OpenFileDialog freezes without being in a STA
                fileSelectThread.Start();
                fileSelectThread.Join();
                if (temp == string.Empty)
                    temp = input;
                input = temp;
            }
            ImGui.SameLine();
            ImGui.Text(text);
        }


        public static void SelectFile(ref string output, string trySetDefault){
            string selectedFilePath = string.Empty;
            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog()){
                openFileDialog.Filter = "|*.*";
                openFileDialog.ShowHiddenFiles = true;
                if (Directory.Exists(Path.GetDirectoryName(trySetDefault)))
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(trySetDefault);
                else
                    openFileDialog.InitialDirectory = @"C:\";

                if (openFileDialog.ShowDialog() == DialogResult.OK){
                    selectedFilePath = openFileDialog.FileName;
                }
            }
            output = selectedFilePath;
        }

        public static void AddToStartup(string appName, string appPath)
        {
            try
            {
                string startupFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), appName + ".url");

                using (StreamWriter writer = new StreamWriter(startupFolderPath))
                {
                    writer.WriteLine("[InternetShortcut]");
                    writer.WriteLine("URL=file:///" + appPath.Replace('\\', '/'));
                    writer.WriteLine("IconIndex=0");
                    writer.WriteLine("IconFile=" + appPath.Replace('\\', '/'));
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                DrawUtilRender.AddDrawUtil(new WarningDialog(), "Something went wrong.\n");
            }
        }

        public static void CheckMinWindowSize(int width, int height)
        {
            if (ImGui.GetWindowWidth() < width)
                ImGui.SetWindowSize(new System.Numerics.Vector2(width, ImGui.GetWindowHeight()));
            else if (ImGui.GetWindowHeight() < height)
                ImGui.SetWindowSize(new System.Numerics.Vector2(ImGui.GetWindowHeight(), height));
        }

        public static void DebugWindowHelper()
        {
            ImGui.Text(ImGui.GetWindowWidth() + " " + ImGui.GetWindowHeight());
        }

        public static void Tooltip(string tooltip)
        {
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.SetTooltip(tooltip);
                ImGui.EndTooltip();
            }
        }

        public static void RemoveFromStartup(string appName)
        {
            try
            {
                string startupFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), appName + ".url");

                if (File.Exists(startupFolderPath))
                {
                    File.Delete(startupFolderPath);
                }
            }
            catch (Exception ex)
            {
                DrawUtilRender.AddDrawUtil(new WarningDialog(), "Something went wrong.\n");
            }
        }

        public static void ChangePositionInList<T>(List<T> list, T obj, int newPosition){
            if (list.Contains(obj)){
                list.Remove(obj);

                list.Insert(newPosition, obj);
            }else{
            }
        }

        public static bool IsInStartup(string appName){
            string startupFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), appName + ".url");

            return File.Exists(startupFolderPath);
        }

        public static Vector4 HexToVector4(string hex){
            if (hex.StartsWith("#")){
                hex = hex.Substring(1);
            }

            if (int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out int colorValue)){
                byte red = (byte)((colorValue >> 16) & 0xFF);
                byte green = (byte)((colorValue >> 8) & 0xFF);
                byte blue = (byte)(colorValue & 0xFF);
                byte alpha = 255;

                return new Vector4(red / 255.0f, green / 255.0f, blue / 255.0f, alpha / 255.0f);
            }else{
                Console.WriteLine("Invalid hex: " + hex);
                return Vector4.One;
            }
        }

        public static void TryToKillProcess(string processName){
            Process[] prc = Process.GetProcessesByName(processName);
            foreach (Process process in prc)
                TryToKillProcess(process);
        }

        public static void TryToKillProcess(Process process){
            try{
                process.Kill();
            }catch (Exception ex){
                try{
                    int HWND = FindWindow(null, process.MainWindowTitle);

                    SendMessage(HWND, WM_SYSCOMMAND, SC_CLOSE, 0);
                }catch (Exception exp){
                    Console.WriteLine("Can't kill " + process.ProcessName + "!");
                    Console.WriteLine(exp.Message);
                    Console.WriteLine(exp.StackTrace);
                }
            }
        }

        public static string GetFileIn(string path){
            if (File.Exists(path)){
                StreamReader readStm2 = new StreamReader(path);
                string fileIn2 = readStm2.ReadToEnd();
                readStm2.Close();

                return fileIn2;
            }else
                return string.Empty;
        }

        public static string[] GetMusicFilesFromDirectory(string directory){
            string[] musicFiles = Directory.GetFiles(directory, "*.*")
                    .Where(file => file.ToLower().EndsWith(".mp3") ||
                                   file.ToLower().EndsWith(".wav") ||
                                   file.ToLower().EndsWith(".m4a"))
                    .ToArray();
            return musicFiles;
        }
        public static void EncryptFile(string filePath, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(key);
                aesAlg.Mode = CipherMode.CFB;

                aesAlg.GenerateIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                byte[] fileContent = File.ReadAllBytes(filePath);

                byte[] encryptedContent;
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(fileContent, 0, fileContent.Length);
                    }
                    encryptedContent = msEncrypt.ToArray();
                }

                byte[] result = new byte[aesAlg.IV.Length + encryptedContent.Length];
                Buffer.BlockCopy(aesAlg.IV, 0, result, 0, aesAlg.IV.Length);
                Buffer.BlockCopy(encryptedContent, 0, result, aesAlg.IV.Length, encryptedContent.Length);

                File.WriteAllBytes(filePath, result);
            }
        }

        public static void DecryptFile(string filePath, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(key);
                aesAlg.Mode = CipherMode.CFB;

                byte[] encryptedData = File.ReadAllBytes(filePath);

                byte[] iv = new byte[aesAlg.IV.Length];
                Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv);

                byte[] decryptedContent;
                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(encryptedData, iv.Length, encryptedData.Length - iv.Length);
                    }
                    decryptedContent = msDecrypt.ToArray();
                }

                File.WriteAllBytes(filePath, decryptedContent);
            }
        }

        public static string GetKey()
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.GenerateKey(); 

                if (aesAlg.Key.Length != 16 && aesAlg.Key.Length != 24 && aesAlg.Key.Length != 32)
                {
                    throw new InvalidOperationException("");
                }

                string base64Key = Convert.ToBase64String(aesAlg.Key);

                return base64Key;
            }
        }

        public static string EncryptString(string plainText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(key);
                aesAlg.Mode = CipherMode.CFB;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    byte[] iv = aesAlg.IV;
                    byte[] encryptedText = msEncrypt.ToArray();

                    byte[] result = new byte[iv.Length + encryptedText.Length];
                    Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                    Buffer.BlockCopy(encryptedText, 0, result, iv.Length, encryptedText.Length);

                    return Convert.ToBase64String(result);
                }
            }
        }

        public static string DecryptString(string cipherText, string key)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(key);
                aesAlg.Mode = CipherMode.CFB;
                aesAlg.IV = fullCipher.Take(aesAlg.BlockSize / 8).ToArray();

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(fullCipher.Skip(aesAlg.BlockSize / 8).ToArray()))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

       public static int GetProcessIdFromFilePath(string fileName){
            try{
                foreach (DriveInfo drive in DriveInfo.GetDrives()){
                    if (drive.IsReady){
                        string[] directories = Directory.GetDirectories(drive.RootDirectory.FullName, "*", SearchOption.AllDirectories);
                        foreach (string directory in directories){
                            try{
                                int processId = RetrieveProcessId(fileName, directory);
                                if (processId != 0)
                                    return processId;
                            }
                            catch (UnauthorizedAccessException) { }
                        }
                    }
                }
            }catch (Exception ex){
            }
            return 0;
        }

        public static int RetrieveProcessId(string fileName, string directory){
            var query = string.Format("SELECT * FROM CIM_DataFile WHERE Name = '{0}'", Path.Combine(directory, fileName).Replace("'", "''"));

            var searcher = new ManagementObjectSearcher(query);
            var results = searcher.Get();

            foreach (var result in results){
                var managementObject = (ManagementObject)result;
                var processId = managementObject["ProcessId"];
                if (processId != null)
                    return Convert.ToInt32(processId);
            }

            return 0;
        }


        public static string[] GetEveryFileName(string path){
            string[] files = Directory.GetFiles(path);

            string[] subDirectories = Directory.GetDirectories(path);

            var allFileNames = new List<string>();

            allFileNames.AddRange(files);

            foreach (string subDir in subDirectories){
                allFileNames.AddRange(GetEveryFileName(subDir));
            }

            return allFileNames.ToArray();
        }

        public static void SaveFile(string path, string content){
            if (File.Exists(path))
                File.Delete(path);

            StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8);
            sw.Write(content);
            sw.Close();
        }

        public static void ForceFullScreen(Process proc){
            IntPtr handle = proc.MainWindowHandle;
            RECT rect;
            GetWindowRect(handle, out rect);

            Screen screen = Screen.AllScreens[MainWindow.currentSelectedScreen];

            long style = GetWindowLong(handle, GWL_STYLE);
            long exStyle = GetWindowLong(handle, GWL_EXSTYLE);

            style &= ~(WS_OVERLAPPEDWINDOW | WS_POPUP);
            exStyle &= ~(WS_EX_TOOLWINDOW);
            exStyle |= WS_EX_TOPMOST;

            SetWindowLong(handle, GWL_STYLE, style);
            SetWindowLong(handle, GWL_EXSTYLE, exStyle);

            SetWindowPos(handle, IntPtr.Zero, screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height, SWP_SHOWWINDOW | SWP_FRAMECHANGED);
        }

        public static void ReverseFullScreen(Process proc){
            IntPtr handle = proc.MainWindowHandle;

            Screen screen = Screen.AllScreens[MainWindow.currentSelectedScreen];

            long style = GetWindowLong(handle, GWL_STYLE);
            long exStyle = GetWindowLong(handle, GWL_EXSTYLE);

            style |= WS_OVERLAPPEDWINDOW;
            style &= ~WS_POPUP;
            exStyle &= ~WS_EX_TOPMOST;

            SetWindowLong(handle, GWL_STYLE, style);
            SetWindowLong(handle, GWL_EXSTYLE, exStyle);

            SetWindowPos(handle, IntPtr.Zero, screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width / 2, screen.Bounds.Height / 2, SWP_SHOWWINDOW | SWP_FRAMECHANGED);
        }

        public static void ForceProcessToForeground(Process process){
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            IntPtr mainWindowHandle = process.MainWindowHandle;

            if (mainWindowHandle != IntPtr.Zero){
                ShowWindow(mainWindowHandle, SW_RESTORE);
                SetForegroundWindow(mainWindowHandle);
            }
        }

        public static void ForceCursorStateInProcess(Process process, bool state){
            IntPtr mainWindowHandle = process.MainWindowHandle;
            uint processId;
            GetWindowThreadProcessId(mainWindowHandle, out processId);

            IntPtr hProcess = OpenProcess(PROCESS_QUERY_INFORMATION, false, processId);
            if (hProcess != IntPtr.Zero){
                try{
                    ShowCursor(state);
                }finally{
                    CloseHandle(hProcess);
                }
            }else{
                throw new InvalidOperationException("Failed to open process.");
            }
        }

        public static Process StartProcess(string path, string[] ?args = null){
            try{
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = path;
                if (args != null && args.Length > 0)
                    startInfo.Arguments = string.Join(" ", args);
                Process process = Process.Start(startInfo);
                return process;
            }catch (Exception ex){
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static void TryToKillByName(string name){
            foreach (Process process in Process.GetProcesses()){
                if (process.ProcessName.StartsWith(name))
                    TryToKillProcess(process);
            }
        }

        public static float StringToFloat(string str){
            float fl = 0f;
            float.TryParse(str, out fl);
            return fl;
        }

        public static int StringToInt(string str){
            int i = 0;
            int.TryParse(str, out i);
            return i;
        }

        public static bool StringToBool(string str){
            bool b = false;
            bool.TryParse(str, out b);
            return b;
        }

    }

}