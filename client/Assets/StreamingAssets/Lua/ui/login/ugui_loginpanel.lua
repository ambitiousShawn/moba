local window_base = require "system.window_base"
local window_manager = require "system.window_manager"
local popup_tip = require 'ui.common.popup_tip'
local check_input_isvalid = require 'ui.login.check_input_isvalid'

local skip_login = Launcher.SkipLogin

-- 初始化ui逻辑
local window = window_base.new()
window:register("ugui_loginpanel", "UIPrefab/UGUI_LoginPanel.prefab", window_manager.Kind.FullScreen)

function window:awake()
    -- 登录逻辑
   LuaHelper.BindClick(self.btn_login, function ()
        -- 拿到用户输入的信息
        local account = self.input_username.text
        local pwd = self.input_password.text
        print(account .. ' ' .. pwd)

        -- 检测合法性
        if check_input_isvalid.check_Email(account) == false 
        and check_input_isvalid.check_name_all_letters_or_alpha(account) == false then
            popup_tip:popup_tip_panel('用户名/邮箱不合法')
            return
        end
        
        local userdata = {
            account = account,
            password = pwd
        }

        popup_tip:popup_tip_panel('免账号登录成功')
        window:close_self(true) 
   end)  

   LuaHelper.BindClick(self.btn_register, function ()
        -- TODO:注册功能
   end) 
   LuaHelper.BindClick(self.btn_forgetPsd, function ()
        -- TODO:找回密码
   end)
   LuaHelper.BindClick(self.btn_offline, function ()
        -- 离线登录
        if skip_login then
            popup_tip:popup_tip_panel('免账号登录成功')
            window:close_self(true) 
            -- window_manager:open("ugui_lobbypanel")
        end
   end)
end  

function window:start()
    -- 输入框设置初始值
    LuaHelper.SetInputFieldInitValue(self.input_username)
    LuaHelper.SetInputFieldInitValue(self.input_password)
end

return window