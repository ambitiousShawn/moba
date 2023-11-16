local item = {}

function item:awake()
    LuaHelper.BindClick(self.btn_click_self, function ()
        print('点击了英雄:' .. self.text_name.text)
    end)
end

function item:set_icon_and_name(sprite, name)
    if sprite ~= nil then
        self.img_icon.sprite = sprite
    end
    self.text_name.text = name
end

return item