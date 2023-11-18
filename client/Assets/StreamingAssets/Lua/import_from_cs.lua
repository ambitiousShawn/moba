-- unity
CS = CS
UnityEngine = CS.UnityEngine
Object = CS.UnityEngine.Object
GameObject = CS.UnityEngine.GameObject
Resources = CS.UnityEngine.Resources
Time = CS.UnityEngine.Time
Launcher = CS.Launcher.Instance

-- tools
LuaHelper = CS.LuaHelper
LuaCallFunc = CS.LuaCallFunc.Instance

-- Service
AssetsSvc = Launcher:GetComponent('AssetsSvc')
NetSvc = Launcher:GetComponent('NetSvc')
BattleSvc = Launcher:GetComponent('BattleSvc')

-- config
ServerConfig = CS.GameProtocol.ServerConfig

-- enum
ETeamType = CS.ETeamType
EUnitType = CS.EUnitType
