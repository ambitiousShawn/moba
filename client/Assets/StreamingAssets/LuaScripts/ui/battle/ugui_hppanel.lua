local window_base = require "system.window_base"

-- 初始化ui逻辑
local window = window_base.new()
window:register("ugui_hppanel", "battle", "UIPrefab/HPPanel", WindowManager.Kind.FullScreen)

-- TIPS:该脚本属于第三方，逻辑在C#中

return window