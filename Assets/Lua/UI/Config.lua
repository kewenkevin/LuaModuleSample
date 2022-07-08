local UIEnums = require("LuaModules.UILib.Enums.UIEnums")
return {
    --uiType: stack 栈类型界面，freedom 自由控制类型界面（不做任何约束），order 顺序显示模式界面，unique 唯一界面（只能显示一个）
    pages = {
        ["ExampleEnterPage"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.replace, assetNameRelative = "ExampleEnterPage/ExampleEnterPage" },
       
        ["E0_1_Page"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.exclusive , assetNameRelative =  "E0_1/E0_1_Page"},

        ["E1_1_Page"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.exclusive , assetNameRelative =  "E1_1/E1_1_Page"},

        ["E1_2_Page"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.exclusive , assetNameRelative =  "E1_2_PageMethodBinded/E1_2_Page"},
        ["E1_4_Page"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.exclusive , assetNameRelative =  "E1_4_TableView/E1_4_Page"},
        ["E1_5_Page"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.exclusive , assetNameRelative =  "E1_5_GridView/E1_5_Page"},
        ["E1_7_Page"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.exclusive , assetNameRelative =  "E1_7_ModuleNested/E1_7_Page"},
        ["E1_12_Page"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.coexist , assetNameRelative =  "E1_12_PageAnimation/E1_12_Page"},
        ["E1_13_Page"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.coexist , assetNameRelative =  "E1_12_PageAnimation/E1_13_Page"},

        ["E2_1_Page"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.exclusive , assetNameRelative =  "E2_1/E2_1_Page"},
        ["E2_2_Page"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.exclusive , assetNameRelative =  "E2_2/E2_2_Page"},
        ["E2_3_Page"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.exclusive , assetNameRelative =  "E2_3/E2_3_Page"},


        ["E3_1_Page"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.exclusive , assetNameRelative =  "E3_1_DriveModuleByData/E3_1_Page"},

        ["E4_1_Page"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.exclusive , assetNameRelative =  "E4_1_StackType/E4_1_Page"},
        ["E4_1_Page_Child"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.exclusive , assetNameRelative =  "E4_1_StackType/E4_1_Page_Child"},
        ["E4_1_Page_ChildAttach"] = { uiType = UIEnums.PageType.freedom, assetNameRelative =  "E4_1_StackType/E4_1_Page_ChildAttach"},
        ["E4_1_Page_Finial"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.coexist , assetNameRelative =  "E4_1_StackType/E4_1_Page_Finial"},
        ["E4_1_Page_Parallel"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.exclusive , assetNameRelative =  "E4_1_StackType/E4_1_Page_Parallel"},
        
        ["E4_2_Page"] = { uiType = UIEnums.PageType.unique, assetNameRelative =  "E4_2_UniqueType/E4_2_Page"},
        ["E4_3_Page"] = { layer = 10, uiType = UIEnums.PageType.order, assetNameRelative =  "E4_3_OrderType/E4_3_Page"},
        ["E4_4_Page"] = { layer = 20, uiType = UIEnums.PageType.freedom, assetNameRelative =  "E4_4_FreedomType/E4_4_Page"},
        
        ["E5_1_Page"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.exclusive , assetNameRelative =  "E5_1_PageChecker/E5_1_Page"},
        ["E5_1_A_Page"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.exclusive ,
                              assetNameRelative =  "E5_1_PageChecker/E5_1_A_Page", checkerName = "unlockLevelChecker"},
        ["E5_1_B_Page"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.exclusive , 
                              assetNameRelative =  "E5_1_PageChecker/E5_1_B_Page", checkerName = "unlockLevelChecker"},
        
        ["E5_2_Page"] = { uiType = UIEnums.PageType.stack, mode = UIEnums.PageMode.exclusive , assetNameRelative =  "E5_2_FlexPanel/E5_2_Page"},
    },
    mainPage = "E1_1_Page"
}