local button

function start()
    button =
        ClickButton:GetComponent("Button").onClick:AddListener(
        function()
           print("onClick")
        end
    )
end


