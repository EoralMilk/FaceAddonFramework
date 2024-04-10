# FaceAddonFramework
Face Addon Framework —— Add any FaceAddon you want for any pawn who has a head!
Adding face addon which can make facial expression to any creatures. It also can be used as a lightweight version of the facial expression framework, inspired by Garam.

## Example Project
- Kurin Meow Edition (WIP)
![Face expression and FaceAddon fox ears](./About/kurinFaceAddon.png "Face expression and FaceAddon fox ears")

![Face animation](./About/face_animation.gif "Face animation")

## HowToUse
- Add a comp `FaceAddon.CompProperties_FaceAddonComps` to your Pawn's ThingDef
- You can also use `FaceAddon.CompProperties_DrawRootOffset` comp to change the render offset of the pawn's rootloc
- All of the FaceAddonDefs in `additionalFaceAddonDefs` will always apply to any pawn, but the pawn will only random choose one faceTypeDef in `faceTypeDefs` to apply, also the `additionalfaceTypeDefs`. 
- eg: If you want your pawn choose one type of cat ears from many kinds. You can use the `additionalfaceTypeDefs`. Define your cat ear as FaceType.
```xml
		<comps>
			<li Class="FaceAddon.CompProperties_FaceAddonComps">
				<faceTypeDefs>
					<li>Kurin_FaceType1</li>
					<li>Kurin_FaceType2</li>
					<li>Kurin_FaceType3</li>
				</faceTypeDefs>
				<additionalfaceTypeDefs>
					...
				</additionalfaceTypeDefs>
				<additionalFaceAddonDefs>
					<li>Kurin_FoxEarL</li>
					<li>Kurin_FoxEarR</li>
				</additionalFaceAddonDefs>
			</li>
			<li Class="FaceAddon.CompProperties_DrawRootOffset">
				<eastOffset>(0,0,0)</eastOffset>
				<westOffset>(0,0,0)</westOffset>
				<southOffset>(0,0,0)</southOffset>
				<northOffset>(0,0,0)</northOffset>
				<offsetScale>1</offsetScale>
				<offsetOnlyStand>true</offsetOnlyStand>
			</li>
		</comps>
```

- The face type def can be looks like this
- If you not set 'requireHeadTypes'. this FaceTypeDef can be apply to any headtypes.
```xml
	<FaceAddon.FaceTypeDef>
		<defName>Kurin_FaceType1</defName>
		<randomWeight>1</randomWeight>

		<Upper>Kurin_Face_Upper_Public</Upper>
		<Lower>Kurin_Face_Lower_A</Lower>
		<Attach>your_attach_FaceAddonDef</Attach>

		<tickBlinkMin>120</tickBlinkMin>
		<tickBlinkMax>120</tickBlinkMax>
		<blinkDurationMin>120</blinkDurationMin>
		<blinkDurationMin>120</blinkDurationMin>
		<winkChance>0.3</winkChance>

		<requireHeadTypes>
			<li>...</li>
		</requireHeadTypes>
	</FaceAddon.FaceTypeDef>
```

