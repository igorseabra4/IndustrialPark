using System.Collections.Generic;
using System.ComponentModel;
using HipHopFile;

namespace IndustrialPark
{
    public class DestState : GenericAssetDataContainer
    {  
        public int percent { get; set; }
        public AssetID Model_AssetID { get; set; }
        public AssetID Shrapnel_Destroy_AssetID { get; set; }
        public AssetID Shrapnel_Hit_AssetID { get; set; }
        public AssetID SoundGroup_Idle_AssetID { get; set; }
        public AssetID SoundGroup_Fx_AssetID { get; set; }
        public AssetID SoundGroup_Hit_AssetID { get; set; }
        public AssetID SoundGroup_Fx_Switch_AssetID { get; set; }
        public AssetID SoundGroup_Hit_Switch_AssetID { get; set; }
        public AssetID Rumble_Hit_AssetID { get; set; }
        public AssetID Rumble_Switch_AssetID { get; set; }
        public int FxFlags { get; set; }
        public int nAnimations { get; set; }
        public AssetID Animation_AssetID { get; set; }

        public DestState() { }
        public DestState(EndianBinaryReader reader)
        {
            percent = reader.ReadInt32();
            Model_AssetID = reader.ReadUInt32();
            Shrapnel_Destroy_AssetID = reader.ReadUInt32();
            Shrapnel_Hit_AssetID = reader.ReadUInt32();
            SoundGroup_Idle_AssetID = reader.ReadUInt32();
            SoundGroup_Fx_AssetID = reader.ReadUInt32();
            SoundGroup_Hit_AssetID = reader.ReadUInt32();
            SoundGroup_Fx_Switch_AssetID = reader.ReadUInt32();
            SoundGroup_Hit_Switch_AssetID = reader.ReadUInt32();
            Rumble_Hit_AssetID = reader.ReadUInt32();
            Rumble_Switch_AssetID = reader.ReadUInt32();
            FxFlags = reader.ReadInt32();
            nAnimations = reader.ReadInt32();
            Animation_AssetID = reader.ReadUInt32();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(percent);
            writer.Write(Model_AssetID);
            writer.Write(Shrapnel_Destroy_AssetID);
            writer.Write(Shrapnel_Hit_AssetID);
            writer.Write(SoundGroup_Idle_AssetID);
            writer.Write(SoundGroup_Fx_AssetID);
            writer.Write(SoundGroup_Hit_AssetID);
            writer.Write(SoundGroup_Fx_Switch_AssetID);
            writer.Write(SoundGroup_Hit_Switch_AssetID);
            writer.Write(Rumble_Hit_AssetID);
            writer.Write(Rumble_Switch_AssetID);
            writer.Write(FxFlags);
            writer.Write(nAnimations);
            writer.Write(Animation_AssetID);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) =>
            Model_AssetID == assetID ||
            Shrapnel_Destroy_AssetID == assetID ||
            Shrapnel_Hit_AssetID == assetID ||
            SoundGroup_Idle_AssetID == assetID ||
            SoundGroup_Fx_AssetID == assetID ||
            SoundGroup_Hit_AssetID == assetID ||
            SoundGroup_Fx_Switch_AssetID == assetID ||
            SoundGroup_Hit_Switch_AssetID == assetID ||
            Rumble_Hit_AssetID == assetID ||
            Rumble_Switch_AssetID == assetID ||
            Animation_AssetID == assetID;
    }

    public class AssetDEST : Asset
    {
        private const string categoryName = "Destructible";

        [Category(categoryName)]
        public AssetID MINF_AssetID { get; set; }
        [Category(categoryName)]
        public int HitPoints { get; set; }
        [Category(categoryName)]
        public int HitFilter { get; set; }
        [Category(categoryName)]
        public int LaunchFlag { get; set; }
        [Category(categoryName)]
        public int Behavior { get; set; }
        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public AssetID SoundGroup_Idle_AssetID { get; set; }
        [Category(categoryName)]
        public AssetSingle Respawn { get; set; }
        [Category(categoryName)]
        public byte TargetPriority { get; set; }
        [Category(categoryName)]
        public DestState[] States { get; set; }

        public AssetDEST(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);

            MINF_AssetID = reader.ReadUInt32();
            int numStates = reader.ReadInt32();
            HitPoints = reader.ReadInt32();
            HitFilter = reader.ReadInt32();
            LaunchFlag = reader.ReadInt32();
            Behavior = reader.ReadInt32();
            Flags.FlagValueInt = reader.ReadUInt32();
            SoundGroup_Idle_AssetID = reader.ReadUInt32();
            Respawn = reader.ReadSingle();
            TargetPriority = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            States = new DestState[numStates];
            for (int i = 0; i < States.Length; i++)
                States[i] = new DestState(reader);
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(MINF_AssetID);
            writer.Write(States.Length);
            writer.Write(HitPoints);
            writer.Write(HitFilter);
            writer.Write(LaunchFlag);
            writer.Write(Behavior);
            writer.Write(Flags.FlagValueInt);
            writer.Write(SoundGroup_Idle_AssetID);
            writer.Write(Respawn);
            writer.Write(TargetPriority);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            foreach (var state in States)
                writer.Write(state.Serialize(game, platform));

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            if (MINF_AssetID == assetID)
                return true;
            foreach (var s in States)
                if (s.HasReference(assetID))
                    return true;

            return false;
        }
        
        public override void Verify(ref List<string> result)
        {
            Verify(MINF_AssetID, ref result);

            if (MINF_AssetID == 0)
                result.Add("DEST with MINF_AssetID set to 0");
        }
    }
}