---@class
local CipherModule = class("Store.CipherModule",G_DataStoreModuleBase)

function CipherModule:initialize()
    self._data = {
        tag = "cipher",

        password = "",
    }
end

---setPassword 设置密码
function CipherModule:setPassword(password)
    self._data.password = password
end

---getPassword 获取密码
function CipherModule:getPassword(password)
    return self._data.password
end

---checkPassword 确认暗号
function CipherModule:checkCipher(cipher)
    if cipher > 88 then
        self:fireEvent("checkCipherResult","太大",cipher)
    elseif cipher < 88  then
        self:fireEvent("checkCipherResult","太小",cipher)
    else
        self:fireEvent("checkCipherResult","正确！",cipher)
    end
end
return CipherModule