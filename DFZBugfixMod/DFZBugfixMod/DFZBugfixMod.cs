using MelonLoader;
using HarmonyLib;
using Game;
using Master;
using Game.Thinkings;

namespace DFZBugrfixMod
{
    public partial class DFZBugrfixMod : MelonMod
    {
        /// <summary>
        /// バージョン番号の変更.
        /// </summary>
        [HarmonyPatch(typeof(GameSetting), "GetVersionText")]
        public static class PatchOverwriteVersionText
        {
            public static void Postfix(ref string __result)
            {
                /// += にしているのは、MOD複数適用のときのため
                __result += "+Bugfix";
            }
        }

    }
}
