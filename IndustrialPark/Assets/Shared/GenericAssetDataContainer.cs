using HipHopFile;
using Newtonsoft.Json;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace IndustrialPark.Assets
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class GenericAssetDataContainer
    {
        [JsonProperty]
        protected Game _game;

        [Browsable(false)]
        public Game game => _game;

        public void SetGame(Game game)
        {
            _game = game;

            foreach (var prop in GetType().GetProperties())
            {
                var type = prop.PropertyType;

                if (typeof(GenericAssetDataContainer).IsAssignableFrom(type))
                {
                    if (prop.GetValue(this) is GenericAssetDataContainer gadc)
                        gadc.SetGame(game);
                }
                else if (typeof(IEnumerable<GenericAssetDataContainer>).IsAssignableFrom(type))
                {
                    if (prop.GetValue(this) is IEnumerable<GenericAssetDataContainer> gadcs)
                        foreach (var gadc in gadcs)
                            gadc?.SetGame(game);
                }
            }
        }

        public abstract void Serialize(EndianBinaryWriter writer);

        public byte[] Serialize(Endianness endianness)
        {
            using var writer = new EndianBinaryWriter(endianness);
            Serialize(writer);
            return writer.ToArray();
        }

        public virtual bool HasReference(uint assetID)
        {
            var properties = GetType().GetProperties();

            foreach (var prop in properties)
            {
                var propValue = prop.GetValue(this);

                if (propValue == null)
                    continue;

                if (prop.PropertyType.Equals(typeof(AssetID)) && ((AssetID)propValue).Equals(assetID))
                    return true;

                if (typeof(GenericAssetDataContainer).IsAssignableFrom(prop.PropertyType))
                {
                    if (((GenericAssetDataContainer)propValue).HasReference(assetID))
                        return true;
                }

                var interfaces = prop.PropertyType.GetInterfaces();
                foreach (var i in interfaces)
                {
                    if (i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
                    {
                        if (i.GenericTypeArguments[0].Equals(typeof(AssetID)))
                        {
                            var enumerable = (IEnumerable<AssetID>)propValue;
                            if (enumerable != null && enumerable.Any(a => a.Equals(assetID)))
                                return true;
                        }
                        else if (typeof(GenericAssetDataContainer).IsAssignableFrom(i.GenericTypeArguments[0]))
                        {
                            var enumerable = (IEnumerable<GenericAssetDataContainer>)propValue;
                            if (enumerable != null && enumerable.Any(a => a != null && a.HasReference(assetID)))
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public virtual List<uint> GetHierarchy()
        {
            var hierarchy = new List<uint>();
            var properties = GetType().GetProperties();

            foreach (var prop in properties)
            {
                var propValue = prop.GetValue(this);

                if (propValue == null)
                    continue;

                if (prop.PropertyType.Equals(typeof(AssetID)) && !hierarchy.Contains((AssetID)propValue))
                    hierarchy.Add((AssetID)propValue);

                if (typeof(GenericAssetDataContainer).IsAssignableFrom(prop.PropertyType))
                    hierarchy.AddRange(((GenericAssetDataContainer)propValue).GetHierarchy());

                var interfaces = prop.PropertyType.GetInterfaces();
                foreach (var i in interfaces)
                {
                    if (i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
                    {
                        if (i.GenericTypeArguments[0].Equals(typeof(AssetID)))
                        {
                            var enumerable = (IEnumerable<AssetID>)propValue;
                            foreach (var u in enumerable)
                                if (!hierarchy.Contains(u))
                                    hierarchy.Add(u);
                        }
                        else if (typeof(GenericAssetDataContainer).IsAssignableFrom(i.GenericTypeArguments[0]))
                        {
                            var enumerable = (IEnumerable<GenericAssetDataContainer>)propValue;
                            foreach (var gadc in enumerable)
                                hierarchy.AddRange(gadc.GetHierarchy());
                        }
                    }
                }
            }

            return hierarchy;
        }

        public virtual bool ReplaceReferences(uint oldAssetId, uint newAssetId)
        {
            if (oldAssetId == 0)
                return false;

            var result = false;

            var typeProperties = GetType().GetProperties();

            foreach (var prop in typeProperties.Where(prop => prop.PropertyType.Equals(typeof(AssetID)) && ((AssetID)prop.GetValue(this)).Equals(oldAssetId)))
            {
                result = true;
                prop.SetValue(this, new AssetID(newAssetId));
            }
            
            foreach (var gadc in typeProperties.Where(prop => typeof(GenericAssetDataContainer).IsAssignableFrom(prop.PropertyType)).Select(prop => (GenericAssetDataContainer)prop.GetValue(this)))
                result |= gadc?.ReplaceReferences(oldAssetId, newAssetId) ?? false;
            
            foreach (var array in typeProperties.Where(prop => prop.PropertyType.Equals(typeof(AssetID[]))).Select(prop => (AssetID[])prop.GetValue(this)))
                for (int i = 0; i < array.Length; i++)
                    if (array[i] == oldAssetId)
                    {
                        result = true;
                        array[i] = newAssetId;
                    }

            foreach (var gadcs in typeProperties.Where(prop => prop.PropertyType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)) && typeof(GenericAssetDataContainer).IsAssignableFrom(i.GenericTypeArguments[0]))).Select(prop => (IEnumerable<GenericAssetDataContainer>)prop.GetValue(this)))
                foreach (var gadc in gadcs)
                    result |= gadc?.ReplaceReferences(oldAssetId, newAssetId) ?? false;

            return result;
        }

        public virtual void Verify(ref List<string> result)
        {
            var typeProperties = GetType().GetProperties();

            foreach (var prop in typeProperties.Where(prop => prop.PropertyType.Equals(typeof(AssetID))))
                if (prop.GetCustomAttribute(typeof(IgnoreVerificationAttribute)) == null)
                    Verify((AssetID)prop.GetValue(this), prop.Name, prop.GetCustomAttribute(typeof(ValidReferenceRequiredAttribute)) != null, ref result);

            foreach (var gadc in typeProperties.Where(prop => typeof(GenericAssetDataContainer).IsAssignableFrom(prop.PropertyType)).Select(prop => (GenericAssetDataContainer)prop.GetValue(this)))
                gadc?.Verify(ref result);

            foreach (var prop in typeProperties.Where(prop => prop.PropertyType.Equals(typeof(AssetID[]))))
                if (prop.GetCustomAttribute(typeof(IgnoreVerificationAttribute)) == null)
                {
                    var array = (AssetID[])prop.GetValue(this);
                    foreach (var assetID in array)
                        Verify(assetID, prop.Name, prop.GetCustomAttribute(typeof(ValidReferenceRequiredAttribute)) != null, ref result);
                }

            foreach (var gadcs in typeProperties.Where(prop => prop.PropertyType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)) && typeof(GenericAssetDataContainer).IsAssignableFrom(i.GenericTypeArguments[0]))).Select(prop => (IEnumerable<GenericAssetDataContainer>)prop.GetValue(this)))
                foreach (var gadc in gadcs)
                    gadc?.Verify(ref result);
        }

        protected static void Verify(uint assetID, string propName, bool validReferenceRequired, ref List<string> result)
        {
            if (assetID != 0 && !Program.NameRepository.AssetExists(assetID))
                result.Add($"Asset 0x{assetID:X8} referenced in {propName} was not found in any open archive.");
            if (validReferenceRequired && assetID == 0)
                result.Add($"{propName} is 0");
        }

        public virtual void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
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

        protected static BoundingBox CreateBoundingBox(List<Vector3> vertexList, Matrix world, float multiplier = 1f)
        {
            if (vertexList == null || vertexList.Count == 0)
                return new BoundingBox();

            var min = new Vector3(float.MaxValue);
            var max = new Vector3(float.MinValue);

            for (int i = 0; i < vertexList.Count; i++)
            {
                var transformed = Vector3.TransformCoordinate(vertexList[i] * multiplier, world);
                min = Vector3.Min(min, transformed);
                max = Vector3.Max(max, transformed);
            }

            return new BoundingBox(min, max);
        }

        protected static float? TriangleIntersection(Ray r, List<RenderWareFile.Triangle> triangles, List<Vector3> vertices, Matrix world)
        {
            Matrix.Invert(ref world, out Matrix invWorld);

            Vector3 localOrigin = Vector3.TransformCoordinate(r.Position, invWorld);
            Vector3 localDir = Vector3.TransformNormal(r.Direction, invWorld);
            float dirLength = localDir.Length();
            if (dirLength == 0f)
                return null;
            localDir /= dirLength;

            Ray localRay = new Ray(localOrigin, localDir);
            float? smallestLocalDist = null;

            for (int i = 0; i < triangles.Count; i++)
            {
                var t = triangles[i];
                Vector3 v1 = vertices[t.vertex1];
                Vector3 v2 = vertices[t.vertex2];
                Vector3 v3 = vertices[t.vertex3];

                if (localRay.Intersects(ref v1, ref v2, ref v3, out float distance) && (smallestLocalDist == null || distance < smallestLocalDist))
                    smallestLocalDist = distance;
            }

            if (smallestLocalDist == null)
                return null;

            // Convert local hit distance back to accurate world-space distance
            Vector3 localHit = localOrigin + localDir * smallestLocalDist.Value;
            Vector3 worldHit = Vector3.TransformCoordinate(localHit, world);
            return Vector3.Distance(r.Position, worldHit);
        }
    }
}
