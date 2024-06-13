using Silk.NET.Windowing;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using ImGuiNET;
using Color = System.Drawing.Color;
using Silk.NET.Maths;
using MSBTRando.Dialogs;
using System.Numerics;
using System.Windows.Forms;

namespace MSBTRando.Windows{
    public class MainWindow{

        public static GL gl = null;

        private Vector4[] colors = new Vector4[10];

        public static float currentVersion = 1.1f;
        public static bool showAdvancedButtons = false;
        public static int currentSelectedScreen = 0;

        public void CalculateColors() {
            colors[0] = Manager.HexToVector4("403037");
            colors[1] = Manager.HexToVector4("AE8F9A");
            colors[2] = Manager.HexToVector4("664F5C");
            colors[3] = Manager.HexToVector4("DBB7C2");
            colors[4] = Manager.HexToVector4("DDA49C");
            colors[5] = Manager.HexToVector4("C27B7F");
            colors[6] = Manager.HexToVector4("B4666F");
            colors[7] = Manager.HexToVector4("79B386");
            colors[8] = Manager.HexToVector4("000000");
        }

        public MainWindow(){
            using var window = Silk.NET.Windowing.Window.Create(WindowOptions.Default);

            ImGuiController controller = null;
            IInputContext inputContext = null;

            window.Load += () =>{
                controller = new ImGuiController(
                    gl = window.CreateOpenGL(),//load OpenGL
                    window,
                    inputContext = window.CreateInput()
                );

                if (window.Native!.Win32.HasValue)
                    WindowsDarkmodeUtil.SetDarkmodeAware(window.Native.Win32.Value.Hwnd);

                window.Size = new Vector2D<int>(1500, 800);

                try{
                    window.SetWindowIcon(Manager.GetRawimageSpan(Environment.ProcessPath.Replace("MSBTRando.exe", "\\resources\\logo.png")));           
                }catch(Exception ex){
                    DrawUtilRender.AddDrawUtil(new WarningDialog(), "Something went wrong.\n" + ex.Message);
                }
            };

            //Handle resizes
            window.FramebufferResize += s =>{
                gl.Viewport(s);
            };

            CalculateColors();
            Manager.CheckVersion();
            WindowManager.InitWindows();
            SaveFileManager.LoadFiles();

            window.Render += delta =>{
                controller.Update((float)delta);

                gl.ClearColor(Color.FromArgb(255, (int)(.45f * 255), (int)(.55f * 255), (int)(.60f * 255)));
                gl.Clear((uint)ClearBufferMask.ColorBufferBit);

                ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
                ImGui.DockSpaceOverViewport();

                // ImGui.PushStyleColor(ImGuiCol.TitleBg, new Vector4(0.2f, 0.2f, 0.8f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.TitleBgActive, colors[2]);

                ImGui.PushStyleColor(ImGuiCol.Button, colors[0]);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, colors[1]);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, colors[3]);

                ImGui.PushStyleColor(ImGuiCol.ResizeGrip, colors[2]);
                ImGui.PushStyleColor(ImGuiCol.ResizeGripHovered, colors[1]);
                ImGui.PushStyleColor(ImGuiCol.ResizeGripActive, colors[3]);

                ImGui.PushStyleColor(ImGuiCol.FrameBg, colors[6]);
                ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, colors[5]);
                ImGui.PushStyleColor(ImGuiCol.FrameBgActive, colors[4]);
               
                ImGui.PushStyleColor(ImGuiCol.CheckMark, colors[7]);

                ImGui.PushStyleColor(ImGuiCol.TextSelectedBg, colors[1]);

                ImGui.PushStyleColor(ImGuiCol.Tab, colors[2]);
                ImGui.PushStyleColor(ImGuiCol.TabHovered, colors[0]);
                ImGui.PushStyleColor(ImGuiCol.TabActive, colors[0]);

               // ImGui.PushStyleColor(ImGuiCol., colors[7]);

                DrawMainMenuBar();
                WindowManager.Draw();

                controller.Render();
            };

            window.Closing += () => {
                controller?.Dispose();

                inputContext?.Dispose();

                gl?.Dispose();

                SaveFileManager.SaveFiles();
            };
    
            window.Run();

            window.Dispose();
        }

        public bool[] checkBoxReferences = new bool[1] { false } ;
        public void DrawMainMenuBar(){
            if (ImGui.BeginMainMenuBar()){
                if (ImGui.BeginMenu("View")){
                    WindowManager.DrawCheckbox();
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Settings")){
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Other")){
                    if (ImGui.MenuItem("Created by PandaHexCode/Nagisa")){
                        DrawUtilRender.AddDrawUtil(new WarningDialog(), "This is an easter egg woooooh idk?\n");
                    }
                    if (ImGui.MenuItem("Close program"))
                        Environment.Exit(1);
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }
        }

    }
}