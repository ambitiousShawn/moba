-- CS import
require 'import_from_cs'

-- 初始化各个管理器
WindowManager = require 'system.window_manager'
WindowManager:init()
NetworkManager = require 'system.network_manager'
NetworkManager:init()

-- 初始化各阶段ui面板
require 'ui.login.ugui_loginpanel'
require 'ui.lobby.ugui_lobbypanel'
require 'ui.lobby.ugui_matchconfirmpanel'

-- 初始化各个系统
local login_system = require 'ui.login.login_system'
login_system:init()
local lobby_system = require 'ui.lobby.lobby_system'
lobby_system:init()

-- 流程进入
login_system:enter_login()

-- 启动测试1:直接进入匹配成功
-- WindowManager:open('ugui_matchconfirmpanel')