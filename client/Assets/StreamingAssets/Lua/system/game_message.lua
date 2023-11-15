local MsgType = CS.GameProtocol.CMD
local PVPType = CS.GameProtocol.PVPType

local msg = {}
msg.__index = msg

-- 生成对应的消息类型(command:string, data:table)
function msg:new_msg(command, data)
    local instance = {
        cmd = nil,
        reqLogin = {
            account = nil,
            password = nil,
        },
        rspLogin = {
            userData = nil,
        },
        reqMatch = {
            pvpType = PVPType.None
        },
        rspMatch = {
            predictTime = nil
        },
        sndConfirm = {
            roomID = nil
        },
    }

    if command == 'reqLogin' then
        instance.cmd = MsgType.ReqLogin
        instance.reqLogin.account = data.account
        instance.reqLogin.password = data.password   
    end
    if command == 'rspLogin' then
        instance.cmd = MsgType.RspLogin
        instance.rspLogin.userData = data.userData
    end
    if command == 'reqMatch' then
        instance.cmd = MsgType.ReqMatch
        instance.reqMatch.pvpType = data.pvpType
    end
    if command == 'rspMatch' then
        instance.cmd = MsgType.RspMatch
        instance.rspMatch.predictTime = data.predictTime
    end
    if command == 'sndConfirm' then
        instance.cmd = MsgType.SndConfirm
        instance.sndConfirm.roomID = data.roomID
    end
    
    setmetatable(instance, self)
    return instance
end

return msg

