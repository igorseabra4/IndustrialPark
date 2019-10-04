using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Assimp;

namespace IndustrialPark.Models
{
    public static class Animation_IO_Assimp
    {
        public class KeyFrame
        {
            public Quaternion Rotation;
            public Vector3D Position;
        }

        public static byte[] CreateANIMFromAssimp(string fileName, bool flipUVs)
        {
            PostProcessSteps pps =
                PostProcessSteps.FindInstances |
                PostProcessSteps.FindInvalidData |
                PostProcessSteps.GenerateNormals |
                PostProcessSteps.JoinIdenticalVertices |
                PostProcessSteps.OptimizeGraph |
                PostProcessSteps.OptimizeMeshes |
                PostProcessSteps.PreTransformVertices |
                PostProcessSteps.Triangulate |
                (flipUVs ? PostProcessSteps.FlipUVs : 0);

            Scene scene = new AssimpContext().ImportFile(fileName, pps);

            List<string> nodeNames = new List<string>();
            TraverseNodes(ref nodeNames, scene.RootNode);

            Dictionary<int, List<KeyFrame>> boneIndicesToKeyFrames = new Dictionary<int, List<KeyFrame>>();
            //short[][] keyFrameMap = new short[][];

            short boneCount = (short)nodeNames.Count;

            Vector3D Maximum = new Vector3D();

            foreach (NodeAnimationChannel nodeAnimationChannel in scene.Animations[0].NodeAnimationChannels)
            {
                int boneIndex = nodeNames.IndexOf(nodeAnimationChannel.NodeName);
                if (!boneIndicesToKeyFrames.ContainsKey(boneIndex))
                    boneIndicesToKeyFrames[boneIndex] = new List<KeyFrame>();

                for (int i = 0; i < nodeAnimationChannel.PositionKeyCount; i++)
                {
                    if (nodeAnimationChannel.PositionKeys[i].Value.X > Maximum.X)
                        Maximum.X = nodeAnimationChannel.PositionKeys[i].Value.X;
                    if (nodeAnimationChannel.PositionKeys[i].Value.Y > Maximum.Y)
                        Maximum.Y = nodeAnimationChannel.PositionKeys[i].Value.Y;
                    if (nodeAnimationChannel.PositionKeys[i].Value.Z > Maximum.Z)
                        Maximum.Z = nodeAnimationChannel.PositionKeys[i].Value.Z;
                    
                    boneIndicesToKeyFrames[boneIndex].Add(new KeyFrame
                    {
                        Position = nodeAnimationChannel.PositionKeys[i].Value,
                        Rotation = nodeAnimationChannel.RotationKeys[i].Value
                    });
                }
            }


            //return new RWSection[] { clump };
            return null;
        }

        private static void TraverseNodes(ref List<string> nodeNames, Node node)
        {
            nodeNames.Add(node.Name);
            foreach (Node n in node.Children)
                TraverseNodes(ref nodeNames, node);
        }
    }
}
