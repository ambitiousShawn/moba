using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[Serializable]
public class Injection
{
    public string name;
    public GameObject value;
    public string type;
}

[LuaCallCSharp]
public class LuaBehavior : MonoBehaviour
{
    [Header("Lua½Å±¾Â·¾¶")]
    public string LuaPath;

    public List<Injection> injections;

    private Action<LuaTable> luaAwake;
    private Action<LuaTable> luaStart;
    private Action<LuaTable> luaUpdate;
    private Action<LuaTable> luaOnDestroy;
    private Action<LuaTable> luaOnEnable;
    private Action<LuaTable> luaOnDisable;

    private LuaTable module;
    public LuaTable Module
    {
        get
        {
            if (module == null)
            {
                ConnectLuaModule();
            }
            return module;
        }
    }


    void ConnectLuaModule()
    {
# if UNITY_EDITOR
        if (string.IsNullOrEmpty(LuaPath))
        {
            Debug.LogError($"GameObject[{this.gameObject.name}] lua path invalid!");
            return;
        }
#endif
        LuaEnv luaEnv = LuaManager.Instance.GlobalLuaEnv;

        luaEnv.DoString($"require '{LuaPath}'");
        object[] list = luaEnv.DoString(@$"
local module = require '{LuaPath}'

local function new()
    -- class_type, use module.new()
    if module.super ~= nil and module.vtbl ~= nil then
        return module.new()
    end
    -- normal object, set metatable
    local obj = {{}}
    setmetatable(obj, {{__index=module}})
    return obj
end

return new()
", LuaPath);

        if (list != null && list.Length > 0)
        {
            module = list[0] as LuaTable;
        }
        else
        {
            Debug.LogError($"Load lua failed, path[{LuaPath}]!");
        }
        module.Set("go", gameObject);
        foreach (var injection in injections)
        {
            if (injection == null)
            {
                Debug.LogError($"Gameobject[{this.name}], Injection[{injection.name}] - [{injection.value}]");
                continue;
            }
            if (string.IsNullOrEmpty(injection.name) || injection.value == null)
            {
                Debug.LogError($"Gameobject[{this.name}], Injection[{injection.name}] - [{injection.value}]");
                continue;
            }

            if (injection.type == "GameObject" || injection.type == "")
            {
                module.Set(injection.name, injection.value);
            }
            else
            {
                module.Set(injection.name, injection.value.GetComponent(injection.type));
            }
        }

        module.Get("awake", out luaAwake);
        module.Get("start", out luaStart);
        module.Get("update", out luaUpdate);
        module.Get("on_destroy", out luaOnDestroy);
        module.Get("on_enable", out luaOnEnable);
        module.Get("on_disable", out luaOnDisable);
    }

    void Awake()
    {
        if (module == null)
        {
            ConnectLuaModule();
        }
        if (luaAwake != null)
        {
            luaAwake(module);
        }
    }
    void OnEnable()
    {
        if (luaOnEnable != null)
        {
            luaOnEnable(module);
        }
    }

    
    void Start()
    {
        if (luaStart != null)
        {
            luaStart(module);
        }
    }

    
    void Update()
    {
        if (luaUpdate != null)
        {
            luaUpdate(module);
        }
    }

    void OnDisable()
    {
        if (luaOnDisable != null)
        {
            luaOnDisable(module);
        }
    }

    void OnDestroy()
    {
        if (luaOnDestroy != null)
        {
            luaOnDestroy(module);
        }
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        module?.Dispose();
        injections = null;
    }
}
