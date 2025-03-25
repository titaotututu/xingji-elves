//导航栏组件
document.getElementById("navbar").innerHTML = `
    <h2>行迹精灵</h2>
    <button onclick="loadPage('travelList')">行程</button>
    <button onclick="loadPage('todoList')">待办</button>
    <button onclick="loadPage('journal')">日志</button>
    <button onclick="loadPage('weather')">天气</button>
    <button onclick="loadPage('map')">地图</button>
`;
