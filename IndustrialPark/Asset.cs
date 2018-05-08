using SharpDX;
using System;
using System.Collections.Generic;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public abstract class Asset
    {
        private int assetID;
        private string name;
        private byte[] data;

        public int AssetID { get => assetID; set => assetID = value; }
        public string Name { get => name; set => name = value; }
        public byte[] Data { get => data; set => data = value; }

        public Asset(int assetID, string name, byte[] data)
        {
            AssetID = assetID;
            Name = name;
            Data = data;
        }

        public abstract void Setup();

        public override string ToString()
        {
            return Name;
        }
    }

    public class AssetGeneric : Asset
    {
        public AssetGeneric(int assetID, string name, byte[] data) : base(assetID, name, data) { }
        public override void Setup() { }
    }

    public abstract class RenderableAsset : Asset
    {
        public RenderableAsset(int assetID, string name, byte[] data) : base(assetID, name, data) { }
        public abstract void Draw(SharpDevice device, SharpShader shader, SharpDX.Direct3D11.Buffer buffer, Matrix viewProjection);
    }

    public class AssetJSP : RenderableAsset
    {
        public AssetJSP(int assetID, string name, byte[] data) : base(assetID, name, data) { }

        RenderWareModelFile model;

        public override void Setup()
        {
            model = new RenderWareModelFile(Name)
            {
                rwChunkList = RenderWareFile.ReadFileMethods.ReadRenderWareFile(Data)
            };
            model.SetForRendering();
            HipHopFunctions.renderableAssetList.Add(this);
        }

        public override void Draw(SharpDevice device, SharpShader shader, SharpDX.Direct3D11.Buffer buffer, Matrix viewProjection)
        {
            model.Render(device, shader, buffer, viewProjection);
        }
    }
    
    public class AssetMINF : Asset
    {
        public AssetMINF(int assetID, string name, byte[] data) : base(assetID, name, data) { }

        public int modelAssetId;
        
        public override void Setup()
        {
            if (Data.Length >= 0x18)
                modelAssetId = Switch(BitConverter.ToInt32(Data, 0x14));
        }

        public void Draw(SharpDevice device, SharpShader shader, SharpDX.Direct3D11.Buffer buffer, Matrix world, Matrix viewProjection)
        {
            if (HipHopFunctions.assetDictionary.ContainsKey(modelAssetId))
            {
                (HipHopFunctions.assetDictionary[modelAssetId] as AssetMODL).Draw(device, shader, buffer, world * viewProjection);
            }
            else
            {
                SharpRenderer.DrawCube(world);
            }
        }
    }

    public class AssetMODL : Asset
    {
        public AssetMODL(int assetID, string name, byte[] data) : base(assetID, name, data) { }

        RenderWareModelFile model;

        public override void Setup()
        {
            model = new RenderWareModelFile(Name)
            {
                rwChunkList = RenderWareFile.ReadFileMethods.ReadRenderWareFile(Data)
            };

            model.SetForRendering();
        }

        public void Draw(SharpDevice device, SharpShader shader, SharpDX.Direct3D11.Buffer buffer, Matrix worldViewProjection)
        {
            model.Render(device, shader, buffer, worldViewProjection);
        }
    }

    public struct PICKentry
    {
        public int unknown1;
        public int unknown2;
        public int unknown3;
        public int unknown4;
        public int unknown5;
    }

    public class AssetPICK : Asset
    {
        public AssetPICK(int assetID, string name, byte[] data) : base(assetID, name, data) { }

        public int pickAmount;
        public Dictionary<int, PICKentry> pickEntries;
        public static AssetPICK pick;

        public override void Setup()
        {
            pickEntries = new Dictionary<int, PICKentry>();
            pickAmount = Switch(BitConverter.ToInt32(Data, 0x4));
            for (int i = 0; i < pickAmount; i++)
            {
                PICKentry entry = new PICKentry()
                {
                    unknown1 = Switch(BitConverter.ToInt32(Data, 8 + i * 0x14)),
                    unknown2 = Switch(BitConverter.ToInt32(Data, 12 + i * 0x14)),
                    unknown3 = Switch(BitConverter.ToInt32(Data, 16 + i * 0x14)),
                    unknown4 = Switch(BitConverter.ToInt32(Data, 20 + i * 0x14)),
                    unknown5 = Switch(BitConverter.ToInt32(Data, 24 + i * 0x14)),
                };

                pickEntries.Add(entry.unknown1, entry);
                pick = this;
            }
        }
    }

    public class AssetPKUP : RenderableAsset
    {
        public AssetPKUP(int assetID, string name, byte[] data) : base(assetID, name, data) { }

        public Matrix world;
        public int pickEntryID;

        public override void Setup()
        {
            pickEntryID = Switch(BitConverter.ToInt32(Data, 0x54));
            world = Matrix.Scaling(Switch(BitConverter.ToSingle(Data, 0x2C)), Switch(BitConverter.ToSingle(Data, 0x30)), Switch(BitConverter.ToSingle(Data, 0x34)))
            //* Matrix.CreateRotationY((float)(Switch(BitConverter.ToSingle(data, 0x14)) * Math.PI))
            //* Matrix.CreateRotationX((float)(Switch(BitConverter.ToSingle(data, 0x18)) * Math.PI))
            //* Matrix.CreateRotationZ((float)(Switch(BitConverter.ToSingle(data, 0x1C)) * Math.PI))
            * Matrix.Translation(Switch(BitConverter.ToSingle(Data, 0x20)), Switch(BitConverter.ToSingle(Data, 0x24)), Switch(BitConverter.ToSingle(Data, 0x28)));
            HipHopFunctions.renderableAssetList.Add(this);
        }

        public override void Draw(SharpDevice device, SharpShader shader, SharpDX.Direct3D11.Buffer buffer, Matrix viewProjection)
        {
            if (AssetPICK.pick.pickEntries.ContainsKey(pickEntryID))
            {
                if (HipHopFunctions.assetDictionary.ContainsKey(AssetPICK.pick.pickEntries[pickEntryID].unknown4))
                {
                    (HipHopFunctions.assetDictionary[AssetPICK.pick.pickEntries[pickEntryID].unknown4] as AssetMODL).Draw(device, shader, buffer, world * viewProjection);
                }
            }
            else
            {
                SharpRenderer.DrawCube(world);
            }
        }
    }

    public class AssetPLAT : RenderableAsset
    {
        public AssetPLAT(int assetID, string name, byte[] data) : base(assetID, name, data) { }

        public Matrix world;
        public int modelAssetId;

        public override void Setup()
        {
            modelAssetId = Switch(BitConverter.ToInt32(Data, 0x4C));
            world = Matrix.Scaling(Switch(BitConverter.ToSingle(Data, 0x2C)), Switch(BitConverter.ToSingle(Data, 0x30)), Switch(BitConverter.ToSingle(Data, 0x34)))
            //* Matrix.CreateRotationY((float)(Switch(BitConverter.ToSingle(data, 0x14)) * Math.PI))
            //* Matrix.CreateRotationX((float)(Switch(BitConverter.ToSingle(data, 0x18)) * Math.PI))
            //* Matrix.CreateRotationZ((float)(Switch(BitConverter.ToSingle(data, 0x1C)) * Math.PI))
            * Matrix.Translation(Switch(BitConverter.ToSingle(Data, 0x20)), Switch(BitConverter.ToSingle(Data, 0x24)), Switch(BitConverter.ToSingle(Data, 0x28)));
            HipHopFunctions.renderableAssetList.Add(this);
        }

        public override void Draw(SharpDevice device, SharpShader shader, SharpDX.Direct3D11.Buffer buffer, Matrix viewProjection)
        {
            if (HipHopFunctions.assetDictionary.ContainsKey(modelAssetId))
            {
                (HipHopFunctions.assetDictionary[modelAssetId] as AssetMODL).Draw(device, shader, buffer, world * viewProjection);
            }
            else
            {
                SharpRenderer.DrawCube(world);
            }
        }
    }

    public class AssetRWTX : Asset
    {
        public AssetRWTX(int assetID, string name, byte[] data) : base(assetID, name, data) { }
        public override void Setup() { }
    }

    public class AssetSIMP : RenderableAsset
    {
        public AssetSIMP(int assetID, string name, byte[] data) : base(assetID, name, data) { }

        public Matrix world;
        public int modelAssetId;

        public override void Setup()
        {
            modelAssetId = Switch(BitConverter.ToInt32(Data, 0x4C));
            world = Matrix.Scaling(Switch(BitConverter.ToSingle(Data, 0x2C)), Switch(BitConverter.ToSingle(Data, 0x30)), Switch(BitConverter.ToSingle(Data, 0x34)))
            //* Matrix.CreateRotationY((float)(Switch(BitConverter.ToSingle(data, 0x14)) * Math.PI))
            //* Matrix.CreateRotationX((float)(Switch(BitConverter.ToSingle(data, 0x18)) * Math.PI))
            //* Matrix.CreateRotationZ((float)(Switch(BitConverter.ToSingle(data, 0x1C)) * Math.PI))
            * Matrix.Translation(Switch(BitConverter.ToSingle(Data, 0x20)), Switch(BitConverter.ToSingle(Data, 0x24)), Switch(BitConverter.ToSingle(Data, 0x28)));
            HipHopFunctions.renderableAssetList.Add(this);
        }

        public override void Draw(SharpDevice device, SharpShader shader, SharpDX.Direct3D11.Buffer buffer, Matrix viewProjection)
        {
            if (HipHopFunctions.assetDictionary.ContainsKey(modelAssetId))
            {
                if (HipHopFunctions.assetDictionary[modelAssetId] is AssetMODL)
                    (HipHopFunctions.assetDictionary[modelAssetId] as AssetMODL).Draw(device, shader, buffer, world * viewProjection);
                else if (HipHopFunctions.assetDictionary[modelAssetId] is AssetMINF)
                    (HipHopFunctions.assetDictionary[modelAssetId] as AssetMINF).Draw(device, shader, buffer, world, viewProjection);
            }
            else
            {
                SharpRenderer.DrawCube(world);
            }
        }
    }

    public class AssetVIL : RenderableAsset
    {
        public AssetVIL(int assetID, string name, byte[] data) : base(assetID, name, data) { }

        public Matrix world;
        public int minfAssetID;

        public override void Setup()
        {
            minfAssetID = Switch(BitConverter.ToInt32(Data, 0x4C));
            world = Matrix.Scaling(Switch(BitConverter.ToSingle(Data, 0x2C)), Switch(BitConverter.ToSingle(Data, 0x30)), Switch(BitConverter.ToSingle(Data, 0x34)))
            //* Matrix.CreateRotationY((float)(Switch(BitConverter.ToSingle(data, 0x14)) * Math.PI))
            //* Matrix.CreateRotationX((float)(Switch(BitConverter.ToSingle(data, 0x18)) * Math.PI))
            //* Matrix.CreateRotationZ((float)(Switch(BitConverter.ToSingle(data, 0x1C)) * Math.PI))
            * Matrix.Translation(Switch(BitConverter.ToSingle(Data, 0x20)), Switch(BitConverter.ToSingle(Data, 0x24)), Switch(BitConverter.ToSingle(Data, 0x28)));
            HipHopFunctions.renderableAssetList.Add(this);
        }

        public override void Draw(SharpDevice device, SharpShader shader, SharpDX.Direct3D11.Buffer buffer, Matrix viewProjection)
        {
            if (HipHopFunctions.assetDictionary.ContainsKey(minfAssetID))
            {
                (HipHopFunctions.assetDictionary[minfAssetID] as AssetMINF).Draw(device, shader, buffer, world, viewProjection);
            }
            else
            {
                SharpRenderer.DrawCube(world);
            }
        }
    }
}