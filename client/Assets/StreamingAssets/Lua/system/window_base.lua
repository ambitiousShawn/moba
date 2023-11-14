
local window_manager = require "system.window_manager"


local window = {}
window.__index = window

-- 为window_base创建了一个实例对象obj并返回 
function window.new()
	local obj = {
		window_name = nil,
		window_id = nil,
		window_obj = nil,
		parent_window = nil,
	}
	setmetatable(obj, window)
	return obj
end

function window:register( window_name, path, kind )
	self.window_name = window_name
	window_manager:register(window_name, path, kind)
end

function window:set_window_id(window_id)
    self.window_id = window_id
end

function window:set_window_obj(window_obj)
    self.window_obj = window_obj
end

function window:set_parent_window(parent_window)
    self.parent_window = parent_window
end

function window:close_self(is_destroy)
	window_manager:close(self.window_name, is_destroy)
end

return window