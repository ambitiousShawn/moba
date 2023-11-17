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
        reqMatch = {
            pvpType = PVPType.None
        },
        sndConfirm = {
            roomID = nil
        },
        sndSelect = {
            roomID = nil,
            heroID = nil,
        },
        sndLoadPrg = {
            roomID = 0,
            percent = 0,
        }
    }

    if command == 'reqLogin' then
        instance.cmd = MsgType.ReqLogin
        instance.reqLogin.account = data.account
        instance.reqLogin.password = data.password   
    end
    if command == 'reqMatch' then
        instance.cmd = MsgType.ReqMatch
        instance.reqMatch.pvpType = data.pvpType
    end
    if command == 'sndConfirm' then
        instance.cmd = MsgType.SndConfirm
        instance.sndConfirm.roomID = data.roomID
    end
    if command == 'sndSelect' then
        instance.cmd = MsgType.SndSelect
        instance.sndSelect.roomID = data.roomID
        instance.sndSelect.heroID = data.heroID
    end
    if command == 'sndLoadPrg' then
        instance.cmd = MsgType.sndLoadPrg
        instance.sndLoadPrg.roomID = data.roomID
        instance.sndLoadPrg.percent = data.percent
    end
    
    setmetatable(instance, self)
    return instance
end

return msg

