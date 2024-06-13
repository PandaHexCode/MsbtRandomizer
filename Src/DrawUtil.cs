﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBTRando{

    public class DrawUtil{
        public bool doShow = false;
        public object parm;
        public virtual void Draw(object parm){
        }
    }

    public class DrawUtilRender{
        public static List<DrawUtil> drawUtils = new List<DrawUtil>();
        public static void Draw(){
            List<DrawUtil> drawUtilsToRemove = new List<DrawUtil>();
            foreach(DrawUtil drawUtil in drawUtils){
                if (!drawUtil.doShow){
                    drawUtilsToRemove.Add(drawUtil);
                    continue;
                }
                drawUtil.Draw(drawUtil.parm);
            }
            foreach(DrawUtil drawUtil in drawUtilsToRemove){
                drawUtils.Remove(drawUtil);
                drawUtil.parm = null;
            }
        }
        public static void AddDrawUtil(DrawUtil drawUtil, object parms){
            drawUtils.Add(drawUtil);
            drawUtil.parm = parms;
            drawUtil.doShow = true;
        }
    }

}