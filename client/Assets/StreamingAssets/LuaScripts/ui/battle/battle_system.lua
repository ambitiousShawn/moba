local ugui_loadpanel = require 'ui.battle.ugui_loadpanel'
local game_message   = require 'system.game_message'
local ugui_playpanel = require 'ui.battle.ugui_playpanel'
require 'ui.battle.ugui_hppanel'
local ugui_resultpanel = require 'ui.battle.ugui_resultpanel'

local EKeyType = CS.GameProtocol.EKeyType

local system = {}

local IsFightTick = false

local mapID 
local battleHeroDatas

local lastPercent = 0
function system:enter_battle()
    -- 拿到地图和英雄数据
    mapID = Launcher.MapID
    battleHeroDatas = Launcher.BattleHeroDatas

    -- 加载 '加载界面' 与 场景信息
    WindowManager:close_all()
    WindowManager:open('ugui_loadpanel')
    AssetsSvc:LoadSceneAsync('map_' .. mapID, function (val)
        local percent = math.floor(val * 100)
        if lastPercent ~= percent then
            local data = {
                roomID = Launcher.RoomID,
                percent = percent
            }
            local msg = game_message:new_msg('sndLoadPrg', data)
            NetworkManager:SendMsg(msg)
            lastPercent = percent
        end
    end, function ()
        -- TODO:场景加载完成
        print('场景加载完成！！！')
        
        -- 拿到地图配置
        local mapCfg = AssetsSvc:GetMapConfigByID(mapID)
        -- 初始化场景和英雄
        if FightManager == nil then
            FightManager = CS.FightManager.Instance    
        end
        
        -- hp一定要在play先打开，InitAll存在对hp的依赖
        WindowManager:open('ugui_hppanel', function (window_obj)
            FightManager.hpPanel = window_obj:GetComponent('HPPanel')
        end)

        WindowManager:open('ugui_playpanel', function (window_obj)
            FightManager.playPanel = window_obj:GetComponent('PlayPanel')
            FightManager:InitAll(battleHeroDatas, mapCfg)
        end)

        -- 加载完成后发送请求开始游戏
        local data = {
            roomID = Launcher.RoomID,
        }
        local msg = game_message:new_msg('reqBattleStart', data)
        NetworkManager:SendMsg(msg)
    end)
end

-- 加载资源的响应回调
function NtfLoadPrgCallBack (msg)
    local percentList = msg.ntfLoadPrg.percentList
    local sum = 0
    local cnt = percentList.Count
    for i = 0, cnt - 1 do
        sum = sum + percentList[i] * 100
    end
    ugui_loadpanel:refresh_progress(sum / cnt)
end

-- 战斗开始的响应回调
function RspBattleStartCallBack (msg)
    -- 相机跟随初始化
    FightManager:InitCamera(Launcher.SelfIndex)
    -- UI管理(创建游戏UI)
    ugui_loadpanel:close_self(true)
    FightManager.playPanel:InitSkillInfo() 
    IsFightTick = true -- 战斗开始
end

-- 获取到新的keyID
-- local keyID = 0
-- local function GetKeyID()
--     keyID = keyID + 1
--     return keyID
-- end
-- -- 由playwnd发送的玩家操作移动的数据
-- function SendMoveOperation (logicDir)
--     local data = {
--         roomID = Launcher.RoomID,
--         opKey = {
--             opIndex = Launcher.SelfIndex,
--             keyType = EKeyType.Move,
--             moveKey = {
--                 x = logicDir.x.ScaledValue,
--                 z = logicDir.z.ScaledValue,
--                 keyID = GetKeyID(),
--             },
--         },
--     }
--     local msg = game_message:new_msg('sndOpKey', data)
--     NetworkManager:SendMsg(msg)
-- end

-- 接收到服务器的操作信息
function NtfOpKeyCallBack (msg)
    if IsFightTick then
        FightManager:InputKey(msg.ntfOpKey.keyList)
        FightManager:Tick()
    end
end

-- 战斗结束(由C#的水晶塔被摧毁时调用)
function EndBattle (result)
    ugui_playpanel:close_self(true)
    -- TODO:可使用协程暂停
    -- 开启结果面板并设置结果
    WindowManager:open('ugui_resultpanel', function (window_obj)
        local module = window_obj:GetComponent('LuaBehavior').Module
        module:show_result(result)
    end)
    -- 关闭FightManager
    IsFightTick = false
end

-- 结果显示完毕的回调
function RspBattleEndCallBack()
    print('haha')
    -- 关闭所有UI
    WindowManager:close_all()
    FightManager:UnInit()
    -- 进入大厅阶段
    LobbySystem:enter_lobby()
end

return system