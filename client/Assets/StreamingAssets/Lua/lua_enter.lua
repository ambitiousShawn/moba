-- CS import
require 'import_from_cs'

-- UI启动
local window_manager = require 'system.window_manager'
window_manager:init()
-- window_manager:register('ugui_loginpanel', 'UIPrefab/UGUI_LoginPanel.prefab', window_manager.Kind.FullScreen)


-- 游戏内容启动
require 'ui.login.ugui_loginpanel'

window_manager:open('ugui_loginpanel', nil)

-- window_manager:close('ugui_loginpanel')
print('lua module has launched!')