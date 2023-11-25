local item = {}

function item:set_member_icon(sprite)
    if sprite ~= nil then
        self.img_member_icon.sprite = sprite
    end
end

function item:set_stage_finish()
    self.text_stage.text = 'Select Finish!'
end

return item