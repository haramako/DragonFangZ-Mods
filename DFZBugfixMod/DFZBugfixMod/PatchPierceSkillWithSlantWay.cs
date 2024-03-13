using MelonLoader;
using HarmonyLib;
using Game;
using Master;
using Game.Thinkings;


namespace DFZBugfixMod
{
    public partial class DFZBugrfixMod : MelonMod
    {
        /// <summary>
        /// 貫通するスキルが斜め方向に壁際を進まないバグの修正.
        /// </summary>
        [HarmonyPatch(typeof(Master.SpecialScope), "GetRange")]
        class PatchPierceSkillWithSlantWay
        {
            public static void Prefix(Master.SpecialScope __instance, Game.ScopeParam p)
            {
                // 特定の状況でのみ、StepFlyableの挙動を変えるフラグをセットする
                if (__instance.Type == ScopeType.Straight && __instance.Pierce)
                {
                    PatchPierceSkillWithSlantWayForStepFlyable.ForceSlantAnywhere = true;
                }
            }

            public static void Postfix(Game.ScopeParam p)
            {
                // Prefixで変更されたものを元に戻す
                PatchPierceSkillWithSlantWayForStepFlyable.ForceSlantAnywhere = false;
            }
        }


        /// <summary>
        /// PatchPierceSkillWithSlantWayのなかで、StepFlyableを調整するたのパッチ.
        /// </summary>
        [HarmonyPatch(typeof(Game.Map), "StepFlyable")]
        class PatchPierceSkillWithSlantWayForStepFlyable
        {
            // 斜め方向に移動可能を強制する
            public static bool ForceSlantAnywhere;

            public static bool Prefix(Game.Map __instance, ref bool slantAnywhere)
            {
                if (ForceSlantAnywhere)
                {
                    slantAnywhere = true;
                }
                return true;
            }
        }
    }
}
