# 项目目录结构
```
/xingji-elves
│── index.html          // 欢迎界面（软件入口）
│── main.html           //主界面，用户成功登录之后才能进入
│── register.html       //注册界面
│── style.css          // 样式文件
│── script.js          // 主 JavaScript 逻辑
│── components/
│   ├── navbar.js      // 导航栏
│   ├── travelList.js  // 旅行列表组件
│   ├── todoList.js    // 待办事项组件
│   ├── journal.js     // 旅行日志组件
│   ├── weather.js     // 天气查询组件
│   ├── map.js         // 地图导航组件
│   ├── auth.js         // 用户认证逻辑
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

4/1
- 点亮逻辑还需要改进
- 个人信息信息界面需要改进
- log out 按键
待做：
  - 社区界面
