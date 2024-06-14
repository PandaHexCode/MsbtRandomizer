using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBTRando.Windows{
    
    public class DrawWindow{

        public string name;
        public bool showWindow;
        public bool isAdvanced;
        public string[] inputRefs;
        public int minX, minY;
        public void StartDraw(){
            ImGui.Begin(name);
            Manager.CheckMinWindowSize(minX, minY);
            Draw();
        }

        public virtual void Draw(){
        }

    }

    public class WindowManager{

        public static List<DrawWindow> drawWindows = new List<DrawWindow>();

        public static void InitWindows(){
            InitWindow(new MSBTWindow(), "MSBTRando", 10, true, false, 455, 310);
        }

        public static void InitWindow(DrawWindow window, string name, int inputRefsSize, bool autoShow, bool isAdvanced, int minX, int minY){
            window.inputRefs = new string[inputRefsSize];
            for (int i = 0; i < window.inputRefs.Length; i++){
                if(window.inputRefs[i] == null)
                    window.inputRefs[i] = string.Empty;
            }
            window.isAdvanced = isAdvanced;
            window.name = name;
            window.showWindow = autoShow;
            window.minX = minX;
            window.minY = minY;
            WindowManager.drawWindows.Add(window);
        }

        public static void Draw(){
            foreach(DrawWindow window in WindowManager.drawWindows){
                if (window.showWindow){
                    window.StartDraw();
                }
            }

            DrawUtilRender.Draw();
        }

        public static void DrawCheckbox(){
            foreach (DrawWindow window in WindowManager.drawWindows){
                if(!window.isAdvanced | (window.isAdvanced && MainWindow.showAdvancedButtons))
                    ImGui.Checkbox(window.name, ref window.showWindow);
            }

            ImGui.Checkbox("Show advanced buttons", ref MainWindow.showAdvancedButtons);
        }

    }

}
