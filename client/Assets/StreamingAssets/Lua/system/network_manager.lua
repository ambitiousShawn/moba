local mgr = {}

mgr.net_svc = nil

function mgr:init()
    self.net_svc = CS.ShawnFramework.CommonModule.NetSvc.Instance
end

-- 发送消息(msg:消息,cb:回调)
function mgr:SendMsg(msg)
    -- self.net_svc:SendMsg(msg, nil)
    CS.ShawnFramework.CommonModule.NetSvc.Instance:SendMsg(msg, nil)
end

return mgr