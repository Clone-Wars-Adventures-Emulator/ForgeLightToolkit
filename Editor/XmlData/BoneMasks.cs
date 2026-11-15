using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace ForgeLightToolkit.Editor.XmlData {
    // IMPORTANT!!! Known Unity Limitation: For AvatarMasks components, even though they serialize a weight as a float, they read it into their systems as a bool.
    // According to Gemini (ew), this means even if you manage to serialize the 0.2f from the file data to Unity, it in theory wont take that
    // SEE: https://github.com/Unity-Technologies/UnityCsReference/blob/2022.3/Modules/Animation/ScriptBindings/AvatarMask.bindings.cs#L73
    // Alternative to AvatarMasks: https://kybernetik.com.au/animancer/docs/manual/blending/layers/weighted/ (requires pro? ($90))
    public class BoneMasks {
        #region FileContents
        private const string FileContents = @"<!-- Bone Mask Types -->
<!-- Each skeleton that requires bone masks must have its own set defined below. -->
<BoneMaskTypes>
    <BoneMaskSuffixes>
        <Suffix name=""UpperBody"" />
        <Suffix name=""UpperBodyCombat"" />
    </BoneMaskSuffixes>
    <BoneMask name=""Char_HumanMaleBody_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.2"" />
        <Bone name=""spineUpper"" weight=""0.4"" />
        <Bone name=""neck"" weight=""0.2"" />
        <Bone name=""head"" weight=""0.4"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_wield"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_wield"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask> 
    <BoneMask name=""Char_HumanMaleBody_UpperBodyCombat"" >
        <Bone name=""spineLower"" weight=""1.0"" />
        <Bone name=""spineUpper"" weight=""1.0"" />
        <Bone name=""COG"" weight=""0.0"" />
        <Bone name=""neck"" weight=""0.2"" />
        <Bone name=""head"" weight=""0.4"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_wield"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_wield"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>     
    <BoneMask name=""Char_HumanFemaleBody_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.2"" />
        <Bone name=""spineUpper"" weight=""0.4"" />
        <Bone name=""neck"" weight=""0.2"" />
        <Bone name=""head"" weight=""0.4"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_wield"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
        <Bone name=""R_wield"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_HumanFemaleBody_UpperBodyCombat"" >
        <Bone name=""spineLower"" weight=""1.0"" />
        <Bone name=""spineUpper"" weight=""1.0"" />
        <Bone name=""neck"" weight=""0.2"" />
        <Bone name=""head"" weight=""0.4"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_wield"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
        <Bone name=""R_wield"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_SuperBattleDroid_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.5"" />
        <Bone name=""spineUpper"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_BattleDroid_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.5"" />
        <Bone name=""spineUpper"" weight=""1.0"" />
        <Bone name=""neck"" weight=""1.0"" />
        <Bone name=""head"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_Droideka_UpperBody"" >
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
    </BoneMask>    
    <BoneMask name=""Char_SuperBattleDroid_UpperBodyCombat"" >
        <Bone name=""spineLower"" weight=""0.5"" />
        <Bone name=""spineUpper"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_BattleDroid_UpperBodyCombat"" >
        <Bone name=""spineLower"" weight=""0.5"" />
        <Bone name=""spineUpper"" weight=""1.0"" />
        <Bone name=""neck"" weight=""1.0"" />
        <Bone name=""head"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_Droideka_UpperBodyCombat"" >
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
    </BoneMask>  
    <BoneMask name=""Char_JarJar_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.5"" />
        <Bone name=""spineUpper"" weight=""1.0"" />
        <Bone name=""neckA"" weight=""1.0"" />
        <Bone name=""neckB"" weight=""1.0"" />
        <Bone name=""head"" weight=""1.0"" />
        <Bone name=""L_earA"" weight=""1.0"" />
        <Bone name=""L_earB"" weight=""1.0"" />
        <Bone name=""L_earC"" weight=""1.0"" />
        <Bone name=""L_earD"" weight=""1.0"" />
        <Bone name=""R_earA"" weight=""1.0"" />
        <Bone name=""R_earB"" weight=""1.0"" />
        <Bone name=""R_earC"" weight=""1.0"" />
        <Bone name=""R_earD"" weight=""1.0"" />
        <Bone name=""L_eyeStalk"" weight=""1.0"" />
        <Bone name=""L_eye"" weight=""1.0"" />
        <Bone name=""L_eyeLid"" weight=""1.0"" />
        <Bone name=""R_eyeStalk"" weight=""1.0"" />
        <Bone name=""R_eye"" weight=""1.0"" />
        <Bone name=""R_eyeLid"" weight=""1.0"" />
        <Bone name=""nose"" weight=""1.0"" />
        <Bone name=""lipUpper"" weight=""1.0"" />
        <Bone name=""L_lipCorner"" weight=""1.0"" />
        <Bone name=""R_lipCorner"" weight=""1.0"" />
        <Bone name=""jaw"" weight=""1.0"" />
        <Bone name=""lipLower"" weight=""1.0"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask> 
    <BoneMask name=""Char_Chewbacca_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.2"" />
        <Bone name=""spineUpper"" weight=""0.4"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_Yoda_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.2"" />
        <Bone name=""spineUpper"" weight=""0.4"" />
        <Bone name=""neck"" weight=""0.2"" />
        <Bone name=""head"" weight=""0.4"" />
        <Bone name=""L_brow"" weight=""1.0"" />
        <Bone name=""R_brow"" weight=""1.0"" />
        <Bone name=""L_eye"" weight=""1.0"" />
        <Bone name=""R_eye"" weight=""1.0"" />
        <Bone name=""L_eyeLid"" weight=""1.0"" />
        <Bone name=""R_eyeLid"" weight=""1.0"" />
        <Bone name=""nose"" weight=""1.0"" />
        <Bone name=""lipUpper"" weight=""1.0"" />
        <Bone name=""L_lipCorner"" weight=""1.0"" />
        <Bone name=""R_lipCorner"" weight=""1.0"" />
        <Bone name=""lipLower"" weight=""1.0"" />
        <Bone name=""jaw"" weight=""1.0"" />
        <Bone name=""L_earA"" weight=""1.0"" />
        <Bone name=""R_earA"" weight=""1.0"" />
        <Bone name=""L_earB"" weight=""1.0"" />
        <Bone name=""R_earB"" weight=""1.0"" />        
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_wield"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
        <Bone name=""R_wield"" weight=""1.0"" />
    </BoneMask> 
    <BoneMask name=""Char_Jawa_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.2"" />
        <Bone name=""spineUpper"" weight=""0.4"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
<BoneMask name=""Char_Anoobus_UpperBody"" >  
        <Bone name=""neck"" weight=""0.75"" />
        <Bone name=""head"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_Talz_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.2"" />
        <Bone name=""spineUpper"" weight=""0.4"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_HuttJabba_UpperBody"" >
        <Bone name=""neck"" weight=""0.2"" />
        <Bone name=""head"" weight=""0.4"" />
        <Bone name=""spineLower"" weight=""0.2"" />
        <Bone name=""spineUpper"" weight=""1.0"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
        <Bone name=""L_brow"" weight=""1.0"" />
        <Bone name=""R_brow"" weight=""1.0"" />
        <Bone name=""L_eye"" weight=""1.0"" />
        <Bone name=""R_eye"" weight=""1.0"" />
        <Bone name=""L_eyeLid"" weight=""1.0"" />
        <Bone name=""R_eyeLid"" weight=""1.0"" />
        <Bone name=""nose"" weight=""1.0"" />
        <Bone name=""lipUpper"" weight=""1.0"" />
        <Bone name=""L_lipCorner"" weight=""1.0"" />
        <Bone name=""R_lipCorner"" weight=""1.0"" />
        <Bone name=""lipLower"" weight=""1.0"" />
        <Bone name=""jaw"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_HuttZero_UpperBody"" >
        <Bone name=""neck"" weight=""0.2"" />
        <Bone name=""head"" weight=""0.4"" />
        <Bone name=""spineLower"" weight=""0.2"" />
        <Bone name=""spineUpper"" weight=""0.4"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
        <Bone name=""L_brow"" weight=""1.0"" />
        <Bone name=""R_brow"" weight=""1.0"" />
        <Bone name=""L_eye"" weight=""1.0"" />
        <Bone name=""R_eye"" weight=""1.0"" />
        <Bone name=""L_eyeLid"" weight=""1.0"" />
        <Bone name=""R_eyeLid"" weight=""1.0"" />
        <Bone name=""nose"" weight=""1.0"" />
        <Bone name=""lipUpper"" weight=""1.0"" />
        <Bone name=""L_lipCorner"" weight=""1.0"" />
        <Bone name=""R_lipCorner"" weight=""1.0"" />
        <Bone name=""lipLower"" weight=""1.0"" />
        <Bone name=""jaw"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_GamorreanGuard_UpperBody"" >
        <Bone name=""neck"" weight=""0.2"" />
        <Bone name=""head"" weight=""0.4"" />
        <Bone name=""spineLower"" weight=""0.6"" />
        <Bone name=""spineUpper"" weight=""0.8"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
        <Bone name=""L_brow"" weight=""1.0"" />
        <Bone name=""R_brow"" weight=""1.0"" />
        <Bone name=""L_eye"" weight=""1.0"" />
        <Bone name=""R_eye"" weight=""1.0"" />
        <Bone name=""L_eyeLid"" weight=""1.0"" />
        <Bone name=""R_eyeLid"" weight=""1.0"" />
        <Bone name=""nose"" weight=""1.0"" />
        <Bone name=""lipUpper"" weight=""1.0"" />
        <Bone name=""L_lipCorner"" weight=""1.0"" />
        <Bone name=""R_lipCorner"" weight=""1.0"" />
        <Bone name=""lipLower"" weight=""1.0"" />
        <Bone name=""jaw"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_Grievous_UpperBody"" >
        <Bone name=""spine"" weight=""0.4"" />
        <Bone name=""L_backShoulder"" weight=""1.0"" />
        <Bone name=""L_backForearm"" weight=""1.0"" />
        <Bone name=""L_backWrist"" weight=""1.0"" />
        <Bone name=""L_backClaw"" weight=""1.0"" />
        <Bone name=""R_backShoulder"" weight=""1.0"" />
        <Bone name=""R_backForearm"" weight=""1.0"" />
        <Bone name=""R_backWrist"" weight=""1.0"" />
        <Bone name=""R_backClaw"" weight=""1.0"" />
        <Bone name=""L_frontShoulder"" weight=""1.0"" />
        <Bone name=""L_frontForearm"" weight=""1.0"" />
        <Bone name=""L_frontWrist"" weight=""1.0"" />
        <Bone name=""L_frontClaw"" weight=""1.0"" />
        <Bone name=""R_frontShoulder"" weight=""1.0"" />
        <Bone name=""R_frontForearm"" weight=""1.0"" />
        <Bone name=""R_frontWrist"" weight=""1.0"" />
        <Bone name=""R_frontClaw"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_SeripasSuit_UpperBody"" >
        <Bone name=""spineLower"" weight=""1.0"" />
        <Bone name=""spineUpper"" weight=""1.0"" />
        <Bone name=""head"" weight=""1.0"" />
        <Bone name=""neck"" weight=""1.0"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""L_armOpenA"" weight=""1.0"" />
        <Bone name=""L_armOpenB"" weight=""1.0"" />
        <Bone name=""L_armRaisewe"" weight=""1.0"" />
        <Bone name=""L_armExtend"" weight=""1.0"" />
        <Bone name=""L_armSpin"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
        <Bone name=""R_armOpenA"" weight=""1.0"" />
        <Bone name=""R_armOpenB"" weight=""1.0"" />
        <Bone name=""R_armRaisewe"" weight=""1.0"" />
        <Bone name=""R_armExtend"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_SavageOpress_UpperBody"" >
        <Bone name=""neck"" weight=""0.2"" />
        <Bone name=""head"" weight=""0.4"" />
        <Bone name=""spineLower"" weight=""0.6"" />
        <Bone name=""spineUpper"" weight=""0.8"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_wield"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
        <Bone name=""L_brow"" weight=""1.0"" />
        <Bone name=""R_brow"" weight=""1.0"" />
        <Bone name=""L_eye"" weight=""1.0"" />
        <Bone name=""R_eye"" weight=""1.0"" />
        <Bone name=""L_eyeLid"" weight=""1.0"" />
        <Bone name=""R_eyeLid"" weight=""1.0"" />
        <Bone name=""nose"" weight=""1.0"" />
        <Bone name=""lipUpper"" weight=""1.0"" />
        <Bone name=""L_lipCorner"" weight=""1.0"" />
        <Bone name=""R_lipCorner"" weight=""1.0"" />
        <Bone name=""lipLower"" weight=""1.0"" />
        <Bone name=""jaw"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_KowakianMonkeyLizard_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.2"" />
        <Bone name=""spineUpper"" weight=""0.4"" />
        <Bone name=""head"" weight=""1.0"" />
        <Bone name=""neck"" weight=""1.0"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
        <BoneMask name=""Char_NossorRi_UpperBody"" >
        <Bone name=""COG"" weight=""0.2"" />
        <Bone name=""pelvis"" weight=""0.2"" />
        <Bone name=""L_hip"" weight=""0.2"" />
        <Bone name=""R_hip"" weight=""0.2"" />
        <Bone name=""L_knee"" weight=""0.2"" />
        <Bone name=""R_knee"" weight=""0.2"" />
        <Bone name=""L_ankle"" weight=""0.2"" />
        <Bone name=""R_ankle"" weight=""0.2"" />
<!-- below entry is a duplicate, CWAEmu's FLTK fork commented it out -->
        <!-- <Bone name=""R_ankle"" weight=""0.2"" /> -->
        <Bone name=""spineLower"" weight=""0.4"" />
        <Bone name=""spineUpper"" weight=""0.7"" />
        <Bone name=""head"" weight=""1.0"" />
        <Bone name=""neck"" weight=""1.0"" />
        <Bone name=""lipUpper"" weight=""1.0"" />
        <Bone name=""lipLower"" weight=""1.0"" />
        <Bone name=""jaw"" weight=""1.0"" />
        <Bone name=""nose"" weight=""1.0"" />
        <Bone name=""L_brow"" weight=""1.0"" />
        <Bone name=""L_lipCorner"" weight=""1.0"" />
        <Bone name=""L_eyeLid"" weight=""1.0"" />
        <Bone name=""L_sideTenticle"" weight=""1.0"" />
        <Bone name=""L_frontTenticleA"" weight=""1.0"" />
        <Bone name=""L_backTenticleA"" weight=""1.0"" />
        <Bone name=""L_backTenticleB"" weight=""1.0"" />
        <Bone name=""L_frontTenticleB"" weight=""1.0"" />
        <Bone name=""L_frontTenticleC"" weight=""1.0"" />
        <Bone name=""L_backTenticleC"" weight=""1.0"" />
        <Bone name=""L_eye"" weight=""1.0"" />
        <Bone name=""R_brow"" weight=""1.0"" />
        <Bone name=""R_eyeLid"" weight=""1.0"" />
        <Bone name=""R_sideTenticle"" weight=""1.0"" />
        <Bone name=""R_frontTenticleA"" weight=""1.0"" />
        <Bone name=""R_backTenticleA"" weight=""1.0"" />
        <Bone name=""R_lipCorner"" weight=""1.0"" />
        <Bone name=""R_backTenticleB"" weight=""1.0"" />
        <Bone name=""R_frontTenticleB"" weight=""1.0"" />
        <Bone name=""R_frontTenticleC"" weight=""1.0"" />
        <Bone name=""R_backTenticleC"" weight=""1.0"" />
        <Bone name=""R_eye"" weight=""1.0"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_Rancor_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.4"" />
        <Bone name=""spineUpper"" weight=""0.7"" />
        <Bone name=""head"" weight=""0.0"" />
        <Bone name=""neck"" weight=""0.0"" />
        <Bone name=""lipUpper"" weight=""1.0"" />
        <Bone name=""lipLower"" weight=""1.0"" />
        <Bone name=""jaw"" weight=""1.0"" />
        <Bone name=""nose"" weight=""1.0"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_CaptainAckbar_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.2"" />
        <Bone name=""spineUpper"" weight=""0.4"" />
        <Bone name=""head"" weight=""1.0"" />
        <Bone name=""neck"" weight=""1.0"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
        <BoneMask name=""Char_Karkarodon_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.2"" />
        <Bone name=""spineUpper"" weight=""0.4"" />
        <Bone name=""head"" weight=""1.0"" />
        <Bone name=""neck"" weight=""1.0"" />
        <Bone name=""lipUpper"" weight=""1.0"" />
        <Bone name=""lipLower"" weight=""1.0"" />
        <Bone name=""jaw"" weight=""1.0"" />
        <Bone name=""nose"" weight=""1.0"" />
        <Bone name=""teethLower"" weight=""1.0"" />
        <Bone name=""teethUpper"" weight=""1.0"" />
        <Bone name=""L_brow"" weight=""1.0"" />
        <Bone name=""L_lipCorner"" weight=""1.0"" />
        <Bone name=""L_eyeLid"" weight=""1.0"" />
        <Bone name=""L_eye"" weight=""1.0"" />
        <Bone name=""R_brow"" weight=""1.0"" />
        <Bone name=""R_eyeLid"" weight=""1.0"" />
        <Bone name=""R_lipCorner"" weight=""1.0"" />
        <Bone name=""R_eye"" weight=""1.0"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""0.5"" />
        <Bone name=""L_elbow"" weight=""0.1"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""0.5"" />
        <Bone name=""R_elbow"" weight=""0.1"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_Umbaran__UpperBody"" >
        <Bone name=""spineLower"" weight=""0.2"" />
        <Bone name=""spineUpper"" weight=""0.4"" />
        <Bone name=""head"" weight=""1.0"" />
        <Bone name=""neck"" weight=""1.0"" />
        <Bone name=""lipUpper"" weight=""1.0"" />
        <Bone name=""lipLower"" weight=""1.0"" />
        <Bone name=""jaw"" weight=""1.0"" />
        <Bone name=""nose"" weight=""1.0"" />
        <Bone name=""L_brow"" weight=""1.0"" />
        <Bone name=""L_lipCorner"" weight=""1.0"" />
        <Bone name=""L_eyeLid"" weight=""1.0"" />
        <Bone name=""L_eye"" weight=""1.0"" />
        <Bone name=""R_brow"" weight=""1.0"" />
        <Bone name=""R_eyeLid"" weight=""1.0"" />
        <Bone name=""R_lipCorner"" weight=""1.0"" />
        <Bone name=""R_eye"" weight=""1.0"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_AleenaChief_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.8"" />
        <Bone name=""spineUpper"" weight=""1.0"" />
        <Bone name=""head"" weight=""1.0"" />
        <Bone name=""neck"" weight=""1.0"" />
        <Bone name=""lipUpper"" weight=""1.0"" />
        <Bone name=""lipLower"" weight=""1.0"" />
        <Bone name=""jaw"" weight=""1.0"" />
        <Bone name=""nose"" weight=""1.0"" />
        <Bone name=""L_brow"" weight=""1.0"" />
        <Bone name=""L_lipCorner"" weight=""1.0"" />
        <Bone name=""L_eyeLid"" weight=""1.0"" />
        <Bone name=""L_eye"" weight=""1.0"" />
        <Bone name=""R_brow"" weight=""1.0"" />
        <Bone name=""R_eyeLid"" weight=""1.0"" />
        <Bone name=""R_lipCorner"" weight=""1.0"" />
        <Bone name=""R_eye"" weight=""1.0"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""0.5"" />
        <Bone name=""L_elbow"" weight=""0.1"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""0.5"" />
        <Bone name=""R_elbow"" weight=""0.1"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""staff"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_Orphne_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.2"" />
        <Bone name=""spineUpper"" weight=""0.4"" />
        <Bone name=""head"" weight=""1.0"" />
        <Bone name=""neck"" weight=""1.0"" />
        <Bone name=""toungeA"" weight=""1.0"" />
        <Bone name=""toungeB"" weight=""1.0"" />
        <Bone name=""toungeC"" weight=""1.0"" />
        <Bone name=""toungeD"" weight=""1.0"" />
        <Bone name=""lipUpper"" weight=""1.0"" />
        <Bone name=""lipLower"" weight=""1.0"" />
        <Bone name=""jaw"" weight=""1.0"" />
        <Bone name=""nose"" weight=""1.0"" />
        <Bone name=""L_brow"" weight=""1.0"" />
        <Bone name=""L_lipCorner"" weight=""1.0"" />
        <Bone name=""L_eyeLid"" weight=""1.0"" />
        <Bone name=""L_eye"" weight=""1.0"" />
        <Bone name=""R_brow"" weight=""1.0"" />
        <Bone name=""R_eyeLid"" weight=""1.0"" />
        <Bone name=""R_lipCorner"" weight=""1.0"" />
        <Bone name=""R_eye"" weight=""1.0"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""0.1"" />
        <Bone name=""L_elbow"" weight=""0.5"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""0.1"" />
        <Bone name=""R_elbow"" weight=""0.5"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_MagnaguardSeason4_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.4"" />
        <Bone name=""spineUpper"" weight=""0.6"" />
        <Bone name=""head"" weight=""0.2"" />
        <Bone name=""neck"" weight=""0.4"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_C21Highsinger_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.4"" />
        <Bone name=""spineUpper"" weight=""0.6"" />
        <Bone name=""head"" weight=""0.2"" />
        <Bone name=""neck"" weight=""0.4"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_DarthMaul_UpperBody"" >
        <Bone name=""spineLower"" weight="".1"" />
        <Bone name=""spineUpper"" weight="".1"" />
        <Bone name=""head"" weight=""0.2"" />
        <Bone name=""neck"" weight=""0.4"" />
        <Bone name=""jaw"" weight=""1.0"" />
        <Bone name=""nose"" weight=""1.0"" />
        <Bone name=""L_brow"" weight=""1.0"" />
        <Bone name=""L_lipCorner"" weight=""1.0"" />
        <Bone name=""L_eyeLid"" weight=""1.0"" />
        <Bone name=""L_eye"" weight=""1.0"" />
        <Bone name=""R_brow"" weight=""1.0"" />
        <Bone name=""R_eyeLid"" weight=""1.0"" />
        <Bone name=""R_lipCorner"" weight=""1.0"" />
        <Bone name=""R_eye"" weight=""1.0"" />
        <Bone name=""L_clavical"" weight=""0.8"" />
        <Bone name=""L_shoulder"" weight=""0.8"" />
        <Bone name=""L_elbow"" weight=""0.8"" />
        <Bone name=""L_forearm"" weight=""0.8"" />
        <Bone name=""L_wrist"" weight=""0.8"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavical"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_KageWarrior_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.4"" />
        <Bone name=""spineUpper"" weight=""0.6"" />
        <Bone name=""head"" weight=""0.2"" />
        <Bone name=""neck"" weight=""0.4"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_MaxRebo_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.4"" />
        <Bone name=""spineUpper"" weight=""0.6"" />
        <Bone name=""head"" weight=""0.2"" />
        <Bone name=""neck"" weight=""0.4"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_Umbaran_UpperBody"" >
        <Bone name=""spineLower"" weight=""1.0"" />
        <Bone name=""spineUpper"" weight=""1.0"" />
        <Bone name=""head"" weight=""1.0"" />
        <Bone name=""neck"" weight=""1.0"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_Narglatch_UpperBody"" >
        <Bone name=""spineUpper"" weight=""0.2"" />
        <Bone name=""head"" weight=""1.0"" />
        <Bone name=""neckA"" weight=""1.0"" />
        <Bone name=""neckB"" weight=""1.0"" />
        <Bone name=""L_ear"" weight=""1.0"" />
        <Bone name=""R_ear"" weight=""1.0"" />
        <Bone name=""jaw"" weight=""1.0"" />
        <Bone name=""nose"" weight=""1.0"" />
        <Bone name=""L_brow"" weight=""1.0"" />
        <Bone name=""L_lipCorner"" weight=""1.0"" />
        <Bone name=""L_eyeLid"" weight=""1.0"" />
        <Bone name=""L_eye"" weight=""1.0"" />
        <Bone name=""R_brow"" weight=""1.0"" />
        <Bone name=""R_eyeLid"" weight=""1.0"" />
        <Bone name=""R_lipCorner"" weight=""1.0"" />
        <Bone name=""R_eye"" weight=""1.0"" />
        <Bone name=""lowerSpines"" weight=""1.0"" />
        <Bone name=""midSpines"" weight=""1.0"" />
        <Bone name=""upperSpines"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_finger"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_MedicalDroid_UpperBody"" >
        <Bone name=""brain"" weight=""1.0"" />
        <Bone name=""chestPiece"" weight=""1.0"" />
        <Bone name=""innerChest"" weight=""1.0"" />
        <Bone name=""L_innerChest"" weight=""1.0"" /> 
        <Bone name=""R_innerChest"" weight=""1.0"" />
        <Bone name=""spineLower"" weight=""0.4"" />
        <Bone name=""spineUpper"" weight=""0.6"" />
        <Bone name=""head"" weight=""1.0"" />
        <Bone name=""neck"" weight=""1.0"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_OldDaka_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.4"" />
        <Bone name=""spineUpper"" weight=""0.6"" />
        <Bone name=""neck"" weight=""0.8"" />
        <Bone name=""head"" weight=""1.0"" />
        <Bone name=""headPiece"" weight=""1.0"" />
        <Bone name=""L_eye"" weight=""1.0"" />
        <Bone name=""L_eyeLid"" weight=""1.0"" />
        <Bone name=""R_eye"" weight=""1.0"" />
        <Bone name=""R_eyeLid"" weight=""1.0"" />
        <Bone name=""nose"" weight=""1.0"" />
        <Bone name=""lipUpper"" weight=""1.0"" />
        <Bone name=""L_lipCorner"" weight=""1.0"" />
        <Bone name=""R_lipCorner"" weight=""1.0"" />
        <Bone name=""jaw"" weight=""1.0"" />
        <Bone name=""lipLower"" weight=""1.0"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_wield"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
        <Bone name=""R_wield"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_DeathWatchCommanderNPC_UpperBody"" >
        <Bone name=""spineLower"" weight=""1.0"" />
        <Bone name=""spineUpper"" weight=""1.0"" />
        <Bone name=""neck"" weight=""0.2"" />
        <Bone name=""head"" weight=""0.4"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_wield"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
        <Bone name=""R_wield"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_DeathWatchEliteNPCFemale_UpperBody"" >
        <Bone name=""spineLower"" weight=""1.0"" />
        <Bone name=""spineUpper"" weight=""1.0"" />
        <Bone name=""neck"" weight=""0.2"" />
        <Bone name=""head"" weight=""0.4"" />
        <Bone name=""L_clavicle"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""L_thumbA"" weight=""1.0"" />
        <Bone name=""L_thumbB"" weight=""1.0"" />
        <Bone name=""L_indexA"" weight=""1.0"" />
        <Bone name=""L_indexB"" weight=""1.0"" />
        <Bone name=""L_flipperA"" weight=""1.0"" />
        <Bone name=""L_flipperB"" weight=""1.0"" />
        <Bone name=""L_wield"" weight=""1.0"" />
        <Bone name=""R_clavicle"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
        <Bone name=""R_thumbA"" weight=""1.0"" />
        <Bone name=""R_thumbB"" weight=""1.0"" />
        <Bone name=""R_indexA"" weight=""1.0"" />
        <Bone name=""R_indexB"" weight=""1.0"" />
        <Bone name=""R_flipperA"" weight=""1.0"" />
        <Bone name=""R_flipperB"" weight=""1.0"" />
        <Bone name=""R_wield"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_AquaDroid_UpperBody"" >
        <Bone name=""spineLower"" weight=""0.5"" />
        <Bone name=""spineUpper"" weight=""1.0"" />
        <Bone name=""L_shoulder"" weight=""1.0"" />
        <Bone name=""L_elbow"" weight=""1.0"" />
        <Bone name=""L_forearm"" weight=""1.0"" />
        <Bone name=""L_wrist"" weight=""1.0"" />
        <Bone name=""R_shoulder"" weight=""1.0"" />
        <Bone name=""R_elbow"" weight=""1.0"" />
        <Bone name=""R_forearm"" weight=""1.0"" />
        <Bone name=""R_wrist"" weight=""1.0"" />
    </BoneMask>
    <BoneMask name=""Char_Gundark_UpperBody"" >
        <Bone name=""spineLower"" weight="".6"" />
        <Bone name=""spineUpper"" weight="".8"" />
        <Bone name=""neck"" weight=""0.2"" />
        <Bone name=""head"" weight=""0.4"" />
        <Bone name=""L_eye"" weight=""1.0"" />
        <Bone name=""L_eyeLid"" weight=""1.0"" />
        <Bone name=""R_eye"" weight=""1.0"" /> 
        <Bone name=""L_brow"" weight=""1.0"" />
        <Bone name=""R_brow"" weight=""1.0"" />
        <Bone name=""R_eyeLid"" weight=""1.0"" />
        <Bone name=""nose"" weight=""1.0"" />
        <Bone name=""lipUpper"" weight=""1.0"" />
        <Bone name=""L_lipCorner"" weight=""1.0"" />
        <Bone name=""R_lipCorner"" weight=""1.0"" />
        <Bone name=""L_earA"" weight=""1.0"" />
        <Bone name=""L_earB"" weight=""1.0"" />
        <Bone name=""R_earA"" weight=""1.0"" />
        <Bone name=""R_earB"" weight=""1.0"" />
        <Bone name=""jaw"" weight=""1.0"" />
        <Bone name=""lipLower"" weight=""1.0"" />
        <Bone name=""tounge"" weight=""1.0"" />	
        <Bone name=""LB_clavicle"" weight=""1.0"" />
        <Bone name=""LB_shoulder"" weight=""1.0"" />
        <Bone name=""LB_elbow"" weight=""1.0"" />
        <Bone name=""LB_forearm"" weight=""1.0"" />
        <Bone name=""LB_wrist"" weight=""1.0"" />
        <Bone name=""LB_thumbA"" weight=""1.0"" />
        <Bone name=""LB_thumbB"" weight=""1.0"" />
        <Bone name=""LB_indexA"" weight=""1.0"" />
        <Bone name=""LB_indexB"" weight=""1.0"" />
        <Bone name=""LB_flipperA"" weight=""1.0"" />
        <Bone name=""LB_flipperB"" weight=""1.0"" />
<!-- CWAEmu's Fork changed the below from LB to LT, seems like that was a typo in the original data -->
        <Bone name=""LT_clavicle"" weight=""1.0"" />
        <Bone name=""LT_shouLder"" weight=""1.0"" />
        <Bone name=""LT_eLbow"" weight=""1.0"" />
        <Bone name=""LT_forearm"" weight=""1.0"" />
        <Bone name=""LT_wrist"" weight=""1.0"" />
        <Bone name=""LT_thumbA"" weight=""1.0"" />
        <Bone name=""LT_thumbB"" weight=""1.0"" />
        <Bone name=""LT_indexA"" weight=""1.0"" />
        <Bone name=""LT_indexB"" weight=""1.0"" />
        <Bone name=""LT_fLipperA"" weight=""1.0"" />
        <Bone name=""LT_fLipperB"" weight=""1.0"" />
        <Bone name=""RB_clavicle"" weight=""1.0"" />
        <Bone name=""RB_shoulder"" weight=""1.0"" />
        <Bone name=""RB_elbow"" weight=""1.0"" />
        <Bone name=""RB_forearm"" weight=""1.0"" />
        <Bone name=""RB_wrist"" weight=""1.0"" />
        <Bone name=""RB_thumbA"" weight=""1.0"" />
        <Bone name=""RB_thumbB"" weight=""1.0"" />
        <Bone name=""RB_indexA"" weight=""1.0"" />
        <Bone name=""RB_indexB"" weight=""1.0"" />
        <Bone name=""RB_flipperA"" weight=""1.0"" />
        <Bone name=""RB_flipperB"" weight=""1.0"" />
        <Bone name=""RT_clavicle"" weight=""1.0"" />
        <Bone name=""RT_shoulder"" weight=""1.0"" />
        <Bone name=""RT_elbow"" weight=""1.0"" />
        <Bone name=""RT_forearm"" weight=""1.0"" />
        <Bone name=""RT_wrist"" weight=""1.0"" />
        <Bone name=""RT_thumbA"" weight=""1.0"" />
        <Bone name=""RT_thumbB"" weight=""1.0"" />
        <Bone name=""RT_indexA"" weight=""1.0"" />
        <Bone name=""RT_indexB"" weight=""1.0"" />
        <Bone name=""RT_flipperA"" weight=""1.0"" />
        <Bone name=""RT_flipperB"" weight=""1.0"" />
    </BoneMask>
</BoneMaskTypes>
";
        #endregion FileContents

        public readonly IReadOnlyCollection<string> boneMaskSuffixes;
        public readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, float>> skeletonToBoneToWeightMap;
        private BoneMasks(IReadOnlyCollection<string> boneMaskSuffixes, IReadOnlyDictionary<string, IReadOnlyDictionary<string, float>> map) {
            this.boneMaskSuffixes = boneMaskSuffixes;
            skeletonToBoneToWeightMap = map;
        }

        public static BoneMasks Instance => instance.Value;
        private static Lazy<BoneMasks> instance = new(Create);
        private static BoneMasks Create() {
            using var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(FileContents));
            var serializer = new XmlSerializer(typeof(BoneMaskTypes));

            var xmlData = serializer.Deserialize(fileStream) as BoneMaskTypes;

            var suffixes = xmlData.suffixData.suffixes.Select(s => s.name).ToList().AsReadOnly();

            var topLevelMap = new Dictionary<string, IReadOnlyDictionary<string, float>>();
            foreach (var boneMask in xmlData.boneMasks) {
                var dict = new Dictionary<string, float>();
                foreach (var bone in boneMask.bones) {
                    dict.Add(bone.name, bone.weight);
                }
                topLevelMap.Add(boneMask.name, dict);
            }

            return new BoneMasks(suffixes, topLevelMap);
        }

        public (string suffix, AvatarMask mask)[] CreateAnyMasks(string skeleName, GameObject ROOT) {
            List<(string suffix, AvatarMask mask)> list = new();
            foreach (var suffix in Instance.boneMaskSuffixes) {
                if (!Instance.skeletonToBoneToWeightMap.TryGetValue($"{skeleName}_{suffix}", out var map)) {
                    continue;
                }

                void walkTransforms(List<Transform> all, Transform t) {

                }
                var transforms = new List<Transform>();
                walkTransforms(transforms, ROOT.transform);

                var mask = new AvatarMask {
                    name = suffix
                };

                foreach (var transform in transforms) {
                    // IMPORTANT!!! see class comment for known limitation of this
                    // Dont recurse
                    mask.AddTransformPath(transform, false);
                }

                list.Add((suffix, mask));
            }

            return list.ToArray();
        }
    }

    [XmlRoot("BoneMaskTypes")]
    [Serializable]
    public class BoneMaskTypes {
        [XmlElement("BoneMaskSuffixes", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public BoneMaskSuffixes suffixData;

        [XmlElement("BoneMask", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public BoneMask[] boneMasks;
    }

    [Serializable]
    public class BoneMaskSuffixes {
        [XmlElement("Suffix", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Suffix[] suffixes;

        [Serializable]
        public class Suffix {
            [XmlAttribute]
            public string name;
        }
    }

    [Serializable]
    public class BoneMask {
        [XmlAttribute]
        public string name;

        [XmlElement("Bone", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Bone[] bones;
    }

    [Serializable]
    public class Bone {
        [XmlAttribute]
        public string name;

        [XmlAttribute]
        public float weight;
    }
}
