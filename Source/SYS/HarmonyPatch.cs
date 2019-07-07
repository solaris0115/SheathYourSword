using System;
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
            if (!harmonyInstance.HasAnyPatches("com.SYS.rimworld.mod"))
            {                
                harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            }
        }
    }

    [HarmonyPatch(typeof(PawnRenderer))]
    [HarmonyPatch("DrawEquipment")]
    public static class DrawEquipment_WeaponBackPatch
    {
        public const float drawYPosition = 0.0390625f;
        public const float littleDown = -0.2f;
        [HarmonyPrefix]
        public static bool DrawEquipmentPrefix(PawnRenderer __instance, Pawn ___pawn, Vector3 rootLoc)
        {
            if (___pawn.Dead || !___pawn.Spawned)
            {
                return false;
            }
            if (___pawn.equipment == null || ___pawn.equipment.Primary == null)
            {

                return false;
            }
            if (___pawn.CurJob != null && ___pawn.CurJob.def.neverShowWeapon)
            {
                return false;
            }
            Stance_Busy stance_Busy = ___pawn.stances.curStance as Stance_Busy;
            CompWeaponExtention compW = ___pawn.equipment.Primary.GetComp<CompWeaponExtention>();
            CompSheath compSheath = ___pawn.equipment.Primary.GetComp<CompSheath>();
            bool busy = (stance_Busy != null) && (!stance_Busy.neverAimWeapon) && (stance_Busy.focusTarg.IsValid);
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
                if ((a - ___pawn.DrawPos).MagnitudeHorizontalSquared() > 0.001f)
                {
                    num = (a - ___pawn.DrawPos).AngleFlat();
                }
                if (compW!=null &&compW.littleDown)
                {
                    drawLoc.z += littleDown;
                }
                drawLoc += new Vector3(0f, 0f, 0.4f).RotatedBy(num);
                drawLoc.y += drawYPosition;
                __instance.DrawEquipmentAiming(___pawn.equipment.Primary, drawLoc, num);
                if (compSheath != null)
                {
                    DrawSheath(compSheath, ___pawn, rootLoc, compSheath.SheathOnlyGraphic);
                }
                return false;
            }
            else if ((___pawn.carryTracker == null || ___pawn.carryTracker.CarriedThing == null) && (___pawn.Drafted || (___pawn.CurJob != null && ___pawn.CurJob.def.alwaysShowWeapon) || (___pawn.mindState.duty != null && ___pawn.mindState.duty.def.alwaysShowWeapon)))
            {
                Vector3 drawLoc = rootLoc;
                if (compW!=null)
                {
                    switch (___pawn.Rotation.AsInt)
                    {
                        case 0:
                            drawLoc += compW.Props.northOffset.position;
                            DrawEquipmentAiming(___pawn.equipment.Primary, drawLoc, compW.Props.northOffset.angle);
                            break;
                        case 1:
                            drawLoc += compW.Props.eastOffset.position;
                            drawLoc.y += drawYPosition;
                            DrawEquipmentAiming(___pawn.equipment.Primary, drawLoc, compW.Props.eastOffset.angle);
                            break;
                        case 2:
                            drawLoc += compW.Props.southOffset.position;
                            drawLoc.y += drawYPosition;
                            DrawEquipmentAiming(___pawn.equipment.Primary, drawLoc, compW.Props.southOffset.angle);
                            break;
                        case 3:
                            drawLoc += compW.Props.westOffset.position;
                            drawLoc.y += drawYPosition;
                            DrawEquipmentAiming(___pawn.equipment.Primary, drawLoc, compW.Props.westOffset.angle);
                            break;
                        default:
                            break;
                            
                    }
                    /*
                    if (___pawn.Rotation == Rot4.South)
                    {
                        drawLoc += compW.Props.southOffset.position;
                        drawLoc.y += drawYPosition;
                        DrawEquipmentAiming(___pawn.equipment.Primary, drawLoc, compW.Props.southOffset.angle);
                    }
                    else if (___pawn.Rotation == Rot4.North)
                    {
                        drawLoc += compW.Props.northOffset.position;
                        DrawEquipmentAiming(___pawn.equipment.Primary, drawLoc, compW.Props.northOffset.angle);
                    }
                    else if (___pawn.Rotation == Rot4.East)
                    {
                        drawLoc += compW.Props.eastOffset.position;
                        drawLoc.y += drawYPosition;
                        DrawEquipmentAiming(___pawn.equipment.Primary, drawLoc, compW.Props.eastOffset.angle);
                    }
                    else if (___pawn.Rotation == Rot4.West)
                    {
                        drawLoc += compW.Props.westOffset.position;
                        drawLoc.y += drawYPosition;
                        DrawEquipmentAiming(___pawn.equipment.Primary, drawLoc, compW.Props.westOffset.angle);
                    }*/
                }
                else
                {
                    if (___pawn.Rotation == Rot4.South)
                    {
                        Vector3 drawLoc2 = rootLoc + new Vector3(0f, 0f, -0.22f);
                        drawLoc2.y += drawYPosition;
                        __instance.DrawEquipmentAiming(___pawn.equipment.Primary, drawLoc2, 143f);
                    }
                    else if (___pawn.Rotation == Rot4.North)
                    {
                        Vector3 drawLoc3 = rootLoc + new Vector3(0f, 0f, -0.11f);
                        __instance.DrawEquipmentAiming(___pawn.equipment.Primary, drawLoc3, 143f);
                    }
                    else if (___pawn.Rotation == Rot4.East)
                    {
                        Vector3 drawLoc4 = rootLoc + new Vector3(0.2f, 0f, -0.22f);
                        drawLoc4.y += drawYPosition;
                        __instance.DrawEquipmentAiming(___pawn.equipment.Primary, drawLoc4, 143f);
                    }
                    else if (___pawn.Rotation == Rot4.West)
                    {
                        Vector3 drawLoc5 = rootLoc + new Vector3(-0.2f, 0f, -0.22f);
                        drawLoc5.y += drawYPosition;
                        __instance.DrawEquipmentAiming(___pawn.equipment.Primary, drawLoc5, 217f);
                    }
                }
                if (compSheath != null)
                {
                    DrawSheath(compSheath, ___pawn, rootLoc, compSheath.SheathOnlyGraphic);
                }
                return false;
            }
            if (!(___pawn.InBed()) && ___pawn.GetPosture()== PawnPosture.Standing)
            {
                if (compSheath != null)
                {
                    Vector3 drawLoc = rootLoc;
                    DrawSheath(compSheath, ___pawn, drawLoc, compSheath.FullGraphic);

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
