------------------------------------------------------------------
--HeroesPowerPlant vertex colored obj importer

--Heavily altered from Chris Cookson's original import script
--by Shadowth117

------------------------------------------------------------------
--Original script

-- Wavefront OBJ/MTL Import Utility	for gmax, v1.1				--
-- by Chris Cookson (cjcookson@hotmail.com)						--
--																--
-- All code (c) Copyright Chris Cookson, 2001					--
-- Please use this script any way you wish - just				--
-- drop me a line if you do something cool with it!				--
-- 																--
-- With thanks to Jack Palevich									--
--																--
-- Changelog:													--
--																--
-- v1.1(a)	Fixed bug with importing objects with no materials	--
-- v1.1		Should now work with 3DSMAX R3/4					--
--																--
------------------------------------------------------------------

rollout objImport "Vcolor OBJ Import"
(
		--------------------
	-- User Interface --
	--------------------
	group "About"
	(
		label lab1 "VColor OBJ Importer"
		HyperLink addy "Originally by Chris Cookson" align:#center address:"mailto:cjcookson@hotmail.com" color:(color 0 0 170) hoverColor:(color 170 0 0)
		Label lab2 "Edited by Shadowth117"
	)
	group "Settings"
	(
		checkBox autoSizeCheckBox "Rescale" checked:false align:#center
		spinner autoSizeSpinner "Scale by:" align:#center type:#float range:[0,10000,100] enabled:false
		checkBox YZCheckBox "Y-Z Axis Flip" checked: true align:#center
		checkBox UVFlip "Vertical UV Flip" checked: false align:#center
	)	
	group "Import"
	(
		button importButton "Import..."
	)
	group "Import from Path"
	(
		button importPath "Import..."
	)
	----------
	-- Vars --
	----------
	local currentPath=""

	-------------
	-- Structs --
	-------------
	struct Tokenizer
	(
		tokens = #(),
		
		fn SetString str=
		(
			tokens = filterString str "\t ,"
		),
		
		fn ReadToken=
		(
			if tokens.count > 0 then
			(
				local tok = tokens[1]
				deleteItem tokens 1
				tok
			) else
			(
				undefined
			)
		),
		
		fn PeekToken=
		(
			if tokens.count > 0 then tokens[1] else undefined
		)
	)
	

	
	----------------------
	-- Helper Functions --
	----------------------

	-- Read a float from a tokenizer with default fallback
	function ReadFloat tkzr default:0.0 =
	(
		local floatStr = tkzr.ReadToken()
		if (floatStr != undefined and floatStr != "nan") then
			return floatStr as float
		else
			return default
	)
	
	-- Read an integer from a tokenizer with default fallback
	function ReadInt tkzr default:0 =
	(
		local intStr = tkzr.ReadToken()
		if intStr != undefined then
			return intStr as float
		else
			return default
	)
	
	-- Read a face vertex definition of the form v/t/n where t or n are optional
	function ReadFaceVertex tkzr =
	(
		local token = tkzr.ReadToken()
		local vdef = filterString token "/"

		local v = vdef[1] as integer
		local t = 0
		local n = 0
		
		-- Is there a texcoord or just a blank?
		if (vdef.count > 1) and (findString token "//") == undefined then
		(
			t = vdef[2] as integer
			if vdef.count == 3 then n = vdef[3] as integer
		) else
		(
			if vdef.count == 2 then n = vdef[2] as integer
		)
		
		[ v, t, n ]
	)
	
	-- Load a MTL material library definition file and return a gmax multi material
	function LoadMaterialLibrary filename: filepath: =
	(
		local newMaterial = multimaterial numsubs:0
		if filename == undefined then
		(
			-- Return a default MAX material
			newMaterial.numsubs = 1
			newMaterial[1] = standard name:"Unnamed"
			return newMaterial
		)
		
		-- Check file exists
		if (getFiles filename).count == 0 then
		(
			newMaterial.numsubs = 1
			newMaterial[1] = standard name:(getFilenameFile filename)
			return newMaterial
		)
		newMaterial.name = (getFilenameFile filename)
		
		local curIndex = 0
		
		mtlStream = openFile filename mode:"r"

		try
		(
			local materialName
			local curMaterialID = -1
			
			local diffuse, specular, ambient, specularPower, diffuseMap
			
			while not (eof mtlStream) do
			(
				local lineTokenizer = Tokenizer()
				local curLine = readLine mtlStream
				lineTokenizer.SetString curLine
				local idToken = lineTokenizer.ReadToken()
				
				case idToken of
				(
					"newmtl": -- Define a new material
					(
						local materialName = lineTokenizer.ReadToken()
						
						curIndex += 1
						newMaterial.numsubs += 1
						newMaterial[curIndex] = standard name:materialName
					)
					
					"Kd": -- Diffuse reflectivity
					(
						local red = ReadFloat lineTokenizer
						local green = ReadFloat lineTokenizer
						local blue = ReadFloat lineTokenizer
						diffuse = color (red*255) (green*255) (blue*255)
						newMaterial[curIndex].diffuse = diffuse
					)
					
					"Ks": -- Specular reflectivity
					(
						red = ReadFloat lineTokenizer
						green = ReadFloat lineTokenizer
						blue = ReadFloat lineTokenizer
						specular = color (red*255) (green*255) (blue*255)
						newMaterial[curIndex].specular = specular
					)
					
					"Ka": -- Ambient reflectivity
					(
						local red = ReadFloat lineTokenizer
						local green = ReadFloat lineTokenizer
						local blue = ReadFloat lineTokenizer
						ambient = color (red*255) (green*255) (blue*255)
						newMaterial[curIndex].ambient = ambient						
					)
					
					"Ns": -- Specular power
					(
						specularPower = ReadFloat lineTokenizer
						newMaterial[curIndex].specularLevel = specularPower
					)
					
					"map_Kd": -- Diffuse texture map
					(
						diffuseMapFile = curLine
						local pos = findString diffuseMapFile "map_Kd"
						diffuseMapFile = substring diffuseMapFile (pos + 6) (diffuseMapFile.count - pos - 5)
						diffuseMapFile = trimRight diffuseMapFile
						diffuseMapFile = trimLeft diffuseMapFile 
						
						if (getFiles (diffuseMapFile)).count != 0 then
						(
							-- Texture map file exists, so set it up and make it visible in viewports
							local diffuseMap = bitmapTexture filename: (filepath + diffuseMapFile) name:(getFilenameFile diffuseMapFile)
							newMaterial[curIndex].maps[2] = diffuseMap	
							newMaterial[curIndex].mapEnables[2] = true
							showTextureMap newMaterial[curIndex] diffuseMap true
						) else
						(
							-- Can't find texture map file :(
							--format "Warning: Cannot find texture map file %\n" diffuseMapFile
							newMaterial[curIndex].mapEnables[2] = true
						)
					)
				)	
			)
		)
		catch
		(
			messageBox "Error loading material file!"
			throw
		)
			
		close mtlStream
		
		newMaterial		-- No need for return!
	)

	--------------------------
	-- Main Import Function --
	--------------------------


    function importObject objFileName =
	(
		--- Imported Data ---
	
		local verts = #()
		local faces = #()
		local normals = #()
	
		local texCoords = #()
		local texCoords2 = #()

		local vertColors = #()
		local colorArray = #()
		local texFaces = #()
		
		local faceMaterialIDs = #()
		local faceGroupIDs = #()
	
		local materialNames = #()
		local groupNames = #()
	
		local groupName = undefined
		local objectName = undefined
		local materialLib = undefined

		-- If user made a selection, begin importing
		if objFileName != undefined then
		(
			currentPath = getFilenamePath objFileName

			local curMaterialID = 0
			local curGroup = 0
			local lineNum = 0
			local isValid = true
			
			-- Open up the file
			vtStream = openFile objFileName mode:"r"	
			--Test if HPP or PSO2 style vt
			try
			(
				gFound = false
				vtBeforeG = false
				vuUsed = false
				-- Go through the whole OBJ file
				while not (eof vtStream) do
				(
					local lineTokenizer = Tokenizer()
					
					lineTokenizer.SetString (readLine vtStream)
					
					lineNum += 1
					
					-- Read the data ID tag
					local idToken = lineTokenizer.ReadToken()
					
					if idToken != undefined do
					(
	
						case idToken of
						(					
							"g": 
							(
								gFound = true
							)
							
							"vt": -- Set vertex texcoord
							(
								if (gFound == false) then
								(
									vtBeforeG = true
								)
							)
							
							"vu": -- Set vertex texcoord
							(
								vuUsed = true
							)
							default:
							(
							)
						)
					)
				)				
			) catch
			(
				close vtStream
			)
			
			vtCount = 0
			vtPad = false
			vuPad = false
			-- Open up the file
			objStream = openFile objFileName mode:"r"	
			try
			(
				-- Go through the whole OBJ file
				while not (eof objStream) do
				(
					local lineTokenizer = Tokenizer()
					
					lineTokenizer.SetString (readLine objStream)
					
					lineNum += 1
					
					-- Read the data ID tag
					local idToken = lineTokenizer.ReadToken()
					
					if idToken != undefined do
					(
	
						case idToken of
						(
							"o": -- Object name
							(
								objectName = lineTokenizer.ReadToken()
							)
							
							"g": -- Define group
							(
								if (vtBeforeG == false) then
								(
									vtCount = 0
									vtPad = true
									vuPad = true
								)
								groupName = lineTokenizer.ReadToken()
								append groupNames groupName
								curGroup = groupNames.count
							)
							
							"mtllib": -- Set material library file
							(
								materialLib = lineTokenizer.ReadToken()
							)
							
							"usemtl": -- Use specified material
							(
								local matName = lineTokenizer.ReadToken()
								
								-- Have we seen this material name before?
								local index = findItem materialNames matName
								if index == 0 then
								(
									-- No, add it to our list
									append materialNames matName
									curMaterialID = materialNames.count
								) else
								(
									-- Yes, we already have an ID
									curMaterialID = index
								)
							)
							
							"v": -- Set vertex co-ordinate
							(
								if (vtBeforeG == false) then
								(
									vtCount += 1
								)
								
								local x = ReadFloat lineTokenizer
								if YZCheckBox.checked then
								(
								    local z = (ReadFloat lineTokenizer)
								    local y = -(ReadFloat lineTokenizer)
								) else 
								(
								    local y = ReadFloat lineTokenizer
									local z = ReadFloat lineTokenizer
								)
								append verts [ x, y, z ]
							)
							
							"vt": -- Set vertex texcoord
							(
								vtPad = false
								
								local u = ReadFloat lineTokenizer
								if UVFlip.checked then
								(
								local v = -(ReadFloat lineTokenizer)
								) else
								(
								local v = (ReadFloat lineTokenizer)
								)
								local w = ReadFloat lineTokenizer
								append texCoords [ u, v, w ]
							)
							
							"vu": -- Set vertex texcoord
							(
								vuPad = false
								
								local u = ReadFloat lineTokenizer
								if UVFlip.checked then
								(
								local v = -(ReadFloat lineTokenizer)
								) else
								(
								local v = (ReadFloat lineTokenizer)
								)
								local w = ReadFloat lineTokenizer
								append texCoords2 [ u, v, w ]
							)
							"vc": -- Set vertex color
							(
								local r = ReadFloat lineTokenizer
								local g = ReadFloat lineTokenizer
							    local b = ReadFloat lineTokenizer
								local a = ReadFloat lineTokenizer
								/*append vertColors [ r, g, b, a ] */
								vertColor = color r g b a
								append colorArray vertColor
							)
							"vn": -- Set vertex normal
							(
								local nx = ReadFloat lineTokenizer
								local ny = ReadFloat lineTokenizer
								local nz = ReadFloat lineTokenizer
								
								append normals [ nx, ny, nx ]
							)
							
							"f": -- Define face (coord index/texcoord index/normal index)
							(
								-- Append null tex coords if they didn't exist for a group set
								if (vtBeforeG == false) then
								(
									if (vtPad == true) then
									(
										for l = 1 to vtCount do
										(
											append texCoords [ 0, 0, 0 ]
										)
									)
									
									if (vuPad == true and vuUsed == true) then
									(
										for l = 1 to vtCount do
										(
											append texCoords2 [ 0, 0, 0 ]
										)
									)
																	
									vtPad = false
									vuPad = false
								)

								
								local v1 = ReadFaceVertex lineTokenizer
								local v2 = ReadFaceVertex lineTokenizer
								local v3 = ReadFaceVertex lineTokenizer
																						
								-- TODO: Add support for n-sided planar polys
								/*if YZCheckBox.checked then
								(
								    append faces [ v2[1], v1[1], v3[1] ]
								    append faceMaterialIDs curMaterialID
								    append texFaces [ v2[2], v1[2], v3[2] ]
								) else */
								(
								    append faces [ v1[1], v2[1], v3[1] ]
								    append faceMaterialIDs curMaterialID
									append faceGroupIDs curGroup
								    append texFaces [ v1[2], v2[2], v3[2] ]
								)
							)
							
							"#": -- Comment (ignore)
							(
							)
						)
					)
				)				
			) catch
			(
				messageBox "Error reading OBJ file!"
				isValid = false
				close objStream
				throw
			)
			
			-- Close the file stream
			close objStream
	
			if isValid then
			(
				-- Load and parse the material library (if present)
				if materialLib != undefined then
				(
					newMaterial = LoadMaterialLibrary filename:(currentPath + materialLib) filepath:(currentPath)
				) else
				(
					newMaterial = multimaterial prefix:(getFilenameFile objFileName)
					newMaterial.numsubs = 1
					newMaterial[1] = standard prefix:"Material"
				)
				
				-- Remap ad-hoc face material IDs onto gmax material IDs
				for i = 1 to faceMaterialIDs.count do
				(

					-- Make sure material IDs are valid - we can be sure there's always 1 submaterial
					if (faceMaterialIDs[i] > newMaterial.numsubs) or (faceMaterialIDs[i] < 1) then
					(
						faceMaterialIDs[i] = 1
					)
					else
					(
						local matName = materialNames[faceMaterialIDs[i]]

						-- Find corresponding gmax material
						for matID in newMaterial.materialIDList where (newMaterial[matID].name == matName) do
						(
							faceMaterialIDs[i] = matID
						)
					)
				)
				
				fileGroupName = (getFilenameFile objFileName)
				
				
				local importList = #()
				
				for k = 1 to groupNames.count do
				(				
					
					--Start a sub array of faces per group, store original face id as well for later use
					local faceArray = #()
				    local faceOrigID = #()
					
					for j = 1 to faces.count do
					(
						if(faceGroupIDs[j] == k) then 
						(
							append faceArray faces[j]
							append faceOrigID j
						)
					)
					-- Construct the mesh object
					local newMesh = mesh name:groupNames[k] pos:[0,0,0] \
										 vertices:verts faces:faceArray \
										 material:newMaterial
					append importList newMesh
					-- Find valid textured 
					/*
					local realVertList = meshop.getVertsUsingFace newMesh newMesh.Faces
					if (texCoords.count < realVertList[realVertList.count]) then
					(
						
					) */
					--local realVertStart = realvertList[0]
					--local realVertEnd = realVertList[realVertList.count]
					
					-- Add texCoords
					if texCoords.count > 0 then
					(

						
						-- Add UV channel 2 if it exists
						if texCoords2.count > 0 do
						(
							meshop.setNumMaps newMesh ((meshop.getNumMaps newMesh) + 1) keep:true
							meshop.setMapSupport newMesh 2 true
							meshop.setNumMapVerts newMesh 2 texCoords2.count
							meshop.setNumMapFaces newMesh 2 (meshop.getNumFaces newMesh) 
							
							for i = 1 to texCoords2.count do
							(
								meshop.setMapVert newMesh 2 i texCoords2[i]
							)
							try(
								for i = 1 to (meshop.getNumFaces newMesh) do
								(
									meshop.setMapFace newMesh 2 i faceArray[i]
								)
							)
							catch()
						)
						meshop.setMapSupport newMesh 1 true
						meshop.setMapSupport newMesh 0 true
						meshop.setMapSupport newMesh -2 true
						meshop.setNumMapVerts newMesh 1 texCoords.count
						meshop.setNumMapFaces newMesh 1 (meshop.getNumFaces newMesh) 
							
						for i = 1 to texCoords.count do
						(
							meshop.setMapVert newMesh 1 i texCoords[i]
						)
							
						for i = 1 to (meshop.getNumFaces newMesh) do
						(
	
							meshop.setMapFace newMesh 1 i faceArray[i]
								
						)
					)
					setNumCPVVerts newMesh newMesh.numverts
										 
					-- Set vertex coloring -- 
					for i = 1 to colorArray.count do
					(
						setvertcolor newMesh i colorArray[i]
						meshop.setvertalpha newMesh -2 i colorArray[i].alpha
					)  
					-- Set face material IDs
					for i = 1 to (meshop.getNumFaces newMesh) do
					(
						setFaceMatID newMesh i faceMaterialIDs[faceOrigID[i]]					
					)
					defaultVCFaces newMesh
					meshop.deleteIsoVerts newMesh
					
					-- Crude scaling, need to make this a bit more intelligent
					if autoSizeCheckBox.checked then
					(
						local scale = [autoSizeSpinner.value, autoSizeSpinner.value, autoSizeSpinner.value]
						
						for i = 1 to newMesh.numverts do
						(
							local v = getvert newMesh i
							v *= scale
							setvert newMesh i v
						)
					)
					
					-- Convert to Editable_Poly to avoid vert color destroying nonsense
					ConvertTo newMesh Editable_Poly
					
					
					--Update mesh
					newMesh.showVertexColors = true 
					update newMesh
					gc()
				)
				group importList name:fileGroupName
				max views redraw
			)
		)
	)
	
	on importButton pressed do
	(
		-- Open up a dialog box
		local objFileName = getOpenFileName caption:"Import VColor OBJ" \
					types:"Vertex Colored OBJ (*.obj)|*.obj|All Files (*.*)|*.*|"
        
		importObject objFileName
		
		-- Redraw max viewports
		--max views redraw
		-- Ta-daa! You should now see a lovely new imported model in all its glory.
		--print "Import completed."
	)

	on importPath pressed do
	(
		-- Open up a dialog box
		local objFileName = getOpenFileName caption:"Import VColor OBJ" \
					types:"Vertex Colored OBJ (*.obj)|*.obj|All Files (*.*)|*.*|"
		
		thePath = getFilenamePath objFileName
		/*thePath = getSavePath()--get a path dialog to specify the path*/
		
		if thePath != undefined do--if the user did not cancel
        (
          theFiles = getFiles (thePath+"\\*.obj") --collect all obj files in the dir.
		  for objFileName = theFiles do-- go through all of them
          (
		    importObject objFileName
		  )
	    )
		
	    -- Redraw max viewports
		--max views redraw
		-- Ta-daa! You should now see a lovely new imported model in all its glory.
		--print "Import completed."
	)
	
	on autoSizeCheckBox changed nowChecked do
	(
		autoSizeSpinner.enabled = nowChecked
	)
)
createDialog objImport 160 320
