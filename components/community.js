
//模拟

// 加载通过审核的日志内容
document.addEventListener("DOMContentLoaded", function () {
    const journalsContainer = document.getElementById("approved-journals");

    // 模拟从服务器获取数据（实际项目中可以用 Fetch API 请求后端接口）
    fetch("/api/approved-journals") // 替换为实际的后端 API 地址
        .then(response => {
            if (!response.ok) {
                throw new Error("网络响应失败");
            }
            return response.json();
        })
        .then(data => {
            if (data.length === 0) {
                journalsContainer.innerHTML = "<p>暂无通过审核的日志。</p>";
                return;
            }

            // 渲染日志列表
            data.forEach(journal => {
                const journalItem = document.createElement("div");
                journalItem.classList.add("journal-item");

                const title = document.createElement("h3");
                title.textContent = journal.title;

                const content = document.createElement("p");
                content.textContent = journal.content;

                journalItem.appendChild(title);
                journalItem.appendChild(content);

                journalsContainer.appendChild(journalItem);
            });
        })
        .catch(error => {
            console.error("加载日志失败:", error);
            journalsContainer.innerHTML = "<p>加载日志失败，请稍后再试。</p>";
        });
});