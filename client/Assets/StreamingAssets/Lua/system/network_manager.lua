local mgr = {}

function mgr:init()
    
end

-- 发送消息(msg:消息,cb:回调)
function mgr:SendMsg(msg)
    NetSvc:SendMsg(msg, nil)
end

return mgr