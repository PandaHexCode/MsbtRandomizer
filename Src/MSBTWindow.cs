using ImGuiNET;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using MSBTRando.Dialogs;
using CLMS;
using System.Globalization;

namespace MSBTRando.Windows{

    public class MSBTWindow : DrawWindow{

        public List<MSBTPyFileEdit> edits = new List<MSBTPyFileEdit>();

        public string endOutput = "en";

        public string outputFolderPath = string.Empty;

        public bool autoStart = false;

        public int translatationsCount = 5;

        public int currentlyInProgress = 0;

        public List<MSBTPyFileEdit> editNext = new List<MSBTPyFileEdit>();

        public bool onlyConvert = false;

        public bool removeNonLatinCharacters = true;

        public override void Draw(){
            ImGui.InputText("##p2", ref inputRefs[1], 1000);
            Manager.SelectFolderButton(ref inputRefs[1], "Language folder");
            ImGui.SetNextItemWidth(25);
            ImGui.InputText("End output language code", ref this.endOutput, 10);

            ImGui.SameLine();
            ImGui.SetNextItemWidth(65);
            ImGui.InputInt("Translations count", ref this.translatationsCount);
            Manager.Tooltip("How often a message get randomly translated, can affect masive the progress duration.");

            if (this.translatationsCount < 0)
                this.translatationsCount = 0;
            if (this.translatationsCount > 10)
                this.translatationsCount = 10;


            ImGui.SameLine();
            ImGui.Checkbox("Remove non latin characters", ref this.removeNonLatinCharacters);
            Manager.Tooltip("*Only* disable that if your end ouput language uses an other font (Like asia,arabia...)\n" +
                "If not dont disable it it can causes crashes!");


            ImGui.SameLine();
            ImGui.Checkbox("Only convert", ref this.onlyConvert);
            Manager.Tooltip("Enable this if you have already the Finish_x files and just want it to convert\n" +
                "it with a newer version of the program.");

            if (this.onlyConvert){
                ImGui.SameLine();
                ImGui.Checkbox("Auto start", ref autoStart);
            }else
                this.autoStart = false;

            if (string.IsNullOrEmpty(this.outputFolderPath))
                this.outputFolderPath = Environment.ProcessPath.Replace("MSBTRando.exe", "\\output\\");

            if (ImGui.Button("Open##55"))
                StartFolder();

            ImGui.SameLine();

            if (ImGui.Button("Kill")){
                Manager.TryToKillByName("MsbtGoogleTranslate");
                this.edits.Clear();
                this.currentlyInProgress = 0;
            }
            Manager.Tooltip("Kill the translator if it is still open somehow in the background.");

            if(this.edits.Count > 0){
                ImGui.SameLine();
                if(ImGui.Button("Start from order"))
                    StartFromOrder();
                Manager.Tooltip("Auto start files from order + give them to edit as next");

                ImGui.SameLine();
                if(ImGui.Button("Smart start from order"))
                    SmartStartFromOrder();
                Manager.Tooltip("Auto start files from order + give them to edit as next\n" +
                    "Starts the important & big files first\n" +
                    "Need to select a game from settings \nWithout finding the right files it's just starts from order.");
            }

            if (this.currentlyInProgress > 0){
                ImGui.Spacing();
                ImGui.Text($"Currently in edit: {this.currentlyInProgress}");
                if (this.editNext.Count > 0){
                    ImGui.SameLine();
                    ImGui.Text($"Currently waiting: {this.editNext.Count}");
                }
                ImGui.Spacing();
            }

            foreach (MSBTPyFileEdit edit in this.edits){
                try{
                    ImGui.Spacing();
                    if (File.Exists(this.outputFolderPath + edit.refName)){
                        ImGui.TextWrapped("[Already exits]");
                        ImGui.SameLine();
                    }
                    if (!edit.isStarted){
                        ImGui.TextWrapped(Path.GetFileNameWithoutExtension(edit.file));
                        CheckTooltips(Path.GetFileNameWithoutExtension(edit.file));
                        ImGui.SameLine();
                        if (ImGui.Button("Start##" + edit.refName))
                            edit.Start();
                        ImGui.SameLine();
                        if (this.editNext.Contains(edit)) {
                            if (ImGui.Button("Remove start as next##" + edit.refName))
                                this.editNext.Remove(edit);
                        }else {
                            if (ImGui.Button("Start as next##" + edit.refName))
                                this.editNext.Add(edit);
                        }
                        continue;
                    }
                    ImGui.SameLine();
                    ImGui.TextWrapped("Watching for " + edit.finishFilePath);
                    ImGui.TextWrapped(Manager.GetFileIn(edit.progressPath));
                    if (File.Exists(edit.finishFilePath)){
                        edit.Save(edit.finishFilePath, this.inputRefs[1] + "\\" + edit.refName);
                        edit.finishFilePath = string.Empty;
                    }
                }catch(Exception ex){
                    Console.WriteLine(ex.Message + ex.StackTrace);
                    continue;
                }
            }
        }

