using ImGuiNET;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using MSBTRando.Dialogs;
using CLMS;
using System.Globalization;

namespace MSBTRando.Windows{

    public class MSBTWindowTestWindow : DrawWindow{

        public string outputFolderPath = string.Empty;

        public override void Draw(){
            ImGui.InputText("##p2", ref inputRefs[1], 1000);
            Manager.SelectFolderButton(ref inputRefs[1], "Language folder");

            if (string.IsNullOrEmpty(this.outputFolderPath))
                this.outputFolderPath = Environment.ProcessPath.Replace("MSBTRando.exe", "\\output\\test\\");

            if (ImGui.Button("Start##55"))
                StartFolder();
        }

        public void StartFolder(){
            foreach(string file in Directory.GetFiles(this.inputRefs[1])){
                try{
                    if (!file.EndsWith(".msbt"))
                        continue;
                    if (file.Contains("ui") | file.Contains("item"))
                        continue;
                    var msbt = new MSBT(File.OpenRead(file), false);

                    if (file.Contains("aaa"))
                        Console.WriteLine(msbt.ToYaml());

                    int m = 0;
                    foreach (Message message in msbt.Messages.Values){
                        try{
                            int line = 0;
                            Tag? tag = null;
                            TagEnd? tagEnd = null;

                            for (int i = 0; i < message.Contents.Count; i++){
                                if (message.Contents[i] is string){
                                    message.Contents[i] = string.Empty;
                                    try{
                                        msbt.Messages.Values.ElementAt(m).Contents[i] = string.Empty;
                                    }catch (Exception ex2){
                                        Console.WriteLine(ex2.Message);
                                        continue;
                                    }
                                    line++;
                                }else if(message.Contents[i] is Tag){
                                    if (tag == null)
                                        tag = (Tag)message.Contents[i];
                                }else if (message.Contents[i] is TagEnd){
                                    if (tagEnd == null)
                                        tagEnd = (TagEnd)message.Contents[i];
                                }
                            }

                            message.Contents.Clear();
                            if(tag != null)
                                message.Contents.Add(tag);
                            if (tagEnd != null)
                                message.Contents.Add(tagEnd);
                        }catch (Exception ex){
                            Console.WriteLine(ex.Message + ex.StackTrace);
                            m++;
                            continue;
                        }
                        m++;
                    }

                    try{
                        string outPath = this.outputFolderPath + Path.GetFileName(file);
                        if (File.Exists(outPath))
                            File.Delete(outPath);
                        File.WriteAllBytes(outPath, msbt.Save());
                        Console.WriteLine("Saved to " + outPath);
                    }catch (Exception ex){
                        Console.WriteLine(ex.Message);
                    }
                }catch(Exception ex){
                    Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    continue;
                }
            }
        }

    }

}