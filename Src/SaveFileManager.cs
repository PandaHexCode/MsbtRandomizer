﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MSBTRando.Dialogs;
using MSBTRando.Windows;

namespace MSBTRando{

    public class SaveFileManager{
        
        public static string dirPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TheIdkTool";
        public static string todosFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
         "\\TheIdkTool\\Todos.dat";

        public static void SaveFiles(){
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            string main = string.Empty;
            main = MainWindow.showAdvancedButtons.ToString() + "\n" + MainWindow.currentSelectedScreen;
            Manager.SaveFile(dirPath + "\\Main.dat", main);

            foreach(DrawWindow window in WindowManager.drawWindows){
                string output = window.showWindow.ToString() + "\n";
                foreach(string input in window.inputRefs)
                    output = output + input + "\n";
                Manager.SaveFile(dirPath + "\\" + window.name + ".dat", output);
            }
        }

        public static void LoadFiles(){
            if (!File.Exists(dirPath + "\\Main.dat"))
                return;

            try{
                string[] lines = Manager.GetFileIn(dirPath + "\\Main.dat").Split('\n');
                MainWindow.showAdvancedButtons = Manager.StringToBool(lines[0]);
                MainWindow.currentSelectedScreen = Manager.StringToInt(lines[1]);
                foreach (DrawWindow window in WindowManager.drawWindows){
                    try{ 
                        string path = dirPath + "\\" + window.name + ".dat";
                        if (!File.Exists(path)){
                            Console.WriteLine("Can't find " + path + ".");
                            continue;
                        }
                        string[] lines2 = Manager.GetFileIn(path).Split('\n');
                        window.showWindow = Manager.StringToBool(lines2[0]);
                        for (int i = 0; i < lines2.Length; i++){
                            if (i > 0 && i - 1 < window.inputRefs.Length){
                                window.inputRefs[i - 1] = lines2[i];
                            }
                        }
                    }catch(Exception ex){
                        Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }catch (Exception ex){
                Console.WriteLine(ex.Message);
                DrawUtilRender.AddDrawUtil(new WarningDialog(), "Error loading settings.\n" + ex.Message);
            }
        }

    }

}
