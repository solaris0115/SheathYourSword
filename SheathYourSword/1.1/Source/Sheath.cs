using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using System.Reflection;
using Verse;
using UnityEngine;
using HarmonyLib;

namespace SYS
{
    public struct Offset
    {
        public Vector3 position;
        public float angle;
    }
    public struct DrawOffsetSet
    {
        public Offset northOffset;
        public Offset eastOffset;
        public Offset southOffset;
        public Offset westOffset;
    }
    //비소집시 드로잉
    public class CompProperties_Sheath : CompProperties
    {
        static GraphicData nullData;
        public static Graphic NullData
        {
            get
            {
                if(nullData==null)
                {
                    nullData = new GraphicData();
                    nullData.graphicClass = typeof(Graphic_Single);
                    nullData.texPath = "Things/NullDraw";
                }
                return nullData.Graphic;
            }
        }
        public GraphicData sheathOnlyGraphicData = null;
        public GraphicData fullGraphicData = null;
        public DrawPosition drawPosition=DrawPosition.None;
        public Offset northOffset;
        public Offset eastOffset;
        public Offset southOffset;
        public Offset westOffset;
        public CompProperties_Sheath()
        {
            compClass = typeof(CompSheath);
        }
    } 
    public class CompSheath : ThingComp
    {
        private Graphic fullGraphicInt;
        private Graphic sheathOnlyGraphicInt;
        public CompProperties_Sheath Props;

        public virtual Graphic FullGraphic
        {
            get
            {
                if (fullGraphicInt == null)
                {
                    if (Props.fullGraphicData == null)
                    {
                        return parent.Graphic;
                    }
                    fullGraphicInt = Props.fullGraphicData.GraphicColoredFor(parent);
                }
                return fullGraphicInt;
            }
        }
        public virtual Graphic SheathOnlyGraphic
        {
            get
            {
                if (sheathOnlyGraphicInt == null)
                {
                    if (Props.sheathOnlyGraphicData == null)
                    {
                        sheathOnlyGraphicInt = CompProperties_Sheath.NullData;
                        return sheathOnlyGraphicInt;
                    }
                    sheathOnlyGraphicInt = Props.sheathOnlyGraphicData.GraphicColoredFor(parent);
                }
                return sheathOnlyGraphicInt;
            }
        }
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            Props = (CompProperties_Sheath)this.props;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Props = (CompProperties_Sheath)props;
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Props = (CompProperties_Sheath)props;
        }
    }
    public enum DrawPosition
    {
        None,
        Back,
        Side
    }




}
