using HipHopFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IndustrialPark
{
    public enum Endianness
    {
        Little,
        Big
    }

    public class EndianConverter
    {
        public static Endianness PlatformEndianness(Platform platform) => platform == Platform.GameCube ? Endianness.Big : Endianness.Little;

        public static Section_AHDR GetReversedEndian(Section_AHDR AHDR, Game previousGame, Game currentGame, Endianness previousEndianness)
        {
            EndianConverter e = new EndianConverter
            {
                previousGame = previousGame,
                newGame = currentGame,
                previousEndianness = previousEndianness,
                reader = new BinaryReader(new MemoryStream(AHDR.data))
            };
            return e.GetReversedEndian(AHDR);
        }

        public static byte[] GetLinksReversedEndian(byte[] data, int count = 1)
        {
            EndianConverter e = new EndianConverter
            {
                reader = new BinaryReader(new MemoryStream(data))
            };

            List<byte> bytes = new List<byte>();
            e.ReverseLinks(ref bytes, count);

            return bytes.ToArray();
        }

        public static byte[] GetTimedLinksReversedEndian(byte[] data, int count = 1)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(data));
            List<byte> bytes = new List<byte>();

            for (int i = 0; i < count; i++)
                for (int j = 0; j < 8; j++)
                    bytes.AddRange(Reverse(reader.ReadInt32()));
            
            return bytes.ToArray();
        }

        private Game previousGame;
        private Game newGame;
        private Endianness previousEndianness;
        private BinaryReader reader;

        private EndianConverter()
        {
        }

        private Section_AHDR GetReversedEndian(Section_AHDR AHDR)
        {
            reader.BaseStream.Position = 0;
            List<byte> bytes = new List<byte>();

            switch (AHDR.assetType)
            {
                case AssetType.ALST:
                case AssetType.COLL:
                case AssetType.LKIT:
                case AssetType.LODT:
                case AssetType.MAPR:
                case AssetType.MRKR:
                case AssetType.PIPT:
                case AssetType.SHDW:
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                        bytes.AddRange(Reverse(reader.ReadInt32()));
                    break;
                case AssetType.BSP:
                case AssetType.JSP:
                case AssetType.MODL:
                case AssetType.RWTX:
                case AssetType.FLY:
                    AHDR.data = ((MemoryStream)reader.BaseStream).ToArray();
                    return AHDR;
                case AssetType.BOUL:
                    ReverseBOUL(ref bytes); break;
                case AssetType.BUTN:
                    ReverseBUTN(ref bytes); break;
                case AssetType.CAM:
                    ReverseCAM(ref bytes); break;
                case AssetType.CNTR:
                    ReverseCNTR(ref bytes); break;
                case AssetType.COND:
                    ReverseCOND(ref bytes); break;
                case AssetType.CSNM:
                    ReverseCSNM(ref bytes); break;
                case AssetType.DPAT:
                    ReverseDPAT(ref bytes); break;
                case AssetType.DSTR:
                    ReverseDSTR(ref bytes); break;
                case AssetType.DYNA:
                    ReverseDYNA(ref bytes); break;
                case AssetType.EGEN:
                    ReverseEGEN(ref bytes); break;
                case AssetType.ENV:
                    ReverseENV(ref bytes); break;
                case AssetType.FOG:
                    ReverseFOG(ref bytes); break;
                case AssetType.GRUP:
                    ReverseGRUP(ref bytes); break;
                case AssetType.GUST:
                    ReverseGUST(ref bytes); break;
                case AssetType.HANG:
                    ReverseHANG(ref bytes); break;
                case AssetType.MVPT:
                    ReverseMVPT(ref bytes); break;
                case AssetType.PEND:
                    ReversePEND(ref bytes); break;
                case AssetType.PKUP:
                    ReversePKUP(ref bytes); break;
                case AssetType.PLAT:
                    ReversePLAT(ref bytes); break;
                case AssetType.PLYR:
                    ReversePLYR(ref bytes); break;
                case AssetType.PORT:
                    ReversePORT(ref bytes); break;
                case AssetType.PRJT:
                    ReversePRJT(ref bytes); break;
                case AssetType.SCRP:
                    ReverseSCRP(ref bytes); break;
                case AssetType.SFX:
                    ReverseSFX(ref bytes); break;
                case AssetType.SGRP:
                    ReverseSGRP(ref bytes); break;
                case AssetType.SIMP:
                    ReverseSIMP(ref bytes); break;
                case AssetType.TEXT:
                    ReverseTEXT(ref bytes); break;
                case AssetType.TIMR:
                    ReverseTIMR(ref bytes); break;
                case AssetType.TRIG:
                    ReverseTRIG(ref bytes); break;
                case AssetType.UI:
                    ReverseUI(ref bytes); break;
                case AssetType.UIFT:
                    ReverseUIFT(ref bytes); break;
                case AssetType.VIL:
                    ReverseVIL(ref bytes); break;
                default:
                    throw new ArgumentException("Unsupported asset type for type conversion: " + AHDR.assetType.ToString());
            }

            AHDR.data = bytes.ToArray();
            return AHDR;
        }

        public static IEnumerable<byte> Reverse(int value) => BitConverter.GetBytes(value).Reverse();
        public static IEnumerable<byte> Reverse(float value) => BitConverter.GetBytes(value).Reverse();
        public static IEnumerable<byte> Reverse(short value) => BitConverter.GetBytes(value).Reverse();

        private void ReverseObject(ref List<byte> bytes)
        {
            bytes.AddRange(Reverse(reader.ReadInt32()));
            bytes.Add(reader.ReadByte());
            bytes.Add(reader.ReadByte());
            bytes.AddRange(Reverse(reader.ReadInt16()));
        }

        private void ReversePlaceable(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);

            for (int i = 0; i < 4; i++)
                bytes.Add(reader.ReadByte());

            if (previousGame == Game.BFBB && newGame != Game.BFBB)
                reader.BaseStream.Position += 4;
            else if (previousGame != Game.BFBB && newGame == Game.BFBB)
                bytes.AddRange(new byte[4]);
            else if (previousGame == Game.BFBB && newGame == Game.BFBB)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            for (int i = 0; i < 17; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));
        }

        private void ReverseLinks(ref List<byte> bytes, int count)
        {
            for (int i = 0; i < count; i++)
            {
                bytes.AddRange(Reverse(reader.ReadInt16()));
                bytes.AddRange(Reverse(reader.ReadInt16()));
                for (int j = 0; j < 7; j++)
                    bytes.AddRange(Reverse(reader.ReadInt32()));
            }
        }

        private void ReverseBOUL(ref List<byte> bytes)
        {
            ReversePlaceable(ref bytes);

            for (int i = 0; i < 4; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            if (previousGame == Game.Incredibles && newGame != Game.Incredibles)
                reader.BaseStream.Position += 4;
            else if (previousGame != Game.Incredibles && newGame == Game.Incredibles)
                bytes.AddRange(new byte[4]);
            else if (previousGame != Game.Incredibles && newGame != Game.Incredibles)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            for (int i = 0; i < 8; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            if (previousGame == Game.Incredibles && newGame != Game.Incredibles)
                reader.BaseStream.Position += 4;
            else if (previousGame != Game.Incredibles && newGame == Game.Incredibles)
                bytes.AddRange(new byte[4]);
            else if (previousGame != Game.Incredibles && newGame != Game.Incredibles)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            for (int i = 0; i < 4; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseCNTR(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);

            for (int i = 0; i < 2; i++)
                bytes.AddRange(Reverse(reader.ReadInt16()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseCOND(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);

            for (int i = 0; i < 3; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            if (previousGame == Game.Scooby && newGame != Game.Scooby)
                bytes.AddRange(new byte[4]);
            else if (previousGame != Game.Scooby && newGame == Game.Scooby)
                reader.BaseStream.Position += 4;
            else if (previousGame != Game.Scooby && newGame != Game.Scooby)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseCSNM(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);

            for (int i = 0; i < 48; i++)
                bytes.AddRange(Reverse(reader.ReadInt16()));

            if (previousGame == Game.Incredibles && newGame != Game.Incredibles)
                reader.BaseStream.Position += 4;
            else if (previousGame != Game.Incredibles && newGame == Game.Incredibles)
                bytes.AddRange(new byte[4]);
            else if (previousGame == Game.Incredibles && newGame == Game.Incredibles)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseDPAT(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);
            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseDSTR(ref List<byte> bytes)
        {
            ReversePlaceable(ref bytes);

            for (int i = 0; i < 5; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            bytes.Add(reader.ReadByte());
            bytes.Add(reader.ReadByte());
            bytes.AddRange(Reverse(reader.ReadInt16()));

            for (int i = 0; i < 2; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            if (previousGame == Game.Scooby && newGame != Game.Scooby)
                bytes.AddRange(new byte[6 * 4]);
            else if (previousGame != Game.Scooby && newGame == Game.Scooby)
                reader.BaseStream.Position += 6 * 4;
            else if (previousGame != Game.Scooby && newGame != Game.Scooby)
                for (int i = 0; i < 6; i++)
                    bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseDYNA(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);

            int dynaType = reader.ReadInt32();
            reader.BaseStream.Position -= 4;

            if (previousEndianness == Endianness.Big)
                dynaType = BitConverter.ToInt32(Reverse(dynaType).ToArray(), 0);

            bytes.AddRange(Reverse(reader.ReadInt32()));
            bytes.AddRange(Reverse(reader.ReadInt16()));
            bytes.AddRange(Reverse(reader.ReadInt16()));

            var linksStart = reader.BaseStream.Length - 32 * bytes[5];

            if ((DynaType_BFBB)dynaType == DynaType_BFBB.hud__meter__font)
            {
                while (reader.BaseStream.Position < linksStart - 12)
                    bytes.AddRange(Reverse(reader.ReadInt32()));
                for (int i = 0; i < 12; i++)
                    bytes.Add(reader.ReadByte());
            }
            else if ((DynaType_BFBB)dynaType == DynaType_BFBB.game_object__NPCSettings)
            {
                bytes.AddRange(Reverse(reader.ReadInt32()));

                for (int i = 0; i < 12; i++)
                    bytes.Add(reader.ReadByte());
            }
            else if ((DynaType_BFBB)dynaType == DynaType_BFBB.game_object__talk_box)
            {
                for (int i = 0; i < 3; i++)
                    bytes.AddRange(Reverse(reader.ReadInt32()));
                for (int i = 0; i < 4; i++)
                    bytes.Add(reader.ReadByte());
                for (int i = 0; i < 2; i++)
                    bytes.AddRange(Reverse(reader.ReadInt32()));
                for (int i = 0; i < 4; i++)
                    bytes.Add(reader.ReadByte());
            }
            else if ((DynaType_BFBB)dynaType == DynaType_BFBB.game_object__task_box)
            {
                for (int i = 0; i < 4; i++)
                    bytes.Add(reader.ReadByte());
            }
            else if ((DynaType_BFBB)dynaType == DynaType_BFBB.game_object__text_box)
            {
                for (int i = 0; i < 10; i++)
                    bytes.AddRange(Reverse(reader.ReadInt32()));
                for (int i = 0; i < 4; i++)
                    bytes.Add(reader.ReadByte());
                for (int i = 0; i < 8; i++)
                    bytes.AddRange(Reverse(reader.ReadInt32()));
                for (int i = 0; i < 4; i++)
                    bytes.Add(reader.ReadByte());
            }
            else if (newGame == Game.Incredibles && new DynaType_TSSM[] {
                DynaType_TSSM.Enemy__SB__BucketOTron,
                DynaType_TSSM.Enemy__SB__CastNCrew,
                DynaType_TSSM.Enemy__SB__Critter,
                DynaType_TSSM.Enemy__SB__Dennis,
                DynaType_TSSM.Enemy__SB__FrogFish,
                DynaType_TSSM.Enemy__SB__Mindy,
                DynaType_TSSM.Enemy__SB__Neptune,
                DynaType_TSSM.Enemy__SB__Standard,
                DynaType_TSSM.Enemy__SB__Turret,
                DynaType_TSSM.Enemy__SB__SupplyCrate
            }.Contains((DynaType_TSSM)dynaType))
            {
                bytes.AddRange(Reverse(reader.ReadInt32()));
                bytes.Add(reader.ReadByte());
                bytes.Add(reader.ReadByte());
                bytes.AddRange(Reverse(reader.ReadInt16()));
                for (int i = 0; i < 4; i++)
                    bytes.Add(reader.ReadByte());
            }

            while (reader.BaseStream.Position < linksStart)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseEGEN(ref List<byte> bytes)
        {
            ReversePlaceable(ref bytes);

            for (int i = 0; i < 3; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            for (int i = 0; i < 4; i++)
                bytes.Add(reader.ReadByte());

            for (int i = 0; i < 2; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseENV(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);

            for (int i = 0; i < 14; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            if (previousGame == Game.Scooby && newGame != Game.Scooby)
                bytes.AddRange(BitConverter.GetBytes(10f));
            else if (previousGame != Game.Scooby && newGame == Game.Scooby)
                reader.BaseStream.Position += 4;
            else if (previousGame != Game.Scooby && newGame != Game.Scooby)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseFOG(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);

            for (int i = 0; i < 8; i++)
                bytes.Add(reader.ReadByte());

            for (int i = 0; i < 4; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            for (int i = 0; i < 4; i++)
                bytes.Add(reader.ReadByte());

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseGRUP(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);

            short grupCount = reader.ReadInt16();
            reader.BaseStream.Position -= 2;

            if (previousEndianness == Endianness.Big)
                grupCount = BitConverter.ToInt16(Reverse(grupCount).ToArray(), 0);

            bytes.AddRange(Reverse(reader.ReadInt16()));
            bytes.AddRange(Reverse(reader.ReadInt16()));

            for (int i = 0; i < grupCount; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseGUST(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);

            for (int i = 0; i < 8; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseHANG(ref List<byte> bytes)
        {
            ReversePlaceable(ref bytes);

            for (int i = 0; i < 8; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseMVPT(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);

            for (int i = 0; i < 3; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));
            bytes.AddRange(Reverse(reader.ReadInt16()));
            for (int i = 0; i < 4; i++)
                bytes.Add(reader.ReadByte());

            short pointCount = reader.ReadInt16();
            reader.BaseStream.Position -= 2;
            if (previousEndianness == Endianness.Big)
                pointCount = BitConverter.ToInt16(Reverse(pointCount).ToArray(), 0);
            bytes.AddRange(Reverse(reader.ReadInt16()));

            if (previousGame == Game.Scooby && newGame != Game.Scooby)
                bytes.AddRange(new byte[4]);
            else if (previousGame != Game.Scooby && newGame == Game.Scooby)
                reader.BaseStream.Position += 4;
            else if (previousGame != Game.Scooby && newGame != Game.Scooby)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            bytes.AddRange(Reverse(reader.ReadInt32()));

            if (previousGame == Game.Scooby && newGame != Game.Scooby)
                bytes.AddRange(new byte[4]);
            else if (previousGame != Game.Scooby && newGame == Game.Scooby)
                reader.BaseStream.Position += 4;
            else if (previousGame != Game.Scooby && newGame != Game.Scooby)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            for (int i = 0; i < pointCount; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReversePEND(ref List<byte> bytes)
        {
            ReversePlaceable(ref bytes);

            for (int i = 0; i < 4; i++)
                bytes.Add(reader.ReadByte());

            for (int i = 0; i < 11; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReversePKUP(ref List<byte> bytes)
        {
            ReversePlaceable(ref bytes);

            bytes.AddRange(Reverse(reader.ReadInt32()));

            for (int i = 0; i < 2; i++)
                bytes.AddRange(Reverse(reader.ReadInt16()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseBUTN(ref List<byte> bytes)
        {
            ReversePlaceable(ref bytes);

            for (int i = 0; i < 6; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseMotion(ref bytes);

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseCAM(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);

            for (int i = 0; i < 15; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            for (int i = 0; i < 2; i++)
                bytes.AddRange(Reverse(reader.ReadInt16()));

            for (int i = 0; i < 15; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            for (int i = 0; i < 4; i++)
                bytes.Add(reader.ReadByte());

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReversePLAT(ref List<byte> bytes)
        {
            ReversePlaceable(ref bytes);

            bytes.Add(reader.ReadByte());
            bytes.Add(reader.ReadByte());
            bytes.AddRange(Reverse(reader.ReadInt16()));

            for (int i = 0; i < 14; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseMotion(ref bytes);

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseMotion(ref List<byte> bytes)
        {
            byte type = reader.ReadByte();
            bytes.Add(type);
            bytes.Add(reader.ReadByte());
            bytes.AddRange(Reverse(reader.ReadInt16()));

            if (type == 4 || type == 5)
            {
                for (int i = 0; i < 4; i++)
                    bytes.Add(reader.ReadByte());
            }
            else
            {
                bytes.AddRange(Reverse(reader.ReadInt32()));
            }

            if (type == 4)
            {
                if (previousGame == Game.Incredibles && newGame != Game.Incredibles)
                    reader.BaseStream.Position += 4;
                else if (previousGame != Game.Incredibles && newGame == Game.Incredibles)
                    bytes.AddRange(new byte[4]);
                else if (previousGame == Game.Incredibles && newGame == Game.Incredibles)
                    for (int i = 0; i < 4; i++)
                        bytes.Add(reader.ReadByte());
            }

            for (int i = 0; i < 10; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            if (previousGame == Game.Incredibles && newGame != Game.Incredibles)
                reader.BaseStream.Position += 8;
            else if (previousGame != Game.Incredibles && newGame == Game.Incredibles)
                bytes.AddRange(new byte[8]);
            else if (previousGame == Game.Incredibles && newGame == Game.Incredibles)
                for (int i = 0; i < 2; i++)
                    bytes.AddRange(Reverse(reader.ReadInt32()));

            if (type != 4)
            {
                if (previousGame == Game.Incredibles && newGame != Game.Incredibles)
                    reader.BaseStream.Position += 4;
                else if (previousGame != Game.Incredibles && newGame == Game.Incredibles)
                    bytes.AddRange(new byte[4]);
                else if (previousGame == Game.Incredibles && newGame == Game.Incredibles)
                    bytes.AddRange(Reverse(reader.ReadInt32()));
            }
        }

        private void ReversePLYR(ref List<byte> bytes)
        {
            ReversePlaceable(ref bytes);

            ReverseLinks(ref bytes, bytes[5]);

            if (previousGame == Game.Scooby && newGame != Game.Scooby)
                bytes.AddRange(new byte[4]);
            else if (previousGame != Game.Scooby && newGame == Game.Scooby)
                reader.BaseStream.Position += 4;
            else if (previousGame != Game.Scooby && newGame != Game.Scooby)
                bytes.AddRange(Reverse(reader.ReadInt32()));
        }

        private void ReversePORT(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);

            for (int i = 0; i < 4; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReversePRJT(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);

            for (int i = 0; i < 13; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseSCRP(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);

            bytes.AddRange(Reverse(reader.ReadInt32()));

            int timedLinkCount = reader.ReadInt32();
            reader.BaseStream.Position -= 4;
            if (previousEndianness == Endianness.Big)
                timedLinkCount = BitConverter.ToInt32(Reverse(timedLinkCount).ToArray(), 0);
            bytes.AddRange(Reverse(reader.ReadInt32()));

            for (int i = 0; i < 4; i++)
                bytes.Add(reader.ReadByte());

            for (int i = 0; i < timedLinkCount * 8; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseSFX(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);

            for (int i = 0; i < 2; i++)
                bytes.AddRange(Reverse(reader.ReadInt16()));

            for (int i = 0; i < 3; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            for (int i = 0; i < 4; i++)
                bytes.Add(reader.ReadByte());

            for (int i = 0; i < 5; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseSGRP(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);

            byte amountEntries = reader.ReadByte();
            bytes.Add(amountEntries);

            for (int i = 0; i < 7; i++)
                bytes.Add(reader.ReadByte());

            for (int i = 0; i < 3; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            for (int i = 0; i < amountEntries * 4; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseSIMP(ref List<byte> bytes)
        {
            ReversePlaceable(ref bytes);

            for (int i = 0; i < 2; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            for (int i = 0; i < 4; i++)
                bytes.Add(reader.ReadByte());

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseTEXT(ref List<byte> bytes)
        {
            bytes.AddRange(Reverse(reader.ReadInt32()));
            while (reader.BaseStream.Position < reader.BaseStream.Length)
                bytes.Add(reader.ReadByte());
        }

        private void ReverseTIMR(ref List<byte> bytes)
        {
            ReverseObject(ref bytes);

            bytes.AddRange(Reverse(reader.ReadInt32()));

            if (bytes[5] > 0)
            {
                if (previousGame == Game.Scooby && newGame != Game.Scooby)
                    bytes.AddRange(new byte[4]);
                else if (previousGame != Game.Scooby && newGame == Game.Scooby)
                    reader.BaseStream.Position += 4;
                else if (previousGame != Game.Scooby && newGame != Game.Scooby)
                    bytes.AddRange(BitConverter.GetBytes(reader.ReadInt32()));

                ReverseLinks(ref bytes, bytes[5]);
            }
        }

        private void ReverseTRIG(ref List<byte> bytes)
        {
            ReversePlaceable(ref bytes);

            for (int i = 0; i < 16; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseUI(ref List<byte> bytes, bool skipLinks = false)
        {
            ReversePlaceable(ref bytes);

            bytes.AddRange(Reverse(reader.ReadInt32()));
            bytes.AddRange(Reverse(reader.ReadInt16()));
            bytes.AddRange(Reverse(reader.ReadInt16()));

            for (int i = 0; i < 9; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));

            if (!skipLinks)
                ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseUIFT(ref List<byte> bytes)
        {
            ReverseUI(ref bytes, true);

            bytes.AddRange(Reverse(reader.ReadInt16()));
            bytes.Add(reader.ReadByte());
            bytes.Add(reader.ReadByte());

            bytes.AddRange(Reverse(reader.ReadInt32()));

            for (int i = 0; i < 8; i++)
                bytes.Add(reader.ReadByte());

            for (int i = 0; i < 8; i++)
                bytes.AddRange(Reverse(reader.ReadInt16()));

            bytes.AddRange(Reverse(reader.ReadInt32()));

            ReverseLinks(ref bytes, bytes[5]);
        }

        private void ReverseVIL(ref List<byte> bytes)
        {
            ReversePlaceable(ref bytes);

            for (int i = 0; i < 6; i++)
                bytes.AddRange(Reverse(reader.ReadInt32()));
            
            ReverseLinks(ref bytes, bytes[5]);
        }
    }
}