﻿using ImGuiNET;
using Silk.NET.Core.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
namespace MSBTRando.Dialogs{

    public class WarningDialog : DrawUtil{

        public override void Draw(object message){
            Vector2 center = ImGui.GetMainViewport().GetCenter();
            ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));

            if (doShow && !ImGui.IsPopupOpen("Warning")){
                Console.WriteLine((string)message);
                ImGui.OpenPopup("Warning");
            }

            if (ImGui.BeginPopupModal("Warning", ref doShow, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoDecoration)){
                float centerXText = (ImGui.GetWindowWidth() - ImGui.CalcTextSize((string)message).X) * 0.5f;
                ImGui.SetCursorPosX(centerXText);

                ImGui.Text((string)message);
                ImGui.NewLine();

                float centerXButtons = (ImGui.GetWindowWidth() - ImGui.CalcTextSize("Okay").X) * 0.45f;
                ImGui.SetCursorPosX(centerXButtons);
                if (ImGui.Button("Okay"))
                    doShow = false;

                ImGui.EndPopup();
            }
        }

    }

}