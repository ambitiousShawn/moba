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
        ntfConfirm = {
            roomID = nil,
            dissmiss = false,
            confirmArr = {}
        }
    }

    if command == 'reqLogin' then
        instance.cmd = MsgType.ReqLogin
        instance.reqLogin.account = data.account
        instance.reqLogin.password = data.password   
    end
    if command == 'rspLogin' then
        instance.cmd = MsgType.RspLogin
        instance.userData = data.userData
    end
    if command == 'reqMatch' then
        instance.cmd = MsgType.ReqMatch
        instance.pvpType = data
    end
    if command == 'rspMatch' then
        instance.cmd = MsgType.RspMatch
        instance.predictTime = data.predictTime
    end
    if command == 'sndConfirm' then
        instance.cmd = MsgType.SndConfirm
        instance.roomID = data
    end
    if command == 'ntfConfirm' then
        instance.cmd = MsgType.NtfConfirm
    end
    
    setmetatable(instance, self)
    return instance
end

return msg