        public void StartFolder(){
            foreach(string file in Directory.GetFiles(this.inputRefs[1])){
                try{
                    if (!file.EndsWith(".msbt"))
                        continue;
                    MSBTPyFileEdit edit = new MSBTPyFileEdit(this, file);
                    this.edits.Add(edit);
                    if (this.autoStart && !File.Exists(this.outputFolderPath + Path.GetFileName(file)))
                        edit.Start();
                }catch(Exception ex){
                    Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    continue;
                }
            }
        }

        public void StartFromOrder(int count = 14){
            if (this.edits.Count < 13)
                count = this.edits.Count - 1;

            for (int i = 0; i < count; i++){
                if (!this.edits[i].isStarted)
                    this.edits[i].Start();
                else
                    count++;
            }

            foreach (MSBTPyFileEdit edit in this.edits){
                if (!edit.isStarted)
                    this.editNext.Add(edit);
            }
        }

        public void SmartStartFromOrder(){
            if(MainWindow.game == MainWindow.Game.PaperMarioRemake){
                StartIfFileExits("gor");
                StartIfFileExits("tou");
                StartIfFileExits("global");
                StartIfFileExits("stage_global");
                StartIfFileExits("ui");
                StartIfFileExits("shop");
                StartIfFileExits("hint_enemy");
                StartIfFileExits("hint_npc");
                StartIfFileExits("hint_party");
                StartIfFileExits("gor_shop");
                StartIfFileExits("field_tutorial");
                StartIfFileExits("item");
                StartIfFileExits("peach");
            }else 
                return;/*TODO*/

            foreach (MSBTPyFileEdit edit in this.edits){
                if (!edit.isStarted)
                    this.editNext.Add(edit);
            }
        }

        private readonly Dictionary<string, string> _stageTooltipsPM2 = new Dictionary<string, string>{
        { "aaa", "Mario's House" },
        { "aji", "X-Naut Fortress" },
        { "bom", "Fahr Outpost" },
        { "bti", "Battle Stages" },
        { "dou", "Pirates Grotto" },
        { "eki", "Riverside Station" },
        { "end", "End Credits" },
        { "gon", "Hooktail Castle" },
        { "gor", "Rogueport" },
        { "gra", "Twilight Trail" },
        { "hei", "Petal Meadows" },
        { "hom", "Excess Express cutscenes, etc." },
        { "jin", "Creepy Steeple" },
        { "jon", "Pit of 100 Trials" },
        { "kpa", "Bowser Missions" },
        { "las", "Palace of Shadow" },
        { "moo", "The Moon" },
        { "mri", "The Great Tree" },
        { "muj", "Keelhaul Key" },
        { "nok", "Petalburg" },
        { "pik", "Poshley Heights" },
        { "rsh", "Excess Express" },
        { "stg", "Backgrounds" },
        { "dmo", "Opening / Intro" },
        { "tik", "Rogueport Sewers" },
        { "tou", "Glitzville" },
        { "usu", "Twilight Town" },
        { "win", "Boggly Woods" },
        { "yuu", "Pianta Parlor Minigames" }
        };

        public void CheckTooltips(string fileName){/*Thanks to https://docs.google.com/document/d/1y6c46fNJ6jesd9as-yB2K4eGmk-WATaGxFX6Os4NweM/edit*/
            foreach (var stageTooltip in this._stageTooltipsPM2)
                CheckStageTooltip(fileName, stageTooltip.Key, stageTooltip.Value);
        }

        public void CheckStageTooltip(string fileName, string checkName, string stageName){
            if (fileName.StartsWith(checkName))
                Manager.Tooltip(stageName);
        }

        public MSBTPyFileEdit GetEditFromFileName(string name){
            foreach(MSBTPyFileEdit edit in this.edits){
                if (Path.GetFileNameWithoutExtension(edit.file).Equals(name, StringComparison.OrdinalIgnoreCase))
                    return edit;
            }

            return null;
        }
        
