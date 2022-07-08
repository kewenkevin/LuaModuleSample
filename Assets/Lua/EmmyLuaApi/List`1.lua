---@class List`1 : Object
---@field public Capacity number
---@field public Count number
---@field public Item LinkerData
local List`1={ }
---@public
---@param item LinkerData
---@return void
function List`1:Add(item) end
---@public
---@param collection IEnumerable`1
---@return void
function List`1:AddRange(collection) end
---@public
---@return ReadOnlyCollection`1
function List`1:AsReadOnly() end
---@public
---@param index number
---@param count number
---@param item LinkerData
---@param comparer IComparer`1
---@return number
function List`1:BinarySearch(index, count, item, comparer) end
---@public
---@param item LinkerData
---@return number
function List`1:BinarySearch(item) end
---@public
---@param item LinkerData
---@param comparer IComparer`1
---@return number
function List`1:BinarySearch(item, comparer) end
---@public
---@return void
function List`1:Clear() end
---@public
---@param item LinkerData
---@return bool
function List`1:Contains(item) end
---@public
---@param array LinkerData[]
---@return void
function List`1:CopyTo(array) end
---@public
---@param index number
---@param array LinkerData[]
---@param arrayIndex number
---@param count number
---@return void
function List`1:CopyTo(index, array, arrayIndex, count) end
---@public
---@param array LinkerData[]
---@param arrayIndex number
---@return void
function List`1:CopyTo(array, arrayIndex) end
---@public
---@param match Predicate`1
---@return bool
function List`1:Exists(match) end
---@public
---@param match Predicate`1
---@return LinkerData
function List`1:Find(match) end
---@public
---@param match Predicate`1
---@return List`1
function List`1:FindAll(match) end
---@public
---@param match Predicate`1
---@return number
function List`1:FindIndex(match) end
---@public
---@param startIndex number
---@param match Predicate`1
---@return number
function List`1:FindIndex(startIndex, match) end
---@public
---@param startIndex number
---@param count number
---@param match Predicate`1
---@return number
function List`1:FindIndex(startIndex, count, match) end
---@public
---@param match Predicate`1
---@return LinkerData
function List`1:FindLast(match) end
---@public
---@param match Predicate`1
---@return number
function List`1:FindLastIndex(match) end
---@public
---@param startIndex number
---@param match Predicate`1
---@return number
function List`1:FindLastIndex(startIndex, match) end
---@public
---@param startIndex number
---@param count number
---@param match Predicate`1
---@return number
function List`1:FindLastIndex(startIndex, count, match) end
---@public
---@param action Action`1
---@return void
function List`1:ForEach(action) end
---@public
---@return Enumerator
function List`1:GetEnumerator() end
---@public
---@param index number
---@param count number
---@return List`1
function List`1:GetRange(index, count) end
---@public
---@param item LinkerData
---@return number
function List`1:IndexOf(item) end
---@public
---@param item LinkerData
---@param index number
---@return number
function List`1:IndexOf(item, index) end
---@public
---@param item LinkerData
---@param index number
---@param count number
---@return number
function List`1:IndexOf(item, index, count) end
---@public
---@param index number
---@param item LinkerData
---@return void
function List`1:Insert(index, item) end
---@public
---@param index number
---@param collection IEnumerable`1
---@return void
function List`1:InsertRange(index, collection) end
---@public
---@param item LinkerData
---@return number
function List`1:LastIndexOf(item) end
---@public
---@param item LinkerData
---@param index number
---@return number
function List`1:LastIndexOf(item, index) end
---@public
---@param item LinkerData
---@param index number
---@param count number
---@return number
function List`1:LastIndexOf(item, index, count) end
---@public
---@param item LinkerData
---@return bool
function List`1:Remove(item) end
---@public
---@param match Predicate`1
---@return number
function List`1:RemoveAll(match) end
---@public
---@param index number
---@return void
function List`1:RemoveAt(index) end
---@public
---@param index number
---@param count number
---@return void
function List`1:RemoveRange(index, count) end
---@public
---@return void
function List`1:Reverse() end
---@public
---@param index number
---@param count number
---@return void
function List`1:Reverse(index, count) end
---@public
---@return void
function List`1:Sort() end
---@public
---@param comparer IComparer`1
---@return void
function List`1:Sort(comparer) end
---@public
---@param index number
---@param count number
---@param comparer IComparer`1
---@return void
function List`1:Sort(index, count, comparer) end
---@public
---@param comparison Comparison`1
---@return void
function List`1:Sort(comparison) end
---@public
---@return LinkerData[]
function List`1:ToArray() end
---@public
---@return void
function List`1:TrimExcess() end
---@public
---@param match Predicate`1
---@return bool
function List`1:TrueForAll(match) end
System.Collections.Generic.List`1 = List`1