local window_base = require "system.window_base"

local window = window_base.new()

function window:popup_tip_panel (desc)
    Launcher:AddTips(desc)
end

return window