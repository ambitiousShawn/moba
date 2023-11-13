local M = {}

-- 将输入的对象转换为字符串形式输出
function dump(obj)
    local getIndent, quoteStr, wrapKey, wrapVal, dumpObj
    -- 指定层级的缩进字符串
    getIndent = function(level)
        return string.rep("\t", level)
    end
    -- 对字符串双引号进行转义
    quoteStr = function(str)
        return '"' .. string.gsub(str, '"', '\\"') .. '"'
    end
    -- 根据键的类型，生成适当表示形式
    wrapKey = function(val)
        if type(val) == "number" then
            return "[" .. val .. "]"
        elseif type(val) == "string" then
            return "[" .. quoteStr(val) .. "]"
        else
            return "[" .. tostring(val) .. "]"
        end
    end
    -- 根据值的类型，生成适当表示形式
    wrapVal = function(val, level)
        if type(val) == "table" then
            return dumpObj(val, level)
        elseif type(val) == "number" then
            return val
        elseif type(val) == "string" then
            return quoteStr(val)
        else
            return tostring(val)
        end
    end
    -- 遍历表生成字符串表示
    dumpObj = function(obj, level)
        if type(obj) ~= "table" then
            return wrapVal(obj)
        end
        level = level + 1
        local tokens = {}
        tokens[#tokens + 1] = "{"
        for k, v in pairs(obj) do
            tokens[#tokens + 1] = getIndent(level) .. wrapKey(k) .. " = " .. wrapVal(v, level) .. ","
        end
        tokens[#tokens + 1] = getIndent(level - 1) .. "}"
        return table.concat(tokens, "\n")
    end
    return dumpObj(obj, 0)
end

local function error_handler(err)
    print('load object from file, got an error: ', err)
end

-- 从目标文件加载
function load_from_file(path)
    if not LuaHelper.FileExists(path) then
        return
    end

    local text = LuaHelper.ReadFromFile(path)
    if text == nil or text == '' then
        return
    end

    local func = load(text)
    if func == nil then
        return
    end

    -- 使用xpcall执行函数并处理可能发生的错误
    local status, result = xpcall(func, error_handler)
    if status then
        return result
    end
end



M.dump = dump
M.load_from_file = load_from_file

return M