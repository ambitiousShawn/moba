local M = {}

-- 检测Email是否满足需求
function CheckEmailValid(email)
    if string.len(email or '') < 6 or string.len(email or '') > 15 then
        return false;
    end
    local b, e = string.find(email or '', '@')
    local bstr = ''
    local estr = ''
    if b then
        bstr = string.sub(email, 1, b - 1)
        estr = string.sub(email, e + 1, -1)
    else
        return false
    end
    return true
end

-- 检测
function CheckNameAllLettersOrAlpha(account)
	-- 检查字符串长度是否大于等于6位
    if string.len(account or '') < 6 or string.len(account or '') > 15 then
        return false;
    end

    -- 检查是否全为数字
    local all_digits = true
    for i = 1, #account do
        local char = string.sub(account, i, i)
        if not tonumber(char) then
            all_digits = false
            break
        end
    end

    -- 检查是否全为字母或字母+数字
    local all_letters_or_alphanumeric = true
    if not all_digits then
        for i = 1, #account do
            local char = string.sub(account, i, i)
            if not (char:match("%a") or tonumber(char)) then
                all_letters_or_alphanumeric = false
                break
            end
        end
    end
    return all_letters_or_alphanumeric
end

M.check_Email = CheckEmailValid
M.check_name_all_letters_or_alpha = CheckNameAllLettersOrAlpha

return M