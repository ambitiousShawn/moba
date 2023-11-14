local mgr = {}
-- UI窗口的类型
mgr.Kind = {
	FullScreen = 1,
	Popup = 2,
	SubPanel = 3,
	Fixed = 4,
}


function mgr:init()
	self.name_obj_dic = {} -- window_name => window_obj, 缓存场景中的预制体
	self.window_stack = {} -- index => window
	self.name_info_dic = {} -- window_name => window_info(name,path,kind)
	self.id_window_dic = {} -- id => window
	self.curr_unique_window_id = 1 -- window唯一序号标识
end

-- 向ui管理类注册面板类
function mgr:register( window_name, path, kind )
	local window_info = {
		window_name = window_name,
		path = path,
		kind = kind,
	}

	self.name_info_dic[window_name] = window_info
	print("window_name:" .. window_name .. "已注册")
end

-- 打开名为 name 的窗口
function mgr:open(window_name, callback)
	local window_obj = self.name_obj_dic[window_name]
	if window_obj ~= nil then
		-- 窗口存在，提高优先级
		window_obj:SetActive(true)
		window_obj.transform:SetAsLastSibling()
	else
		-- 窗口不存在，加载预制体
		local info = self.name_info_dic[window_name]
		if info == nil then
			print('open window failed: [info][' .. window_name .. ']')
			return
		end
		local path = info.path
		if string.sub(path, -7) == '.prefab' then
			path = string.sub(path, 1, -8)
		end
		local prefab = Resources.Load(path)
		if prefab == nil then
			print('open window failed: [resource][' .. window_name .. ']')
			return
		end
		-- 实例化物体，并加入缓存容器
		window_obj = GameObject.Instantiate(prefab, self:get_ui_root_transform())
		self.name_obj_dic[window_name] = window_obj
		window_obj.name = window_name
		window_obj:SetActive(true)
	end

	local window = window_obj:GetComponent('LuaBehavior').Module
	-- 填充容器的对应关系：id_window_dic
	local current_id = self.curr_unique_window_id
	self.curr_unique_window_id = self.curr_unique_window_id + 1
	self.id_window_dic[current_id] = window
	-- 设置面板自身属性
	window:set_window_id(current_id)
	window:set_window_obj(window_obj)

	-- 填充容器对应关系：window_stack
	local index_window_lst = self:find_window_in_stack(window.window_name)
	local index = index_window_lst[1]
	if index ~= nil then
		-- 窗口已存在
		table.remove(self.window_stack, index)
	end
	table.insert(self.window_stack, #self.window_stack + 1, window)
	
	-- callback
	if callback then
		callback(window)
	end

	-- self:rearrage_stack_view()
end

-- window_id可传入标识（名字 / id）
function mgr:close(window_id, is_destroy)
	-- 查找对应的ui 界面window
	local window = nil
	if type(window_id) == 'number' then
		window = self.id_window_dic[window_id]
	elseif type(window_id) == 'string' then
		local idx_window_table = self:find_window_in_stack(window_id)
		window = idx_window_table[2]
	end
	if window == nil then
		print('window not found!')
		return
	end

	local window_obj = window.window_obj
	if window_obj ~= nil then
		-- id_window_dic元素移除
		self.id_window_dic[window.window_id] = nil
		-- window_stack元素移除
		local index_window_lst = self:find_window_in_stack(window.window_name)
		local index = index_window_lst[1]
		if index ~= nil then
			table.remove(self.window_stack, index)
		end
		-- 对象失活操作 / 销毁对象操作
		if is_destroy == false then
			window_obj:SetActive(false)	
		else
			local window_name = window.window_name
			self.name_obj_dic[window_name] = nil -- 去掉对象缓存
			GameObject.Destroy(window_obj)
		end
	end 
end

-- 关闭所有窗口
function mgr:close_all()
	for i = #self.window_stack, 1, -1 do
		local window = self.window_stack[i]
		window:close_self(true)
	end
	self.window_stack = {}
	self.id_window_dic = {}
	self.name_obj_dic = {}
end

-- 从排序ui栈中查找对应窗口
function mgr:find_window_in_stack(window_name)
	-- 返回栈顶
	for i = #self.window_stack, 1, -1 do
		local window = self.window_stack[i]
		if window.window_name == window_name then
			local ret = {i, window}
			return ret
		end
	end
	return {nil, nil}
end

-- 重新排列窗口中的顺序(TODO)
function mgr:rearrage_stack_view()
	local top = nil
	for i = #self.window_stack, 1, -1 do
		local panel = self.window_stack[i]
		local info = self.name_info_dic[panel.window_name]
		-- 全屏放最后
		if top == nil and info.kind == self.Kind.FullScreen then
			top = i
		end
	end
end

-- 在场景中查找UI的根节点
function mgr:get_ui_root_transform()
	if self.ui_root_transform == nil then
		self.ui_root_transform = GameObject.Find("GameRoot/UIRoot").transform
	end
	return self.ui_root_transform
end

return mgr