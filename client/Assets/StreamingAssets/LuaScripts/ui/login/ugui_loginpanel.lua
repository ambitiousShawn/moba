local window_base = require "system.window_base"
local check_input_isvalid = require 'ui.login.check_input_isvalid'
local game_message = require 'system.game_message'

-- 初始化ui逻辑
local window = window_base.new()
window:register("ugui_loginpanel", "login", "UIPrefab/UGUI_LoginPanel.prefab", WindowManager.Kind.FullScreen)

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
           return
        end
        
        local userdata = {
            account = account,
            password = pwd
        }
        
        -- 封装消息信息
        local msg = game_message:new_msg('reqLogin', userdata)
        -- 发送网络请求
        NetworkManager:SendMsg(msg)
   end)  

   LuaHelper.BindClick(self.btn_register, function ()
        -- TODO:注册功能
   end) 
   LuaHelper.BindClick(self.btn_forgetPsd, function ()
        -- TODO:找回密码
   end)
   LuaHelper.BindClick(self.btn_offline, function ()
        -- 离线登录
   end)
end  

function window:start()
    -- 输入框设置初始值
    LuaHelper.SetInputFieldInitValue(self.input_username)
    LuaHelper.SetInputFieldInitValue(self.input_password)
end

return window