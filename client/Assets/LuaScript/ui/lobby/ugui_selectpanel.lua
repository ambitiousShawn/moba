local window_base = require "system.window_base"
local game_message = require 'system.game_message'
local network_manager = require 'system.network_manager'

-- 初始化ui逻辑
local window = window_base.new()
window:register("ugui_selectpanel", "lobby", "UIPrefab/UGUI_SelectPanel.prefab", WindowManager.Kind.FullScreen)

local is_selected = false -- 是否已经选择英雄 
local time_count = 90  -- 当前倒计时
local hero_data_list = {}  -- 玩家所对应的英雄数据
local select_hero_id = 101   -- 当前选择的英雄ID(默认选择第一个)


local self_player       -- 保存侧边栏自身客户端对应哪个玩家

-- 刷新侧边栏英雄图标
local function refresh_self_hero_icon(image)
    if self_player ~= nil then
        self_player:set_member_icon(image.sprite) 
    end
end

function window:awake()
    -- 绑定确认按钮
    LuaHelper.BindClick(self.btn_sure, function ()
        self:ClickSureBtnEvent()
    end)

    is_selected = false
    time_count = ServerConfig.SelectCountDown
    self.img_shadow:SetActive(false) -- 防止确认按钮被遮挡
    hero_data_list = Launcher.UserData.heroDatas

    local i
    -- ===== 初始化两边队伍信息 =====
    -- 先清除原本预制体(跳过销毁第一个预制体)
    for i = 1, self.blue_team_parent.childCount - 1 do
        local go = self.blue_team_parent:GetChild(i).gameObject
        Object.DestroyImmediate(go)
    end
    for i = 1, self.red_team_parent.childCount - 1 do
        local go = self.red_team_parent:GetChild(i).gameObject
        Object.DestroyImmediate(go)
    end
    -- TODO 实例化两边队伍的成员信息(暂定1v1)
    for i = 1, 1 do
        local go = GameObject.Instantiate(self.Item_BluePlayer_Prefab)
        go.name = 'BluePlayer_1'
        local tran = go:GetComponent('Transform')
        tran:SetParent(self.blue_team_parent)
        self_player = go:GetComponent('LuaBehavior').Module -- 默认自身是蓝色方第一个玩家
    end
    self.Item_BluePlayer_Prefab:SetActive(false) -- 创建完后隐藏预制体
    for i = 1, 1 do
        local go = GameObject.Instantiate(self.Item_RedPlayer_Prefab)
        go.name = 'RedPlayer_1'
        local tran = go:GetComponent('Transform')
        tran:SetParent(self.red_team_parent)
    end
    self.Item_RedPlayer_Prefab:SetActive(false) -- 创建完后隐藏预制体

    -- ===== 初始化选择英雄列表信息 =====
    -- (跳过销毁第一个预制体)
    for i = 1, self.content.childCount - 1 do
        local go = self.content:GetChild(i).gameObject
        Object.DestroyImmediate(go)
    end
    -- List类型无法通过 # 取长度
    for i = 0, hero_data_list.Count - 1 do
        local heroID = hero_data_list[i].heroId
        local go = GameObject.Instantiate(self.Item_Hero_Prefab, self.content)
        go.name = tostring(heroID)

        -- 获取到组件，刷新item数据与表现
        local module = go:GetComponent('LuaBehavior').Module
        -- 获取英雄配置数据
        local cfg = AssetsSvc:GetHeroConfigByID(heroID)
        -- UI表现刷新赋值
        -- module.img_icon.sprite = cfg.
        module.text_name.text = cfg.unitName
        -- 绑定按钮事件
        LuaHelper.BindClick(go, function ()
            print('点击了英雄： ' .. cfg.unitName)
            select_hero_id = heroID -- 保存当前选中
        end)
    end
    self.Item_Hero_Prefab:SetActive(false) -- 创建完成后隐藏预制体
end

local delta_time = 0
function window:update()
    delta_time = delta_time + Time.deltaTime
    if delta_time >= 1 then
        delta_time = delta_time - 1
        time_count = time_count - 1
        if time_count < 0 then
            time_count = 0
            -- 倒计时结束后
            self:ClickSureBtnEvent()
        end

        self.text_countdown.text = tostring(time_count)
    end
end

-- 按下 'btn_sure' 或 倒计时结束后触发
function window:ClickSureBtnEvent()
    -- 如果已经选择英雄，则无需过多操作
    if is_selected then
        return
    end

    local data = {
        roomID = Launcher.RoomID,
        heroID = select_hero_id,
    }
    local msg = game_message:new_msg('sndSelect', data)
    NetworkManager:SendMsg(msg)

    self.img_shadow:SetActive(true) -- 遮挡按钮
    is_selected = true
    
    -- 更改侧边栏状态信息
    self_player:set_stage_finish()
    -- 关闭选择英雄界面
    self.hero_list_parent:SetActive(false)
end

return window