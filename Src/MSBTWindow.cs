using ImGuiNET;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using MSBTRando.Dialogs;
using CLMS;

namespace MSBTRando.Windows{

    public class MSBTWindow : DrawWindow{

        public List<MSBTPyFileEdit> edits = new List<MSBTPyFileEdit>();

        public string endOutput = "en";

        public string outputFolderPath = string.Empty;

        public bool autoStart = false;

        public override void Draw(){
            ImGui.InputText("##p2", ref inputRefs[1], 1000);
            Manager.SelectFolderButton(ref inputRefs[1], "Language folder");
            ImGui.InputText("End output language code", ref this.endOutput, 10);

            if (string.IsNullOrEmpty(this.outputFolderPath))
                this.outputFolderPath = Environment.ProcessPath.Replace("MSBTRando.exe", "\\output\\");

            if (ImGui.Button("Start"))
                StartFolder();

            ImGui.SameLine();

            if (ImGui.Button("Kill")){
                Manager.TryToKillByName("MsbtGoogleTranslate");
                this.edits.Clear();
            }

            Manager.Tooltip("Kill the translator if it is still open somehow in the background.");

            foreach(MSBTPyFileEdit edit in this.edits){
                try{
                    if (!edit.isStarted){
                        ImGui.TextWrapped(Path.GetFileNameWithoutExtension(edit.file));
                        ImGui.SameLine();
                        if (ImGui.Button("Start##" + edit.refName))
                            edit.Start(edit.file);
                        continue;
                    }
                    ImGui.TextWrapped("Watching for " + edit.finishFilePath);
                    ImGui.TextWrapped(Manager.GetFileIn(edit.progressPath));
                    if (File.Exists(edit.finishFilePath)){
                        edit.Save(edit.finishFilePath, this.inputRefs[1] + "\\" + edit.refName);
                        edit.finishFilePath = string.Empty;
                    }
                }catch(Exception ex){
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }
        }

        public void StartFolder(){
            foreach(string file in Directory.GetFiles(this.inputRefs[1])){
                try{
                    MSBTPyFileEdit edit = new MSBTPyFileEdit(this, file);
                    this.edits.Add(edit);
                    if (this.autoStart)
                        edit.Start(file);
                }catch(Exception ex){
                    Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    continue;
                }
            }
        }

    }
    public class MSBTPyFileEdit{

        public bool isStarted = false;

        public string finishFilePath = string.Empty;
        public string progressPath = string.Empty;
        public string refName = string.Empty;
        public MSBTWindow window;

        public string file;

        public MSBTPyFileEdit(MSBTWindow window, string refName){
            this.window = window;
            this.file = refName;
            this.refName = Path.GetFileName(refName);
        }

        public void Start(string path){
            this.isStarted = true;

            string content = string.Empty;

            var msbt = new MSBT(File.OpenRead(path), false);
            foreach (Message message in msbt.Messages.Values){
                 content = content + message.Text + "##!#";
            }

            string tempFilePath = "Temp_" + Path.GetFileNameWithoutExtension(path);
            Manager.SaveFile(tempFilePath, content);

            string msbtTransPyPath = Environment.ProcessPath.Replace("MSBTRando.exe", "\\resources\\MsbtGoogleTranslate.exe");
            this.finishFilePath = tempFilePath.Replace("Temp", "Finish");
            this.progressPath = tempFilePath.Replace("Temp", "Progress");
            if (File.Exists(this.finishFilePath))
                File.Delete(this.finishFilePath);
            if (File.Exists(this.progressPath))
                File.Delete(this.progressPath);
            Manager.StartProcess(msbtTransPyPath, new string[5] { "", tempFilePath, this.finishFilePath, this.window.endOutput, this.progressPath });
        }

       public void Save(string path, string refMsbtPath){
            string[] lines = Manager.GetFileIn(path).Split("##!#");

            int i = 0;
            var msbt = new MSBT(File.OpenRead(refMsbtPath), false);
            foreach (Message message in msbt.Messages.Values){
                try{
                    message.Text = lines[i];
                }catch(IndexOutOfRangeException ex){
                    break;
                }catch(Exception ex){
                    Console.WriteLine(ex.Message + ex.StackTrace);
                    i++;
                    continue;
                }
                i++;
            }

            try{
                string outPath = this.window.outputFolderPath + this.refName;
                if (File.Exists(outPath))
                    File.Delete(outPath);
                File.WriteAllBytes(outPath, msbt.Save());
                Console.WriteLine("Saved to " + outPath);
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
        }

    }

}