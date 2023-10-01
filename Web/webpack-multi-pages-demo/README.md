# Webpack Multi Pages Demo

## 简介

这是一个简单实现了使用Webpack来构建多页面应用的示例，并对Vue与Element Plus进行了简单的整合。

请仿照[pages](../../tree/master/src/pages)目录中的结构，添加需要的页面。一个页面一个文件夹，文件夹名、html文件名、js文件名均需保持一致。

环境配置文件位于[env](../../tree/master/env)目录中，定义的环境变量可在代码中通过`process.env`对象访问到。

页面编译后，在访问指定的html页面时，将自动加载与之对应的js文件，不需要事先在html中引入。

工程的编译结果存放在dist目录下，html在根目录，js代码在dist目录下的js目录下。

## 运行
构建：`npm run build:dev`

运行：`npm run dev`，执行完成后访问[http://localhost:8080](http://localhost:8080)