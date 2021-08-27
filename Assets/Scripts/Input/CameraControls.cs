// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Input/CameraControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Elektronik.Input
{
    public class @CameraControls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @CameraControls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""CameraControls"",
    ""maps"": [
        {
            ""name"": ""Controls"",
            ""id"": ""3296de75-f043-48f4-b597-b0057bb2af6b"",
            ""actions"": [
                {
                    ""name"": ""Move Forward"",
                    ""type"": ""Value"",
                    ""id"": ""e24ac579-c14e-4cc3-8d8c-2050856141c0"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move Sides"",
                    ""type"": ""Value"",
                    ""id"": ""6fde4b62-094a-4702-beb7-2a3c4c811732"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": ""ScaleVector2(x=5,y=5)"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Value"",
                    ""id"": ""c6e8d5a2-225f-44ab-bfc8-926cb03ce48d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Boost"",
                    ""type"": ""Value"",
                    ""id"": ""82748281-e420-40ac-b2a5-ada86d74f579"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Reset"",
                    ""type"": ""Button"",
                    ""id"": ""e56fad53-e5b3-4ab9-bf9d-41b347ab0cda"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""f801220f-f4a9-468f-a19a-329159d9de18"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=5)"",
                    ""groups"": """",
                    ""action"": ""Move Forward"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""c1dcd14f-1171-4a22-b6e0-f2ed1a4bf346"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""cb6d741f-e8b8-4696-8e83-b92f9a7073f5"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""d4f9aa70-0909-4bdd-a467-edf74852d87f"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=5)"",
                    ""groups"": """",
                    ""action"": ""Move Forward"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""e21b39f6-208b-4b06-a406-247ba1d88059"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""4ad80931-4371-4a50-977a-c960823ec543"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""8d35484f-01be-4c01-8c10-4e67651e1527"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": ""Scale"",
                    ""groups"": """",
                    ""action"": ""Move Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""518a5f76-4951-4875-9ce9-b082c52e44b5"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Sides"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""208219a5-5e3f-4f39-b312-68b84540a685"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Sides"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""cf8cbcac-bee6-4127-b905-525ea1d97636"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Sides"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""6ac52c8d-53e4-41a1-869b-5180f77ddf38"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Sides"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f77d55e2-f6e5-40ef-a1e9-10d349c64470"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Sides"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""b6370aae-aa65-4980-946e-89eaef76305f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=50,y=50)"",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""2fb5a899-cbd5-47c8-ac13-88ef09e9d72b"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""05e58344-c5eb-4090-8782-c9854558708c"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""116a429f-defc-4221-bda4-27f0e867e96e"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""40b5d6a8-6ec0-4781-b3f1-ab676df01263"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Mouse Drag"",
                    ""id"": ""1e48f88a-cc4c-4deb-b202-8e44ef84a4e7"",
                    ""path"": ""MouseDrag"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Button"",
                    ""id"": ""177df200-e88d-4360-bb85-912de5b11265"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Axis1"",
                    ""id"": ""263baa66-2769-4ee9-ab61-963948cc978f"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=5)"",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Axis2"",
                    ""id"": ""79dc4f4d-c856-4eaa-a155-ceaf410559fa"",
                    ""path"": ""<Mouse>/delta/y"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=5),Invert"",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a1759788-8537-468d-af90-d6bd991c4dda"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=5)"",
                    ""groups"": """",
                    ""action"": ""Boost"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""055c0d1d-9c00-43d1-8be3-cf3d0a22b8b3"",
                    ""path"": ""<Keyboard>/backspace"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Reset"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Default"",
            ""id"": ""43f7d51d-be1e-4a53-a233-f4123404cb47"",
            ""actions"": [
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""ada2bf4e-483f-4dea-bf0a-75d09fdfce1c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Click"",
                    ""type"": ""Button"",
                    ""id"": ""a792ad56-9862-4821-bc67-3d01cac4345e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""54ca85f0-f733-41aa-8596-4003d74acbd3"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""59635337-d9ef-4526-a93d-50051d6cf12a"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""242158ca-c282-4009-b032-603d9b1aa2d0"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Hotkeys"",
            ""id"": ""daeb5577-47e6-4cb4-a374-1a22ea6fc6cc"",
            ""actions"": [
                {
                    ""name"": ""Help"",
                    ""type"": ""Button"",
                    ""id"": ""d6b580bc-d9df-4302-8c70-b01817542579"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PlayPause"",
                    ""type"": ""Button"",
                    ""id"": ""d7e340fb-eaa5-45dd-a01e-95317ed3db29"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""OneFrameForward"",
                    ""type"": ""Button"",
                    ""id"": ""1cff640d-1972-4b99-8225-fe99f563ad23"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""OneFrameBackward"",
                    ""type"": ""Button"",
                    ""id"": ""4008ae00-a520-4d6c-87e0-1471d5f786c3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RewindForward"",
                    ""type"": ""Button"",
                    ""id"": ""1fb6aec3-d95b-4a71-81ee-8a2d622ae23f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RewindBackward"",
                    ""type"": ""Button"",
                    ""id"": ""30b96827-df4d-470c-905b-dd9d4dfe33fc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TakeSnapshot"",
                    ""type"": ""Button"",
                    ""id"": ""92b0beab-8c74-4f62-8518-652ba6b8257b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LoadSnapshot"",
                    ""type"": ""Button"",
                    ""id"": ""1d14a5e2-2228-4449-b109-32fda2ce97c8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""StartRecording"",
                    ""type"": ""Button"",
                    ""id"": ""0d12cb3f-f0fa-4b33-8c93-5b995b34a79d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ShowSourceTree"",
                    ""type"": ""Button"",
                    ""id"": ""d2804aeb-79b6-48c2-a581-08ca2ea4419d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ShowSceneTools"",
                    ""type"": ""Button"",
                    ""id"": ""7ed61ee3-6d7c-4509-a16c-c9c0ffd537f7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ShowAnalyticsTools"",
                    ""type"": ""Button"",
                    ""id"": ""b7a3e428-f12a-422d-82f9-eaf32e18a928"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ToggleGrid"",
                    ""type"": ""Button"",
                    ""id"": ""30316fd2-8659-4ac0-8884-b1e6995527c4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ToggleAxis"",
                    ""type"": ""Button"",
                    ""id"": ""d14902d4-a26f-4bd6-90ec-db27163027aa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ToggleMeshColor"",
                    ""type"": ""Button"",
                    ""id"": ""8ffc8d36-4edd-416e-915e-63ac04b0b7c4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""GoToVR"",
                    ""type"": ""Button"",
                    ""id"": ""8680593c-5e0b-46d5-9f99-5a6d9ce620f6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""VR Help"",
                    ""type"": ""Button"",
                    ""id"": ""64dd0d82-c9e9-4476-a999-083dae638754"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ClearMap"",
                    ""type"": ""Button"",
                    ""id"": ""2a1283f7-f76c-4c56-ad69-8a455f1b5969"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Stop"",
                    ""type"": ""Button"",
                    ""id"": ""bcd2f14d-8de7-4af6-82d3-c0ce081053fc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""86248f50-84ec-4c3d-8639-41aa40bc5947"",
                    ""path"": ""<Keyboard>/f1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Help"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d4ab0f84-d65a-4b6a-9184-5cb9fdab2cb8"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayPause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""66724de5-7330-4b57-a9ad-a2b99ef70c3f"",
                    ""path"": ""<Keyboard>/rightBracket"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RewindForward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""095b905e-2d7e-44e0-afff-0b4387ec1d05"",
                    ""path"": ""<Keyboard>/leftBracket"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RewindBackward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""25e23fdd-8ebe-4435-8789-8f9f36e2d4de"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TakeSnapshot"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""86a0aa6d-8343-4999-9487-256e7447dd84"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TakeSnapshot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""ed5fa626-9c6a-4fd7-bcb6-d5f8989f6e83"",
                    ""path"": ""<Keyboard>/t"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TakeSnapshot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""2ef04971-f8a0-4c0f-9edb-c36992d88466"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LoadSnapshot"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""6bcd0ffe-edce-4907-8f55-a2d6a36c674e"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LoadSnapshot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""f161bc8e-6fd9-4040-8ada-a5d7c230dc50"",
                    ""path"": ""<Keyboard>/o"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LoadSnapshot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""7a08503f-29fa-4a97-a1c4-6364bc14c490"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""StartRecording"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""6782d82b-c3af-476d-8427-8bbd5cf2b30f"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""StartRecording"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""5251f21f-2638-42c3-9acb-16d1f1fccf48"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""StartRecording"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""3932a35a-b791-43e8-a52c-9635de656a8c"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShowSourceTree"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""53da081d-086c-41e5-8b1d-a71e9436248f"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShowSourceTree"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""4373062e-4109-4c6b-bbf5-c739b5baeb18"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShowSourceTree"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""4721338a-1612-47a3-a9cb-fecc8cf15ce2"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShowSceneTools"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""6353a6b3-31fd-44c3-89dd-696b3fba898f"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShowSceneTools"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""d080cb3c-950b-4599-b2b2-20ff6c2324e2"",
                    ""path"": ""<Keyboard>/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShowSceneTools"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""43e76ef6-f1ae-457c-80c8-877bfe15878d"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleGrid"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""38fcdf55-2d52-4d7d-b9b3-7fbee6b4b0ff"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleGrid"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""d89b5bce-c080-49bb-8849-2ccf403d59b1"",
                    ""path"": ""<Keyboard>/g"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleGrid"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""490edd34-894d-4302-82d5-362a8c1de233"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleAxis"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""f0312ae3-5d52-4c4d-a705-4977d0fceef0"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""7bb81fb3-800a-4675-b2e5-e386432bb6b8"",
                    ""path"": ""<Keyboard>/h"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""b9e4f1a8-35a5-41eb-8e90-fd92fd14b1a4"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleMeshColor"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""8286d291-f6e5-447a-bc08-cee986a5eeb3"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleMeshColor"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""46a67976-83fa-4e74-96b1-06274492a7fc"",
                    ""path"": ""<Keyboard>/m"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleMeshColor"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f27bb30d-52f9-48e2-b4b9-081c1b7d57bf"",
                    ""path"": ""<Keyboard>/f2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GoToVR"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""eb53e89e-b282-484c-b231-ad8eebd31eae"",
                    ""path"": ""<Keyboard>/f3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VR Help"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""f0822777-0384-4614-9982-fe4753b4ea4f"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ClearMap"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""676b3a37-9ee6-40d5-a9cb-97a65110b1e6"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ClearMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""c852da72-0b6e-4f25-947e-8e520f9659dc"",
                    ""path"": ""<Keyboard>/delete"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ClearMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""721f1e38-6b86-4554-804d-ca1e630a3340"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Stop"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""23d5c86a-bae6-4fb4-a547-0976ca24764d"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Stop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""f088259d-3403-4cbf-a6ba-1a6ecec9d1d1"",
                    ""path"": ""<Keyboard>/delete"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Stop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""9ff4a6ad-bc97-47a5-93e8-1292a5c6f7c8"",
                    ""path"": ""<Keyboard>/period"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OneFrameForward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f202ef88-d6e0-4183-a3db-a0e876b1324d"",
                    ""path"": ""<Keyboard>/comma"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OneFrameBackward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""0360d809-c546-4aac-9434-b8231da7decb"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShowAnalyticsTools"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""a5025e9d-51a5-40e2-9a77-4f80eef88a8c"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShowAnalyticsTools"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""44bf99e9-a5c0-499e-b587-162472fd9663"",
                    ""path"": ""<Keyboard>/u"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShowAnalyticsTools"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Controls
            m_Controls = asset.FindActionMap("Controls", throwIfNotFound: true);
            m_Controls_MoveForward = m_Controls.FindAction("Move Forward", throwIfNotFound: true);
            m_Controls_MoveSides = m_Controls.FindAction("Move Sides", throwIfNotFound: true);
            m_Controls_Rotate = m_Controls.FindAction("Rotate", throwIfNotFound: true);
            m_Controls_Boost = m_Controls.FindAction("Boost", throwIfNotFound: true);
            m_Controls_Reset = m_Controls.FindAction("Reset", throwIfNotFound: true);
            // Default
            m_Default = asset.FindActionMap("Default", throwIfNotFound: true);
            m_Default_Cancel = m_Default.FindAction("Cancel", throwIfNotFound: true);
            m_Default_Click = m_Default.FindAction("Click", throwIfNotFound: true);
            // Hotkeys
            m_Hotkeys = asset.FindActionMap("Hotkeys", throwIfNotFound: true);
            m_Hotkeys_Help = m_Hotkeys.FindAction("Help", throwIfNotFound: true);
            m_Hotkeys_PlayPause = m_Hotkeys.FindAction("PlayPause", throwIfNotFound: true);
            m_Hotkeys_OneFrameForward = m_Hotkeys.FindAction("OneFrameForward", throwIfNotFound: true);
            m_Hotkeys_OneFrameBackward = m_Hotkeys.FindAction("OneFrameBackward", throwIfNotFound: true);
            m_Hotkeys_RewindForward = m_Hotkeys.FindAction("RewindForward", throwIfNotFound: true);
            m_Hotkeys_RewindBackward = m_Hotkeys.FindAction("RewindBackward", throwIfNotFound: true);
            m_Hotkeys_TakeSnapshot = m_Hotkeys.FindAction("TakeSnapshot", throwIfNotFound: true);
            m_Hotkeys_LoadSnapshot = m_Hotkeys.FindAction("LoadSnapshot", throwIfNotFound: true);
            m_Hotkeys_StartRecording = m_Hotkeys.FindAction("StartRecording", throwIfNotFound: true);
            m_Hotkeys_ShowSourceTree = m_Hotkeys.FindAction("ShowSourceTree", throwIfNotFound: true);
            m_Hotkeys_ShowSceneTools = m_Hotkeys.FindAction("ShowSceneTools", throwIfNotFound: true);
            m_Hotkeys_ShowAnalyticsTools = m_Hotkeys.FindAction("ShowAnalyticsTools", throwIfNotFound: true);
            m_Hotkeys_ToggleGrid = m_Hotkeys.FindAction("ToggleGrid", throwIfNotFound: true);
            m_Hotkeys_ToggleAxis = m_Hotkeys.FindAction("ToggleAxis", throwIfNotFound: true);
            m_Hotkeys_ToggleMeshColor = m_Hotkeys.FindAction("ToggleMeshColor", throwIfNotFound: true);
            m_Hotkeys_GoToVR = m_Hotkeys.FindAction("GoToVR", throwIfNotFound: true);
            m_Hotkeys_VRHelp = m_Hotkeys.FindAction("VR Help", throwIfNotFound: true);
            m_Hotkeys_ClearMap = m_Hotkeys.FindAction("ClearMap", throwIfNotFound: true);
            m_Hotkeys_Stop = m_Hotkeys.FindAction("Stop", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Controls
        private readonly InputActionMap m_Controls;
        private IControlsActions m_ControlsActionsCallbackInterface;
        private readonly InputAction m_Controls_MoveForward;
        private readonly InputAction m_Controls_MoveSides;
        private readonly InputAction m_Controls_Rotate;
        private readonly InputAction m_Controls_Boost;
        private readonly InputAction m_Controls_Reset;
        public struct ControlsActions
        {
            private @CameraControls m_Wrapper;
            public ControlsActions(@CameraControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @MoveForward => m_Wrapper.m_Controls_MoveForward;
            public InputAction @MoveSides => m_Wrapper.m_Controls_MoveSides;
            public InputAction @Rotate => m_Wrapper.m_Controls_Rotate;
            public InputAction @Boost => m_Wrapper.m_Controls_Boost;
            public InputAction @Reset => m_Wrapper.m_Controls_Reset;
            public InputActionMap Get() { return m_Wrapper.m_Controls; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(ControlsActions set) { return set.Get(); }
            public void SetCallbacks(IControlsActions instance)
            {
                if (m_Wrapper.m_ControlsActionsCallbackInterface != null)
                {
                    @MoveForward.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMoveForward;
                    @MoveForward.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMoveForward;
                    @MoveForward.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMoveForward;
                    @MoveSides.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMoveSides;
                    @MoveSides.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMoveSides;
                    @MoveSides.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMoveSides;
                    @Rotate.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnRotate;
                    @Rotate.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnRotate;
                    @Rotate.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnRotate;
                    @Boost.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnBoost;
                    @Boost.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnBoost;
                    @Boost.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnBoost;
                    @Reset.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnReset;
                    @Reset.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnReset;
                    @Reset.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnReset;
                }
                m_Wrapper.m_ControlsActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @MoveForward.started += instance.OnMoveForward;
                    @MoveForward.performed += instance.OnMoveForward;
                    @MoveForward.canceled += instance.OnMoveForward;
                    @MoveSides.started += instance.OnMoveSides;
                    @MoveSides.performed += instance.OnMoveSides;
                    @MoveSides.canceled += instance.OnMoveSides;
                    @Rotate.started += instance.OnRotate;
                    @Rotate.performed += instance.OnRotate;
                    @Rotate.canceled += instance.OnRotate;
                    @Boost.started += instance.OnBoost;
                    @Boost.performed += instance.OnBoost;
                    @Boost.canceled += instance.OnBoost;
                    @Reset.started += instance.OnReset;
                    @Reset.performed += instance.OnReset;
                    @Reset.canceled += instance.OnReset;
                }
            }
        }
        public ControlsActions @Controls => new ControlsActions(this);

        // Default
        private readonly InputActionMap m_Default;
        private IDefaultActions m_DefaultActionsCallbackInterface;
        private readonly InputAction m_Default_Cancel;
        private readonly InputAction m_Default_Click;
        public struct DefaultActions
        {
            private @CameraControls m_Wrapper;
            public DefaultActions(@CameraControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Cancel => m_Wrapper.m_Default_Cancel;
            public InputAction @Click => m_Wrapper.m_Default_Click;
            public InputActionMap Get() { return m_Wrapper.m_Default; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(DefaultActions set) { return set.Get(); }
            public void SetCallbacks(IDefaultActions instance)
            {
                if (m_Wrapper.m_DefaultActionsCallbackInterface != null)
                {
                    @Cancel.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnCancel;
                    @Cancel.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnCancel;
                    @Cancel.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnCancel;
                    @Click.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnClick;
                    @Click.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnClick;
                    @Click.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnClick;
                }
                m_Wrapper.m_DefaultActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Cancel.started += instance.OnCancel;
                    @Cancel.performed += instance.OnCancel;
                    @Cancel.canceled += instance.OnCancel;
                    @Click.started += instance.OnClick;
                    @Click.performed += instance.OnClick;
                    @Click.canceled += instance.OnClick;
                }
            }
        }
        public DefaultActions @Default => new DefaultActions(this);

        // Hotkeys
        private readonly InputActionMap m_Hotkeys;
        private IHotkeysActions m_HotkeysActionsCallbackInterface;
        private readonly InputAction m_Hotkeys_Help;
        private readonly InputAction m_Hotkeys_PlayPause;
        private readonly InputAction m_Hotkeys_OneFrameForward;
        private readonly InputAction m_Hotkeys_OneFrameBackward;
        private readonly InputAction m_Hotkeys_RewindForward;
        private readonly InputAction m_Hotkeys_RewindBackward;
        private readonly InputAction m_Hotkeys_TakeSnapshot;
        private readonly InputAction m_Hotkeys_LoadSnapshot;
        private readonly InputAction m_Hotkeys_StartRecording;
        private readonly InputAction m_Hotkeys_ShowSourceTree;
        private readonly InputAction m_Hotkeys_ShowSceneTools;
        private readonly InputAction m_Hotkeys_ShowAnalyticsTools;
        private readonly InputAction m_Hotkeys_ToggleGrid;
        private readonly InputAction m_Hotkeys_ToggleAxis;
        private readonly InputAction m_Hotkeys_ToggleMeshColor;
        private readonly InputAction m_Hotkeys_GoToVR;
        private readonly InputAction m_Hotkeys_VRHelp;
        private readonly InputAction m_Hotkeys_ClearMap;
        private readonly InputAction m_Hotkeys_Stop;
        public struct HotkeysActions
        {
            private @CameraControls m_Wrapper;
            public HotkeysActions(@CameraControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Help => m_Wrapper.m_Hotkeys_Help;
            public InputAction @PlayPause => m_Wrapper.m_Hotkeys_PlayPause;
            public InputAction @OneFrameForward => m_Wrapper.m_Hotkeys_OneFrameForward;
            public InputAction @OneFrameBackward => m_Wrapper.m_Hotkeys_OneFrameBackward;
            public InputAction @RewindForward => m_Wrapper.m_Hotkeys_RewindForward;
            public InputAction @RewindBackward => m_Wrapper.m_Hotkeys_RewindBackward;
            public InputAction @TakeSnapshot => m_Wrapper.m_Hotkeys_TakeSnapshot;
            public InputAction @LoadSnapshot => m_Wrapper.m_Hotkeys_LoadSnapshot;
            public InputAction @StartRecording => m_Wrapper.m_Hotkeys_StartRecording;
            public InputAction @ShowSourceTree => m_Wrapper.m_Hotkeys_ShowSourceTree;
            public InputAction @ShowSceneTools => m_Wrapper.m_Hotkeys_ShowSceneTools;
            public InputAction @ShowAnalyticsTools => m_Wrapper.m_Hotkeys_ShowAnalyticsTools;
            public InputAction @ToggleGrid => m_Wrapper.m_Hotkeys_ToggleGrid;
            public InputAction @ToggleAxis => m_Wrapper.m_Hotkeys_ToggleAxis;
            public InputAction @ToggleMeshColor => m_Wrapper.m_Hotkeys_ToggleMeshColor;
            public InputAction @GoToVR => m_Wrapper.m_Hotkeys_GoToVR;
            public InputAction @VRHelp => m_Wrapper.m_Hotkeys_VRHelp;
            public InputAction @ClearMap => m_Wrapper.m_Hotkeys_ClearMap;
            public InputAction @Stop => m_Wrapper.m_Hotkeys_Stop;
            public InputActionMap Get() { return m_Wrapper.m_Hotkeys; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(HotkeysActions set) { return set.Get(); }
            public void SetCallbacks(IHotkeysActions instance)
            {
                if (m_Wrapper.m_HotkeysActionsCallbackInterface != null)
                {
                    @Help.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnHelp;
                    @Help.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnHelp;
                    @Help.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnHelp;
                    @PlayPause.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnPlayPause;
                    @PlayPause.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnPlayPause;
                    @PlayPause.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnPlayPause;
                    @OneFrameForward.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnOneFrameForward;
                    @OneFrameForward.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnOneFrameForward;
                    @OneFrameForward.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnOneFrameForward;
                    @OneFrameBackward.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnOneFrameBackward;
                    @OneFrameBackward.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnOneFrameBackward;
                    @OneFrameBackward.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnOneFrameBackward;
                    @RewindForward.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnRewindForward;
                    @RewindForward.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnRewindForward;
                    @RewindForward.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnRewindForward;
                    @RewindBackward.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnRewindBackward;
                    @RewindBackward.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnRewindBackward;
                    @RewindBackward.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnRewindBackward;
                    @TakeSnapshot.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnTakeSnapshot;
                    @TakeSnapshot.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnTakeSnapshot;
                    @TakeSnapshot.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnTakeSnapshot;
                    @LoadSnapshot.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnLoadSnapshot;
                    @LoadSnapshot.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnLoadSnapshot;
                    @LoadSnapshot.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnLoadSnapshot;
                    @StartRecording.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnStartRecording;
                    @StartRecording.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnStartRecording;
                    @StartRecording.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnStartRecording;
                    @ShowSourceTree.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnShowSourceTree;
                    @ShowSourceTree.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnShowSourceTree;
                    @ShowSourceTree.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnShowSourceTree;
                    @ShowSceneTools.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnShowSceneTools;
                    @ShowSceneTools.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnShowSceneTools;
                    @ShowSceneTools.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnShowSceneTools;
                    @ShowAnalyticsTools.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnShowAnalyticsTools;
                    @ShowAnalyticsTools.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnShowAnalyticsTools;
                    @ShowAnalyticsTools.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnShowAnalyticsTools;
                    @ToggleGrid.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnToggleGrid;
                    @ToggleGrid.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnToggleGrid;
                    @ToggleGrid.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnToggleGrid;
                    @ToggleAxis.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnToggleAxis;
                    @ToggleAxis.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnToggleAxis;
                    @ToggleAxis.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnToggleAxis;
                    @ToggleMeshColor.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnToggleMeshColor;
                    @ToggleMeshColor.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnToggleMeshColor;
                    @ToggleMeshColor.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnToggleMeshColor;
                    @GoToVR.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnGoToVR;
                    @GoToVR.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnGoToVR;
                    @GoToVR.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnGoToVR;
                    @VRHelp.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnVRHelp;
                    @VRHelp.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnVRHelp;
                    @VRHelp.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnVRHelp;
                    @ClearMap.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnClearMap;
                    @ClearMap.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnClearMap;
                    @ClearMap.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnClearMap;
                    @Stop.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnStop;
                    @Stop.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnStop;
                    @Stop.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnStop;
                }
                m_Wrapper.m_HotkeysActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Help.started += instance.OnHelp;
                    @Help.performed += instance.OnHelp;
                    @Help.canceled += instance.OnHelp;
                    @PlayPause.started += instance.OnPlayPause;
                    @PlayPause.performed += instance.OnPlayPause;
                    @PlayPause.canceled += instance.OnPlayPause;
                    @OneFrameForward.started += instance.OnOneFrameForward;
                    @OneFrameForward.performed += instance.OnOneFrameForward;
                    @OneFrameForward.canceled += instance.OnOneFrameForward;
                    @OneFrameBackward.started += instance.OnOneFrameBackward;
                    @OneFrameBackward.performed += instance.OnOneFrameBackward;
                    @OneFrameBackward.canceled += instance.OnOneFrameBackward;
                    @RewindForward.started += instance.OnRewindForward;
                    @RewindForward.performed += instance.OnRewindForward;
                    @RewindForward.canceled += instance.OnRewindForward;
                    @RewindBackward.started += instance.OnRewindBackward;
                    @RewindBackward.performed += instance.OnRewindBackward;
                    @RewindBackward.canceled += instance.OnRewindBackward;
                    @TakeSnapshot.started += instance.OnTakeSnapshot;
                    @TakeSnapshot.performed += instance.OnTakeSnapshot;
                    @TakeSnapshot.canceled += instance.OnTakeSnapshot;
                    @LoadSnapshot.started += instance.OnLoadSnapshot;
                    @LoadSnapshot.performed += instance.OnLoadSnapshot;
                    @LoadSnapshot.canceled += instance.OnLoadSnapshot;
                    @StartRecording.started += instance.OnStartRecording;
                    @StartRecording.performed += instance.OnStartRecording;
                    @StartRecording.canceled += instance.OnStartRecording;
                    @ShowSourceTree.started += instance.OnShowSourceTree;
                    @ShowSourceTree.performed += instance.OnShowSourceTree;
                    @ShowSourceTree.canceled += instance.OnShowSourceTree;
                    @ShowSceneTools.started += instance.OnShowSceneTools;
                    @ShowSceneTools.performed += instance.OnShowSceneTools;
                    @ShowSceneTools.canceled += instance.OnShowSceneTools;
                    @ShowAnalyticsTools.started += instance.OnShowAnalyticsTools;
                    @ShowAnalyticsTools.performed += instance.OnShowAnalyticsTools;
                    @ShowAnalyticsTools.canceled += instance.OnShowAnalyticsTools;
                    @ToggleGrid.started += instance.OnToggleGrid;
                    @ToggleGrid.performed += instance.OnToggleGrid;
                    @ToggleGrid.canceled += instance.OnToggleGrid;
                    @ToggleAxis.started += instance.OnToggleAxis;
                    @ToggleAxis.performed += instance.OnToggleAxis;
                    @ToggleAxis.canceled += instance.OnToggleAxis;
                    @ToggleMeshColor.started += instance.OnToggleMeshColor;
                    @ToggleMeshColor.performed += instance.OnToggleMeshColor;
                    @ToggleMeshColor.canceled += instance.OnToggleMeshColor;
                    @GoToVR.started += instance.OnGoToVR;
                    @GoToVR.performed += instance.OnGoToVR;
                    @GoToVR.canceled += instance.OnGoToVR;
                    @VRHelp.started += instance.OnVRHelp;
                    @VRHelp.performed += instance.OnVRHelp;
                    @VRHelp.canceled += instance.OnVRHelp;
                    @ClearMap.started += instance.OnClearMap;
                    @ClearMap.performed += instance.OnClearMap;
                    @ClearMap.canceled += instance.OnClearMap;
                    @Stop.started += instance.OnStop;
                    @Stop.performed += instance.OnStop;
                    @Stop.canceled += instance.OnStop;
                }
            }
        }
        public HotkeysActions @Hotkeys => new HotkeysActions(this);
        public interface IControlsActions
        {
            void OnMoveForward(InputAction.CallbackContext context);
            void OnMoveSides(InputAction.CallbackContext context);
            void OnRotate(InputAction.CallbackContext context);
            void OnBoost(InputAction.CallbackContext context);
            void OnReset(InputAction.CallbackContext context);
        }
        public interface IDefaultActions
        {
            void OnCancel(InputAction.CallbackContext context);
            void OnClick(InputAction.CallbackContext context);
        }
        public interface IHotkeysActions
        {
            void OnHelp(InputAction.CallbackContext context);
            void OnPlayPause(InputAction.CallbackContext context);
            void OnOneFrameForward(InputAction.CallbackContext context);
            void OnOneFrameBackward(InputAction.CallbackContext context);
            void OnRewindForward(InputAction.CallbackContext context);
            void OnRewindBackward(InputAction.CallbackContext context);
            void OnTakeSnapshot(InputAction.CallbackContext context);
            void OnLoadSnapshot(InputAction.CallbackContext context);
            void OnStartRecording(InputAction.CallbackContext context);
            void OnShowSourceTree(InputAction.CallbackContext context);
            void OnShowSceneTools(InputAction.CallbackContext context);
            void OnShowAnalyticsTools(InputAction.CallbackContext context);
            void OnToggleGrid(InputAction.CallbackContext context);
            void OnToggleAxis(InputAction.CallbackContext context);
            void OnToggleMeshColor(InputAction.CallbackContext context);
            void OnGoToVR(InputAction.CallbackContext context);
            void OnVRHelp(InputAction.CallbackContext context);
            void OnClearMap(InputAction.CallbackContext context);
            void OnStop(InputAction.CallbackContext context);
        }
    }
}