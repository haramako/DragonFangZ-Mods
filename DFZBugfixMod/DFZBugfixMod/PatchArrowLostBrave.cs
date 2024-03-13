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
        /// ブレイブMAX時に、弓矢が当たった時にブレイブを維持できない問題の修正.
        /// </summary>
        [HarmonyPatch(typeof(Game.G), "loadOther")]
        public static class PatchArrowLostBrave
        {
            public static void Postfix()
            {
                // 弓矢のスキル(ID=5)を変更する
                var s = G.FindSkillById(5);
                s.Codes[0].Specials[0].Attribute = SpecialAttribute.ArrowAttack; // SpecialAttribute.MultiAttackを消す
            }
        }
    }
}

