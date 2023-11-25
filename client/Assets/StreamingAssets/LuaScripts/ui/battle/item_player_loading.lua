local item = {}

function item:set_hero_icon_and_name (sprite, name)
    if sprite ~= nil then
        self.img_hero_icon.sprite = sprite
    end
    self.text_hero_name.text = name
end

function item:set_player_name(name)
    self.text_player_name.text = name
end

return item