using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

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

    public class AnimationFile : GenericAssetDataContainer
    {
        public int FileFlags { get; set; }
        public AssetSingle Duration { get; set; }
        public AssetSingle TimeOffset { get; set; }
        public int[][] RawData { get; set; }
        public int Physics { get; set; }
        public int StartPose { get; set; }
        public int EndPose { get; set; }

        public AnimationFile()
        {
            RawData = new int[1][];
            RawData[1] = new int[1];
        }

        public AnimationFile(EndianBinaryReader reader)
        {
            FileFlags = reader.ReadInt32();
            Duration = reader.ReadSingle();
            TimeOffset = reader.ReadSingle();
            var animCount1 = reader.ReadInt16();
            var animCount2 = reader.ReadInt16();
            var rawDataOffset = reader.ReadUInt32();
            Physics = reader.ReadInt32();
            StartPose = reader.ReadInt32();
            EndPose = reader.ReadInt32();

            RawData = new int[animCount1][];

            var currPos = reader.BaseStream.Position;
            reader.BaseStream.Position = rawDataOffset;
            for (int i = 0; i < animCount1; i++)
            {
                RawData[i] = new int[animCount2];
                for (int j = 0; j < animCount2; j++)
                    RawData[i][j] = reader.ReadInt32();
            }
            reader.BaseStream.Position = currPos;
        }

        public int RawDataOffset;

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(FileFlags);
            writer.Write(Duration);
            writer.Write(TimeOffset);
            writer.Write((short)RawData.Length);
            writer.Write((short)RawData[0].Length);
            writer.Write(RawDataOffset);
            writer.Write(Physics);
            writer.Write(StartPose);
            writer.Write(EndPose);
        }
    }

    public class AnimationState : GenericAssetDataContainer
    {
        public AssetID StateID { get; set; }
        public int AnimFileIndex { get; set; }
        public AssetSingle Speed { get; set; }
        public int SubStateID { get; set; }
        public int SubStateCount { get; set; }

        [Editor(typeof(DynamicTypeDescriptorCollectionEditor), typeof(UITypeEditor))]
        public AnimationEffect[] AnimationEffects { get; set; }

        public AnimationState(Game game)
        {
            _game = game;
        }

        public AnimationState(EndianBinaryReader reader, Game game) : this(game)
        {
            StateID = reader.ReadUInt32();
            AnimFileIndex = reader.ReadInt32();
            var effectCount = reader.ReadInt32();
            var effectOffset = reader.ReadUInt32();
            Speed = reader.ReadSingle();

            if (game != Game.Scooby)
            {
                SubStateID = reader.ReadInt32();
                SubStateCount = reader.ReadInt32();
            }

            AnimationEffects = new AnimationEffect[effectCount];

            if (effectCount > 0)
            {
                var currPos = reader.BaseStream.Position;
                reader.BaseStream.Position = effectOffset;

                for (int i = 0; i < AnimationEffects.Length; i++)
                    AnimationEffects[i] = new AnimationEffect(reader, game);

                reader.BaseStream.Position = currPos;
            }
        }

        public int EffectOffset;

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(StateID);
            writer.Write(AnimFileIndex);
            writer.Write(AnimationEffects.Length);
            writer.Write(EffectOffset);
            writer.Write(Speed);
            if (game != Game.Scooby)
            {
                writer.Write(SubStateID);
                writer.Write(SubStateCount);
            }
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            base.SetDynamicProperties(dt);
        }
    }

    public class AnimationEffect : GenericAssetDataContainer
    {
        public int StateID { get; set; }
        public AssetSingle StartTime { get; set; }
        public AssetSingle EndTime { get; set; }
        public int Flags { get; set; }

        [Description("BFBB/Scooby only")]
        public int EffectType { get; set; }
        [Description("BFBB/Scooby only")]
        public byte[] UserDataBytes { get; set; }

        [Description("Movie/Incredibles only")]
        public byte UnknownByte10 { get; set; }
        [Description("Movie/Incredibles only")]
        public byte UnknownByte11 { get; set; }
        [Description("Movie/Incredibles only")]
        public byte UnknownByte12 { get; set; }
        [Description("Movie/Incredibles only")]
        public byte UnknownByte13 { get; set; }
        [Description("Movie/Incredibles only")]
        public AssetID Unknown14 { get; set; }
        [Description("Movie/Incredibles only")]
        public byte UnknownByte18 { get; set; }
        [Description("Movie/Incredibles only")]
        public byte UnknownByte19 { get; set; }
        [Description("Movie/Incredibles only")]
        public byte UnknownByte1A { get; set; }
        [Description("Movie/Incredibles only")]
        public byte UnknownByte1B { get; set; }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Incredibles)
            {
                dt.RemoveProperty("EffectType");
                dt.RemoveProperty("UserDataBytes");
            }
            else
            {
                dt.RemoveProperty("UnknownByte10");
                dt.RemoveProperty("UnknownByte11");
                dt.RemoveProperty("UnknownByte12");
                dt.RemoveProperty("UnknownByte13");
                dt.RemoveProperty("Unknown14");
                dt.RemoveProperty("UnknownByte18");
                dt.RemoveProperty("UnknownByte19");
                dt.RemoveProperty("UnknownByte1A");
                dt.RemoveProperty("UnknownByte1B");
            }
        }

        public AnimationEffect(Game game)
        {
            _game = game;

            UserDataBytes = new byte[0];
        }

        public AnimationEffect(EndianBinaryReader reader, Game game)
        {
            _game = game;

            StateID = reader.ReadInt32();
            StartTime = reader.ReadSingle();
            EndTime = reader.ReadSingle();
            Flags = reader.ReadInt32();
            if (game != Game.Incredibles)
            {
                EffectType = reader.ReadInt32();
                var userDataSize = reader.ReadInt32();
                UserDataBytes = reader.ReadBytes(userDataSize);
            }
            else
            {
                UnknownByte10 = reader.ReadByte();
                UnknownByte11 = reader.ReadByte();
                UnknownByte12 = reader.ReadByte();
                UnknownByte13 = reader.ReadByte();
                Unknown14 = reader.ReadUInt32();
                UnknownByte18 = reader.ReadByte();
                UnknownByte19 = reader.ReadByte();
                UnknownByte1A = reader.ReadByte();
                UnknownByte1B = reader.ReadByte();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(StateID);
            writer.Write(StartTime);
            writer.Write(EndTime);
            writer.Write(Flags);

            if (game != Game.Incredibles)
            {
                writer.Write(EffectType);
                writer.Write(UserDataBytes.Length);
                writer.Write(UserDataBytes);
            }
            else
            {
                writer.Write(UnknownByte10);
                writer.Write(UnknownByte11);
                writer.Write(UnknownByte12);
                writer.Write(UnknownByte13);
                writer.Write(Unknown14);
                writer.Write(UnknownByte18);
                writer.Write(UnknownByte19);
                writer.Write(UnknownByte1A);
                writer.Write(UnknownByte1B);
            }
        }
    }

    public class AssetATBL : Asset
    {
        private const string categoryName = "Animation Table";

        [Category(categoryName)]
        public ConstructFunc ConstructFunc_Enum
        {
            get => (ConstructFunc)(uint)ConstructFunc_Hash;
            set => ConstructFunc_Hash = (uint)value;
        }

        [Category(categoryName)]
        public AssetID ConstructFunc_Hash { get; set; }
        [Category(categoryName), ValidReferenceRequired]
        public AssetID[] Animations { get; set; }
        [Category(categoryName)]
        public AnimationFile[] AnimationFiles { get; set; }

        [Category(categoryName), Editor(typeof(DynamicTypeDescriptorCollectionEditor), typeof(UITypeEditor))]
        public AnimationState[] AnimationStates { get; set; }

        public AssetATBL(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.ReadInt32();
                int numRaw = reader.ReadInt32();
                int numFiles = reader.ReadInt32();
                int numStates = reader.ReadInt32();
                ConstructFunc_Hash = reader.ReadUInt32();

                Animations = new AssetID[numRaw];
                for (int i = 0; i < Animations.Length; i++)
                    Animations[i] = reader.ReadUInt32();

                AnimationFiles = new AnimationFile[numFiles];
                for (int i = 0; i < AnimationFiles.Length; i++)
                    AnimationFiles[i] = new AnimationFile(reader);

                AnimationStates = new AnimationState[numStates];
                for (int i = 0; i < AnimationStates.Length; i++)
                    AnimationStates[i] = new AnimationState(reader, game);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.WriteMagic("ATBL");

            writer.Write(Animations.Length);
            writer.Write(AnimationFiles.Length);
            writer.Write(AnimationStates.Length);
            writer.Write(ConstructFunc_Hash);

            foreach (var anim in Animations)
                writer.Write(anim);

            var filesStartPos = writer.BaseStream.Position;

            writer.BaseStream.Position += 32 * AnimationFiles.Length + (game == Game.Scooby ? 20 : 28) * AnimationStates.Length;

            for (int i = 0; i < AnimationStates.Length; i++)
            {
                AnimationStates[i].EffectOffset = (int)writer.BaseStream.Position;
                foreach (var j in AnimationStates[i].AnimationEffects)
                    j.Serialize(writer);
            }

            for (int i = 0; i < AnimationFiles.Length; i++)
            {
                AnimationFiles[i].RawDataOffset = (int)writer.BaseStream.Position;

                foreach (var j in AnimationFiles[i].RawData)
                    foreach (var k in j)
                        writer.Write(k);
            }

            writer.BaseStream.Position = filesStartPos;

            foreach (var animFile in AnimationFiles)
                animFile.Serialize(writer);

            foreach (var animState in AnimationStates)
                animState.Serialize(writer);
        }
    }
}