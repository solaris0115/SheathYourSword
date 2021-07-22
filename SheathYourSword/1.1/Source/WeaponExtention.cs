﻿using System;
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
    public class CompProperties_WeaponExtention : CompProperties
    {
        public Offset northOffset;
        public Offset eastOffset;
        public Offset southOffset;
        public Offset westOffset;
        public bool littleDown=false;
        public CompProperties_WeaponExtention()
        {
            compClass = typeof(CompWeaponExtention);
        }
    }
    public class CompWeaponExtention : ThingComp
    {
        public bool littleDown = false;
        public CompProperties_WeaponExtention Props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Props = (CompProperties_WeaponExtention)props;
            if (Props.littleDown)
            {
                littleDown = true;
            }
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Props = (CompProperties_WeaponExtention)props;
        }
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            Props = (CompProperties_WeaponExtention)props;
            if (Props.littleDown)
            {
                littleDown = true;
            }
        }
    }
}
