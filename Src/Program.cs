using System.Diagnostics;
using MSBTRando;
using MSBTRando.Windows;


try{
    new MainWindow();
}catch (Exception ex){
    string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
    string filename = $"CrashLog_{timestamp}.txt";
    string logContent = $"Error Message: {ex.Message}\nStack Trace: {ex.StackTrace}";

    File.WriteAllText(filename, logContent);
}
