using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public enum ezTaskAvailability
    {
        Unavailable = 0,
        Available = 1
    }

    public class DynaLogicTask : AssetDYNA
    {
        private const string dynaCategoryName = "logic:Task";
        public override string TypeString => dynaCategoryName;
        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public AssetID CheckpointID { get; set; }
        [Category(dynaCategoryName)]
        public bool Required { get; set; }
        [Category(dynaCategoryName)]
        public ezTaskAvailability InitialAvailability { get; set; }
        private AssetID[] _requiredTask;
        [Category(dynaCategoryName)]
        public AssetID[] RequiredTask
        {
            get => _requiredTask;
            set
            {
                List<AssetID> list = value.ToList();
                if (list.Count != 16)
                    MessageBox.Show("Array of RequiredTask AssetID's must have exactly 16 entries!");
                while (list.Count < 16)
                    list.Add(new AssetID(0));
                while (list.Count > 16)
                    list.RemoveAt(list.Count - 1);
                _requiredTask = list.ToArray();
            }
        }

        public DynaLogicTask(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.logic__Task, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                CheckpointID = reader.ReadUInt32();
                Required = reader.ReadByteBool();
                reader.BaseStream.Position += 3;
                InitialAvailability = (ezTaskAvailability)reader.ReadUInt32();
                RequiredTask = new AssetID[16];
                for (int i = 0; i < 16; i++)
                    RequiredTask[i] = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(CheckpointID);
            writer.Write(Required);
            writer.Write(new byte[3]);
            writer.Write((uint)InitialAvailability);
            for (int i = 0; i < 16; i++)
                writer.Write(RequiredTask[i]);
        }
    }
}