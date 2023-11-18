local ugui_loadpanel = require 'ui.battle.ugui_loadpanel'
local game_message   = require 'system.game_message'
local network_manager= require 'system.network_manager'

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
            network_manager:SendMsg(msg)
            lastPercent = percent
        end
    end, function ()
        -- TODO:场景加载完成
        print('场景加载完成！！！')
    end)
end

-- 加载资源的响应回调
function NtfLoadPrgCallBack (msg)
    ugui_loadpanel:refresh_progress(msg.ntfLoadPrg.percentList)
end

return system;