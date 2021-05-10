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
        public AssetSingle Duration { get; set; }
        public AssetSingle TimeOffset { get; set; }
        public byte NumAnims1 { get; set; }
        public byte NumAnims2 { get; set; }
        public int[] RawData { get; set; }
        public int Physics { get; set; }
        public int StartPose { get; set; }
        public int EndPose { get; set; }

        public AnimationFile() { }
        public AnimationFile(EndianBinaryReader reader)
        {
            FileFlags = reader.ReadInt32();
            Duration = reader.ReadSingle();
            TimeOffset = reader.ReadSingle();
            var animCount = reader.ReadInt16();
            NumAnims1 = reader.ReadByte();
            NumAnims2 = reader.ReadByte();
            var rawDataOffset = reader.ReadUInt32();
            Physics = reader.ReadInt32();
            StartPose = reader.ReadInt32();
            EndPose = reader.ReadInt32();

            RawData = new int[animCount];

            var currPos = reader.BaseStream.Position;
            reader.BaseStream.Position = rawDataOffset;
            for (int i = 0; i < animCount; i++)
                RawData[i] = reader.ReadInt32();
            reader.BaseStream.Position = currPos;
        }

        public int RawDataOffset;

        public void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(FileFlags);
            writer.Write(Duration);
            writer.Write(TimeOffset);
            writer.Write((short)RawData.Length);
            writer.Write(NumAnims1);
            writer.Write(NumAnims2);
            writer.Write(RawDataOffset);
            writer.Write(Physics);
            writer.Write(StartPose);
            writer.Write(EndPose);
        }
    }

    public class AnimationState
    {
        public AssetID StateID { get; set; }
        public int AnimFileIndex { get; set; }
        public AssetSingle Speed { get; set; }
        public int SubStateID { get; set; }
        public int SubStateCount { get; set; }

        public AnimationEffect[] AnimationEffects { get; set; }

        public AnimationState() { }
        public AnimationState(EndianBinaryReader reader, Game game)
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
                    AnimationEffects[i] = new AnimationEffect(reader);
                reader.BaseStream.Position = currPos;
            }
        }

        public int EffectOffset;

        public void Serialize(EndianBinaryWriter writer, Game game)
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
    }

    public class AnimationEffect : GenericAssetDataContainer
    {
        public int StateID { get; set; }
        public AssetSingle StartTime { get; set; }
        public AssetSingle EndTime { get; set; }
        public int Flags { get; set; }
        public int EffectType { get; set; }
        public byte[] UserDataBytes { get; set; }

        public AnimationEffect() { }
        public AnimationEffect(EndianBinaryReader reader)
        {
            StateID = reader.ReadInt32();
            StartTime = reader.ReadSingle();
            EndTime = reader.ReadSingle();
            Flags = reader.ReadInt32();
            EffectType = reader.ReadInt32();
            var userDataSize = reader.ReadInt32();
            UserDataBytes = reader.ReadBytes(userDataSize);
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(StateID);
            writer.Write(StartTime);
            writer.Write(EndTime);
            writer.Write(Flags);
            writer.Write(EffectType);
            writer.Write(UserDataBytes.Length);
            writer.Write(UserDataBytes);
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
        [Category(categoryName)]
        public AssetID[] Animations { get; set; }
        [Category(categoryName)]
        public AnimationFile[] AnimationFiles { get; set; }

        [Category(categoryName)]
        public AnimationState[] AnimationStates { get; set; }

        public AssetATBL(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);

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

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
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
                        writer.Write(j);
                }

                writer.BaseStream.Position = filesStartPos;
                
                foreach (var animFile in AnimationFiles)
                    animFile.Serialize(writer);

                foreach (var animState in AnimationStates)
                    animState.Serialize(writer, game);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            foreach (var a in Animations)
                if (a == assetID)
                    return true;

            foreach (var a in AnimationStates)
                if (a.StateID == assetID)
                    return true;

            return false;
        }

        public override void Verify(ref List<string> result)
        {
            foreach (var a in Animations)
            {
                if (a == 0)
                    result.Add("ATBL entry with animation asset ID set to 0");
                Verify(a, ref result);
            }
        }
    }
}