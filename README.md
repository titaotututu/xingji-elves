# 项目目录结构
```
/xingji-elves
│── index.html          // 欢迎界面（软件入口）
│── main.html           //主界面，用户成功登录之后才能进入
│── register.html       //注册界面
│── style.css          // 样式文件
│── script.js          // 主 JavaScript 逻辑
│── components/
│   ├── community.js     // 社区的逻辑 不知道数据库怎么连接
│   ├── auth.js         // 退出逻辑
│── views/
│   │── index.html       //主界面，用户成功登录之后才能
│   │── style-views.css       //views中界面的美化
│   │── travel.html     //行程
│   │── map.html        //导航
│   │── weather.html    //天气
│   │── lighting.html   //点亮地标
│   │── journal.html    //旅行日志
│   │── assistant.html  //智能助手
│   │── feedback.html   //反馈通道
│   │── profile.html    //个人信息
│   │── assistant.html    //智能助手
│   |── travel/
|   |   |──create-todo.html  //创建待办
|   |   |──create-travel.html //创建旅程
|   |   |──todo.html       //待办信息
│   |── map/
|   |   |── Bike_Map.html  //骑行路径规划
|   |   |── Bus_Map.html   //公交路径规划
|   |   |── Car_Map.html   //驾驶路径规划
|   |   |── Walk_Map.html  //步行路径规划
│   |── journal/
|   |   |── journal-detail //日志的具体内容
│── images/            // 存放静态资源（图片、图标等）

```

# TODO

3/19 
- 再写两个页面

const apiKey = 'dc49e4062796f8aa751aadd950f417ed';   //天气查询的密钥

4/8 
点亮的前端已经做了 看连上的效果吧 我先不在Localstorage模拟了，只要获取的字段正常就可以正常显示 跟之前在winform中是一样的
