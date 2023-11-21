-- unity
CS = CS
UnityEngine = CS.UnityEngine
Object = CS.UnityEngine.Object
GameObject = CS.UnityEngine.GameObject
Resources = CS.UnityEngine.Resources
Time = CS.UnityEngine.Time
Launcher = CS.Launcher.Instance
PrimitiveType = CS.UnityEngine.PrimitiveType

-- tools
LuaHelper = CS.LuaHelper

-- Service
AssetsSvc = Launcher:GetComponent('AssetsSvc')
NetSvc = Launcher:GetComponent('NetSvc')
BattleSvc = Launcher:GetComponent('BattleSvc')
FightManager = CS.FightManager.Instance

-- config
ServerConfig = CS.GameProtocol.ServerConfig

-- enum
ETeamType = CS.ETeamType
EUnitType = CS.EUnitType

