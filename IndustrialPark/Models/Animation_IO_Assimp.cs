using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Assimp;

namespace IndustrialPark
{
    public class Animation_IO_Assimp : EndianConvertible
    {
        public Animation_IO_Assimp(Endianness endianness) : base(endianness) { }

        public class KeyFrame
        {
            public int Index;
            public Quaternion Rotation;
            public Vector3D Position;
        }

        const int shortMaxVal = 32767;

        public byte[] CreateANIMFromAssimp(string fileName)
        {
            using (AssimpContext context = new AssimpContext())
            {
                Scene scene = context.ImportFile(fileName, 0);
                
                List<string> nodeNames = new List<string>();
                TraverseNodes(ref nodeNames, scene.RootNode);

                List<KeyFrame> keyFrames = new List<KeyFrame>();
                Dictionary<short, short[]> keyFrameMap = new Dictionary<short, short[]>();

                short boneCount = (short)nodeNames.Count;

                System.Windows.Forms.MessageBox.Show(scene.Animations[0].DurationInTicks.ToString());
                short tickCount = (short)scene.Animations[0].DurationInTicks;

                Vector3D Maximum = new Vector3D();

                foreach (NodeAnimationChannel nodeAnimationChannel in scene.Animations[0].NodeAnimationChannels)
                {
                    List<short> keyFrameIndices = new List<short>();

                    for (int i = 0; i < nodeAnimationChannel.PositionKeyCount; i++)
                    {
                        if (nodeAnimationChannel.PositionKeys[i].Value.X > Maximum.X)
                            Maximum.X = nodeAnimationChannel.PositionKeys[i].Value.X;
                        if (nodeAnimationChannel.PositionKeys[i].Value.Y > Maximum.Y)
                            Maximum.Y = nodeAnimationChannel.PositionKeys[i].Value.Y;
                        if (nodeAnimationChannel.PositionKeys[i].Value.Z > Maximum.Z)
                            Maximum.Z = nodeAnimationChannel.PositionKeys[i].Value.Z;

                        keyFrames.Add(new KeyFrame
                        {
                            Index = i,
                            Position = nodeAnimationChannel.PositionKeys[i].Value,
                            Rotation = nodeAnimationChannel.RotationKeys[i].Value
                        });

                        keyFrameIndices.Add((short)(keyFrames.Count - 1));
                    }

                    short boneIndex = (short)nodeNames.IndexOf(nodeAnimationChannel.NodeName);
                    if (!keyFrameMap.ContainsKey(boneIndex))
                        keyFrameMap[boneIndex] = keyFrameIndices.ToArray();
                }

                Vector3D Scale = new Vector3D(1 / Maximum.X, 1 / Maximum.Y, 1 / Maximum.Z);

                List<byte> result = new List<byte>();
                result.AddRange(Switch("SKB1").Cast<byte>());
                result.AddRange(BitConverter.GetBytes(Switch(0)));
                result.AddRange(BitConverter.GetBytes(Switch(boneCount)));
                result.AddRange(BitConverter.GetBytes(Switch(tickCount)));
                result.AddRange(BitConverter.GetBytes(Switch(keyFrames.Count)));
                result.AddRange(BitConverter.GetBytes(Switch(Scale.X)));
                result.AddRange(BitConverter.GetBytes(Switch(Scale.Y)));
                result.AddRange(BitConverter.GetBytes(Switch(Scale.Z)));

                foreach (KeyFrame k in keyFrames)
                {
                    result.AddRange(BitConverter.GetBytes(Switch(k.Index)));
                    result.AddRange(BitConverter.GetBytes(Switch((short)(k.Rotation.X * shortMaxVal))));
                    result.AddRange(BitConverter.GetBytes(Switch((short)(k.Rotation.Y * shortMaxVal))));
                    result.AddRange(BitConverter.GetBytes(Switch((short)(k.Rotation.Z * shortMaxVal))));
                    result.AddRange(BitConverter.GetBytes(Switch((short)(k.Rotation.W * shortMaxVal))));
                    result.AddRange(BitConverter.GetBytes(Switch((short)(k.Position.X / Scale.X))));
                    result.AddRange(BitConverter.GetBytes(Switch((short)(k.Position.Y / Scale.Y))));
                    result.AddRange(BitConverter.GetBytes(Switch((short)(k.Position.Z / Scale.Z))));
                }

                for (int i = 0; i < scene.Animations[0].DurationInTicks; i++)
                    result.AddRange(BitConverter.GetBytes(Switch((float)(i / scene.Animations[0].TicksPerSecond))));

                for (short i = 0; i < boneCount; i++)
                    foreach (int j in keyFrameMap[i])
                        result.AddRange(BitConverter.GetBytes(Switch(j)));

                return result.ToArray();
            }
        }

        private static void TraverseNodes(ref List<string> nodeNames, Node node)
        {
            nodeNames.Add(node.Name);
            foreach (Node n in node.Children)
                TraverseNodes(ref nodeNames, n);
        }
    }
}
