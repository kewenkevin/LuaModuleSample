# String.Format 扩展

扩展 string.Format


## 使用

使用string扩展方法

1. using 命名空间

   ```c#
   using System.StringFormats;
   ```

2. `FormatString` 扩展方法

   ```c#
   //"ABC".ToLower()
   "{0:@ToLower()}".FormatString("ABC")
   => "abc"
   ```

3. `FormatStringWithKey` 扩展方法，支持字典参数

   ```c#
   Dictionary<string, object> dic = new Dictionary<string, object>();
   dic["Text"] = "ABC";
   "{$Text:@ToLower}".FormatStringWithKey(dic)
   => "abc"
   ```

   

## 调用方法

```c#
@[@][Type.]Member([arg0][,arg1]...)[,format]...
```

调用方法

- @

  参数实例 `GetType()` 类型

- @@

  参数类型

- Member

  参数成员(方法，属性，字段)

- arg 

  参数

- $

  引用当前实例

- format

  返回值格式化

  

调用方法

```c#
//"ABC".ToLower()
"{0:@ToLower()}".FormatString("ABC")
=> "abc"

//"ABC".ToLower()
"{0:@ToLower}".FormatString("ABC")
=> "abc"    

//"ABC".Replace("BC","AA")
"{0:@Replace(\"BC\",\"AA\")}".FormatString("ABC")
=> "AAA"


//string.Empty    
"{0:@Empty}".FormatString(typeof(string)))
=> ""

//"ABC".Substring(1,2).ToLower()
"{0:@Substring(1,2)@ToLower}".FormatString("ABC")
=> "bc"
    
//string.Format("{0:yyyy",DateTime.Now)
"{0:@Now,yyyy}".FormatString(typeof(DateTime))
=> 2019
```



获取属性

```c#
"{0:@Length}".FormatString("abc")
=> "3"
```



`$` 引用当前实例

```c#
//String.Join(" ","Hello","World")
"{0:@String.Join(\" \",$,\"World\")}".FormatString("Hello")
=> "Hello World"
```





## 定制格式

实现 `INameFormatter` 接口，`#`开始

```c#
#Name
```

```c#
"{0:#FileName}".FormatString("Dir/Sub/file.txt")
=> "file.txt"
```





## 正则表达式提取值

`/`开始和结束，返回值组名为`result`

```javascript
/(?<result>regex expression)/
```

使用正则表达式提取字符串，提取内容result匹配组

```c#
"{0:/(?<result>h.*d)/}".FormatString("say hello world .")
=> "hello world"
```





## 路径格式

```c#
Name[,Offset...][,SeparatorChar]
```

**Name**

- FilePath

  不做处理

- FileName

  文件名

- FileNameWithoutExtension

  不带扩展名的文件名

- FileExtension

  文件扩展名

- DirectoryName

  文件父目录名

- DirectoryPath

  文件夹路径

- FullPath

  完整路径名称

- FullDirectoryPath

  完整文件夹路径

**Offset:** 

​	目录偏移，数字，正数右边开始，负数左边开始

**SeparatorChar:** 

​	`/`, `\\` 强制目录分隔符

 

```c#
string path= "Dir/Sub/file.txt"

"{0:#FilePath}".FormatString(path)
=> Dir/Sub/file.txt"

#FileName
=> "file.txt"

#FileNameWithoutExtension
=> "file"

#FileExtension
=> ".txt"

#DirectoryName
=> "Sub"

#DirectoryPath
=> "Dir/Sub"

#DirectoryPath,1
=> "Dir"

#DirectoryPath,-1
=> "Sub"

#FilePath,\\
=> "Dir\\Sub\\file.txt"
```



## 字典参数

```c#
{$key:format}
```

key名称用`$`开头

```c#
Dictionary<string, object> dic = new Dictionary<string, object>();
dic["Path"] = "parent/child/file.txt";
"{$Path:#FileName}".FormatStringWithKey(dic)
=> "file.txt"
```





## 组合格式

```c#
format[:format...]
```

通过 `:` 连接多个格式

```c#
"{0:#FileName:/(?<result>.*)\\./}".FormatString("parent/child/file.txt")
=> "file"

Dictionary<string, object> values = new Dictionary<string, object>();
values["Path"] = "parent/child/file.txt";
"{$Path:#FileName:/(?<result>.*)\\./}".FormatStringWithKey(values)
=> "file"
```
