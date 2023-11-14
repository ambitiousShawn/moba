local window_base = require "system.window_base"
local window_manager = require "system.window_manager"
local popup_tip = require 'ui.common.popup_tip'

-- 初始化ui逻辑
local window = window_base.new()
window:register("ugui_lobbypanel", "UIPrefab/UGUI_LobbyPanel.prefab", window_manager.Kind.FullScreen)

function window:awake()
    -- 匹配逻辑
    LuaHelper.BindClick(self.btn_match, function ()
        popup_tip:popup_tip_panel('已开始匹配')
        self.text_predict_time.text = '预计匹配时长: 10s'
        self.text_count_time.text = '0'
        self.match_parent:SetActive(true)
    end)
end  

function window:start()
    
end

return window