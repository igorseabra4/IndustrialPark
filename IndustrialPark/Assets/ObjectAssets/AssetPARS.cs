using HipHopFile;
using SharpDX;
using System;
using System.ComponentModel;
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
        [Category(categoryName)]
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
                            break; ;
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

        public void AddEntry(ParticleCommandType type)
        {
            ParticleCommand parCmd;
            switch (type)
            {
                case ParticleCommandType.Move:
                    parCmd = new ParticleCommand_Move();
                    break;
                case ParticleCommandType.MoveRandom:
                    parCmd = new ParticleCommand_MoveRandom();
                    break;
                case ParticleCommandType.MoveRandomPar:
                    parCmd = new ParticleCommand_MoveRandomPar();
                    break;
                case ParticleCommandType.Accelerate:
                    parCmd = new ParticleCommand_Accelerate();
                    break;
                case ParticleCommandType.VelocityApply:
                    parCmd = new ParticleCommand_VelocityApply();
                    break;
                case ParticleCommandType.Jet:
                    parCmd = new ParticleCommand_Jet();
                    break;
                case ParticleCommandType.KillSlow:
                    parCmd = new ParticleCommand_KillSlow();
                    break;
                case ParticleCommandType.Follow:
                    parCmd = new ParticleCommand_Follow();
                    break;
                case ParticleCommandType.OrbitPoint:
                    parCmd = new ParticleCommand_OrbitPoint();
                    break;
                case ParticleCommandType.OrbitLine:
                    parCmd = new ParticleCommand_OrbitLine();
                    break;
                case ParticleCommandType.Scale3rdPolyReg:
                    parCmd = new ParticleCommand_Scale3rdPolyReg();
                    break;
                case ParticleCommandType.Alpha3rdPolyReg:
                    parCmd = new ParticleCommand_Alpha3rdPolyReg();
                    break;
                case ParticleCommandType.Tex:
                    parCmd = new ParticleCommand_Tex();
                    break;
                case ParticleCommandType.TexAnim:
                    parCmd = new ParticleCommand_TexAnim();
                    break;
                case ParticleCommandType.PlayerCollision:
                    parCmd = new ParticleCommand_PlayerCollision();
                    break;
                case ParticleCommandType.RotPar:
                    parCmd = new ParticleCommand_RotPar();
                    break;
                case ParticleCommandType.RandomVelocityPar:
                    parCmd = new ParticleCommand_RandomVelocityPar();
                    break;
                case ParticleCommandType.Custom:
                    parCmd = new ParticleCommand_Custom();
                    break;
                case ParticleCommandType.KillDistance:
                    parCmd = new ParticleCommand_KillDistance();
                    break;
                case ParticleCommandType.Age:
                    parCmd = new ParticleCommand_Age();
                    break;
                case ParticleCommandType.ApplyWind:
                    parCmd = new ParticleCommand_ApplyWind();
                    break;
                case ParticleCommandType.ApplyCamMat:
                    parCmd = new ParticleCommand_ApplyCamMat();
                    break;
                case ParticleCommandType.RotateAround:
                    parCmd = new ParticleCommand_RotateAround();
                    break;
                case ParticleCommandType.SmokeAlpha:
                    parCmd = new ParticleCommand_SmokeAlpha();
                    break;
                case ParticleCommandType.Scale:
                    parCmd = new ParticleCommand_Scale();
                    break;
                case ParticleCommandType.ClipVolumes:
                    parCmd = new ParticleCommand_ClipVolumes();
                    break;
                case ParticleCommandType.AnimalMagentism:
                    parCmd = new ParticleCommand_AnimalMagentism();
                    break;
                case ParticleCommandType.DamagePlayer:
                    parCmd = new ParticleCommand_DamagePlayer();
                    break;
                case ParticleCommandType.CollideFall:
                    parCmd = new ParticleCommand_CollideFall();
                    break;
                case ParticleCommandType.FallSticky:
                    parCmd = new ParticleCommand_CollideFallSticky();
                    break;
                case ParticleCommandType.Shaper:
                    parCmd = new ParticleCommand_Shaper();
                    break;
                case ParticleCommandType.AlphaInOut:
                    parCmd = new ParticleCommand_AlphaInOut();
                    break;
                case ParticleCommandType.SizeInOut:
                    parCmd = new ParticleCommand_SizeInOut();
                    break;
                case ParticleCommandType.DampenSpeed:
                    parCmd = new ParticleCommand_DampenData();
                    break;
                default:
                    throw new Exception($"Unsupported Particle command: {type}");
            }
            var commands = Commands.ToList();
            commands.Add(parCmd);
            Commands = commands.ToArray();
        }
    }
}