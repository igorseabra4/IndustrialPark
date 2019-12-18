ClearListener()

struct AnimAsset
(
	flags,
	boneCount,
	scale,
	keys = #(),
	times = #(),
	offsets = #()
)

struct AnimKey
(
	timeIndex,
	rot,
	pos
)

global swapBytes = false

utility BFBBAnimTools "BFBB Anim Tools"
(
	group "About"
	(
		label lab1 "BFBB Anim Tools"
		label lab2 "by Seil and igorseabra4"
	)
	group "Import"
	(
		button importButton "Import..."
	)
	group "Export"
	(
		radiobuttons platform labels:#("GameCube", "PlayStation2/Xbox")
		button exportButton "Export..."
	)
	
	function SwapInt x =
	(
		x = bit.swapBytes x 1 4
		x = bit.swapBytes x 2 3
		
		-- should sign extend but ehh don't care
		
		x
	)
	
	function SwapShort x =
	(
		x = bit.swapBytes x 1 2
		
		-- sign extend
		local b = bit.get x 16
		for i = 17 to 32 do x = bit.set x i b
		
		x
	)
	
	function _ReadInt stream sign:#signed =
	(
		local x = ReadLong stream sign
		
		if swapBytes then x = SwapInt x
		
		x
	)
	
	function _ReadShort stream sign:#signed =
	(
		if swapBytes then sign = #unsigned
		
		local x = ReadShort stream sign
		
		if swapBytes then x = SwapShort x
		
		x
	)
	
	function _ReadFloat stream =
	(
		bit.intAsFloat (_ReadInt stream)
	)
	
	function _WriteInt stream x =
	(
		if swapBytes then
			WriteLong stream (SwapInt x)
		else
			WriteLong stream x
	)
	
	function _WriteShort stream x =
	(
		if swapBytes then
			WriteShort stream (SwapShort x)
		else
			WriteShort stream x
	)
	
	function _WriteFloat stream x =
	(
		if swapBytes then
			WriteLong stream (SwapInt (bit.floatAsInt x))
		else
			WriteFloat stream x
	)
	
	function ReadAnimFile filename =
	(
		local stream = fopen filename "rb"
		swapBytes = false
		
		local magic = _ReadInt stream
		
		if (magic == 0x534B4231) then
			swapBytes = true
		else if (magic == 0x31424B53) then
			swapBytes = false
		else
		(
			messageBox "Not a valid ANIM file!" title:"Error"
			undefined
		)
		
		local asset = AnimAsset()
		
		asset.flags = _ReadInt stream
		asset.boneCount = _ReadShort stream
		
		local timeCount = _ReadShort stream
		local keyCount = _ReadInt stream
		
		asset.scale = point3 \
			(_ReadFloat stream) \
			(_ReadFloat stream) \
			(_ReadFloat stream)
		
		for i = 1 to keyCount do
		(
			local key = AnimKey()
			
			key.timeIndex = _ReadShort stream
			
			key.rot = quat \
				(_ReadShort stream / 32767.0) \
				(_ReadShort stream / 32767.0) \
				(_ReadShort stream / 32767.0) \
				(_ReadShort stream / 32767.0)
			
			key.pos = point3 \
				(_ReadShort stream * asset.scale.x) \
				(_ReadShort stream * asset.scale.y) \
				(_ReadShort stream * asset.scale.z)
			
			append asset.keys key
		)
		
		for i = 1 to timeCount do
		(
			append asset.times (_ReadFloat stream)
		)
		
		for i = 1 to timeCount - 1 do
		(
			local boneOffsets = #()
			
			for j = 1 to asset.boneCount do
			(
				append boneOffsets (_ReadShort stream sign:#unsigned)
			)
			
			append asset.offsets boneOffsets
		)
		
		fclose stream
		
		asset
	)
	
	function WriteAnimFile asset filename =
	(
		local stream = fopen filename "wb"
		
		swapBytes = (platform.state == 1)
		
		_WriteInt stream 0x31424B53
		_WriteInt stream asset.flags
		_WriteShort stream asset.boneCount
		_WriteShort stream asset.times.count
		_WriteInt stream asset.keys.Count
		_WriteFloat stream asset.scale.x
		_WriteFloat stream asset.scale.y
		_WriteFloat stream asset.scale.z
		
		for key in asset.keys do
		(
			_WriteShort stream key.timeIndex
			_WriteShort stream (key.rot.x * 32767)
			_WriteShort stream (key.rot.y * 32767)
			_WriteShort stream (key.rot.z * 32767)
			_WriteShort stream (key.rot.w * 32767)
			_WriteShort stream (key.pos.x / asset.scale.x)
			_WriteShort stream (key.pos.y / asset.scale.y)
			_WriteShort stream (key.pos.z / asset.scale.z)
		)
		
		for t in asset.times do _WriteFloat stream t
		
		for i in asset.offsets do
			for j in i do _WriteShort stream j
		
		fclose stream
	)
    
    function TraverseNode curNode &nodes =
    (
        append nodes curNode
        for childNode in curNode.Children do
            TraverseNode childNode &nodes
    )
    
    function GetDummyNodes =
    (		
        local nodes = #()
        
        if ( nodes.Count == 0 ) then
        (
            local animRootNode = GetNodeByName "Dummy002"
            
            TraverseNode animRootNode &nodes
        )
        
        nodes
    )
	
	function AnimFrameRate asset =
	(
		local minDiff = -1
		
		for i = 1 to asset.times.count - 1 do
		(
			local diff = asset.times[i+1] - asset.times[i]
			
			if minDiff == -1 or diff < minDiff then
				minDiff = diff
		)
		
		1 / minDiff
	)
	
	function AnimDuration asset =
	(
		asset.times[asset.times.count]
	)
	
	function LoadAnim asset =
	(
		frameRate = AnimFrameRate asset
		
		local numframes = AnimDuration asset * frameRate
		animationRange = interval 0 (numframes - 1)
		
		local nodes = GetDummyNodes()
		
		for node in nodes do
			deleteKeys node.controller #allKeys
		
		with animate on
		(
			for i = 1 to asset.times.count - 1 do
			(				
				for j = 1 to asset.boneCount do
				(
					local index = asset.offsets[i][j] + 1
					local key = asset.keys[index]
					
					local t = asset.times[key.timeIndex + 1] * frameRate
					
					at time t
					(
						nodes[j].controller.rotation = inverse key.rot
						nodes[j].controller.position = key.pos
					)
				)
			)
		)
	)
	
	function KeysAreEqual k1 k2 =
	(
		k1.rot == k2.rot and k1.pos == k2.pos
	)
	
	function Point3Abs p =
		[abs p.x, abs p.y, abs p.z]
	
	function Point3Max p1 p2 =
		[amax p1.x p2.x, amax p1.y p2.y, amax p1.z p2.z]
	
	function SaveAnim =
	(
		local asset = AnimAsset()
		asset.flags = 0
		
		local nodes = GetDummyNodes()
		local nodeCount = nodes.count
		
		asset.boneCount = nodeCount
		asset.scale = point3 0 0 0
		
		for t = 0 to animationRange.end do
		(
			append asset.times (t.frame as float / frameRate)
			append asset.offsets #()
		)
		
		for n = 1 to nodeCount do
		(
			local node = nodes[n]
			
			for t = 0 to animationRange.end do
			(
				local key = AnimKey()
				key.timeIndex = t
				
				at time t
				(
					key.rot = inverse node.controller.rotation
					key.pos = node.controller.position
				)
				
				local prevKey
				
				if asset.keys.count > 0 then
					prevKey = asset.keys[asset.keys.count]
				
				if t == 0 or prevKey == undefined or not KeysAreEqual key prevKey then
				(
					append asset.keys key
					asset.scale = Point3Max (Point3Abs key.pos) asset.scale
				)
				
				asset.offsets[t.frame+1][n] = asset.keys.count - 1
			)
		)
		
		asset.scale /= 32767
		
		asset
	)

	on importButton pressed do
	(
		local filename = getOpenFileName caption:"Import ANIM File" \
					types:"ANIM files (*.anm)|*.anm|ANIM files (*.anim)|*.anim|All Files (*.*)|*.*|"
		
		if filename != undefined then
			LoadAnim (ReadAnimFile filename)
	)
	
	on exportButton pressed do
	(
		local filename = getSaveFileName caption:"Export ANIM File" \
					types:"ANIM files (*.anm)|*.anm|ANIM files (*.anim)|*.anim|All Files (*.*)|*.*|"
		
		if filename != undefined then
			WriteAnimFile (SaveAnim()) (filename)
	)
)