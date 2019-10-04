ClearListener()

struct AnimationAsset
(
	public BoneCount,
	public FrameCount,
	public KeyframeCount,
	public Keyframes = #(),
	public TimeMap = #(),
	public KeyframeMap = #()
)

struct Keyframe
(
    public Frame,
    public Rotation,
    public Translation
)

global glAsset = undefined

global QUAT_SCALE = 1.0 / 32767

rollout animLoader "ANIM Importer"
(
	group "About"
	(
		label lab1 "ANIM Asset Importer"
		label lab2 "Platform: Xbox/PS2"
		label lab3 "by igorseabra4, Seil"
	)
	group "Import"
	(
		button importButton "Import..."
	)
	
	function ReadAnimationsFromFile filePath =
	(
		local reader = fopen filePath "rb"
		
		local asset = AnimationAsset()
		
		local magic = ReadLong reader
		
		local unkInt1 = ReadLong reader
		
		asset.BoneCount = ReadShort reader
		asset.FrameCount = ReadShort reader
		asset.KeyframeCount =  ReadLong reader
		
		local scaleX = ReadFloat reader
		local scaleY = ReadFloat reader
		local scaleZ = ReadFloat reader
				
		for i = 1 to asset.KeyframeCount do
		(
			local kf = Keyframe()
			
			kf.Frame = ReadShort reader
				
			local RotationX = (ReadShort reader) * QUAT_SCALE
			local RotationY = (ReadShort reader) * QUAT_SCALE
			local RotationZ = (ReadShort reader) * QUAT_SCALE
			local RotationW = (ReadShort reader) * QUAT_SCALE
			
			local TranslationX = (ReadShort reader) * scaleX
			local TranslationY = (ReadShort reader) * scaleY
			local TranslationZ = (ReadShort reader) * scaleZ
			
			kf.Rotation = quat RotationX RotationY RotationZ RotationW
			kf.Translation = [ TranslationX, TranslationY, TranslationZ ]
			
			append asset.Keyframes kf
		)
		
		for i = 1 to asset.FrameCount do
		(
			append asset.TimeMap (ReadFloat reader)
		)
		
		for i = 1 to (asset.FrameCount - 1) do
		(
			local frameList = #()
			
			for j = 1 to asset.BoneCount do
			(
				local frameIndex = ReadShort reader				
				append framelist frameIndex
			)
			
			append asset.KeyframeMap framelist
		)
		
		fclose reader
						
		glAsset = asset
	)
    
    function TraverseNode curNode &nodes =
    (
        append nodes curNode
        for childNode in curNode.Children do
            TraverseNode childNode &nodes
    )
    
    function GetAnimationNodes =
    (		
        local nodes = #()
        
        if ( nodes.Count == 0 ) then
        (
            local animRootNode = GetNodeByName "Dummy002"
            
            TraverseNode animRootNode &nodes
        )
        
        return nodes
    )
    
    function LoadAnimation =
	(
		if ( mAnimationNodes == undefined ) then
		mAnimationNodes = GetAnimationNodes()
		
		animationRange = interval 0 (glAsset.FrameCount)
		
		with animate on
		(
			for frameIndex = 1 to (glAsset.FrameCount - 1) do
			(
				local keyframeList = glAsset.KeyframeMap[frameIndex]
				for boneIndex = 1 to glAsset.BoneCount do
				(
					local keyframeIndex = keyframeList[boneIndex] + 1
					local kf = glAsset.Keyframes[keyframeIndex]
					at time (kf.Frame)
					(
						mAnimationNodes[boneIndex].Controller.Rotation = inverse kf.Rotation
						mAnimationNodes[boneIndex].Controller.Position = kf.Translation
					)
				)
			)
		)
	)

	on importButton pressed do
	(
		local filePath = getOpenFileName caption:"Select Xbox/PS2 ANM file" \
					types:"ANM Files (*.anm)|*.anm|All Files (*.*)|*.*|"
	
		ReadAnimationsFromFile(filePath)
		LoadAnimation()
	)
)

createDialog animLoader 160 160