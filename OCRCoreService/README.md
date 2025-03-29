# PaddleOCRWebAPI接口文档
## 简介
实现在线调用OCR识别的WebAPI服务

## 运行环境
项目运行环境为.net8.0：

1、使用IIS：服务器环境推荐，建议操作系统Windows Server2016以上，
安装IIS及.net8环境，下载地址：
https://dotnet.microsoft.com/zh-cn/download/dotnet/8.0，找到 ASP.NET Core
运行时 8.0.14，点击Windows平台 Hosting Bundle 下载：
https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-aspnetcore-8.0.14-windows-hosting-bundle-installer

2、独立运行服务：建议操作系统Win10以上64位，安装.NET桌面运行时 8.0.14：
https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-desktop-8.0.14-windows-x64-installer，然后点击 OCRCoreService.exe 运行，默
认端口5000，浏览器打开 http://localhost:5000 提示服务正在运行即正常。

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
