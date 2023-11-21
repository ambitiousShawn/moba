local ugui_loadpanel = require 'ui.battle.ugui_loadpanel'
local ugui_playpanel = require 'ui.battle.ugui_playpanel' 
local game_message   = require 'system.game_message'

local system = {}

local mapID 
local battleHeroDatas

local lastPercent = 0
function system:enter_battle()
    -- 拿到地图和英雄数据
    mapID = Launcher.MapID
    battleHeroDatas = Launcher.BattleHeroDatas

    -- 加载 '加载界面' 与 场景信息
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
        
        ugui_loadpanel:close_self(true)
        -- 拿到地图配置
        local mapCfg = AssetsSvc:GetMapConfigByID(mapID)
        -- 初始化场景和英雄
        if FightManager == nil then
            FightManager = CS.FightManager.Instance    
        end
        
        FightManager:InitCollisionEnv()
        FightManager:InitHero(battleHeroDatas, mapCfg)

        -- 加载完成后发送请求开始游戏
        local data = {
            roomID = Launcher.RoomID,
        }
        local msg = game_message:new_msg('reqBattleStart', data)
        -- NetworkManager:SendMsg(msg)
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
function RspBattleStart (msg)
    -- 相机跟随初始化
    FightManager:InitCamera(Launcher.SelfIndex)
    -- UI管理(创建游戏UI)
    WindowManager:open('ugui_playpanel')
    ugui_loadpanel:close_self(true)
    FightManager.IsFightTick = true -- 战斗开始
end

return system