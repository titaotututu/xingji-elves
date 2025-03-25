//地图
function loadMap() {
    document.getElementById("content").innerHTML = `
        <h2>地图导航</h2>
        <iframe 
            width="600" 
            height="450" 
            src="https://uri.amap.com/navigation?from=116.478346,39.997361&to=116.478346,39.997361&mode=car">
        </iframe>
    `;
}
