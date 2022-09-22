using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AttackTableSection : GenericAssetDataContainer
    {
        public AssetID section { get; set; }
        public ushort start { get; set; }
        public ushort count { get; set; }

        public AttackTableSection(EndianBinaryReader reader)
        {
            section = reader.ReadUInt32();
            start = reader.ReadUInt16();
            count = reader.ReadUInt16();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(section);
            writer.Write(start);
            writer.Write(count);
        }
    }

    public class AttackTableEntry : GenericAssetDataContainer
    {
        public AssetID animationStateID { get; set; }
        public uint animationState { get; set; }
        public ushort animationStart { get; set; }
        public ushort animationCount { get; set; }
        public ushort start { get; set; }
        public ushort count { get; set; }
        public ushort onFlags { get; set; }
        public ushort offFlags { get; set; }
        public byte input { get; set; }
        public byte power { get; set; }
        public AssetSingle startTime { get; set; }

        public AttackTableEntry(EndianBinaryReader reader)
        {
            animationStateID = reader.ReadUInt32();
            animationState = reader.ReadUInt32();
            animationStart = reader.ReadUInt16();
            animationCount = reader.ReadUInt16();
            start = reader.ReadUInt16();
            count = reader.ReadUInt16();
            onFlags = reader.ReadUInt16();
            offFlags = reader.ReadUInt16();
            input = reader.ReadByte();
            power = reader.ReadByte();
            reader.BaseStream.Position += 2;
            startTime = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(animationStateID);
            writer.Write(animationState);
            writer.Write(animationStart);
            writer.Write(animationCount);
            writer.Write(start);
            writer.Write(count);
            writer.Write(onFlags);
            writer.Write(offFlags);
            writer.Write(input);
            writer.Write(power);
            writer.Write(new byte[2]);
            writer.Write(startTime);
        }
    }

    public class AttackTableTransition : GenericAssetDataContainer
    {
        public AssetID sourceState { get; set; }
        public AssetID destinationState { get; set; }
        public AssetSingle sourceTime { get; set; }
        public AssetSingle throughTime { get; set; }
        public AssetSingle destinationTime { get; set; }
        public AssetSingle blendTime { get; set; }
        public uint flags { get; set; }

        public AttackTableTransition(EndianBinaryReader reader)
        {
            sourceState = reader.ReadUInt32();
            destinationState = reader.ReadUInt32();
            sourceTime = reader.ReadSingle();
            throughTime = reader.ReadSingle();
            destinationTime = reader.ReadSingle();
            blendTime = reader.ReadSingle();
            flags = reader.ReadUInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(sourceState);
            writer.Write(destinationState);
            writer.Write(sourceTime);
            writer.Write(throughTime);
            writer.Write(destinationTime);
            writer.Write(blendTime);
            writer.Write(flags);
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AttackTableStateHitBoneInfo : GenericAssetDataContainer
    {
        public short bone { get; set; }
        public AssetSingle boneOffsetX { get; set; }
        public AssetSingle boneOffsetY { get; set; }
        public AssetSingle boneOffsetZ { get; set; }
        public short atomic { get; set; }

        public AttackTableStateHitBoneInfo(EndianBinaryReader reader)
        {
            bone = reader.ReadInt16();
            reader.BaseStream.Position += 2;
            boneOffsetX = reader.ReadSingle();
            boneOffsetY = reader.ReadSingle();
            boneOffsetZ = reader.ReadSingle();
            atomic = reader.ReadInt16();
            reader.BaseStream.Position += 2;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(bone);
            writer.Write(new byte[2]);
            writer.Write(boneOffsetX);
            writer.Write(boneOffsetY);
            writer.Write(boneOffsetZ);
            writer.Write(atomic);
            writer.Write(new byte[2]);
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AttackTableStateEffectBone : GenericAssetDataContainer
    {
        public short bone { get; set; }
        public int position { get; set; }

        public AttackTableStateEffectBone(EndianBinaryReader reader)
        {
            bone = reader.ReadInt16();
            reader.BaseStream.Position += 2;
            position = reader.ReadInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(bone);
            writer.Write(new byte[2]);
            writer.Write(position);
        }
    }

    public class AttackTableState : GenericAssetDataContainer
    {
        public AssetID state { get; set; }
        public AssetSingle moveDistanceZ { get; set; }
        public AssetSingle moveDistanceY { get; set; }
        public AssetSingle moveTime { get; set; }
        public AssetSingle attackStart { get; set; }
        public AssetSingle attackEnd { get; set; }
        public AssetSingle attackRadius { get; set; }
        public AttackTableStateHitBoneInfo hitBone1 { get; set; }
        public AttackTableStateHitBoneInfo hitBone2 { get; set; }
        public AttackTableStateHitBoneInfo hitBone3 { get; set; }
        public AttackTableStateHitBoneInfo hitBone4 { get; set; }
        public short damage { get; set; }
        public ushort source { get; set; }
        public ushort effect { get; set; }
        public ushort hitEffect { get; set; }
        public AssetSingle effectStart { get; set; }
        public AssetSingle effectEnd { get; set; }
        public AttackTableStateEffectBone effectBoneOutside1 { get; set; }
        public AttackTableStateEffectBone effectBoneOutside2 { get; set; }
        public AttackTableStateEffectBone effectBoneInside1 { get; set; }
        public AttackTableStateEffectBone effectBoneInside2 { get; set; }
        public AssetID bonePosition0 { get; set; }
        public AssetID bonePosition1 { get; set; }
        public AssetSingle rumbleStartTime { get; set; }
        public AssetID rumbleEmitterID { get; set; }
        public AssetID shrapID { get; set; }
        public AssetID shrapAsset { get; set; }
        public AssetSingle shrapStartTime { get; set; }
        public AssetSingle velocityUp { get; set; }
        public AssetSingle velocityAway { get; set; }
        public uint flags { get; set; }
        public AssetSingle holdTime { get; set; }
        public AssetSingle jumpBreakTime { get; set; }
        public AssetSingle crouchBreakTime { get; set; }
        public AssetSingle turnLockStart { get; set; }
        public AssetSingle turnLockStop { get; set; }
        public AssetSingle climaxTime { get; set; }
        public AssetSingle climaxOffsetX { get; set; }
        public AssetSingle climaxOffsetY { get; set; }
        public AssetSingle climaxOffsetZ { get; set; }
        public AssetSingle drainRate { get; set; }
        public AssetSingle blurStart { get; set; }
        public AssetSingle blurEnd { get; set; }
        public AssetSingle blurLife { get; set; }
        public AssetSingle blurAlpha { get; set; }
        public AssetSingle blurFadeInTime { get; set; }
        public AssetSingle blurFadeOutTime { get; set; }
        public short flashAlpha { get; set; }
        public AssetSingle flashTime { get; set; }
        public AssetSingle comboBonus { get; set; }
        public short comboType { get; set; }
        public short powerBonus { get; set; }

        public AttackTableState(EndianBinaryReader reader)
        {
            state = reader.ReadUInt32();
            moveDistanceZ = reader.ReadSingle();
            moveDistanceY = reader.ReadSingle();
            moveTime = reader.ReadSingle();
            attackStart = reader.ReadSingle();
            attackEnd = reader.ReadSingle();
            attackRadius = reader.ReadSingle();
            hitBone1 = new AttackTableStateHitBoneInfo(reader);
            hitBone2 = new AttackTableStateHitBoneInfo(reader);
            hitBone3 = new AttackTableStateHitBoneInfo(reader);
            hitBone4 = new AttackTableStateHitBoneInfo(reader);
            damage = reader.ReadInt16();
            source = reader.ReadUInt16();
            effect = reader.ReadUInt16();
            hitEffect = reader.ReadUInt16();
            effectStart = reader.ReadSingle();
            effectEnd = reader.ReadSingle();
            effectBoneOutside1 = new AttackTableStateEffectBone(reader);
            effectBoneOutside2 = new AttackTableStateEffectBone(reader);
            effectBoneInside1 = new AttackTableStateEffectBone(reader);
            effectBoneInside2 = new AttackTableStateEffectBone(reader);
            bonePosition0 = reader.ReadUInt32();
            bonePosition1 = reader.ReadUInt32();
            rumbleStartTime = reader.ReadSingle();
            rumbleEmitterID = reader.ReadUInt32();
            shrapID = reader.ReadUInt32();
            shrapAsset = reader.ReadUInt32();
            shrapStartTime = reader.ReadSingle();
            velocityUp = reader.ReadSingle();
            velocityAway = reader.ReadSingle();
            flags = reader.ReadUInt32();
            holdTime = reader.ReadSingle();
            jumpBreakTime = reader.ReadSingle();
            crouchBreakTime = reader.ReadSingle();
            turnLockStart = reader.ReadSingle();
            turnLockStop = reader.ReadSingle();
            climaxTime = reader.ReadSingle();
            climaxOffsetX = reader.ReadSingle();
            climaxOffsetY = reader.ReadSingle();
            climaxOffsetZ = reader.ReadSingle();
            drainRate = reader.ReadSingle();
            blurStart = reader.ReadSingle();
            blurEnd = reader.ReadSingle();
            blurLife = reader.ReadSingle();
            blurAlpha = reader.ReadSingle();
            blurFadeInTime = reader.ReadSingle();
            blurFadeOutTime = reader.ReadSingle();
            flashAlpha = reader.ReadInt16();
            reader.BaseStream.Position += 2;
            flashTime = reader.ReadSingle();
            comboBonus = reader.ReadSingle();
            comboType = reader.ReadInt16();
            powerBonus = reader.ReadInt16();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(state);
            writer.Write(moveDistanceZ);
            writer.Write(moveDistanceY);
            writer.Write(moveTime);
            writer.Write(attackStart);
            writer.Write(attackEnd);
            writer.Write(attackRadius);
            hitBone1.Serialize(writer);
            hitBone2.Serialize(writer);
            hitBone3.Serialize(writer);
            hitBone4.Serialize(writer);
            writer.Write(damage);
            writer.Write(source);
            writer.Write(effect);
            writer.Write(hitEffect);
            writer.Write(effectStart);
            writer.Write(effectEnd);
            effectBoneOutside1.Serialize(writer); 
            effectBoneOutside2.Serialize(writer);
            effectBoneInside1.Serialize(writer);
            effectBoneInside2.Serialize(writer);
            writer.Write(bonePosition0);
            writer.Write(bonePosition1);
            writer.Write(rumbleStartTime);
            writer.Write(rumbleEmitterID);
            writer.Write(shrapID);
            writer.Write(shrapAsset);
            writer.Write(shrapStartTime);
            writer.Write(velocityUp);
            writer.Write(velocityAway);
            writer.Write(flags);
            writer.Write(holdTime);
            writer.Write(jumpBreakTime);
            writer.Write(crouchBreakTime);
            writer.Write(turnLockStart);
            writer.Write(turnLockStop);
            writer.Write(climaxTime);
            writer.Write(climaxOffsetX);
            writer.Write(climaxOffsetY);
            writer.Write(climaxOffsetZ);
            writer.Write(drainRate);
            writer.Write(blurStart);
            writer.Write(blurEnd);
            writer.Write(blurLife);
            writer.Write(blurAlpha);
            writer.Write(blurFadeInTime);
            writer.Write(blurFadeOutTime);
            writer.Write(flashAlpha);
            writer.Write(new byte[2]);
            writer.Write(flashTime);
            writer.Write(comboBonus);
            writer.Write(comboType);
            writer.Write(powerBonus);
        }
    }

    public class AssetATKT : Asset
    {
        private const string categoryName = "Attack Table";

        [Category(categoryName)]
        public AttackTableSection[] Sections { get; set; }
        [Category(categoryName)]
        public AttackTableEntry[] Entries { get; set; }
        [Category(categoryName)]
        public AttackTableTransition[] Transitions { get; set; }
        [Category(categoryName)]
        public AttackTableState[] States { get; set; }
        
        public AssetATKT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            if (AHDR.data.Length == 0)
                return;

            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                var sectionCount = reader.ReadInt16();
                var entryCount = reader.ReadInt16();
                var transitionCount = reader.ReadInt16();
                var stateCount = reader.ReadInt16();

                Sections = new AttackTableSection[sectionCount];
                for (int i = 0; i < sectionCount; i++)
                    Sections[i] = new AttackTableSection(reader);

                Entries = new AttackTableEntry[entryCount];
                for (int i = 0; i < entryCount; i++)
                    Entries[i] = new AttackTableEntry(reader);

                Transitions = new AttackTableTransition[transitionCount];
                for (int i = 0; i < transitionCount; i++)
                    Transitions[i] = new AttackTableTransition(reader);

                States = new AttackTableState[stateCount];
                for (int i = 0; i < stateCount; i++)
                    States[i] = new AttackTableState(reader);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            if (Sections == null || Entries == null || Transitions == null || States == null)
                return;

            writer.Write((short)Sections.Length);
            writer.Write((short)Entries.Length);
            writer.Write((short)Transitions.Length);
            writer.Write((short)States.Length);

            foreach (var s in Sections)
                s.Serialize(writer);
            foreach (var s in Entries)
                s.Serialize(writer);
            foreach (var s in Transitions)
                s.Serialize(writer);
            foreach (var s in States)
                s.Serialize(writer);
        }
    }
}