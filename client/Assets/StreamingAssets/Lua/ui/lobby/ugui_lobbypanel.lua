local window_base = require "system.window_base"
local game_message = require 'system.game_message'
local network_manager = require 'system.network_manager'

-- 初始化ui逻辑
local window = window_base.new()
window:register("ugui_lobbypanel", "UIPrefab/UGUI_LobbyPanel.prefab", WindowManager.Kind.FullScreen)

local is_matching = false
local timeCount = 0
local deltaTime = 0 

function window:awake()
    LuaHelper.BindClick(self.btn_match, function ()
        -- 按下匹配按钮逻辑
        local msg = game_message:new_msg('reqMatch', CS.GameProtocol.PVPType._1v1)
        network_manager:SendMsg(msg)
    end)
end  

-- TODO:不知道为什么不缓存会报空
local match_parent, text_start_match, text_cancel_match, text_predict_time, text_count_time
function window:start()
    match_parent = self.match_parent
    text_start_match = self.text_start_match
    text_cancel_match = self.text_cancel_match
    text_predict_time = self.text_predict_time
    text_count_time = self.text_count_time

    self.match_parent:SetActive(false)      -- 显示匹配界面
    self.text_start_match:SetActive(true) -- 显示 "取消匹配"
    self.text_cancel_match:SetActive(false)
end

function window:update()
    if is_matching then
        deltaTime = deltaTime + Time.deltaTime
        if deltaTime >= 1 then
            deltaTime = deltaTime - 1
            timeCount = timeCount + 1
            self:RefreshMatchCountView()
        end
    end
end

-- 转换格式为 00:00
local function time_convert_to_standard(cnt)
    local min = math.floor(timeCount / 60)
    local sec = math.floor(timeCount % 60)
    local minStr, secStr
    if min < 10 then
        minStr = '0' .. min .. ':'
    else
        minStr = tostring(min) .. ':'
    end

    if sec < 10 then
        secStr = '0' .. sec
    else
        secStr = tostring(sec)
    end
    return minStr .. secStr
end

-- 刷新UI显示(刷新时间间隔:收到匹配响应时)
function RefreshView(predict_time)
    is_matching = not is_matching

    match_parent:SetActive(is_matching)      -- 显示匹配界面
    text_start_match:SetActive(not is_matching) -- 显示 "取消匹配"
    text_cancel_match:SetActive(is_matching)
    
    if is_matching then
        -- 开始匹配
        timeCount = 0
        deltaTime = 0
        
        text_predict_time.text = '预计匹配时长: ' .. time_convert_to_standard(predict_time)
        text_count_time.text = '00:00'                     -- 计时开始
    end
end

-- 刷新倒计时计数UI组件显示(刷新时间间隔:1s)
function window:RefreshMatchCountView ()
    text_count_time.text = time_convert_to_standard(timeCount)
end

return window