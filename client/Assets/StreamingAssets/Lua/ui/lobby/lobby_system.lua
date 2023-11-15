
local system = {}

function system:init()
    
end

-- 进入大厅系统
function system:enter_lobby()
    WindowManager:close_all()
    WindowManager:open('ugui_lobbypanel')
end

function RspMatchCallBack(msg)
    local predict_time = msg.rspMatch.predictTime
    RefreshPredictTimeView(predict_time)
end

function NtfConfirmCallBack(msg)
    local ntf = msg.ntfConfirm
    if ntf.dissmiss then
        -- 房间解散
        WindowManager:close('ugui_matchconfirmpanel', true)
        WindowManager:open('ugui_lobbypanel')
    else
        Launcher.RoomID = ntf.roomID
        local table = WindowManager:find_window_in_stack('ugui_matchconfirmpanel')
        if table[0] == nil then
            -- 匹配界面还没开启
            WindowManager:close('ugui_lobbypanel', true)
            WindowManager:open('ugui_matchconfirmpanel')
        end
    end
end


return system;