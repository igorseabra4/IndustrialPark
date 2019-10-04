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
global bitConverter = dotNetClass "System.BitConverter"

global QUAT_SCALE = 1.0 / 32767

rollout animLoader "ANIM Importer"
(
	group "About"
	(
		label lab1 "ANIM Asset Importer"
		label lab2 "Platform: GameCube"
		label lab3 "by igorseabra4, Seil"
	)
	group "Import"
	(
		button importButton "Import..."
	)
	
	function ReadLongReversed input =
	(
		local intArray = #()
		local byte1 = ReadByte input
		local byte2 = ReadByte input
		local byte3 = ReadByte input
		local byte4 = ReadByte input
		append intArray byte4
		append intArray byte3
		append intArray byte2
		append intArray byte1
		
		local byteArray = dotnet.valueToDotNetObject intArray (dotnetclass "System.Byte[]")
		local returnValue = bitConverter.ToInt32 byteArray 0
		
		returnValue
	)
	
	function ReadFloatReversed input =
	(
		local intArray = #()
		local byte1 = ReadByte input
		local byte2 = ReadByte input
		local byte3 = ReadByte input
		local byte4 = ReadByte input
		append intArray byte4
		append intArray byte3
		append intArray byte2
		append intArray byte1
		
		local byteArray = dotnet.valueToDotNetObject intArray (dotnetclass "System.Byte[]")
		local returnValue = bitConverter.ToSingle byteArray 0
		
		returnValue
	)
	
	function ReadShortReversed input =
	(
		local intArray = #()
		local byte1 = ReadByte input
		local byte2 = ReadByte input
		append intArray byte2
		append intArray byte1
		
		local byteArray = dotnet.valueToDotNetObject intArray (dotnetclass "System.Byte[]")
		local returnValue = bitConverter.ToInt16 byteArray 0
		
		returnValue
	)
	
	function ReadAnimationsFromFile filePath =
	(
		local reader = fopen filePath "rb"
		
		local asset = AnimationAsset()
		
		local magic = ReadLong reader
		
		local unkInt1 = ReadLongReversed reader
		
		asset.BoneCount = ReadShortReversed reader		
		asset.FrameCount = ReadShortReversed reader		
		asset.KeyframeCount =  ReadLongReversed reader
		
		local scaleX = ReadFloatReversed reader
		local scaleY = ReadFloatReversed reader
		local scaleZ = ReadFloatReversed reader
				
		for i = 1 to asset.KeyframeCount do
		(
			local kf = Keyframe()
			
			kf.Frame = ReadShortReversed reader
				
			local RotationX = (ReadShortReversed reader) * QUAT_SCALE
			local RotationY = (ReadShortReversed reader) * QUAT_SCALE
			local RotationZ = (ReadShortReversed reader) * QUAT_SCALE
			local RotationW = (ReadShortReversed reader) * QUAT_SCALE
			
			local TranslationX = (ReadShortReversed reader) * scaleX
			local TranslationY = (ReadShortReversed reader) * scaleY
			local TranslationZ = (ReadShortReversed reader) * scaleZ
			
			kf.Rotation = quat RotationX RotationY RotationZ RotationW
			kf.Translation = [ TranslationX, TranslationY, TranslationZ ]
			
			append asset.Keyframes kf
		)
		
		for i = 1 to asset.FrameCount do
		(
			append asset.TimeMap (ReadFloatReversed reader)
		)
		
		for i = 1 to (asset.FrameCount - 1) do
		(
			local frameList = #()
			
			for j = 1 to asset.BoneCount do
			(
				local frameIndex = ReadShortReversed reader				
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
		local filePath = getOpenFileName caption:"Select GCN ANM file" \
					types:"ANM Files (*.anm)|*.anm|All Files (*.*)|*.*|"
	
		ReadAnimationsFromFile(filePath)
		LoadAnimation()
	)
)

createDialog animLoader 160 160