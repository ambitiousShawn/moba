local system = {}

local hero_list = {} -- 存储所有场上的英雄
local collider_list = {} -- 存储场景所有碰撞体容器

-- 初始化场景碰撞
function system:init_env_collision()
    collider_list = LuaCallFunc:InitCollisionEnv()
    print('初始化碰撞体成功')
end

-- 初始化英雄信息
function system:init_hero(battleHeroDatas, mapCfg)
    local cnt = battleHeroDatas.Count / 2
    -- 实例化各个英雄
    for i = 0, cnt - 1 do
        local heroData = {
            heroID = battleHeroDatas[i].heroID,
            posIndex = i,
            userName = battleHeroDatas[i].userName,
            heroCfg = AssetsSvc:GetHeroConfigByID(battleHeroDatas[i].heroID)
        }

        local hero = {}
        if i < cnt then
            -- 蓝色方英雄
            heroData.teamType = ETeamType.Blue
            heroData.bornPos = mapCfg.blueBornPos
            hero = {
                heroID = heroData.heroID,
                posIndex = heroData.posIndex,
                userName = heroData.userName,
                
            }
        end
    end
end

return system