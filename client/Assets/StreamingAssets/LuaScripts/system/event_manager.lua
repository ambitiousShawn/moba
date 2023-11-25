local event_system = {}

event_system.event_table = {} -- eventName -> handler

-- 注册事件(事件名 / 标识， 监听方法)
function event_system:AddListener(eventName, handler)
    -- eventName 不合法
    if eventName == nil or ((type(eventName) ~= 'string') and (type(eventName) ~= 'number')) then
        print('EventSystem::AddListener -> eventName不合法')
        return
    end
    -- handler不合法
    if handler == nil or (type(handler) ~= 'function') then
        print('EventSystem::AddListener -> handler不合法')
        return
    end

    event_system.event_table[eventName] = event_system.event_table[eventName] or {}
    table.insert(event_system.event_table[eventName], handler)
end

-- 移除事件(事件标识, 监听方法)
function event_system:RemoveListener(eventName, handler)
    if not eventName or not event_system.event_table[eventName] then
        print('EventSystem::RemoveListener -> Can\'t find eventName'..eventName)
        return
    end

    for k, v in pairs(event_system.event_table[eventName]) do
        if v == handler then
            table.remove(event_system.event_table[eventName], k)
        end
    end
end

-- 分发事件(Trigger)
function event_system:Dispatch(event, ...)
    print('EventSystem::Dispatch ' .. event)
    if not event or event_system.event_table[event] == nil then
        return
    end

    for _, v in pairs(event_system.event_table[event]) do
        v(...)
    end
end

return event_system