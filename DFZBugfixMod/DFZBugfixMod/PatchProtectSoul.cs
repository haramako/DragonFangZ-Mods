using MelonLoader;
using HarmonyLib;
using Game;
using Master;

namespace DFZBugfixMod
{
    public partial class DFZBugrfixMod : MelonMod
    {
        /// <summary>
        /// 複数のProtect系ソウルがある場合に、一つしか発動しない問題の修正.
        /// </summary>
        [HarmonyPatch(typeof(Game.FieldAction.CharacterSoul), "CheckSoulFilter")]
        public static class PatchProtectSouls
        {
            public static bool Prefix(ref bool __result, Field f, Character c, SoulCondType cond, FilterOption opt)
            {
                if (cond == SoulCondType.WithProtect)
                {
                    __result = CheckSoulFilterPatched(f, c, cond, opt);
                    return false;
                }
                else
                {
                    return true;
                }
            }

            public static bool CheckSoulFilterPatched(Field f, Character c, SoulCondType cond, FilterOption opt)
            {
                if (c.IsStatusActive(CharacterStatus.Seal))
                {
                    return false;
                }

                var souls = c.Souls;
                var len = souls.Count;
                for (int i = 0; i < len; i++)
                {
                    var soul = souls[i];
                    if (soul.Type == cond && c.IsSoulActive(f, soul))
                    {
                        if (opt.Certain)
                        {
                            return true;
                        }

                        if (opt.Status != CharacterStatus.NoneCharacterStatus)
                        {
                            if (soul.Status.Contains(opt.Status))
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }
    }
}
