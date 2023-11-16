local window_base = require "system.window_base"
local game_message = require 'system.game_message'
local network_manager = require 'system.network_manager'

-- 初始化ui逻辑
local window = window_base.new()
window:register("ugui_selectpanel", "UIPrefab/UGUI_SelectPanel.prefab", WindowManager.Kind.FullScreen)

local is_selected = false -- 是否已经选择英雄 
local time_count = 90  -- 当前倒计时
local hero_data_list = {}  -- 玩家所对应的英雄数据
local select_hero_id = -1   -- 当前选择的英雄ID

function window:awake()
    LuaHelper.BindClick(self.btn_sure, function ()
        self:ClickSureBtnEvent()
    end)

    is_selected = false
    time_count = ServerConfig.SelectCountDown
    self.img_shadow:SetActive(false) -- 防止确认按钮被遮挡
    hero_data_list = Launcher.UserData.heroDatas

    -- 初始化蓝方英雄选择
    local i
    -- 先清除原本预制体
    for i = 0, self.blue_team_parent.childCount - 1 do
        local go = self.blue_team_parent:GetChild(i).gameObject
        Object.DestroyImmediate(go)
    end
    for i = 0, self.red_team_parent.childCount - 1 do
        local go = self.red_team_parent:GetChild(i).gameObject
        Object.DestroyImmediate(go)
    end
    -- TODO 实例化两边队伍的成员信息

    -- 初始化英雄信息(跳过销毁第一个预制体)
    for i = 1, self.hero_list_parent.childCount - 1 do
        local go = self.hero_list_parent:GetChild(i).gameObject
        Object.DestroyImmediate(go)
    end
    -- List类型无法通过 # 取长度
    for i = 0, hero_data_list.Count - 1 do
        local heroID = hero_data_list[i].heroId
        local go = GameObject.Instantiate(self.Item_Hero_Prefab)
        go.name = tostring(heroID)
        local tran = go:GetComponent('Transform')
        tran:SetParent(self.hero_list_parent)

        -- 获取到组件，刷新item数据与表现
        local module = go:GetComponent('LuaBehavior').Module
        module:set_icon_and_name(nil, '暗夜猎手')
        -- 获取英雄配置数据
        -- TODO:AssetsSvc调用为空？？？
        -- local cfg = AssetsSvc:GetHeroConfigByID(heroID)
        -- TODO:UI赋值
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
    network_manager:SendMsg(msg)

    self.img_shadow:SetActive(true) -- 遮挡按钮
    is_selected = true
end

return window