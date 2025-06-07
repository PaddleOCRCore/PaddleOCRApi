# PaddleOCRWebAPI接口文档
## 简介
实现在线调用OCR识别的WebAPI服务

## 运行环境
项目运行环境为.net8.0：

1、使用IIS：服务器环境推荐，建议操作系统Windows Server2016 Data Center，
安装IIS，及.net8 环境，下载地址：
https://dotnet.microsoft.com/zh-cn/download/dotnet/8.0，找到ASP.NET Core
运行时 8.0.14，点击Windows 平台Hosting Bundle 下载：
https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-aspnetcore-8.0.14-win
dows-hosting-bundle-installer
2、独立运行服务：建议操作系统Win10 以上64 位，
安装ASP.NET Core 运行时 8.0.14：
https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-as
pnetcore-8.0.14-windows-x64-installer
安装.NET 桌面运行时 8.0.14：
https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-d
esktop-8.0.14-windows-x64-installer
创建一个批处理文件：StartOCRApi.bat，输入以下内容：
@echo off
set CURRENT_DIR=%~dp0
echo StartingOCRCoreService.dll..
dotnet "%CURRENT_DIR%OCRCoreService.dll" --urls http://*:5000
pause
并将批处理发送至桌面快捷方式
双击批处理文件StartOCRApi.bat，启动服务，默认端口5000(批处理中可修改)，浏览器打开http://localhost:5000 提示服务正在运行即正常。

### 修改Web.Config 配置文件，将hostingModel="inprocess"改为hostingModel=" OutOfProcess "

## 请求与响应协议
接口采用Post请求，具体依所访问接口定义为准。

请求Content-Type设定 application/json 

## 接口返回结果说明
请求的返回参数格式为 JSON，编码为UTF-8 

`
{
 "status": 200,
 "data": object
 "errorMessage": ""
}`

| 参数名      | 描述   | 
| ----------  | ------ |
| status      | 接口请求校验结果代码  如：200 表示成功 ,其它为失败| 
| data        | 返回数据 文字或 Json 数据| 
| errorMessage|  调用接口返回的说明| 

## 接口清单

|序号| 类型| 接口地址| 接口名称| 创建日期| 最后发布日期| 备注|
| -- | --- |-------- | ------- |---------| ------------|-----|
|1| OCR| /OCRService/GetOCRText| 图片OCR识别| 2025/03/28| 2025/03/28| 上传Base64|
|2| OCR| /OCRService/GetOCRFile| 图片OCR识别| 2025/04/27| 2025/04/27| 上传图片|

图片OCR识别：/OCRService/GetOCRText 

提交方式：POST

传入参数：

`
{
 "Base64String ":"",
 " ResultType ":"text"
} `

| 序号|  参数名称   | 描述  | 类型   |  是否必填 |  备注  | 
| --- | ----------  |-------| ------ |-----------| ------ |
| 1   | Base64String|  图片Base6 编码|  字符串|  必填|
| 2   | ResultType | text/json|  字符串 | 必填 | Text仅返回文字| 

### 返回结果示例：

`
{
 "status": 200,
 "data": "纯臻营养护发素\r\n 产品信息/参数\r\n（45 元/每公斤，100 公斤起订）
\r\n 每瓶 22 元，1000 瓶起订）\r\n【品牌】：代加工方式/OEMODM\r\n【品名】：纯
臻营养护发素\r\n【产品编号】：YM-X-3011\r\nODMOEM\r\n【净含量】：220ml\r\n
【适用人群】：适合所有肤质\r\n【主要成分】：鲸蜡硬脂醇、燕麦 β-葡聚\r\n 糖、椰
油酰胺丙基甜菜碱、泛醌\r\n（成品包材）\r\n【主要功能】：可紧致头发磷层，从而
达到\r\n 即时持久改善头发光泽的效果，给干燥的头\r\n 发足够的滋养",
 "errorMessage": ""
}
`
