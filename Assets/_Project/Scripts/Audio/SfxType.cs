using FMODUnity;

namespace CocoDoogy.Audio
{
    public enum SfxType
    {
        //라이더의 초록줄은 언더바 때문이니 무시하세요
        None = 0,
        //상호 작용 계열 - event:/Sfx/Interaction/
        Interaction_LeverOn,
        Interaction_LeverOff,
        Interaction_SwitchOn,
        Interaction_SwitchOff,
        Interaction_PressurePlate,
        Interaction_PushChest,
        
        //발자국 소리 - event:/Sfx/Footstep/
        Footstep_Water,
        Footstep_Grass,
        Footstep_Snow,
        Footstep_Sand,
        Footstep_Wood,
        Footstep_Dirt,
        
        //이벤트 개시 계열 - event:/Sfx/Gimmick/
        Gimmick_HouseEnter,
        Gimmick_OasisEnter,
        Gimmick_DockEnter,
        
        //날씨 이벤트 - event:/Sfx/Weather/
        Weather_Clear,
        Weather_Rain,
        Weather_Snow,
        Weather_Wind,
        Weather_Hail,
        Weather_Mirage,
        
        //미니게임 - event:/Sfx/Minigame/
        Minigame_PickTrash,
        Minigame_DropTrash,
        //Minigame_PickUmbrella,
        Minigame_PickCloth,
        Minigame_DropCloth,
        Minigame_DigSand,
        Minigame_ShakeUmbrella,
        
        //UI계열 - event:/Sfx/UI/
        UI_SuccessMission,
        UI_SuccessStage,
        UI_FailStage,
        UI_ButtonDown,
        UI_ButtonUp1,
        UI_ButtonUp2,
        UI_PopUp,
        
        //감정표현 계열 - event:/Sfx/Emote/
        Emote_Positive,
        Emote_Neutral,
        Emote_Negative,
        
        //Loop계열 - event:/Sfx/Loop/
        Loop_Detecting,
        Loop_WaterSplash,
        Loop_ShakeUmbrella
    }
    
    [System.Serializable]
    public struct SfxReference
    {
        public SfxType type;
        public EventReference eventReference;
    }
}
