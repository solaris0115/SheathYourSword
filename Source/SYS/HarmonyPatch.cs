﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using System.Reflection;
using Verse;
using UnityEngine;
using Harmony;

namespace SYS
{

    [StaticConstructorOnStartup]
    public static class DrawPatch
    {
        static DrawPatch()
        {
            HarmonyInstance harmonyInstance = HarmonyInstance.Create("com.SYS.rimworld.mod");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(PawnRenderer))]
    [HarmonyPatch("DrawEquipment")]
    public static class DrawEquipment_WeaponBackPatch
    {
        public const float drawYPosition = 0.0390625f;
        [HarmonyPrefix]
        public static bool DrawEquipmentPrefix(ref PawnRenderer __instance, ref Vector3 rootLoc)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn.Dead || !pawn.Spawned)
            {
                return false;
            }
            if (pawn.equipment == null || pawn.equipment.Primary == null)
            {

                return false;
            }
            if (pawn.CurJob != null && pawn.CurJob.def.neverShowWeapon)
            {
                return false;
            }

            Stance_Busy stance_Busy = pawn.stances.curStance as Stance_Busy;
            CompWeaponExtention compW = pawn.equipment.Primary.GetComp<CompWeaponExtention>();
            bool busy = stance_Busy != null && !stance_Busy.neverAimWeapon && stance_Busy.focusTarg.IsValid;
            if (busy)
            {
                Vector3 drawLoc = rootLoc;
                Vector3 a;
                if (stance_Busy.focusTarg.HasThing)
                {
                    a = stance_Busy.focusTarg.Thing.DrawPos;
                }
                else
                {
                    a = stance_Busy.focusTarg.Cell.ToVector3Shifted();
                }
                float num = 0f;
                if ((a - pawn.DrawPos).MagnitudeHorizontalSquared() > 0.001f)
                {
                    num = (a - pawn.DrawPos).AngleFlat();
                }
                drawLoc += new Vector3(0f, 0f, 0.4f).RotatedBy(num);
                drawLoc.y += drawYPosition;
                __instance.DrawEquipmentAiming(pawn.equipment.Primary, drawLoc, num);
            }
            else if ((pawn.carryTracker == null || pawn.carryTracker.CarriedThing == null) && (pawn.Drafted || (pawn.CurJob != null && pawn.CurJob.def.alwaysShowWeapon) || (pawn.mindState.duty != null && pawn.mindState.duty.def.alwaysShowWeapon)))
            {
                Vector3 drawLoc = rootLoc;
                if (compW!=null)
                {
                    if (pawn.Rotation == Rot4.South)
                    {
                        drawLoc += compW.Props.southOffset.position;
                        drawLoc.y += drawYPosition;
                        DrawEquipmentAiming(pawn.equipment.Primary, drawLoc, compW.Props.southOffset.angle);
                    }
                    else if (pawn.Rotation == Rot4.North)
                    {
                        drawLoc += compW.Props.northOffset.position;
                        DrawEquipmentAiming(pawn.equipment.Primary, drawLoc, compW.Props.northOffset.angle);
                    }
                    else if (pawn.Rotation == Rot4.East)
                    {
                        drawLoc += compW.Props.eastOffset.position;
                        drawLoc.y += drawYPosition;
                        DrawEquipmentAiming(pawn.equipment.Primary, drawLoc, compW.Props.eastOffset.angle);
                    }
                    else if (pawn.Rotation == Rot4.West)
                    {
                        drawLoc += compW.Props.westOffset.position;
                        drawLoc.y += drawYPosition;
                        DrawEquipmentAiming(pawn.equipment.Primary, drawLoc, compW.Props.westOffset.angle);
                    }
                }
                else
                {
                    if (pawn.Rotation == Rot4.South)
                    {
                        Vector3 drawLoc2 = rootLoc + new Vector3(0f, 0f, -0.22f);
                        drawLoc2.y += drawYPosition;
                        __instance.DrawEquipmentAiming(pawn.equipment.Primary, drawLoc2, 143f);
                    }
                    else if (pawn.Rotation == Rot4.North)
                    {
                        Vector3 drawLoc3 = rootLoc + new Vector3(0f, 0f, -0.11f);
                        __instance.DrawEquipmentAiming(pawn.equipment.Primary, drawLoc3, 143f);
                    }
                    else if (pawn.Rotation == Rot4.East)
                    {
                        Vector3 drawLoc4 = rootLoc + new Vector3(0.2f, 0f, -0.22f);
                        drawLoc4.y += drawYPosition;
                        __instance.DrawEquipmentAiming(pawn.equipment.Primary, drawLoc4, 143f);
                    }
                    else if (pawn.Rotation == Rot4.West)
                    {
                        Vector3 drawLoc5 = rootLoc + new Vector3(-0.2f, 0f, -0.22f);
                        drawLoc5.y += drawYPosition;
                        __instance.DrawEquipmentAiming(pawn.equipment.Primary, drawLoc5, 217f);
                    }
                }
            }
            if(!pawn.InBed())
            {
                CompSheath compSheath = pawn.equipment.Primary.GetComp<CompSheath>();
                if (compSheath != null)
                {
                    Vector3 drawLoc = rootLoc;
                    if (!pawn.Drafted && !busy)
                    {
                        DrawSheath(compSheath, pawn, drawLoc, compSheath.FullGraphic);
                    }
                    else
                    {
                        DrawSheath(compSheath, pawn, drawLoc, compSheath.SheathOnlyGraphic);
                    }
                }
            }            
            return false;
        }
        public static void DrawEquipmentAiming(Thing eq, Vector3 drawLoc, float aimAngle)
        {
            float num = aimAngle;
            Mesh mesh;
            if (aimAngle > 20f && aimAngle < 160f)
            {
                mesh = MeshPool.plane10;
            }
            else if (aimAngle > 200f && aimAngle < 340f)
            {
                mesh = MeshPool.plane10Flip;
            }
            else
            {
                mesh = MeshPool.plane10;
            }
            num %= 360f;
            Material matSingle = eq.Graphic.MatSingle;
            Graphics.DrawMesh(mesh, drawLoc, Quaternion.AngleAxis(num, Vector3.up), matSingle, 0);
        }
        public static void DrawSheath(Pawn pawn, Thing eq, Vector3 drawLoc, float aimAngle,Graphic graphic)
        {
            float num = aimAngle;
            num %= 360f;
            CompSheath comp = eq.TryGetComp<CompSheath>();
            if(comp!=null)
            {
                Graphics.DrawMesh(graphic.MeshAt(pawn.Rotation), drawLoc, Quaternion.AngleAxis(num, Vector3.up), graphic.MatAt(pawn.Rotation), 0);
            }
        }
        public static void DrawSheath(CompSheath compSheath, Pawn pawn, Vector3 drawLoc, Graphic graphic)
        {
            switch (compSheath.Props.drawPosition)
            {
                case DrawPosition.Side:
                    if (pawn.Rotation == Rot4.South)
                    {
                        drawLoc += compSheath.Props.northOffset.position;
                        drawLoc.y += drawYPosition;
                        DrawSheath(pawn, pawn.equipment.Primary, drawLoc, compSheath.Props.northOffset.angle, graphic);
                        return;
                    }
                    if (pawn.Rotation == Rot4.North)
                    {
                        drawLoc += compSheath.Props.southOffset.position;
                        drawLoc.y += drawYPosition;
                        DrawSheath(pawn, pawn.equipment.Primary, drawLoc, compSheath.Props.southOffset.angle, graphic);
                        return;
                    }
                    if (pawn.Rotation == Rot4.East)
                    {
                        drawLoc += compSheath.Props.eastOffset.position;
                        drawLoc.y += drawYPosition;
                        DrawSheath(pawn, pawn.equipment.Primary, drawLoc, compSheath.Props.eastOffset.angle, graphic);
                        return;
                    }
                    if (pawn.Rotation == Rot4.West)
                    {
                        drawLoc += compSheath.Props.westOffset.position;
                        drawLoc.y += drawYPosition;
                        DrawSheath(pawn, pawn.equipment.Primary, drawLoc, compSheath.Props.westOffset.angle, graphic);
                        return;
                    }
                    break;
                case DrawPosition.Back:
                    if (pawn.Rotation == Rot4.South)
                    {
                        drawLoc += compSheath.Props.southOffset.position;
                        DrawSheath(pawn, pawn.equipment.Primary, drawLoc, compSheath.Props.southOffset.angle, graphic);
                        return;
                    }
                    if (pawn.Rotation == Rot4.North)
                    {
                        drawLoc += compSheath.Props.northOffset.position;
                        drawLoc.y += drawYPosition;
                        DrawSheath(pawn, pawn.equipment.Primary, drawLoc, compSheath.Props.northOffset.angle, graphic);
                        return;
                    }
                    if (pawn.Rotation == Rot4.East)
                    {
                        drawLoc += compSheath.Props.eastOffset.position;
                        drawLoc.y += drawYPosition;
                        DrawSheath(pawn, pawn.equipment.Primary, drawLoc, compSheath.Props.eastOffset.angle, graphic);
                        return;
                    }
                    if (pawn.Rotation == Rot4.West)
                    {
                        drawLoc += compSheath.Props.westOffset.position;
                        drawLoc.y += drawYPosition;
                        DrawSheath(pawn, pawn.equipment.Primary, drawLoc, compSheath.Props.westOffset.angle, graphic);
                        return;
                    }
                    break;
                default:
                    return;
            }
        }
    }

}