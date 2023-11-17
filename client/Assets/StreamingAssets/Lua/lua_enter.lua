-- CS import
require 'import_from_cs'

-- 初始化各个管理器
WindowManager = require 'system.window_manager'
WindowManager:init()
NetworkManager = require 'system.network_manager'
NetworkManager:init()
EventSystem = require 'system.event_manager'


-- 初始化各阶段ui面板
--require 'ui.login.ugui_loginpanel'
--require 'ui.lobby.ugui_lobbypanel'
--require 'ui.lobby.ugui_matchconfirmpanel'

-- 初始化各个系统
LoginSystem = require 'ui.login.login_system'
LobbySystem = require 'ui.lobby.lobby_system'
BattleSystem = require 'ui.battle.battle_system'

-- 流程进入
LoginSystem:enter_login()
print(CS.Launcher.Instance.RoomID)
print(CS.Launcher.Instance.MapID)
-- 启动测试1:直接进入匹配成功
-- WindowManager:open('ugui_matchconfirmpanel')