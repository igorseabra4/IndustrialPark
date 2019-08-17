using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public enum ConstructFunc : uint
    {
        zEntPlayer_AnimTable = 0,
        ZNPC_AnimTable_Common = 1,
        zPatrick_AnimTable = 2,
        zSandy_AnimTable = 3,
        ZNPC_AnimTable_Villager = 4,
        zSpongeBobTongue_AnimTable = 5,
        ZNPC_AnimTable_LassoGuide = 6,
        ZNPC_AnimTable_Hammer = 7,
        ZNPC_AnimTable_TarTar = 8,
        ZNPC_AnimTable_GLove = 9,
        ZNPC_AnimTable_Monsoon = 10,
        ZNPC_AnimTable_SleepyTime = 11,
        ZNPC_AnimTable_ArfDog = 12,
        ZNPC_AnimTable_ArfArf = 13,
        ZNPC_AnimTable_Chuck = 14,
        ZNPC_AnimTable_Tubelet = 15,
        ZNPC_AnimTable_Slick = 16,
        ZNPC_AnimTable_Ambient = 17,
        ZNPC_AnimTable_Tiki = 18,
        ZNPC_AnimTable_Fodder = 19,
        ZNPC_AnimTable_Duplotron = 20,
        ZNPC_AnimTable_Jelly = 21,
        ZNPC_AnimTable_Test = 22,
        ZNPC_AnimTable_Neptune = 23,
        ZNPC_AnimTable_KingJelly = 24,
        ZNPC_AnimTable_Dutchman = 25,
        ZNPC_AnimTable_Prawn = 26,
        ZNPC_AnimTable_BossSandy = 27,
        ZNPC_AnimTable_BossPatrick = 28,
        ZNPC_AnimTable_BossSB1 = 29,
        ZNPC_AnimTable_BossSB2 = 30,
        ZNPC_AnimTable_BossSBobbyArm = 31,
        ZNPC_AnimTable_BossPlankton = 32,
        zEntPlayer_BoulderVehicleAnimTable = 33,
        ZNPC_AnimTable_BossSandyHead = 34,
        ZNPC_AnimTable_BalloonBoy = 35,
        xEnt_AnimTable_AutoEventSmall = 36,
        ZNPC_AnimTable_SlickShield = 37,
        ZNPC_AnimTable_SuperFriend = 38,
        ZNPC_AnimTable_ThunderCloud = 39,
        XHUD_AnimTable_Idle = 40,
        ZNPC_AnimTable_NightLight = 41,
        ZNPC_AnimTable_HazardStd = 42,
        ZNPC_AnimTable_FloatDevice = 43,
        cruise_bubble__anim_table = 44,
        ZNPC_AnimTable_BossSandyScoreboard = 45,
        zEntPlayer_TreeDomeSBAnimTable = 46
    }

    public class AnimationFile
    {
        public int FileFlags { get; set; }
        public float Duration { get; set; }
        public float TimeOffset { get; set; }
        public short NumAnims1 { get; set; }
        public short NumAnims2 { get; set; }
        public int RawData { get; set; }
        public int Physics { get; set; }
        public int StartPose { get; set; }
        public int EndPose { get; set; }
    }

    public class AnimationState
    {
        public AssetID StateID { get; set; }
        public int FileIndex { get; set; }
        public int EffectCount { get; set; }
        public int EffectOffset { get; set; }
        public float Speed { get; set; }
        public int SubStateID { get; set; }
        public int SubStateCount { get; set; }

        public AnimationState()
        {
            StateID = 0;
        }
    }

    public class AnimationEffect
    {
        public int StateID { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public int Flags { get; set; }
        public int EffectType { get; set; }
        public int UserDataSize { get; set; }
        public int Unknown { get; set; }

        public AnimationEffect()
        {
            StateID = 0;
        }
    }

    public class AssetATBL : Asset
    {
        public AssetATBL(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override bool HasReference(uint assetID)
        {
            foreach (AssetID a in Animations)
                if (a == assetID)
                    return true;

            foreach (AnimationState a in AnimationStates)
                if (a.StateID == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            foreach (AssetID a in Animations)
            {
                if (a == 0)
                    result.Add("ATBL entry with animation asset ID set to 0");
                Verify(a, ref result);
            }

            AnimationFile[] b = AnimationFiles;
            AnimationState[] c = AnimationStates;
            AnimationEffect[] d = AnimationEffects;
            int[] e = Entries_Unknown;
        }

        [Category("Animation Table"), ReadOnly(true)]
        public int NumRaw
        {
            get => ReadInt(0x04);
            set => Write(0x04, value);
        }

        [Category("Animation Table"), ReadOnly(true)]
        public int NumFiles
        {
            get => ReadInt(0x08);
            set => Write(0x08, value);
        }

        [Category("Animation Table"), ReadOnly(true)]
        public int NumStates
        {
            get => ReadInt(0x0C);
            set => Write(0x0C, value);
        }

        [Category("Animation Table")]
        public ConstructFunc ConstructFunc_Enum
        {
            get => (ConstructFunc)(uint)ConstructFunc_Hash;
            set => ConstructFunc_Hash = (uint)value;
        }

        [Category("Animation Table")]
        public AssetID ConstructFunc_Hash
        {
            get => ReadUInt(0x10);
            set => Write(0x10, value);
        }

        private int RawStart => 0x14;

        [Category("Animation Table")]
        public AssetID[] Animations
        {
            get
            {
                AssetID[] animations = new AssetID[NumRaw];
                for (int i = 0; i < NumRaw; i++)
                    animations[i] = ReadUInt(RawStart + 4 * i);

                return animations;
            }
            set
            {
                List<byte> newData = Data.Take(RawStart).ToList();
                List<byte> restOfOldData = Data.Skip(RawStart + 4 * NumRaw).ToList();

                foreach (AssetID i in value)
                {
                    if (platform == Platform.GameCube)
                        newData.AddRange(BitConverter.GetBytes(i).Reverse());
                    else
                        newData.AddRange(BitConverter.GetBytes(i));
                }

                newData.AddRange(restOfOldData);

                Data = newData.ToArray();

                NumRaw = value.Length;
            }
        }

        private int AnimationFilesStart => RawStart + NumRaw * 4;

        [Category("Animation Table")]
        public AnimationFile[] AnimationFiles
        {
            get
            {
                List<AnimationFile> entries = new List<AnimationFile>();

                for (int i = 0; i < NumFiles; i++)
                {
                    entries.Add(new AnimationFile()
                    {
                        FileFlags = ReadInt(AnimationFilesStart + i * 0x20),
                        Duration = ReadInt(AnimationFilesStart + i * 0x20 + 0x4),
                        TimeOffset = ReadFloat(AnimationFilesStart + i * 0x20 + 0x8),
                        NumAnims1 = ReadShort(AnimationFilesStart + i * 0x20 + 0xC),
                        NumAnims2 = ReadShort(AnimationFilesStart + i * 0x20 + 0xE),
                        RawData = ReadInt(AnimationFilesStart + i * 0x20 + 0x10),
                        Physics = ReadInt(AnimationFilesStart + i * 0x20 + 0x14),
                        StartPose = ReadInt(AnimationFilesStart + i * 0x20 + 0x18),
                        EndPose = ReadInt(AnimationFilesStart + i * 0x20 + 0x1C),
                    });
                }

                return entries.ToArray();
            }
            set
            {
                List<byte> newData = Data.Take(AnimationFilesStart).ToList();
                List<byte> restOfData = Data.Skip(AnimationStatesStart).ToList();

                foreach (AnimationFile i in value)
                {
                    newData.AddRange(BitConverter.GetBytes(Switch(i.FileFlags)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.Duration)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.TimeOffset)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.NumAnims1)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.NumAnims2)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.RawData)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.Physics)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.StartPose)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.EndPose)));
                }

                newData.AddRange(restOfData);

                Data = newData.ToArray();
                NumFiles = value.Length;
            }
        }

        private int AnimationStatesStart => AnimationFilesStart + NumFiles * 0x20;

        [Category("Animation Table")]
        public AnimationState[] AnimationStates
        {
            get
            {
                List<AnimationState> entries = new List<AnimationState>();

                for (int i = 0; i < NumStates; i++)
                {
                    entries.Add(new AnimationState()
                    {
                        StateID = ReadUInt(AnimationStatesStart + i * 0x1C),
                        FileIndex = ReadInt(AnimationStatesStart + i * 0x1C + 0x4),
                        EffectCount = ReadInt(AnimationStatesStart + i * 0x1C + 0x8),
                        EffectOffset = ReadInt(AnimationStatesStart + i * 0x1C + 0xC),
                        Speed = ReadFloat(AnimationStatesStart + i * 0x1C + 0x10),
                        SubStateID = ReadInt(AnimationStatesStart + i * 0x1C + 0x14),
                        SubStateCount = ReadInt(AnimationStatesStart + i * 0x1C + 0x18),
                    });
                }

                return entries.ToArray();
            }
            set
            {
                List<byte> newData = Data.Take(AnimationStatesStart).ToList();
                List<byte> restOfData = Data.Skip(AnimationEffectsStart).ToList();

                foreach (AnimationState i in value)
                {
                    newData.AddRange(BitConverter.GetBytes(Switch(i.StateID)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.FileIndex)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.EffectCount)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.EffectOffset)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.Speed)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.SubStateID)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.SubStateCount)));
                }

                newData.AddRange(restOfData);

                Data = newData.ToArray();
                NumStates = value.Length;
            }
        }

        private int AnimationEffectsStart => AnimationStatesStart + NumStates * 0x1C;

        [Category("Animation Table")]
        public AnimationEffect[] AnimationEffects
        {
            get
            {
                List<AnimationEffect> entries = new List<AnimationEffect>();

                for (int i = AnimationEffectsStart; i < ListUnknown5Start; i += 0x1C)
                {
                    entries.Add(new AnimationEffect()
                    {
                        StateID = ReadInt(i),
                        StartTime = ReadFloat(i + 4),
                        EndTime = ReadFloat(i + 8),
                        Flags = ReadInt(i + 12),
                        EffectType = ReadInt(i + 16),
                        UserDataSize = ReadInt(i + 20),
                        Unknown = ReadInt(i + 24)
                    });
                }

                return entries.ToArray();
            }
            set
            {
                List<byte> newData = Data.Take(AnimationEffectsStart).ToList();
                List<byte> restOfData = Data.Skip(ListUnknown5Start).ToList();

                foreach (AnimationEffect i in value)
                {
                    newData.AddRange(BitConverter.GetBytes(Switch(i.StateID)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.StartTime)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.EndTime)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.Flags)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.EffectType)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.UserDataSize)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.Unknown)));
                }

                newData.AddRange(restOfData);

                Data = newData.ToArray();
            }
        }
        private int ListUnknown5Start => Data.Count() - NumRaw * 0x04;

        [Category("Animation Table")]
        public int[] Entries_Unknown
        {
            get
            {
                List<int> entries = new List<int>();

                for (int i = ListUnknown5Start; i < Data.Length; i += 4)
                    entries.Add(ReadInt(i));

                return entries.ToArray();
            }
            set
            {
                List<byte> newData = Data.Take(ListUnknown5Start).ToList();

                foreach (int i in value)
                    newData.AddRange(BitConverter.GetBytes(Switch(i)));
                
                Data = newData.ToArray();
            }
        }
    }
}