-- unity
CS = CS
UnityEngine = CS.UnityEngine
GameObject = CS.UnityEngine.GameObject
Resources = CS.UnityEngine.Resources
Launcher = CS.Launcher.Instance

-- tools
LuaHelper = CS.LuaHelper

-- path
PathDefine = {
    -- 存档目录
    FolderSave = CS.UnityEngine.Application.persistentDataPath .. '/Save',
    -- GameSettings 路径
    PathGameSettings = CS.UnityEngine.Application.persistentDataPath .. '/game_settings',
    PathAudioCache = CS.UnityEngine.Application.persistentDataPath .. '/audio_cache',
}

return PathDefine
