using HipHopFile;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace IndustrialPark
{
    public class AssetPARS : BaseAsset
    {
        private const string categoryName = "Particle System";
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Texture);

        [Category(categoryName)]
        public int Type { get; set; }
        [Category(categoryName), ValidReferenceRequired]
        public AssetID ParticleSystem { get; set; }
        [Category(categoryName), ValidReferenceRequired]
        public AssetID Texture { get; set; }
        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = ByteFlagsDescriptor();
        [Category(categoryName)]
        public byte Priority { get; set; }
        [Category(categoryName)]
        public short MaxParticles { get; set; }
        [Category(categoryName)]
        public byte RenderFunction { get; set; }
        [Category(categoryName)]
        public byte RenderSourceBlendMode { get; set; }
        [Category(categoryName)]
        public byte RenderDestBlendMode { get; set; }
        [Category(categoryName), Editor(typeof(AssetPropertyCollectionEditor), typeof(UITypeEditor))]
        public ParticleCommand[] Commands { get; set; }
        [Category(categoryName)]
        public FlagBitmask ParFlags { get; set; } = IntFlagsDescriptor();
        public AssetPARS(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                Type = reader.ReadInt32();
                ParticleSystem = reader.ReadUInt32();
                Texture = reader.ReadUInt32();
                Flags.FlagValueByte = reader.ReadByte();
                Priority = reader.ReadByte();
                MaxParticles = reader.ReadInt16();
                RenderFunction = reader.ReadByte();
                RenderSourceBlendMode = reader.ReadByte();
                RenderDestBlendMode = reader.ReadByte();
                int cmdCount = reader.ReadByte();
                int cmdSize = reader.ReadInt32();

                if (game >= Game.Incredibles)
                    ParFlags.FlagValueInt = reader.ReadUInt32();

                Commands = new ParticleCommand[cmdCount];
                for (int i = 0; i < Commands.Length; i++)
                {
                    var parType = (ParticleCommandType)reader.ReadInt32();

                    switch (parType)
                    {
                        case ParticleCommandType.Move:
                            Commands[i] = new ParticleCommand_Move(reader);
                            break;
                        case ParticleCommandType.MoveRandom:
                            Commands[i] = new ParticleCommand_MoveRandom(reader);
                            break;
                        case ParticleCommandType.MoveRandomPar:
                            Commands[i] = new ParticleCommand_MoveRandomPar(reader);
                            break;
                        case ParticleCommandType.Accelerate:
                            Commands[i] = new ParticleCommand_Accelerate(reader);
                            break;
                        case ParticleCommandType.VelocityApply:
                            Commands[i] = new ParticleCommand_VelocityApply(reader);
                            break;
                        case ParticleCommandType.Jet:
                            Commands[i] = new ParticleCommand_Jet(reader);
                            break;
                        case ParticleCommandType.KillSlow:
                            Commands[i] = new ParticleCommand_KillSlow(reader);
                            break;
                        case ParticleCommandType.Follow:
                            Commands[i] = new ParticleCommand_Follow(reader);
                            break;
                        case ParticleCommandType.OrbitPoint:
                            Commands[i] = new ParticleCommand_OrbitPoint(reader);
                            break;
                        case ParticleCommandType.OrbitLine:
                            Commands[i] = new ParticleCommand_OrbitLine(reader);
                            break;
                        case ParticleCommandType.Scale3rdPolyReg:
                            Commands[i] = new ParticleCommand_Scale3rdPolyReg(reader);
                            break;
                        case ParticleCommandType.Alpha3rdPolyReg:
                            Commands[i] = new ParticleCommand_Alpha3rdPolyReg(reader);
                            break;
                        case ParticleCommandType.Tex:
                            Commands[i] = new ParticleCommand_Tex(reader);
                            break;
                        case ParticleCommandType.TexAnim:
                            Commands[i] = new ParticleCommand_TexAnim(reader);
                            break;
                        case ParticleCommandType.PlayerCollision:
                            Commands[i] = new ParticleCommand_Scale3rdPolyReg(reader);
                            break;
                        case ParticleCommandType.RotPar:
                            Commands[i] = new ParticleCommand_RotPar(reader);
                            break;
                        case ParticleCommandType.RandomVelocityPar:
                            Commands[i] = new ParticleCommand_RandomVelocityPar(reader);
                            break;
                        case ParticleCommandType.Custom:
                            Commands[i] = new ParticleCommand_Custom(reader);
                            break;
                        case ParticleCommandType.KillDistance:
                            Commands[i] = new ParticleCommand_KillDistance(reader);
                            break;
                        case ParticleCommandType.Age:
                            Commands[i] = new ParticleCommand_Age(reader);
                            break;
                        case ParticleCommandType.ApplyWind:
                            Commands[i] = new ParticleCommand_ApplyWind(reader);
                            break;
                        case ParticleCommandType.ApplyCamMat:
                            Commands[i] = new ParticleCommand_ApplyCamMat(reader);
                            break;
                        case ParticleCommandType.RotateAround:
                            Commands[i] = new ParticleCommand_RotateAround(reader);
                            break;
                        case ParticleCommandType.SmokeAlpha:
                            Commands[i] = new ParticleCommand_SmokeAlpha(reader);
                            break;
                        case ParticleCommandType.Scale:
                            Commands[i] = new ParticleCommand_Scale(reader);
                            break;
                        case ParticleCommandType.ClipVolumes:
                            Commands[i] = new ParticleCommand_ClipVolumes(reader);
                            break;
                        case ParticleCommandType.AnimalMagentism:
                            Commands[i] = new ParticleCommand_AnimalMagentism(reader);
                            break;
                        case ParticleCommandType.DamagePlayer:
                            Commands[i] = new ParticleCommand_DamagePlayer(reader);
                            break;
                        case ParticleCommandType.CollideFall:
                            Commands[i] = new ParticleCommand_CollideFall(reader);
                            break;
                        case ParticleCommandType.FallSticky:
                            Commands[i] = new ParticleCommand_CollideFallSticky(reader);
                            break;
                        case ParticleCommandType.Shaper:
                            Commands[i] = new ParticleCommand_Shaper(reader);
                            break;
                        case ParticleCommandType.AlphaInOut:
                            Commands[i] = new ParticleCommand_AlphaInOut(reader);
                            break;
                        case ParticleCommandType.SizeInOut:
                            Commands[i] = new ParticleCommand_SizeInOut(reader);
                            break;
                        case ParticleCommandType.DampenSpeed:
                            Commands[i] = new ParticleCommand_DampenData(reader);
                            break;
                        default:
                            throw new Exception($"Unsupported Particle command: {parType}");
                    }
                }
            }
        }
        public override bool SkipBuildTesting => Commands.Any(i => i.NeedsFix == true);
        public override void Serialize(EndianBinaryWriter writer)
        {

            base.Serialize(writer);
            writer.Write(Type);
            writer.Write(ParticleSystem);
            writer.Write(Texture);
            writer.Write(Flags.FlagValueByte);
            writer.Write(Priority);
            writer.Write(MaxParticles);
            writer.Write(RenderFunction);
            writer.Write(RenderSourceBlendMode);
            writer.Write(RenderDestBlendMode);
            writer.Write((byte)Commands.Length);
            var cmdSizeStart = writer.BaseStream.Position;
            writer.Write(0);
            if (game >= Game.Incredibles)
                writer.Write(ParFlags.FlagValueInt);

            var cmdStart = writer.BaseStream.Position;
            foreach (var c in Commands)
                c.Serialize(writer);
            var cmdEnd = writer.BaseStream.Position;
            writer.BaseStream.Position = cmdSizeStart;
            writer.Write((int)(cmdEnd - cmdStart));
            writer.BaseStream.Position = cmdEnd;

            SerializeLinks(writer);

        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game < Game.Incredibles)
            {
                dt.RemoveProperty("ParFlags");
            }
        }

        public static ParticleCommand CreateEntry(ParticleCommandType type)
        {
            return type switch
            {
                ParticleCommandType.Move => new ParticleCommand_Move(),
                ParticleCommandType.MoveRandom => new ParticleCommand_MoveRandom(),
                ParticleCommandType.MoveRandomPar => new ParticleCommand_MoveRandomPar(),
                ParticleCommandType.Accelerate => new ParticleCommand_Accelerate(),
                ParticleCommandType.VelocityApply => new ParticleCommand_VelocityApply(),
                ParticleCommandType.Jet => new ParticleCommand_Jet(),
                ParticleCommandType.KillSlow => new ParticleCommand_KillSlow(),
                ParticleCommandType.Follow => new ParticleCommand_Follow(),
                ParticleCommandType.OrbitPoint => new ParticleCommand_OrbitPoint(),
                ParticleCommandType.OrbitLine => new ParticleCommand_OrbitLine(),
                ParticleCommandType.Scale3rdPolyReg => new ParticleCommand_Scale3rdPolyReg(),
                ParticleCommandType.Alpha3rdPolyReg => new ParticleCommand_Alpha3rdPolyReg(),
                ParticleCommandType.Tex => new ParticleCommand_Tex(),
                ParticleCommandType.TexAnim => new ParticleCommand_TexAnim(),
                ParticleCommandType.PlayerCollision => new ParticleCommand_PlayerCollision(),
                ParticleCommandType.RotPar => new ParticleCommand_RotPar(),
                ParticleCommandType.RandomVelocityPar => new ParticleCommand_RandomVelocityPar(),
                ParticleCommandType.Custom => new ParticleCommand_Custom(),
                ParticleCommandType.KillDistance => new ParticleCommand_KillDistance(),
                ParticleCommandType.Age => new ParticleCommand_Age(),
                ParticleCommandType.ApplyWind => new ParticleCommand_ApplyWind(),
                ParticleCommandType.ApplyCamMat => new ParticleCommand_ApplyCamMat(),
                ParticleCommandType.RotateAround => new ParticleCommand_RotateAround(),
                ParticleCommandType.SmokeAlpha => new ParticleCommand_SmokeAlpha(),
                ParticleCommandType.Scale => new ParticleCommand_Scale(),
                ParticleCommandType.ClipVolumes => new ParticleCommand_ClipVolumes(),
                ParticleCommandType.AnimalMagentism => new ParticleCommand_AnimalMagentism(),
                ParticleCommandType.DamagePlayer => new ParticleCommand_DamagePlayer(),
                ParticleCommandType.CollideFall => new ParticleCommand_CollideFall(),
                ParticleCommandType.FallSticky => new ParticleCommand_CollideFallSticky(),
                ParticleCommandType.Shaper => new ParticleCommand_Shaper(),
                ParticleCommandType.AlphaInOut => new ParticleCommand_AlphaInOut(),
                ParticleCommandType.SizeInOut => new ParticleCommand_SizeInOut(),
                ParticleCommandType.DampenSpeed => new ParticleCommand_DampenData(),
                _ => throw new Exception($"Unsupported Particle command: {type}"),
            };
        }
    }
}