local window_base = require "system.window_base"

-- 初始化ui逻辑
local window = window_base.new()
window:register("ugui_loadpanel", "lobby", "UIPrefab/UGUI_LoadPanel.prefab", WindowManager.Kind.FullScreen)

local battleHeroDatas = {}

function window:awake()
    battleHeroDatas = Launcher.BattleHeroDatas
    local count = battleHeroDatas.Count / 2
    -- BlueTeam
    for i = 0, 4 do
        -- local go = GameObject.Instantiate(self.Item_Player_Loading_Prefab, self.blue_team_root)
        local tran = self.blue_team_root:GetChild(i).transform
        if i < count then
            local cfg = AssetsSvc:GetHeroConfigByID(battleHeroDatas[i].heroID)
            local module = tran.gameObject:GetComponent('LuaBehavior').Module
            module:set_hero_icon_and_name(nil, cfg.unitName)
            module:set_player_name(battleHeroDatas[i].userName)
        else
            tran.gameObject:SetActive(false)
        end
    end
    -- RedTeam
    for i = 0, 4 do
        -- local go = GameObject.Instantiate(self.Item_Player_Loading_Prefab, self.red_team_root)
        local tran = self.red_team_root:GetChild(i).transform
        if i < count then
            local cfg = AssetsSvc:GetHeroConfigByID(battleHeroDatas[i + count].heroID)
            local module = tran.gameObject:GetComponent('LuaBehavior').Module
            module:set_hero_icon_and_name(nil, cfg.unitName)
            module:set_player_name(battleHeroDatas[i + count].userName)
        else
            tran.gameObject:SetActive(false)
        end
    end
    
    -- 进度条初始化
    self.text_progress.text = '0%'
end

function window:refresh_progress(percent)
    self.text_progress.text = tostring(percent) .. '%' 
end

return window