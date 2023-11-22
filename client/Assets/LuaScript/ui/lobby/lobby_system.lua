local ugui_lobbypanel = require 'ui.lobby.ugui_lobbypanel'
local ugui_matchconfirmpanel = require 'ui.lobby.ugui_matchconfirmpanel'
local popup_tip = require 'ui.common.popup_tip'
local ugui_selectpanel = require 'ui.lobby.ugui_selectpanel'

local system = {}

-- 进入大厅系统
function system:enter_lobby()
    WindowManager:close_all()
    WindowManager:open('ugui_lobbypanel')
end

-- 玩家点击 "开始匹配后" ，传回根据排队人数计算的预测时间信息
function RspMatchCallBack(msg)
    local predict_time = msg.rspMatch.predictTime
    RefreshPredictTimeView(predict_time)
end

-- 各个玩家点击 "确认后"， 传回的通知信息
function NtfConfirmCallBack(msg)
    local ntf = msg.ntfConfirm
    if ntf.dissmiss then
        -- 房间解散
        ugui_matchconfirmpanel:close_self(true)
        WindowManager:open('ugui_lobbypanel')
        popup_tip:popup_tip_panel('队伍已解散')
    else
        Launcher.RoomID = ntf.roomID
        local table = WindowManager:find_window_in_stack('ugui_matchconfirmpanel')
        if table[2] == nil then
            -- 匹配界面还没开启
            ugui_lobbypanel:close_self(true)
            WindowManager:open('ugui_matchconfirmpanel')
        end
    end
end

-- 所有玩家确认后，传回的进入 "选择英雄阶段" 的消息
function NtfSelectCallBack(msg)
    ugui_matchconfirmpanel:close_self(true)
    WindowManager:open('ugui_selectpanel')
end

-- 选择英雄完成，进入资源加载阶段
function NtfLoadResCallBack (msg)
    local ntf = msg.ntfLoadRes
    Launcher.MapID = ntf.mapID
    Launcher.BattleHeroDatas = ntf.battleHeroDatas
    Launcher.SelfIndex = ntf.posIndex

    ugui_selectpanel:close_self()
    -- 进入战斗阶段
    BattleSystem:enter_battle()
end


return system;