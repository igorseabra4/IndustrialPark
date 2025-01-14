using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RenderWareFile;
using HipHopFile;
using RenderWareFile.Sections;

namespace IndustrialPark.Dialogs
{
    public partial class ChangeRWVersion : Form
    {
        public ChangeRWVersion(Section_AHDR ahdr)
        {
            InitializeComponent();
            Text = ahdr.ADBG.assetName;
            labelCurVersion.Text = new RWVersion(BitConverter.ToInt32(ahdr.data.Skip(8).Take(4).ToArray())).ToString();
        }

        public static byte[] ChangeVersion(Section_AHDR ahdr, ref RWVersion? targetVersion, ref bool rememberForAll)
        {
            if (rememberForAll && targetVersion.HasValue)
            {
                return ModifyVersion(ahdr.data, targetVersion.Value);
            }

            using (var dialog = new ChangeRWVersion(ahdr))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    rememberForAll = dialog.checkBoxRemember.Checked;
                    targetVersion = dialog.TargetRWVersion;
                    return ModifyVersion(ahdr.data, dialog.TargetRWVersion);
                }
                else
                {
                    rememberForAll = dialog?.checkBoxRemember?.Checked ?? true;
                    targetVersion = RWVersion.Undefined;
                    return ahdr.data;
                }
            }
        }

        private RWVersion TargetRWVersion => new RWVersion((byte)numericUpDownRenderware.Value, (byte)numericUpDownMajor.Value, (byte)numericUpDownMinor.Value, (byte)numericUpDownBinary.Value);

        private static byte[] ModifyVersion(byte[] file, RWVersion? version)
        {
            if (version == null || version == RWVersion.Undefined)
                return file;

            ReadFileMethods.treatStuffAsByteArray = true;
            RWSection[] rws = ReadFileMethods.ReadRenderWareFile(file);

            if (rws[0] is Clump_0010 clump)
                clump.geometryList.geometryList.ForEach(g => g.geometryExtension.extensionSectionList
                .RemoveAll(ex => ex.sectionIdentifier == RenderWareFile.Section.CollisionPLG));

            byte[] data = ReadFileMethods.ExportRenderWareFile(rws, version.Value);
            ReadFileMethods.treatStuffAsByteArray = false;
            return data;
        }

        /// <summary>
        /// Checks for a renderware version mismatch. Assumes platform is the same since we only change the version, not converting the binary
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="game"></param>
        /// <param name="type"></param>
        /// <param name="ver"></param>
        /// <returns></returns>
        public static bool IsVersionMismatch(Platform platform, Game game, AssetType type, RWVersion? ver)
        {
            if (ver == null || ver == RWVersion.Undefined) return false;

            if (type == AssetType.Texture || type == AssetType.TextureStream)
            {
                switch (platform)
                {
                    case Platform.GameCube when game >= Game.Incredibles && ver < (3, 5):
                    case Platform.GameCube when game == Game.BFBB && !ver.Value.InRange((3, 4, 0, 3), (3, 5)):
                    case Platform.GameCube when game == Game.Scooby && ver != (3, 3, 0, 2):
                    case Platform.PS2 when game == Game.Scooby && ver != (3, 1):
                    case Platform.PS2 when game == Game.ROTU && ver < (3, 7, 0, 2):
                    case Platform.PS2 when game >= Game.BFBB && game < Game.ROTU && ver != (3, 5):
                    case Platform.Xbox when game == Game.Scooby && ver != (3, 4, 0, 5):
                    case Platform.Xbox when game == Game.BFBB && ver != (3, 5):
                    case Platform.Xbox when game >= Game.Incredibles && ver < (3, 5):
                        return true;
                    default:
                        return false;
                }
            }
            else if (type == AssetType.Model || type == AssetType.JSP)
            {
                switch (platform)
                {
                    case Platform.GameCube when game >= Game.Incredibles && ver < (3, 5):
                    case Platform.GameCube when game == Game.BFBB && !ver.Value.InRange((3, 4, 0, 2), (3, 5)):
                        return true;
                    default:
                        return false; // Havent tested everything else
                }
            }
            return false;

        }

    }
}