        public void StartIfFileExits(string name){
            MSBTPyFileEdit edit = GetEditFromFileName(name);
            if (edit != null)
                edit.Start();
            else
                Console.WriteLine($"Can't find edit {name}.");
        }

        public static int BrokenTagIdFixer(string tag){
            Match match = Regex.Match(tag, @"\d+");
            if (match.Success){
                int val = int.Parse(match.Value);
                return val;
            }else{
                Console.WriteLine("Not a tag found " + tag);
                return 0;
            }
        }

        private static string RemoveNonLatinCharacters(string input){
            StringBuilder sb = new StringBuilder();
            foreach (char c in input){
                if (IsLatinLetterOrAllowedSymbol(c)){
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static bool IsLatinLetterOrAllowedSymbol(char c){
            UnicodeCategory category = Char.GetUnicodeCategory(c);
            return (category == UnicodeCategory.UppercaseLetter && c <= 'Z') ||
                   (category == UnicodeCategory.LowercaseLetter && c <= 'z') ||
                   (category == UnicodeCategory.DecimalDigitNumber) ||
                   (c == ' ' || c == '\n' || c == '\r' || Char.IsPunctuation(c) ||
                    c == '<' || c == '>' || c == '\\');
        }

        public string LineFixer(string line){
            string pattern = @"<[^>]+>";

            string replaced = Regex.Replace(line, pattern, match => {
                return $"<Tag_1>";
            });
            line = replaced;

            if (this.removeNonLatinCharacters)
                line = RemoveNonLatinCharacters(line);

            return line;
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

        public void Start(){
            string path = this.file;
            string tempFilePath = "Temp_" + Path.GetFileNameWithoutExtension(path);

            this.isStarted = true;
            if(this.window.edits.Contains(this))
                this.window.editNext.Remove(this);

            if (this.window.onlyConvert){
                this.finishFilePath = tempFilePath.Replace("Temp", "Finish");
                return;
            }
            this.window.currentlyInProgress++;

            string content = string.Empty;

            var msbt = new MSBT(File.OpenRead(path), false);
            foreach (Message message in msbt.Messages.Values){
                string lineContent = string.Empty;
                for (int i = 0; i < message.Contents.Count; i++){
                    if (message.Contents[i] is string){
                        if (string.IsNullOrEmpty(lineContent))
                            lineContent = (string)message.Contents[i];
                        else
                            lineContent = lineContent + "<Tag_1>" + message.Contents[i];
                    }
                }
                content = content + lineContent + "##!#";
            }

            msbt = null;

            Manager.SaveFile(tempFilePath, content);

            string msbtTransPyPath = Environment.ProcessPath.Replace("MSBTRando.exe", "\\resources\\MsbtGoogleTranslate.exe");
            this.finishFilePath = tempFilePath.Replace("Temp", "Finish");
            this.progressPath = tempFilePath.Replace("Temp", "Progress");
            if (File.Exists(this.finishFilePath))
                File.Delete(this.finishFilePath);
            if (File.Exists(this.progressPath))
                File.Delete(this.progressPath);
            Manager.StartProcess(msbtTransPyPath, new string[6] { "", tempFilePath, this.finishFilePath, this.window.endOutput, this.progressPath, this.window.translatationsCount.ToString() });
        }

       public void Save(string path, string refMsbtPath){
            if (!File.Exists(refMsbtPath)){
                Console.WriteLine("Can't find " + refMsbtPath);
                return;
            }

            var msbt = new MSBT(File.OpenRead(refMsbtPath), false);

            string[] lines = Manager.GetFileIn(path).Split("##!#");

            int m = 0;
            foreach (Message message in msbt.Messages.Values){
                try{
                    lines[m] = this.window.LineFixer(lines[m]);
                    string[] lineContent = lines[m].Split(new string[] { "<Tag_1>" }, StringSplitOptions.None);
                    int line = 0;
                    Console.WriteLine(lines[m]);
                    for (int i = 0; i < message.Contents.Count; i++){
                        if (message.Contents[i] is string){
                            msbt.Messages.Values.ElementAt(m).Contents[i] = lineContent[line];
                            line++;
                        }
                    }
                }catch(Exception ex){
                    Console.WriteLine(ex.Message + ex.StackTrace);
                    m++;
                    continue;
                }
                m++;
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
            this.isStarted = false;
            this.window.currentlyInProgress--;
            try{
                if (this.window.editNext.Count != 0){
                    this.window.editNext[0].Start();
                    this.window.editNext.RemoveAt(0);
                }
            }catch(Exception ex){
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }
    }

}