- The face addon def can be looks like this
```xml
	<FaceAddon.FaceAddonDef Name="KurinEar">
		<defName>Kurin_FoxEarL</defName>
		<shaderType>CutoutComplex</shaderType>
		<layerOffset>20</layerOffset>
		<layerOffsetAlwaysPositive>true</layerOffsetAlwaysPositive>
		<colorBase>Hair</colorBase>
		<colorSub>Skin</colorSub>
		<fixedPath>KurinMeowEdition/Ear/Left/FoxEarLA</fixedPath>
	</FaceAddon.FaceAddonDef>

	<FaceAddon.FaceAddonDef ParentName="KurinEar">
		<defName>Kurin_FoxEarR</defName>
		<fixedPath>KurinMeowEdition/Ear/Right/FoxEarRA</fixedPath>
	</FaceAddon.FaceAddonDef>

	<FaceAddon.FaceAddonDef Name="KurinFaceDefbase" Abstract="True">
		<shaderType>Transparent</shaderType>
		<colorBase>White</colorBase>
		<colorSub>Skin</colorSub>
	</FaceAddon.FaceAddonDef>

	<FaceAddon.FaceAddonDef Name="Kurin_Face_Lower_A" ParentName="KurinFaceDefbase">
		<defName>Kurin_Face_Lower_A</defName>
		<!--Normally Path By Mood-->
		<mentalBreakPath>KurinMeowEdition/Face_New/LowerA/MentalBreak</mentalBreakPath>
		<aboutToBreakPath>KurinMeowEdition/Face_New/LowerA/AboutToBreak</aboutToBreakPath>
		<onEdgePath>KurinMeowEdition/Face_New/LowerA/OnEdge</onEdgePath>
		<stressedPath>KurinMeowEdition/Face_New/LowerA/Stress</stressedPath>
		<neutralPath>KurinMeowEdition/Face_New/LowerA/Neutral</neutralPath>
		<contentPath>KurinMeowEdition/Face_New/LowerA/Content</contentPath>
		<happyPath>KurinMeowEdition/Face_New/LowerA/Happy</happyPath>
		<!--Special Path-->
		<attackingPath>KurinMeowEdition/Face_New/LowerA/shout</attackingPath>
		<damagedPath>KurinMeowEdition/Face_New/LowerA/AboutToBreak</damagedPath>

		<!-- <draftedPath>KurinMeowEdition/Face_New/LowerA/Neutral</draftedPath> -->
		<sleepingPath>KurinMeowEdition/Face_New/LowerA/Sleeping1</sleepingPath>
		<painShockPath>KurinMeowEdition/Face_New/LowerA/PainShock</painShockPath>
		<deadPath>KurinMeowEdition/Face_New/LowerA/Dead1</deadPath>
		<blinkPath>KurinMeowEdition/Face_New/LowerA/Blink</blinkPath>
		<winkPath>KurinMeowEdition/Face_New/LowerA/wink_close</winkPath>
	</FaceAddon.FaceAddonDef>

	<FaceAddon.FaceAddonDef ParentName="Kurin_Face_Lower_A">
		<defName>Kurin_Face_Lower_B</defName>
		<sleepingPath>KurinMeowEdition/Face_New/LowerA/Sleeping2</sleepingPath>
		<deadPath>KurinMeowEdition/Face_New/LowerA/Dead2</deadPath>
		<blinkPath>KurinMeowEdition/Face_New/LowerA/wink_close2</blinkPath>
		<winkPath>KurinMeowEdition/Face_New/LowerA/wink_close2</winkPath>
	</FaceAddon.FaceAddonDef>

	<FaceAddon.FaceAddonDef ParentName="Kurin_Face_Lower_A">
		<defName>Kurin_Face_Lower_C</defName>
		<deadPath>KurinMeowEdition/Face_New/LowerA/Dead2</deadPath>
		<blinkPath>KurinMeowEdition/Face_New/LowerA/wink_close2</blinkPath>
		<sleepingPath>KurinMeowEdition/Face_New/LowerA/wink_close2</sleepingPath>
		<winkPath>KurinMeowEdition/Face_New/LowerA/wink_close</winkPath>
	</FaceAddon.FaceAddonDef>

	<FaceAddon.FaceAddonDef ParentName="KurinFaceDefbase">
		<defName>Kurin_Face_Upper_Public</defName>
		<layerOffset>12</layerOffset>
		<!--Normally Path By Mood-->
		<mentalBreakPath>KurinMeowEdition/Face_New/UpperA/Angry</mentalBreakPath>
		<aboutToBreakPath>KurinMeowEdition/Face_New/UpperA/Worried</aboutToBreakPath>
		<onEdgePath>KurinMeowEdition/Face_New/UpperA/Worried</onEdgePath>
		<stressedPath>KurinMeowEdition/Face_New/UpperA/Worried</stressedPath>
		<neutralPath>KurinMeowEdition/Face_New/UpperA/Normal</neutralPath>
		<contentPath>KurinMeowEdition/Face_New/UpperA/Happy</contentPath>
		<happyPath>KurinMeowEdition/Face_New/UpperA/Happy</happyPath>
		<!--Special Path-->
		<attackingPath>KurinMeowEdition/Face_New/UpperA/OnArm</attackingPath>
		<draftedPath>KurinMeowEdition/Face_New/UpperA/OnArm</draftedPath>

		<sleepingPath>KurinMeowEdition/Face_New/UpperA/Normal</sleepingPath>
		<painShockPath>KurinMeowEdition/Face_New/UpperA/Worried</painShockPath>
		<deadPath>KurinMeowEdition/Face_New/UpperA/Worried</deadPath>
		<blinkPath>KurinMeowEdition/Face_New/UpperA/Normal</blinkPath>
		<winkPath>KurinMeowEdition/Face_New/UpperA/Happy</winkPath>
		<damagedPath>KurinMeowEdition/Face_New/UpperA/Angry</damagedPath>

	</FaceAddon.FaceAddonDef>
	
```