using HipHopFile;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class LayerComparer : IComparer<int>
    {
        private Game game;

        public LayerComparer(Game game)
        {
            this.game = game;
        }

        private static readonly List<int> layerOrderBFBB = new List<int> {
                (int)LayerType_BFBB.TEXTURE,
                (int)LayerType_BFBB.BSP,
                (int)LayerType_BFBB.JSPINFO,
                (int)LayerType_BFBB.MODEL,
                (int)LayerType_BFBB.ANIMATION,
                (int)LayerType_BFBB.DEFAULT,
                (int)LayerType_BFBB.CUTSCENE,
                (int)LayerType_BFBB.SRAM,
                (int)LayerType_BFBB.SNDTOC
            };
        private static readonly List<int> layerOrderTSSM = new List<int> {
                (int)LayerType_TSSM.TEXTURE,
                (int)LayerType_TSSM.TEXTURE_STRM,
                (int)LayerType_TSSM.BSP,
                (int)LayerType_TSSM.JSPINFO,
                (int)LayerType_TSSM.MODEL,
                (int)LayerType_TSSM.ANIMATION,
                (int)LayerType_TSSM.DEFAULT,
                (int)LayerType_TSSM.CUTSCENE,
                (int)LayerType_TSSM.SRAM,
                (int)LayerType_TSSM.SNDTOC,
                (int)LayerType_TSSM.CUTSCENETOC
            };

        public int Compare(int l1, int l2)
        {
            if (l1 == l2)
                return 0;

            if (game == Game.Scooby && layerOrderBFBB.Contains(l1) && layerOrderBFBB.Contains(l2))
                return layerOrderBFBB.IndexOf(l1) > layerOrderBFBB.IndexOf(l2) ? 1 : -1;

            if (game == Game.BFBB && layerOrderBFBB.Contains(l1) && layerOrderBFBB.Contains(l2))
                return layerOrderBFBB.IndexOf(l1) > layerOrderBFBB.IndexOf(l2) ? 1 : -1;

            if (game >= Game.Incredibles)
            {
                if ((l1 == 3 && l2 == 11) || (l1 == 11 && l2 == 3))
                    return 0;

                if (layerOrderTSSM.Contains(l1) && layerOrderTSSM.Contains(l2))
                    return layerOrderTSSM.IndexOf(l1) > layerOrderTSSM.IndexOf(l2) ? 1 : -1;
            }

            return 0;
        }
    }
}