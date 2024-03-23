using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public enum ezMissionStatus
    {
        Locked = 0,
        Unlocked = 1,
        completed = 2
    }

    public class DynaLogicMission : AssetDYNA
    {
        private const string dynaCategoryName = "logic:Mission";
        public override string TypeString => dynaCategoryName;
        protected override short constVersion => 2;

        private AssetID[] _requiredMission { get; set; }
        [Category(dynaCategoryName)]
        public AssetID[] RequiredMission
        {
            get => _requiredMission;
            set
            {
                List<AssetID> list = value.ToList();
                if (list.Count != 4)
                    MessageBox.Show("Array of RequiredMission AssetID's must have exactly 4 entries!");
                while (list.Count < 4)
                    list.Add(new AssetID(0));
                while (list.Count > 4)
                    list.RemoveAt(list.Count - 1);
                _requiredMission = list.ToArray();
            }
        }
        [Category(dynaCategoryName)]
        public ezMissionStatus InitialLockStatus { get; set; }
        [Category(dynaCategoryName)]
        public bool OnceOnly { get; set; }
        [Category(dynaCategoryName)]
        public bool LoadInSlot { get; set; }
        private AssetID[] _task { get; set; }
        [Category(dynaCategoryName)]
        public AssetID[] Task
        {
            get => _task;
            set
            {
                List<AssetID> list = value.ToList();
                if (list.Count != 16)
                    MessageBox.Show("Array of Task AssetID's must have exactly 16 entries!");
                while (list.Count < 16)
                    list.Add(new AssetID(0));
                while (list.Count > 16)
                    list.RemoveAt(list.Count - 1);
                _task = list.ToArray();
            }
        }
        [Category(dynaCategoryName)]
        public AssetID ExternalLoadID { get; set; }

        public DynaLogicMission(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.logic__Mission, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                RequiredMission = new AssetID[4];
                for (int i = 0; i < 4; i++)
                    RequiredMission[i] = reader.ReadUInt32();
                InitialLockStatus = (ezMissionStatus)reader.ReadUInt32();
                OnceOnly = reader.ReadByteBool();
                LoadInSlot = reader.ReadByteBool();
                reader.ReadUInt16();
                Task = new AssetID[16];
                for (int i = 0; i < 16; i++)
                    Task[i] = reader.ReadUInt32();
                ExternalLoadID = reader.ReadUInt32();

            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            for (int i = 0; i < 4; i++)
                writer.Write(RequiredMission[i]);
            writer.Write((uint)InitialLockStatus);
            writer.Write(OnceOnly);
            writer.Write(LoadInSlot);
            writer.Write((short)0);
            for (int i = 0; i < 16; i++)
                writer.Write(Task[i]);
            writer.Write(ExternalLoadID);
        }
    }
}