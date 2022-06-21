using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark
{
    public abstract class GenericAssetDataContainer
    {
        public virtual byte[] Serialize(Game game, Endianness endianness) => new byte[0];

        public virtual bool HasReference(uint assetID) =>
            GetType().GetProperties().Any(prop => (
                prop.PropertyType.Equals(typeof(AssetID)) &&
                    ((AssetID)prop.GetValue(this)).Equals(assetID)) ||
                (typeof(GenericAssetDataContainer).IsAssignableFrom(prop.PropertyType) &&
                    ((GenericAssetDataContainer)prop.GetValue(this)).HasReference(assetID)) ||
                prop.PropertyType.GetInterfaces().Any(i => i.IsGenericType &&
                    i.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)) &&
                        ((i.GenericTypeArguments[0].Equals(typeof(AssetID)) &&
                            ((IEnumerable<AssetID>)prop.GetValue(this)).Any(a => a.Equals(assetID))) ||
                        (typeof(GenericAssetDataContainer).IsAssignableFrom(i.GenericTypeArguments[0]) &&
                            ((IEnumerable<GenericAssetDataContainer>)prop.GetValue(this)).Any(a => a.HasReference(assetID))))));

        public virtual void Verify(ref List<string> result) { }

        public static void Verify(uint assetID, ref List<string> result)
        {
            if (assetID != 0 && !Program.MainForm.AssetExists(assetID))
                result.Add("Referenced asset 0x" + assetID.ToString("X8") + " was not found in any open archive.");
        }

        protected static FlagBitmask ByteFlagsDescriptor(params string[] flagNames) => FlagsDescriptor(8, flagNames);

        protected static FlagBitmask ShortFlagsDescriptor(params string[] flagNames) => FlagsDescriptor(16, flagNames);

        protected static FlagBitmask IntFlagsDescriptor(params string[] flagNames) => FlagsDescriptor(32, flagNames);

        private static FlagBitmask FlagsDescriptor(int bitSize, params string[] flagNames)
        {
            var dt = new FlagBitmask(typeof(FlagField));
            var ff = new FlagField(bitSize, flagNames, dt);
            return dt.DFD_FromComponent(ff);
        }

        protected static float? TriangleIntersection(Ray r, IList<Models.Triangle> triangles, IList<Vector3> vertices, Matrix world)
        {
            float? smallestDistance = null;

            foreach (var t in triangles)
            {
                var v1 = (Vector3)Vector3.Transform(vertices[t.vertex1], world);
                var v2 = (Vector3)Vector3.Transform(vertices[t.vertex2], world);
                var v3 = (Vector3)Vector3.Transform(vertices[t.vertex3], world);

                if (r.Intersects(ref v1, ref v2, ref v3, out float distance))
                {
                    if (smallestDistance == null || distance < smallestDistance)
                        smallestDistance = distance;
                }
            }

            return smallestDistance;
        }
    }
}
