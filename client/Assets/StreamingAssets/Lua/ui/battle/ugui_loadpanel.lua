local window_base = require "system.window_base"

-- 初始化ui逻辑
local window = window_base.new()
window:register("ugui_loadpanel", "UIPrefab/UGUI_LoadPanel.prefab", WindowManager.Kind.FullScreen)

local battleHeroDatas

function window:awake()
    battleHeroDatas = Launcher.BattleHeroDatas
    local count = battleHeroDatas.Count / 2
    -- BlueTeam
    for i = 0, count - 1 do
        local go = GameObject.Instantiate(self.Item_Player_Loading_Prefab, self.blue_team_root)
        local cfg = AssetsSvc:GetHeroConfigByID(battleHeroDatas[i].heroID)
        local module = go:GetComponent('LuaBehavior').Module
        module:set_hero_icon_and_name(nil, cfg.heroName)
        module:set_player_name(battleHeroDatas[i].userName)
    end
    -- RedTeam
    for i = 0, count - 1 do
        local go = GameObject.Instantiate(self.Item_Player_Loading_Prefab, self.red_team_root)
        local cfg = AssetsSvc:GetHeroConfigByID(battleHeroDatas[i + count].heroID)
        local module = go:GetComponent('LuaBehavior').Module
        module:set_hero_icon_and_name(nil, cfg.heroName)
        module:set_player_name(battleHeroDatas[i + count].userName)
    end
    self.Item_Player_Loading_Prefab:SetActive(false) -- 隐藏预制体
    
    -- 进度条初始化
    self.text_progress.text = '0%'
end

function window:refresh_progress(percentList)
    local sum = 0
    local cnt = percentList.Count
    for i = 0, cnt - 1 do
        sum = sum + percentList[i]
    end
    self.text_progress.text = tostring(math.floor(sum / cnt)) .. '%' 
end

return window