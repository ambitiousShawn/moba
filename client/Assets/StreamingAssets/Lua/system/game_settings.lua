local Settings = {}
Settings.__index = Settings

-- 正则表达式分割字符串
local function split(str, delimiter)
    local ret = {}
    for match in (str .. delimiter):gmatch("([^"..delimiter.."]+)") do
        table.insert(ret, match)
    end
    return ret
end

-- 新建一个数据存储集
function Settings.new()
    local self = setmetatable({
        _data = nil
    }, Settings)
    self:_load()
    return self
end

function Settings:get(key, default_value)
    local keys = split(key, ".")
    local found = true
    local node = self._data
    for i, k in ipairs(keys) do
        if type(node) ~= "table" then
            found = false
            break
        end
        node = node[k]
    end

    if found then
        return node
    else
        return default_value
    end
end

function Settings:set(key, value, options)
    local keys = split(key, ".")
    local node = self._data
    for i, k in ipairs(keys) do
        if i == #keys then
            node[k] = value
        else
            if node[k] == nil then
                node[k] = {}
            end
            node = node[k]
        end
    end

    -- 默认同步写文件
    local skip_save = options and options.skip_save == true
    if not skip_save then
        self:_save()
    end
end

function Settings:_save()
    LuaHelper.WriteToFile(PathDefine.PathGameSettings, 'return ' .. dump(self._data))
end

function Settings:_load()
    self._data = nil -- TODO:可设置默认值

    local data_saved = load_from_file(PathDefine.PathGameSettings)
    if data_saved ~= nil then
        data_saved.core = nil
        table.override(self._data, data_saved)
    else
        self:_save()
    end
end

function Settings:reload(data)
    table.override(self._data, data)
end

local game_settings = Settings.new()

return game_settings