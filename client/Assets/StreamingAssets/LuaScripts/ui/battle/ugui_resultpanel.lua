local window_base = require "system.window_base"
local game_message = require 'system.game_message'

-- 初始化ui逻辑
local window = window_base.new()
window:register("ugui_resultpanel", "battle", "UIPrefab/UGUI_ResultPanel", WindowManager.Kind.FullScreen)

function window:awake()
    LuaHelper.BindClick(self.btn_continue, function ()
        self:ClickSureBtnEvent()
    end)
end

local last_time = 5
local time_count = 5 -- 倒计时五秒
function window:update()
    last_time = last_time - Time.deltaTime
    if last_time <= 0 then
        self:ClickSureBtnEvent()
    end
    self.img_progress.fillAmount = last_time / time_count
end

function window:ClickSureBtnEvent()
    -- 按下确认键的回调  
    if self.go.activeSelf then
        local data = {
            roomID = Launcher.RoomID
        }
        local msg = game_message:new_msg('reqBattleEnd', data)
        NetworkManager:SendMsg(msg)
    end  
end

-- 展示战斗结果，通过battlesystem调用
function window:show_result(result)
    if result then
        -- 获胜
        self.img_result.sprite = AssetsSvc:LoadSprite('battle', 'win', 1)
    else
        -- 失败
        self.img_result.sprite = AssetsSvc:LoadSprite('battle', 'lose', 1)
    end
    self.img_result:SetNativeSize()
end

return window