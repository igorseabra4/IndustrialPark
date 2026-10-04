using HipHopFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        protected List<Layer> Layers;

        private bool _noLayers = false;
        public bool NoLayers
        {
            get => _noLayers;
            set
            {
                if (value)
                {
                    Layers = new List<Layer>();
                }
                else
                {
                    try
                    {
                        Layers = BuildLayers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                }
                _noLayers = value;
                UnsavedChanges = true;
                SelectedLayerIndex = -1;
            }
        }

        public int SelectedLayerIndex = -1;

        public int LayerCount => Layers.Count;
        public int GetLayerType() => LayerTypeGenericToSpecific(Layers[SelectedLayerIndex].Type, game);

        public void SetLayerType(int type) => Layers[SelectedLayerIndex].Type = LayerTypeSpecificToGeneric(type, game);

        public string LayerToString() => LayerToString(SelectedLayerIndex);

        public string LayerToString(int index) => "Layer " + index.ToString("D2") + ": "
            + (string.IsNullOrWhiteSpace(Layers[index].LayerName) ? Layers[index].Type.ToString() : Layers[index].LayerName)
            + " [" + Layers[index].AssetIDs.Count() + "]";

        public List<uint> GetAssetIDsOnLayer(int layer = -1) => NoLayers ?
            (from Asset a in assetDictionary.Values select a.assetID).ToList() :
            Layers[layer == -1 ? SelectedLayerIndex : layer].AssetIDs;

        public int GetFirstActiveLayerIndex()
        {
            if (NoLayers || Layers.Count == 0)
                return -1;
            for (int i = 0; i < Layers.Count; i++)
                if (Layers[i].AssetIDs.Count > 0)
                    return i;
            return 0;
        }

        private static Layer LHDRToLayer(Section_LHDR LHDR, Game game, string layerName)
        {
            var layer = new Layer(LayerTypeSpecificToGeneric(LHDR.layerType, game), LHDR.assetIDlist.Count, layerName);
            foreach (var u in LHDR.assetIDlist)
                layer.AssetIDs.Add(u);
            return layer;
        }

        private static int LayerTypeGenericToSpecific(LayerType layerType, Game game)
        {
            if (game >= Game.Incredibles || layerType < LayerType.BSP)
                return (int)layerType;
            return (int)layerType - 1;
        }

        private static LayerType LayerTypeSpecificToGeneric(int layerType, Game game)
        {
            if (game >= Game.Incredibles || layerType < 2)
                return (LayerType)layerType;
            return (LayerType)(layerType + 1);
        }

        public void AddLayer(LayerType layerType = LayerType.DEFAULT, int index = -1)
        {
            if (NoLayers)
                return;

            if (index >= Layers.Count)
                index = Layers.Count;
            int newIndex = index != -1 ? index : Layers.Count;

            Layers.Insert(newIndex, new Layer(layerType));

            SelectedLayerIndex = newIndex;

            UnsavedChanges = true;
        }

        public void RemoveLayer()
        {
            if (NoLayers)
                return;

            foreach (uint u in Layers[SelectedLayerIndex].AssetIDs.ToArray())
                RemoveAsset(u);

            Layers.RemoveAt(SelectedLayerIndex);

            SelectedLayerIndex--;

            UnsavedChanges = true;
        }

        public void RemoveLayerOfType(LayerType type)
        {
            if (NoLayers)
                return;

            foreach (uint u in Layers.Where(l => l.Type == type).SelectMany(l => l.AssetIDs).ToArray())
                RemoveAsset(u);
            Layers.RemoveAll(l => l.Type == type);

            SelectedLayerIndex = Layers.Count - 1;
            UnsavedChanges = true;
        }

        public void MoveLayerUp()
        {
            if (NoLayers)
                return;

            if (SelectedLayerIndex > 0)
            {
                var previous = Layers[SelectedLayerIndex - 1];
                Layers[SelectedLayerIndex - 1] = Layers[SelectedLayerIndex];
                Layers[SelectedLayerIndex] = previous;
                UnsavedChanges = true;
            }
        }

        public void MoveLayerDown()
        {
            if (NoLayers)
                return;

            if (SelectedLayerIndex < Layers.Count - 1)
            {
                var post = Layers[SelectedLayerIndex + 1];
                Layers[SelectedLayerIndex + 1] = Layers[SelectedLayerIndex];
                Layers[SelectedLayerIndex] = post;
                UnsavedChanges = true;
            }
        }

        public int GetLayerFromAssetID(uint assetID)
        {
            if (NoLayers)
                return -1;

            for (int i = 0; i < Layers.Count; i++)
                if (Layers[i].AssetIDs.Contains(assetID))
                    return i;

            throw new Exception($"Asset ID {assetID:X8} is not present in any layer.");
        }

        /// <summary>
        /// Rename a layer
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <returns>True if layer has been successfully renamed, false otherwise</returns>
        public bool RenameLayer(int selectedIndex)
        {
            if (NoLayers)
                return false;

            var layer = Layers[selectedIndex];
            var rn = new RenameLayer(layer.LayerName);
            if (rn.ShowDialog() == DialogResult.OK)
            {
                layer.LayerName = string.IsNullOrWhiteSpace(rn.LayerName) ? null : rn.LayerName;
                UnsavedChanges = true;
                return true;
            }
            return false;
        }

        public List<AssetType> AssetTypesOnLayer() => NoLayers ?
            (from Asset a in assetDictionary.Values select a.assetType).Distinct().ToList() :
            (from uint a in Layers[SelectedLayerIndex].AssetIDs select assetDictionary[a].assetType).Distinct().ToList();

        public Dictionary<LayerType, HashSet<AssetType>> AssetTypesPerLayer()
        {
            var result = new Dictionary<LayerType, HashSet<AssetType>>();
            foreach (var l in Layers)
                result[l.Type] = (from uint a in l.AssetIDs select assetDictionary[a].assetType).Distinct().ToHashSet();
            return result;
        }
    }
}
