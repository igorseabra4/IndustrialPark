using HipHopFile;
using Newtonsoft.Json;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace IndustrialPark
{
    public abstract class GenericAssetDataContainer
    {
        [JsonProperty]
        protected Game _game;

        [Browsable(false)]
        public Game game => _game;
        
        public void SetGame(Game game)
        {
            _game = game;

            var typeProperties = GetType().GetProperties();

            foreach (var gadc in typeProperties.Where(prop => typeof(GenericAssetDataContainer).IsAssignableFrom(prop.PropertyType)).Select(prop => (GenericAssetDataContainer)prop.GetValue(this)))
                gadc.SetGame(game);

            foreach (var gadcs in typeProperties.Where(prop => prop.PropertyType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)) && typeof(GenericAssetDataContainer).IsAssignableFrom(i.GenericTypeArguments[0]))).Select(prop => (IEnumerable<GenericAssetDataContainer>)prop.GetValue(this)))
                foreach (var gadc in gadcs)
                    gadc.SetGame(game);
        }

        public abstract void Serialize(EndianBinaryWriter writer);

        public byte[] Serialize(Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                Serialize(writer);
                return writer.ToArray();
            }
        }

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

        public virtual void ReplaceReferences(uint oldAssetId, uint newAssetId)
        {
            if (oldAssetId == 0)
                return;

            var typeProperties = GetType().GetProperties();

            foreach (var prop in typeProperties.Where(prop => prop.PropertyType.Equals(typeof(AssetID)) && ((AssetID)prop.GetValue(this)).Equals(oldAssetId)))
                prop.SetValue(this, new AssetID(newAssetId));

            foreach (var gadc in typeProperties.Where(prop => typeof(GenericAssetDataContainer).IsAssignableFrom(prop.PropertyType)).Select(prop => (GenericAssetDataContainer)prop.GetValue(this)))
                gadc.ReplaceReferences(oldAssetId, newAssetId);

            foreach (var array in typeProperties.Where(prop => prop.PropertyType.Equals(typeof(AssetID[]))).Select(prop => (AssetID[])prop.GetValue(this)))
                for (int i = 0; i < array.Length; i++)
                    if (array[i] == oldAssetId)
                        array[i] = newAssetId;

            foreach (var gadcs in typeProperties.Where(prop => prop.PropertyType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)) && typeof(GenericAssetDataContainer).IsAssignableFrom(i.GenericTypeArguments[0]))).Select(prop => (IEnumerable<GenericAssetDataContainer>)prop.GetValue(this)))
                foreach (var gadc in gadcs)
                    gadc.ReplaceReferences(oldAssetId, newAssetId);
        }

        public virtual void Verify(ref List<string> result)
        {
            var typeProperties = GetType().GetProperties();

            foreach (var prop in typeProperties.Where(prop => prop.PropertyType.Equals(typeof(AssetID))))
                if (prop.GetCustomAttribute(typeof(IgnoreVerificationAttribute)) == null)
                    Verify((AssetID)prop.GetValue(this), prop.Name, prop.GetCustomAttribute(typeof(ValidReferenceRequiredAttribute)) != null, ref result);

            foreach (var gadc in typeProperties.Where(prop => typeof(GenericAssetDataContainer).IsAssignableFrom(prop.PropertyType)).Select(prop => (GenericAssetDataContainer)prop.GetValue(this)))
                gadc.Verify(ref result);

            foreach (var prop in typeProperties.Where(prop => prop.PropertyType.Equals(typeof(AssetID[]))))
                if (prop.GetCustomAttribute(typeof(IgnoreVerificationAttribute)) == null)
                {
                    var array = (AssetID[])prop.GetValue(this);
                    foreach (var assetID in array)
                        Verify(assetID, prop.Name, prop.GetCustomAttribute(typeof(ValidReferenceRequiredAttribute)) != null, ref result);
                }

            foreach (var gadcs in typeProperties.Where(prop => prop.PropertyType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)) && typeof(GenericAssetDataContainer).IsAssignableFrom(i.GenericTypeArguments[0]))).Select(prop => (IEnumerable<GenericAssetDataContainer>)prop.GetValue(this)))
                foreach (var gadc in gadcs)
                    gadc.Verify(ref result);
        }

        protected static void Verify(uint assetID, string propName, bool validReferenceRequired, ref List<string> result)
        {
            if (assetID != 0 && !Program.MainForm.AssetExists(assetID))
                result.Add("Referenced asset 0x" + assetID.ToString("X8") + " was not found in any open archive.");
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
