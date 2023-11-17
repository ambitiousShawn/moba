local popup_tip = require 'ui.common.popup_tip'
local ugui_loginpanel = require 'ui.login.ugui_loginpanel'

local system = {}

-- 进入登录系统
function system:enter_login()
    WindowManager:close_all()
    WindowManager:open('ugui_loginpanel')
end

-- 登录响应回调(响应信息lua表)
function RspLoginCallBack(msg)
    popup_tip:popup_tip_panel('登录成功')
    Launcher.UserData = msg.rspLogin.userData -- TODO
    ugui_loginpanel:close_self(true)
    LobbySystem:enter_lobby()
end

return system;