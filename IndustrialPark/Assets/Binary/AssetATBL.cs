using HipHopFile;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace IndustrialPark
{
    public enum ConstructFunc_BFBB : uint
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

    public enum ConstructFunc_TSSM : uint
    {
        CruiseBubble = 0x19FB3325,
        NME_TYPE_COMMON = 0x5E693043,
        NME_TYPE_TIKI = 0x55D49953,
        NME_TYPE_TIKI_WOOD = 0xD68C117D,
        NME_TYPE_TIKI_LOVEY = 0x0897F397,
        NME_TYPE_TIKI_QUIET = 0x6126ED54,
        NME_TYPE_TIKI_THUNDER = 0xC6B08BF8,
        NME_TYPE_TIKI_STONE = 0x8421A31D,
        NME_TYPE_FIRST_SEE_TYPE = 0x463D31C1,
        NME_TYPE_CRITTER = 0xFD1B257F,
        NME_TYPE_CRITBASIC = 0xC444EF00,
        NME_TYPE_CRITJELLY = 0x513A0454,
        NME_TYPE_BUCKETJELLY = 0x6714A3B4,
        NME_TYPE_TURRET = 0x7C3146E8,
        NME_TYPE_TURBARREL = 0x704F274B,
        NME_TYPE_TURBARREL_V1 = 0x06CD65F3,
        NME_TYPE_TURBARREL_V2 = 0x06CD65F4,
        NME_TYPE_TURBARREL_V3 = 0x06CD65F5,
        NME_TYPE_TURSPIRAL = 0x2A311C2E,
        NME_TYPE_TURPOPUP = 0x2184F13F,
        NME_TYPE_TURTURNER = 0x7EADA797,
        NME_TYPE_TURARTY = 0x8AF3893B,
        NME_TYPE_TURTRACE = 0x681EBC8C,
        NME_TYPE_STANDARD = 0x1D0AE151,
        NME_TYPE_FOGGER = 0x5026A9AA,
        NME_TYPE_FOGGER_V1 = 0xB11DE878,
        NME_TYPE_FOGGER_V2 = 0xB11DE879,
        NME_TYPE_FOGGER_V3 = 0xB11DE87A,
        NME_TYPE_SLAMMER = 0xD5CB7759,
        NME_TYPE_SLAMMER_V1 = 0x7F02146D,
        NME_TYPE_SLAMMER_V2 = 0x7F02146E,
        NME_TYPE_SLAMMER_V3 = 0x7F02146F,
        NME_TYPE_FLINGER = 0x3547FB41,
        NME_TYPE_FLINGER_V1 = 0x47E2B9E5,
        NME_TYPE_FLINGER_V2 = 0x47E2B9E6,
        NME_TYPE_FLINGER_V3 = 0x47E2B9E7,
        NME_TYPE_SPINNER = 0x506CE851,
        NME_TYPE_SPINNER_V1 = 0x4EAC1295,
        NME_TYPE_SPINNER_V2 = 0x4EAC1296,
        NME_TYPE_SPINNER_V3 = 0x4EAC1297,
        NME_TYPE_POPPER = 0x2488B1EC,
        NME_TYPE_POPPER_V1 = 0xDDB8C26E,
        NME_TYPE_POPPER_V2 = 0xDDB8C26F,
        NME_TYPE_POPPER_V3 = 0xDDB8C270,
        NME_TYPE_ZAPPER_V1 = 0x50D4FA1E,
        NME_TYPE_ZAPPER_V2 = 0x50D4FA1F,
        NME_TYPE_ZAPPER_V3 = 0x50D4FA20,
        NME_TYPE_MERVYN = 0x82B8E80D,
        NME_TYPE_MERVYN_V1 = 0x95937569,
        NME_TYPE_MERVYN_V2 = 0x9593756A,
        NME_TYPE_MERVYN_V3 = 0x9593756B,
        NME_TYPE_BUCKOTRON = 0xE44C6011,
        NME_TYPE_BUCKOTRON_V1 = 0xF57E53D5,
        NME_TYPE_BUCKOTRON_V2 = 0xF57E53D6,
        NME_TYPE_BUCKOTRON_V3 = 0xF57E53D7,
        NME_TYPE_BUCKOTRON_V4 = 0xF57E53D8,
        NME_TYPE_BUCKOTRON_V5 = 0xF57E53D9,
        NME_TYPE_BUCKOTRON_V6 = 0xF57E53DA,
        NME_TYPE_BUCKOTRON_V7 = 0xF57E53DB,
        NME_TYPE_LAST_SEE_TYPE = 0xE09D874F,
        NME_TYPE_FROGFISH = 0xEA1DC128,
        NME_TYPE_DENNIS = 0xAA86EA23,
        NME_TYPE_DENNIS_V1 = 0xECDB56BB,
        NME_TYPE_DENNIS_V2 = 0xECDB56BC,
        NME_TYPE_NEPTUNE = 0x75F046C9,
        NME_TYPE_SBBAT = 0xD94A40D6,
        NME_TYPE_TONGUESPIN = 0x4E48554E,
        NME_TYPE_MINDY = 0x70EB4027,
        NME_TYPE_NPC_PAT = 0x61E54281,
        NME_TYPE_NPC_BOB = 0x61E19F1B,
        PlayerCar = 0x60333F13,
        PlayerPat = 0x6036A68A,
        PlayerSB = 0xF502609C,
        PlayerSBTongue = 0xF8A4C574,
        PlayerSlide = 0xE5A1B324,
        SpongeBall = 0xF09A1415,
        xHUD = 0x0BDDB393,
    }

    public enum ConstructEnum_Incredibles : uint
    {
        NPC_TYPE_UNKNOWN = 0xB5C7DEBF,
        NPC_TYPE_BASIC = 0x331F9E57,
        NPC_TYPE_COMMON = 0x19A58B5A,
        NPC_TYPE_MELEE = 0xF4BDD2F5,
        NPC_TYPE_MELEE_BOT = 0xE6C72B1D,
        NPC_TYPE_MINE = 0x639510BA,
        NPC_TYPE_ORACLE = 0x16E3F481,
        NPC_TYPE_TURRET = 0x376DA1FF,
        NPC_TYPE_SPIN_TURRET = 0x813D203A,
        NPC_TYPE_SHOOTER = 0xADA34F35,
        NPC_TYPE_VIOLET_GUN = 0x88993383,
        NPC_TYPE_SECURITY_BIRD = 0xC24E1849,
        NPC_TYPE_STREAMER = 0x936AE8EC,
        NPC_TYPE_WATER_STREAMER = 0x709594F8,
        NPC_TYPE_CAR = 0xA2F2C739,
        NPC_TYPE_FRIENDLY = 0x43EFE2CA,
        NPC_TYPE_TANK = 0x648317B5,
        NPC_TYPE_MELEE_SHIELDED = 0xA2B13CB2,
        NPC_TYPE_LOBBER = 0xEFCFF63F,
        NPC_TYPE_FLY_ROCKET = 0x13D1CC07,
        NPC_TYPE_FLY_SHOOTER = 0xCBA19BAD,
        NPC_TYPE_FLY_LOBBER = 0x2E935E67,
        NPC_TYPE_VELOCIPOD = 0x758322BE,
        NPC_TYPE_VELOCIPOD_DASH = 0xF5D2BA9F,
        NPC_TYPE_MISSLE = 0x844D9A38,
        NPC_TYPE_HELIBOT = 0xC1EE1472,
        NPC_TYPE_VIPER = 0x93437E6B,
        NPC_TYPE_BOT_LOB_WATER = 0x9FB058C0,
        NPC_TYPE_BOT_LEAP = 0xDF107367,
        NPC_TYPE_MONOPOD = 0x4082FADF,
        NPC_TYPE_INCREDIBALL = 0x353B027E,
        NPC_TYPE_FROZONE = 0x553462B8,
        NPC_TYPE_ELASTIGIRL = 0x2450E369,
        NPC_TYPE_BOSS_OMNIDROID = 0x9419B3B8,
        NPC_TYPE_BOSS_OMNIDROID10 = 0xEEF092BB,
        NPC_TYPE_BOSS_BOMB_CHOPPER = 0x0AE14299,
        PLAYER_TYPE_MR_INCREDIBLE_YOUNG = 0x01E200B9,
        PLAYER_TYPE_MR_INCREDIBLE_DISGUISED = 0xCEB83B5A,
        PLAYER_TYPE_MR_INCREDIBLE_OLD_FAT_BLUE = 0x8BAE6887,
        PLAYER_TYPE_MR_INCREDIBLE_OLD_FAT_RED = 0xBEA3CC00,
        PLAYER_TYPE_MR_INCREDIBLE_OLD_FIT = 0x0D79673E,
        PLAYER_TYPE_ELASTI_GIRL_YOUNG = 0xCB3B7F2B,
        PLAYER_TYPE_ELASTI_GIRL_OLD = 0x2AEB5BB6,
        PLAYER_TYPE_DASH_REGULAR = 0xF96CE25E,
        PLAYER_TYPE_DASH_COSTUMED = 0x6C99577E,
        PLAYER_TYPE_INCREDI_BALL = 0x9817A993,
        PLAYER_TYPE_VIOLET = 0x3892E3BA
    }

    public enum ConstructEnum_ROTU : uint
    {
        Unknown = 0,
        NPC_TYPE_UNKNOWN = 0xB5C7DEBF,
        NPC_TYPE_HUMANOID = 0xF8B06AB8,
        NPC_TYPE_RAT = 0xA2F6B4C2,
        NPC_TYPE_CHICKEN = 0xF7894D26,
        NPC_TYPE_DRILLER = 0x809BD8FD,
        NPC_TYPE_SHOOTER = 0xADA34F35,
        NPC_TYPE_BOMBER = 0x1E1E596A,
        NPC_TYPE_ROBOTTANK = 0xD81BF935,
        NPC_TYPE_ENFORCER = 0xB744DFF3,
        NPC_TYPE_SCIENTIST = 0x0CCF63BD,
        NPC_TYPE_BOSSUNDERMINERDRILL = 0x51C97958,
        NPC_TYPE_BOSSUNDERMINERUM = 0xA3406B63,
        PLAYER_TYPE_MR_INCREDIBLE_OLD_FIT = 0xE51FD4DA,
        PLAYER_TYPE_FROZONE = 0x818A4BB0,
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
        public AnimTableStates StateID_Enum
        {
            get => Enum.GetValues(typeof(AnimTableStates)).Cast<AnimTableStates>().DefaultIfEmpty(AnimTableStates.Unknown).FirstOrDefault(p => StateID.Equals((uint)p));
            set
            {
                StateID = (uint)value;
            }
        }
        public int AnimFileIndex { get; set; }
        public AssetSingle Speed { get; set; }
        public AssetID SubStateID { get; set; }
        public AnimTableStates SubStateID_Enum
        {
            get => Enum.GetValues(typeof(AnimTableStates)).Cast<AnimTableStates>().DefaultIfEmpty(AnimTableStates.Unknown).FirstOrDefault(p => SubStateID.Equals((uint)p));
            set
            {
                SubStateID = (uint)value;
            }
        }
        public int SubStateCount { get; set; }

        private AnimationEffect[] _animationeffects;
        [Editor(typeof(DynamicTypeDescriptorCollectionEditor), typeof(UITypeEditor))]
        public AnimationEffect[] AnimationEffects
        {
            get
            {
                DynamicTypeDescriptorCollectionEditor.game = game;
                return _animationeffects;
            }
            set => _animationeffects = value;
        }

        public AnimationState(Game game)
        {
            _game = game;
            _animationeffects = new AnimationEffect[0];
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
                SubStateID = reader.ReadUInt32();
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

        public override string ToString()
        {
            return $"[{StateID_Enum}]";
        }
    }

    public class AnimationEffect : GenericAssetDataContainer
    {
        public AssetID StateID { get; set; }
        public AnimTableStates StateID_Enum
        {
            get => Enum.GetValues(typeof(AnimTableStates)).Cast<AnimTableStates>().DefaultIfEmpty(AnimTableStates.Unknown).FirstOrDefault(p => StateID.Equals((uint)p));
            set
            {
                StateID = (uint)value;
            }
        }
        public AssetSingle StartTime { get; set; }
        public AssetSingle EndTime { get; set; }
        public FlagBitmask Flags_BFBB { get; set; } = IntFlagsDescriptor();
        public FlagBitmask Flags_TSSM { get; set; } = ByteFlagsDescriptor();
        public int EffectType_BFBB { get; set; }
        public AssetByte EffectType_TSSM { get; set; }
        public AssetByte Probability { get; set; }
        public byte[] UserDataBytes { get; set; }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game >= Game.Incredibles)
            {
                dt.RemoveProperty("EffectType_BFBB");
                dt.RemoveProperty("Flags_BFBB");
            }
            else
            {
                dt.RemoveProperty("EffectType_TSSM");
                dt.RemoveProperty("Flags_TSSM");
                dt.RemoveProperty("Probability");
            }
        }

        public AnimationEffect(Game game)
        {
            _game = game;

            UserDataBytes = new byte[0];
        }

        public AnimationEffect(EndianBinaryReader reader, Game game) : this(game)
        {
            StateID = reader.ReadUInt32();
            StartTime = reader.ReadSingle();
            EndTime = reader.ReadSingle();
            if (game < Game.Incredibles)
            {
                Flags_BFBB.FlagValueInt = reader.ReadUInt32();
                EffectType_BFBB = reader.ReadInt32();
                int userDataSize = reader.ReadInt32();
                UserDataBytes = reader.ReadBytes(userDataSize);
            }
            else
            {
                int userDataSize = reader.ReadInt32();
                Flags_TSSM.FlagValueByte = reader.ReadByte();
                EffectType_TSSM = reader.ReadByte();
                Probability = reader.ReadByte();
                reader.ReadByte();
                UserDataBytes = reader.ReadBytes(userDataSize);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(StateID);
            writer.Write(StartTime);
            writer.Write(EndTime);

            if (game < Game.Incredibles)
            {
                writer.Write(Flags_BFBB.FlagValueInt);
                writer.Write(EffectType_BFBB);
                writer.Write(UserDataBytes.Length);
                writer.Write(UserDataBytes);
            }
            else
            {
                writer.Write(UserDataBytes.Length);
                writer.Write(Flags_TSSM.FlagValueByte);
                writer.Write(EffectType_TSSM);
                writer.Write(Probability);
                writer.Write((byte)0xCC);
                writer.Write(UserDataBytes);
            }
        }
    }

    public class AssetATBL : Asset
    {
        private const string categoryName = "Animation Table";

        [Category(categoryName)]
        public ConstructFunc_BFBB ConstructFunc_BFBB
        {
            get => (ConstructFunc_BFBB)(uint)ConstructFunc_Hash;
            set => ConstructFunc_Hash = (uint)value;
        }
        [Category(categoryName)]
        public ConstructFunc_TSSM ConstructFunc_TSSM
        {
            get => (ConstructFunc_TSSM)(uint)ConstructFunc_Hash;
            set => ConstructFunc_Hash = (uint)value;
        }
        [Category(categoryName)]
        public ConstructEnum_Incredibles ConstructFunc_Incredibles
        {
            get => (ConstructEnum_Incredibles)(uint)ConstructFunc_Hash;
            set => ConstructFunc_Hash = (uint)value;
        }
        [Category(categoryName)]
        public ConstructEnum_ROTU ConstructFunc_ROTU
        {
            get => (ConstructEnum_ROTU)(uint)ConstructFunc_Hash;
            set => ConstructFunc_Hash = (uint)value;
        }

        [Category(categoryName)]
        public AssetID ConstructFunc_Hash { get; set; }
        [Category(categoryName), ValidReferenceRequired]
        public AssetID[] Animations { get; set; }
        [Category(categoryName)]
        public AnimationFile[] AnimationFiles { get; set; }

        private AnimationState[] _animationstate;
        [Category(categoryName), Editor(typeof(DynamicTypeDescriptorCollectionEditor), typeof(UITypeEditor))]
        public AnimationState[] AnimationStates
        {
            get
            {
                DynamicTypeDescriptorCollectionEditor.game = game;
                return _animationstate;
            }
            set => _animationstate = value;
        }

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

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game != Game.BFBB)
                dt.RemoveProperty("ConstructFunc_BFBB");
            if (game != Game.Incredibles)
            {
                dt.RemoveProperty("ConstructFunc_TSSM");
                dt.RemoveProperty("ConstructFunc_Incredibles");
            }
            if (game != Game.ROTU)
                dt.RemoveProperty("ConstructFunc_ROTU");
        }
    }
}