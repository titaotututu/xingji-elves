//旅行列表组件
async function loadTravelList() {
    let response = await fetch("http://localhost:5000/api/travel");
    let travels = await response.json();

    let content = "<h2>我的旅行</h2><ul>";
    travels.forEach(travel => {
        content += `<li>${travel.TravelTitle} - ${travel.TravelCity} (${travel.TravelTime})</li>`;
    });
    content += "</ul>";
    
    document.getElementById("content").innerHTML = content;
}

function loadPage(page) {
    switch (page) {
        case "travelList":
            loadTravelList();
            break;
        case "todoList":
            loadTodoList();
            break;
        case "journal":
            loadJournal();
            break;
        case "weather":
            loadWeather();
            break;
        case "map":
            loadMap();
            break;
    }
}
