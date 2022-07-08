# Grid View

# 属性
![Grid View](images/gridview.png)

|属性|功能|
|---|---|
|Prefab| 可以选择一个 [TableViewCell](table_view_cell.md)类型组件的对象。|
|Constraint Count|将网格约束为固定数量的行或列以便支持自动布局系统。|
|Grid Spacing|布局元素之间的间距。|
|Total Count|内容数量。|
|Direction|滚动方向。|
|Snap|停止滚动时是否对齐到内容。|

<br />

# 事件

|属性|功能|
|---|---|
|On Value Changed|滚动矩形的滚动位置发生变化时调用的 [YEvent](event2.md)。该事件可将当前滚动位置作为 Vector2 类型动态参数发送。|