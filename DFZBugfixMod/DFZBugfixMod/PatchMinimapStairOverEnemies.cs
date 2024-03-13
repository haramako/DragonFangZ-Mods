using System;
using MelonLoader;
using HarmonyLib;
using Game;
using Master;
using UnityEngine;
using UnityEngine.UI;


namespace DFZBugfixMod
{
    public partial class DFZBugrfixMod : MelonMod
    {
        /// <summary>
        /// ミニマップで、敵より階段の表示を優先させる変更.
        /// </summary>
        [HarmonyPatch(typeof(MiniMap), "UpdateMinimap2")]
        public static class MinimapStairOverEnemies
        {
            // Copy from MiniMap class.
            const int Width = 64;
            const int Height = Width;

            enum Color2Enum : byte
            {
                None = 0,
                Player = 1 * 2,
                Enemy = 2 * 2,
                Item = 3 * 2,
                Rock = 4 * 2,
                StairUp = 5 * 2,
                StairDown = 6 * 2,
                Trap = 7 * 2,
                Obj = 8 * 2,
            };

            // Copy from MiniMap instance/class.
            static Texture2D texture2_;
            static byte[] texture2Buf_;
            static Floor[] data_;
            static int yokoSabun_;
            static bool viewAll_;
            static bool viewTrap_;

            public static bool Prefix(MiniMap __instance, Field f)
            {
                try
                {
                    var obj = Traverse.Create(__instance);
                    texture2_ = obj.Field("texture2_").GetValue() as Texture2D;
                    texture2Buf_ = obj.Field("texture2Buf_").GetValue() as byte[];
                    yokoSabun_ = (int)obj.Field("yokoSabun_").GetValue();
                    viewAll_ = (bool)obj.Field("viewAll_").GetValue();
                    viewTrap_ = (bool)obj.Field("viewTrap_").GetValue();
                    data_ = obj.Field("data_").GetValue() as Floor[];

                    updateMinimap2Inner(__instance, f);

                    obj.Field("texture2_").SetValue(texture2_);
                    obj.Field("texture2Buf_").SetValue(texture2Buf_);

                    return false;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    return true;
                }
            }

            static void updateMinimap2Inner(MiniMap __instance, Field f)
            {
                // Add by this mod
                // Temporary variables same name as original.
                int p1_;
                int p2_;
                int yoko_, tate_;
                // End add.

                if (texture2_ == null)
                {
                    texture2_ = new Texture2D(Width, Height, TextureFormat.Alpha8, false, true);
                    texture2_.hideFlags = HideFlags.HideAndDontSave;
                    texture2_.wrapMode = TextureWrapMode.Clamp;
                    texture2_.filterMode = FilterMode.Point;
                    texture2Buf_ = new byte[Width * Height];
                }

                Array.Clear(texture2Buf_, 0, Width * Height);

                p1_ = p2_ = 0;

                for (tate_ = 0; tate_ < f.Map.Height; tate_++)
                {
                    for (yoko_ = 0; yoko_ < f.Map.Width; yoko_++)
                    {
                        if (data_[p2_].Open || viewAll_)
                        {
                            // 表示優先順があるので注意

                            // 岩
                            if (data_[p2_].Val == (int)FloorType.Rock)
                            {
                                texture2Buf_[p1_] = (byte)Color2Enum.Rock;
                            }

                            // アイテム類
                            if (data_[p2_].Item != null)
                            {
                                if (data_[p2_].Item.IsStair)
                                {
                                    if (!data_[p2_].Item.Hidden)
                                    {
                                        texture2Buf_[p1_] = (byte)Color2Enum.StairUp;
                                    }
                                }
                                else if (data_[p2_].Item.IsTrap)
                                {
                                    if (viewTrap_ || data_[p2_].Item.Hidden == false)
                                    {
                                        texture2Buf_[p1_] = (byte)Color2Enum.Trap;
                                    }
                                }
                                else
                                {
                                    texture2Buf_[p1_] = (byte)Color2Enum.Item;
                                }
                            }

                            // キャラクタ
                            if (data_[p2_].Character != null && (data_[p2_].Viewport || viewAll_))
                            {
                                if (data_[p2_].Character.IsPlayer)
                                {
                                    texture2Buf_[p1_] = (byte)Color2Enum.Player;
                                }
                                else if (data_[p2_].Character.IsObj)
                                {
                                    texture2Buf_[p1_] = (byte)Color2Enum.Obj;
                                }
                                else if (!data_[p2_].Character.IsStatusActive(CharacterStatus.Transparent))
                                {
                                    texture2Buf_[p1_] = (byte)Color2Enum.Enemy;
                                }
                            }

                            // Add by this mod
                            // 階段だけ、再度上書きする
                            if (data_[p2_].Item != null)
                            {
                                if (data_[p2_].Item.IsStair)
                                {
                                    if (!data_[p2_].Item.Hidden)
                                    {
                                        if (texture2Buf_[p1_] != (byte)Color2Enum.Player) // プレイヤーは優先
                                        {
                                            texture2Buf_[p1_] = (byte)Color2Enum.StairUp;
                                        }
                                    }
                                }
                            }
                            // End add.
                        }
                        p1_++;
                        p2_++;
                    }
                    p1_ += yokoSabun_;
                }

                texture2_.LoadRawTextureData(texture2Buf_);
                texture2_.Apply();

                __instance.GetComponent<RawImage>().material.SetTexture("_MinimapObjectMaskTex", texture2_);
            }
        }
    }
}
