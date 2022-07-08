---@param name string 类名
---@param super table 父类
---@return Class
class = function(name, super)
    ---@class Class
    local cls = {}
    if type(super) ~= "table" then
        super = nil
    end

    cls.new = function(...)
        local instance = setmetatable({}, cls)
        ---@private
        instance.class = cls
        instance:ctor(...)
        return instance
    end
    
    local function __call(tb,...)
        return tb.new(...)
    end

    if super then
        setmetatable(cls, { __index = super, __call = __call })
        cls.super = super
    else
        setmetatable(cls, { __call = __call })
        cls.ctor = function()
        end
    end
    
    ---@param obj table
    cls.cast = function(obj)
        obj.class = cls
        return setmetatable(obj, cls)
    end

    
    
    
    ---@private
    cls.__index = cls
    ---@private
    cls.__cname = name
    
    return cls
end

class2 =  function(classname, super)
    local cls = {}

    cls.classname = classname
    cls.__cname = classname
    cls.class = cls
    cls.Get = {}
    cls.Set = {}

    local Get = cls.Get
    local Set = cls.Set

    if super then
        -- copy super method 
        for key, value in pairs(super) do
            if type(value) == "function" and key ~= "ctor" then
                cls[key] = value
            end
        end

        -- copy super getter
        for key, value in pairs(super.Get) do
            Get[key] = value
        end

        -- copy super setter
        for key, value in pairs(super.Set) do
            Set[key] = value
        end

        cls.super = super
    end

    function cls.__index(self, key)
        local func = cls[key]
        if func then
            return func
        end

        local getter = Get[key]
        if getter then
            return getter(self)
        end

        return nil
    end

    function cls.__newindex(self, key, value)
        local setter = Set[key]
        if setter then
            setter(self, value or false)
            return
        end

        if Get[key] then
            assert(false, "readonly property")
        end

        rawset(self, key, value)
    end

    function cls.new(...)
        local self = setmetatable({}, cls)
        local function create(cls, ...)
            if cls.super then
                create(cls.super, ...)
            end
            if cls.ctor then
                cls.ctor(self, ...)
            end
        end
        create(cls, ...)

        return self
    end

    return cls
end