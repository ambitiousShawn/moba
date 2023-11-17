local window_base = require "system.window_base"
local game_message = require 'system.game_message'
local network_manager = require 'system.network_manager'

-- 初始化ui逻辑
local window = window_base.new()
window:register("ugui_matchconfirmpanel", "UIPrefab/UGUI_MatchConfirmPanel.prefab", WindowManager.Kind.FullScreen)

local time_count = 15
function window:awake()
    time_count = ServerConfig.ConfirmCountDown -- 确认倒计时

    LuaHelper.BindClick(self.btn_confirm, function ()
        local data = {
            roomID = Launcher.RoomID
        }
        local msg = game_message:new_msg('sndConfirm', data)
        network_manager:SendMsg(msg)
        -- 发送确认信息后 遮挡按钮
        self.img_shadow:SetActive(true)
    end)
end

local time = time_count
function window:start()
    self.img_time_countdown_bar.fillAmount = 1   -- 阴影覆盖
    self.img_shadow:SetActive(false)             --隐藏遮罩
    time = time_count
end

function window:update()
    time = time - Time.deltaTime
    self:RefreshView()
end

function window:RefreshView()
    self.img_time_countdown_bar.fillAmount = time / time_count
end

return window