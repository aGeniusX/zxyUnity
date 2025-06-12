local breakSocketHandle, debugXpCall = require("Assets.Script.Lua.LuaDebugjit")("localhost", 7003)
breakSocketHandle()

print("hello")

function MyFunction()
    print("123")
end

local a
local b
local button
function start()
    a = 1.5
    b = 2
    button = CS.UnityEngine.GameObject.GetComponent("Button").onClick:AddListener()
end
function update()
    CS.UnityEngine.Transform.GetComponent("BoxCollider")
    
end
