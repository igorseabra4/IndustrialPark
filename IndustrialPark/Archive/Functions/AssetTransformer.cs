using SharpDX;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark.Archive;

public static class AssetTransformer
{
    public static void SetAssetPositionToView(Asset asset)
    {
        Vector3 Position = Program.Renderer.Camera.Position + 3 * Program.Renderer.Camera.Forward;

        if (asset is AssetTRIG trig)
        {
            trig.PositionX = Position.X;
            trig.PositionY = Position.Y;
            trig.PositionZ = Position.Z;

            trig.MinimumX = Position.X;
            trig.MinimumY = Position.Y;
            trig.MinimumZ = Position.Z;
        }
        else if (asset is AssetCAM cam)
        {
            cam.SetPosition(Program.Renderer.Camera.Position);
            cam.SetNormalizedForward(Program.Renderer.Camera.Forward);
            cam.SetNormalizedUp(Program.Renderer.Camera.Up);
            cam.SetNormalizedLeft(Program.Renderer.Camera.Right);
            cam.CreateTransformMatrix(Program.Renderer);
        }
        else if (asset is IClickableAsset ir)
        {
            ir.PositionX = Position.X;
            ir.PositionY = Position.Y;
            ir.PositionZ = Position.Z;
        }
    }

    public static void SetAssetPositionsPreserveRelative(IEnumerable<IClickableAsset> assets, Vector3 position)
    {
        if (assets == null || !assets.Any())
            return;

        Vector3 center = Vector3.Zero;

        foreach (var ca in assets)
            center += new Vector3(ca.PositionX, ca.PositionY, ca.PositionZ);
                
        center /= assets.Count();

        if (assets.Count() > 1 && Vector3.Distance(center, Vector3.Zero) < 0.001f)
            center = Vector3.Zero;

        foreach (var ca in assets)
        {
            Vector3 groupDelta = (assets.Count() == 1) ? Vector3.Zero : (new Vector3(ca.PositionX, ca.PositionY, ca.PositionZ) - center);

            ca.PositionX = position.X + groupDelta.X;
            ca.PositionY = position.Y + groupDelta.Y;
            ca.PositionZ = position.Z + groupDelta.Z;

            if (ca is AssetTRIG trig)
            {
                Vector3 trigDelta = position - new Vector3(ca.PositionX, ca.PositionY, ca.PositionZ);
                if (trig.Shape == TriggerShape.Box)
                {
                    trig.SetPositions(
                        trig.MinimumX + trigDelta.X + groupDelta.X,
                        trig.MinimumY + trigDelta.Y + groupDelta.Y,
                        trig.MinimumZ + trigDelta.Z + groupDelta.Z,
                        trig.MaximumX + trigDelta.X + groupDelta.X,
                        trig.MaximumY + trigDelta.Y + groupDelta.Y,
                        trig.MaximumZ + trigDelta.Z + groupDelta.Z);
                }
                else
                {
                    trig.MinimumX = position.X + groupDelta.X;
                    trig.MinimumY = position.Y + groupDelta.Y;
                    trig.MinimumZ = position.Z + groupDelta.Z;
                }
            }
            else if (ca is AssetVOLU volu && volu.VolumeShape is VolumeBox box)
            {
                Vector3 trigDelta = position - new Vector3(ca.PositionX, ca.PositionY, ca.PositionZ);
                box.SetPositions(
                    box.MinimumX + trigDelta.X + groupDelta.X,
                    box.MinimumY + trigDelta.Y + groupDelta.Y,
                    box.MinimumZ + trigDelta.Z + groupDelta.Z,
                    box.MaximumX + trigDelta.X + groupDelta.X,
                    box.MaximumY + trigDelta.Y + groupDelta.Y,
                    box.MaximumZ + trigDelta.Z + groupDelta.Z);
            }
        }
    }
}
