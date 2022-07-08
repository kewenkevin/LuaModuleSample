# Coroutine

Lua 使用 Unity StartCoroutine 协程



## 使用

1. 导入 `yield.lua`

```lua
YIELD = require "yield"
StartCoroutine = YIELD.StartCoroutine
StopCoroutine = YIELD.StopCoroutine
```
2. 等待3秒后输出 `HelloWorld`

```lua
StartCoroutine(function()
    print('wait for 3 seconds...')
    YIELD.WaitForSeconds(3)
    print('Hello World')
end)
```

C# 代码

```c#
IEnumerator Func()
{
    Debug.Log('wait for 3 seconds...');
    yield return new WaitForSeconds(3f);
    Debug.Log('Hello World');
}
StartCoroutine(Func());
```



## 语法

### Wait

**Wait(object)**

```lua
 StartCoroutine(function()
        local obj = { isDone = false }
        StartCoroutine(function()
            YIELD.WaitForSeconds(3)
            obj.isDone = true
        end)
        YIELD.Wait(obj)
        -- obj.isDone = true
    end)
```

**Wait(func)**

```lua
YIELD.Wait(function()                
        if ... then
            -- 终止
            return true
        end
        return false
    end)
```

**Wait(co)**

等待协程

```lua
YIELD.Wait(StartCoroutine(function()
    	...
	end))
```

顺序执行

```lua
StartCoroutine(function()
    print('start')    
    YIELD.Wait(StartCoroutine(function()
        print('wait 1')
        YIELD.WaitForSeconds(3)
    end))
    YIELD.Wait(StartCoroutine(function()
        print('wait 2')
        YIELD.WaitForSeconds(3)
    end))
    print('end')
end)

-- output
start
wait 1
wait 2
end
```



### WaitForSeconds

等待时间 `Time.time`

```lua
-- 等待3s
YIELD.WaitForSeconds(3)
```
### WaitForSecondsRealtime

等待未缩放时间`Time.unscaledTime`

```lua
YIELD.WaitForSecondsRealtime(3)
```

### WaitForEndOfFrame

等待帧 `Time.frameCount`

```lua
YIELD.WaitForEndOfFrame()
```
**WaitForEndOfFrame(n)**

等待多帧

```lua
-- 等待3帧
YIELD.WaitForEndOfFrame(3)
```

### WaitForFixedUpdate

等待物理帧

```lua
YIELD.WaitForFixedUpdate()
```
### WaitWhile

等待直到返回 `false`

**WaitWhile(func)**

```lua
YIELD.WaitWhile(function()        
        if ... then
            -- 终止
            return false
        end
        return true
    end)
```
### WaitUntil

等待直到返回 `true`

**WaitUntil(func)**

```lua
YIELD.WaitUntil(function()                
        if ... then
            -- 终止
            return true
        end
        return false
    end)
```

## 加载资源

需要 `Resource [0.7.4]`

```lua
local loader = C_ResLoader.Alloc()
StartCoroutine(function()
    loader:Add2Load(assetPath):Load()    
    YIELD.Wait(loader)
    print('GetAsset: ', loader:GetAsset(assetPath), 'MainAsset: ', loader.MainAsset)    
end)
```

