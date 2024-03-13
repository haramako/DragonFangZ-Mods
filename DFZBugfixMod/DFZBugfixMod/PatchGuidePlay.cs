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
        /// 水路の上にいる敵が、自動プレイで認識されない問題の修正.
        /// </summary>
        [HarmonyPatch(typeof(Game.Thinkings.FindTarget), "Think")]
        public static class PatchGuidePlay
        {
            static MoveType savedMoveType;

            public static void Prefix(Field f, ThinkingCode code, Character c, Context ctx)
            {
                if (c.IsPlayer && code.Find == ThinkingCode.Types.FindType.Monster)
                {
                    savedMoveType = c.T.MoveType;
                    c.T.MoveType = MoveType.Fly;
                    c.SetDirty();
                }

            }

            public static void Postfix(Field f, ThinkingCode code, Character c, Context ctx)
            {
                if (c.IsPlayer && code.Find == ThinkingCode.Types.FindType.Monster)
                {
                    c.T.MoveType = savedMoveType;
                    c.SetDirty();
                }
            }
        }
    }
}

