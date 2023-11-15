-- unity
CS = CS
UnityEngine = CS.UnityEngine
GameObject = CS.UnityEngine.GameObject
Resources = CS.UnityEngine.Resources
Time = CS.UnityEngine.Time
Launcher = CS.Launcher.Instance

-- tools
LuaHelper = CS.LuaHelper

-- path
PathDefine = {
    -- 存档目录
    FolderSave = CS.UnityEngine.Application.persistentDataPath .. '/Save',
    -- 音频文件缓存目录
    PathAudioCache = CS.UnityEngine.Application.persistentDataPath .. '/audio_cache',
}

return PathDefine
